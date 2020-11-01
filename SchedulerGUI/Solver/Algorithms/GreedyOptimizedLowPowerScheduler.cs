using System.Collections.Generic;
using System.Linq;
using SchedulerDatabase.Models;
using SchedulerGUI.Enums;
using SchedulerGUI.Interfaces;
using SchedulerGUI.Models;

namespace SchedulerGUI.Solver.Algorithms
{
    /// <summary>
    /// <see cref="GreedyOptimizedLowPowerScheduler"/> solves the scheduling problem using a greedy algorithm
    /// to always pick the lowest power device for each pass that can compute the AES encryption problem within the
    /// allowed time window. If a solution can be found, it will be globally optimized to use the minimal amount of power,
    /// however, it is NOT guaranteed to be optimized to be the fastest. Alternative profiles may be possible to allow
    /// for solving the problem with the the same or greater level of energy (but NOT less), but at a faster rate.
    /// </summary>
    public class GreedyOptimizedLowPowerScheduler : IScheduleSolver
    {
        /// <inheritdoc/>
        public string Name => "Low Power Optimized";

        /// <inheritdoc/>
        public object Tag { get; set; }

        /// <inheritdoc/>
        public ScheduleSolution Solve(IEnumerable<PassOrbit> passes, IEnumerable<IByteStreamProcessor> summarizedEncryptors, IEnumerable<IByteStreamProcessor> summarizedCompressors, Battery battery)
        {
            var solution = new ScheduleSolution()
            {
                IsSolvable = true, // Start out assuming it is solvable unless we encounter a problem.
                ViableProfiles = new Dictionary<PassOrbit, Dictionary<PhaseType, IByteStreamProcessor>>(),
                Problems = new List<ScheduleSolution.SchedulerProblem>(),
            };

            // Zero out all the energies used before calculation.
            // That way, if the scheduler fails, the only values that will be set
            // are the passes that were completed successfully.
            foreach (var pass in passes)
            {
                pass.PassPhases.First(p => p.PhaseName == Enums.PhaseType.Encryption).TotalEnergyUsed = 0;
                pass.PassPhases.First(p => p.PhaseName == Enums.PhaseType.Datalink).TotalEnergyUsed = 0;
                pass.IsScheduledSuccessfully = false;

                solution.ViableProfiles[pass] = new Dictionary<PhaseType, IByteStreamProcessor>();
            }

            var optimizedAES = this.BuildOptimizationMap(summarizedEncryptors, solution);
            var optimizedCompression = this.BuildOptimizationMap(summarizedCompressors, solution);

            double currentCapacityJoules = 0;
            foreach (var pass in passes)
            {
                // Encryption and Datalink require special treatment - find them first for exacting key parameters.
                var encryptionPhase = pass.PassPhases.First(p => p.PhaseName == Enums.PhaseType.Encryption) as EncryptionPassPhase;
                var downlinkPhase = pass.PassPhases.First(p => p.PhaseName == Enums.PhaseType.Datalink);

                var succeededPhasesInPass = 0;

                // Run through every phase to check the cumulative energy status
                foreach (var phase in pass.PassPhases.OrderBy(p => p.StartTime))
                {
                    if (phase is EncryptionPassPhase enc)
                    {
                        // Handle encryption - Compute valid profile if possible, set energy, and apply
                        var foundViableAESProfile = this.FindProfileForPhase(optimizedAES, solution, pass, encryptionPhase, encryptionPhase.BytesToEncrypt, ref currentCapacityJoules);
                        if (!foundViableAESProfile)
                        {
                            this.ReportProfileNotAvailable(solution, pass, "AES encryption");
                        }
                        else
                        {
                            // Indicate that encryption was successful.
                            succeededPhasesInPass += 1;
                        }
                    }
                    else if (phase.PhaseName == Enums.PhaseType.Datalink)
                    {
                        // Handle datalink - Compute valid profile if possible, set energy, and apply
                        // Use the number of bytes from the encryption phase as the input size to the compressor.
                        var foundViableCompressionProfile = this.FindProfileForPhase(optimizedCompression, solution, pass, downlinkPhase, encryptionPhase.BytesToEncrypt, ref currentCapacityJoules);
                        if (!foundViableCompressionProfile)
                        {
                            this.ReportProfileNotAvailable(solution, pass, "data compression");
                        }
                        else
                        {
                            // Indicate that compression was successful.
                            succeededPhasesInPass += 1;
                        }
                    }
                    else
                    {
                        // Regular phase - just use the listed energy values and move along.
                        currentCapacityJoules -= phase.TotalEnergyUsed;
                        var success = this.EnforceBatteryLimits(ref currentCapacityJoules, battery, solution, pass, phase);
                        if (success)
                        {
                            // Indicate that compression was successful.
                            succeededPhasesInPass += 1;
                        }
                    }
                }

                // Only mark the pass successful if each phase was done successfully
                pass.IsScheduledSuccessfully = succeededPhasesInPass == pass.PassPhases.Count;
            }

            return solution;
        }

