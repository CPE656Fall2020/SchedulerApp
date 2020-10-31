using System.Collections.Generic;
using System.Linq;
using SchedulerDatabase;
using SchedulerDatabase.Models;
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
                ViableProfiles = new Dictionary<PassOrbit, IByteStreamProcessor>(),
                Problems = new List<ScheduleSolution.SchedulerProblem>(),
            };

            // Zero out all the energies used before calculation.
            // That way, if the scheduler fails, the only values that will be set
            // are the passes that were completed successfully.
            foreach (var pass in passes)
            {
                pass.PassPhases.First(p => p.PhaseName == Enums.PhaseType.Encryption).TotalEnergyUsed = 0;
                pass.IsScheduledSuccessfully = false;
            }

            var optimizedAES = this.BuildOptimizationMap(summarizedEncryptors, solution);
            var optimizedCompression = this.BuildOptimizationMap(summarizedCompressors, solution);

            double currentCapacityJoules = 0;
            foreach (var pass in passes)
            {
                // Run through every phase to find the energy budget for encryption.
                foreach (var phase in pass.PassPhases.OrderBy(p => p.StartTime))
                {
                    if (!(phase is EncryptionPassPhase))
                    {
                        currentCapacityJoules -= phase.TotalEnergyUsed;

                        if (currentCapacityJoules < 0)
                        {
                            // Orbit parameters are impossible - we've run out of power.
                            // Error is fatal - denote the problem and abort scheduling.
                            solution.IsSolvable = false;
                            solution.Problems.Add(new ScheduleSolution.SchedulerProblem(
                                ScheduleSolution.SchedulerProblem.SeverityLevel.Fatal,
                                $"Orbit parameters for {pass.Name} are impossible. No power remains after scheduling all prior passes optimally."));
                            return solution;
                        }
                        else if (currentCapacityJoules > battery.EffectiveCapacityJ)
                        {
                            solution.Problems.Add(new ScheduleSolution.SchedulerProblem(
                                ScheduleSolution.SchedulerProblem.SeverityLevel.Warning,
                                $"The battery was a contraint during {pass.Name}. {currentCapacityJoules} J were available, but max charge capacity is {battery.EffectiveCapacityJ}"));

                            // Apply the cap.
                            currentCapacityJoules = battery.EffectiveCapacityJ;
                        }
                    }
                }

                var encryptionPhase = pass.PassPhases.First(p => p.PhaseName == Enums.PhaseType.Encryption) as EncryptionPassPhase;

                /* ----- Handle AES Processing -------- */
                var foundViableAESProfile = this.FindProfileForPhase(optimizedAES, solution, pass, encryptionPhase, encryptionPhase.BytesToEncrypt, ref currentCapacityJoules);

                if (!foundViableAESProfile)
                {
                    // No profile could be found that fits the available power and time
                    // All previous schedules have already been done with the lowest-power option that fits
                    // so if we're out of power, there is no solution possible.
                    // If we're out of time, faster devices are needed, or the encryption phase needs lengthened.
                    solution.IsSolvable = false;
                    solution.Problems.Add(new ScheduleSolution.SchedulerProblem(
                              ScheduleSolution.SchedulerProblem.SeverityLevel.Fatal,
                              $"Orbit parameters for {pass.Name} are impossible. No devices are capable of performing the encryption in the required time window."));
                    solution.ViableProfiles[pass] = null;
                }

                /* ----- Handle Compression Processing -------- */

                var downlinkPhase = pass.PassPhases.First(p => p.PhaseName == Enums.PhaseType.Datalink);
                var foundViableCompressionProfile = this.FindProfileForPhase(optimizedCompression, solution, pass, downlinkPhase, encryptionPhase.BytesToEncrypt, ref currentCapacityJoules);

                if (!foundViableCompressionProfile)
                {
                    // No profile could be found that fits the available power and time
                    // All previous schedules have already been done with the lowest-power option that fits
                    // so if we're out of power, there is no solution possible.
                    // If we're out of time, faster devices are needed, or the compression phase needs lengthened.
                    solution.IsSolvable = false;
                    solution.Problems.Add(new ScheduleSolution.SchedulerProblem(
                              ScheduleSolution.SchedulerProblem.SeverityLevel.Fatal,
                              $"Orbit parameters for {pass.Name} are impossible. No devices are capable of performing the compression in the required time window."));
                    solution.ViableProfiles[pass] = null;
                }
            }

            return solution;
        }

        /// <summary>
        /// Builds a dictionary of Byte Processor Profiles sorted according to energy efficency.
        /// </summary>
        /// <param name="availableProfiles">The profiles that need to be ordered.</param>
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

                    /*
                    var description1 = $"{existingProfile.PlatformName} Accelerator: {existingProfile.PlatformAccelerator} Cores: {existingProfile.NumCores} Provider: {existingProfile.ProviderName} Clock: {existingProfile.TestedFrequency}";
                    var description2 = $"{profile.PlatformName} Accelerator: {profile.PlatformAccelerator} Cores: {profile.NumCores} Provider: {profile.ProviderName} Clock: {profile.TestedFrequency}";
                    */

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
                    solution.ViableProfiles[pass] = profile;
                    phase.TotalEnergyUsed = energyRequired;
                    pass.IsScheduledSuccessfully = true;
                    foundViableProfile = true;
                    currentCapacityJoules -= energyRequired;
                    break;
                }
            }

            return foundViableProfile;
        }
    }
}