        /// <summary>
        /// Builds a dictionary of Byte Processor Profiles sorted according to energy efficency.
        /// </summary>
        /// <param name="summarizedProfiles">The profiles that need to be ordered.</param>
        /// <param name="solution">A solution profile to recording warnings into.</param>
        /// <returns>A dictionary sorted by energy efficiency as the key, most efficient first.</returns>
        private SortedDictionary<double, IByteStreamProcessor> BuildOptimizationMap(IEnumerable<IByteStreamProcessor> summarizedProfiles, ScheduleSolution solution)
        {
            var optimizationMap = new SortedDictionary<double, IByteStreamProcessor>();

            // Characterize each profile based on how low-power it can be
            foreach (var profile in summarizedProfiles)
            {
                if (optimizationMap.ContainsKey(profile.JoulesPerByte))
                {
                    var existingProfile = optimizationMap[profile.JoulesPerByte];
                    var fasterProfile = existingProfile.BytesPerSecond > profile.BytesPerSecond ? existingProfile : profile;
                    optimizationMap[profile.JoulesPerByte] = fasterProfile;

                    var fasterNumber = (fasterProfile == existingProfile) ? "1" : "2";

                    solution.Problems.Add(new ScheduleSolution.SchedulerProblem(
                        ScheduleSolution.SchedulerProblem.SeverityLevel.Warning,
                        $"Two profiles appear to offer identical power efficiency. Profile 1: {existingProfile.ShortProfileClassDescription}, Profile 2: {profile.ShortProfileClassDescription}. Selecting the faster one: Profile {fasterNumber}"));
                }
                else
                {
                    optimizationMap.Add(profile.JoulesPerByte, profile);
                }
            }

            return optimizationMap;
        }

        private bool FindProfileForPhase(SortedDictionary<double, IByteStreamProcessor> optimizationMap, ScheduleSolution solution, PassOrbit pass, IPassPhase phase, long bytes, ref double currentCapacityJoules)
        {
            var allowedTime = phase.EndTime.Subtract(phase.StartTime);

            // For the encryption phase, we have an abs max of currentCapacityJoules
            // and the time allocated to encryptionPhase.

            // Start with the most ideal profile and keep testing until we find one that fits
            var foundViableProfile = false;
            foreach (var profile in optimizationMap.Values)
            {
                var timeRequired = bytes / profile.BytesPerSecond;
                var energyRequired = bytes * profile.JoulesPerByte;

                if (timeRequired > allowedTime.TotalSeconds)
                {
                    // Out of time
                    foundViableProfile = false;
                }
                else if (energyRequired > currentCapacityJoules)
                {
                    // Out of power
                    foundViableProfile = false;
                }
                else
                {
                    // This solution works for this part
                    solution.ViableProfiles[pass][phase.PhaseName] = profile;
                    phase.TotalEnergyUsed = energyRequired;
                    foundViableProfile = true;
                    currentCapacityJoules -= energyRequired;
                    break;
                }
            }

            return foundViableProfile;
        }

        /// <summary>
        /// Applies a bounding function to the current capacity of the battery, to prohibit discharging below 0 joules,
        /// and the prevent charging above the maximum specified capabilities. If the battery exceeds the specification,
        /// a warning (for overcharge) or critical error (for discharged) will be logged to the solution.
        /// </summary>
        /// <param name="currentCapacityJoules">The charge level of the battery assuming the previous operation ran to completion.</param>
        /// <param name="battery">The battery parameters to enforce.</param>
        /// <param name="solution">The solution to log warnings and errors to.</param>
        /// <param name="pass">The last pass that was executed.</param>
        /// <returns>A value indicating whether the requested capacity could be accomplished successfully.</returns>
        private bool EnforceBatteryLimits(ref double currentCapacityJoules, Battery battery, ScheduleSolution solution, PassOrbit pass, IPassPhase phase)
        {
            var success = true;

            if (currentCapacityJoules < 0)
            {
                // Orbit parameters are impossible - we've run out of power.
                // Error is fatal - denote the problem and abort scheduling.
                solution.IsSolvable = false;
                solution.Problems.Add(new ScheduleSolution.SchedulerProblem(
                    ScheduleSolution.SchedulerProblem.SeverityLevel.Fatal,
                    $"Orbit parameters for {pass.Name} are impossible. No power remains while scheduling the {phase.PhaseName} for {pass.Name}."));

                currentCapacityJoules = 0;

                // Discharging completely is a fatal error that will prevent the pass from completing
                success = false;
            }
            else if (currentCapacityJoules > battery.EffectiveCapacityJ)
            {
                solution.Problems.Add(new ScheduleSolution.SchedulerProblem(
                    ScheduleSolution.SchedulerProblem.SeverityLevel.Warning,
                    $"The battery was a contraint during {pass.Name}. {currentCapacityJoules:n} J were available, but max charge capacity is {battery.EffectiveCapacityJ:n} J"));

                // Apply the cap.
                currentCapacityJoules = battery.EffectiveCapacityJ;

                // Overcharging doesn't prevent the mission from continuing, but indicates inefficiency
                success = true;
            }

            return success;
        }

        private void ReportProfileNotAvailable(ScheduleSolution solution, PassOrbit pass, string operation)
        {
            // No profile could be found that fits the available power and time
            // All previous schedules have already been done with the lowest-power option that fits
            // so if we're out of power, there is no solution possible.
            // If we're out of time, faster devices are needed, or the phase needs lengthened.

            solution.IsSolvable = false;
            solution.Problems.Add(new ScheduleSolution.SchedulerProblem(
                      ScheduleSolution.SchedulerProblem.SeverityLevel.Fatal,
                      $"Orbit parameters for {pass.Name} are impossible. No devices are capable of performing the {operation} in the required power and time window."));
        }
    }
}
