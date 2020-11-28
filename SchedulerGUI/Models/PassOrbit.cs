using System;
using System.Collections.Generic;
using System.Linq;
using SchedulerGUI.Enums;
using SchedulerGUI.Interfaces;

namespace SchedulerGUI.Models
{
    /// <summary>
    /// <see cref="PassOrbit"/> represents an orbit of a satelite and the various phases that comprise that orbit.
    /// </summary>
    public class PassOrbit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PassOrbit"/> class.
        /// </summary>
        /// <param name="name">The pass name.</param>
        /// <param name="startTime">The beginning time of the pass.</param>
        /// <param name="endTime">The ending time of the pass.</param>
        /// <param name="random">A random number generator for initial values.</param>
        public PassOrbit(string name, DateTime startTime, DateTime endTime, Random random = null)
        {
            this.Name = name;
            this.StartTime = startTime;
            this.EndTime = endTime;

            if (random != null)
            {
                // 30 watts for 2 hours is 216,000 Joules.
                // That's in the ballpark of running a fairly decent (eg laptop) computer equipment
                // and seems to be a reasonable default max for the random values.
                // 15,000 Joules is around a 4.2 watt minimum.
                this.PassEnergy = random.Next(minValue: 15000, maxValue: 216000);
                this.InitPhases(random);
            }
        }

        /// <summary>
        /// Gets the name of the pass.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the energy of the pass.
        /// </summary>
        public int PassEnergy { get; }

        /// <summary>
        /// Gets or sets the phases of the pass orbit.
        /// </summary>
        public List<IPassPhase> PassPhases { get; set; }

        /// <summary>
        /// Gets the starting time for this pass.
        /// </summary>
        public DateTime StartTime { get; } = default;

        /// <summary>
        /// Gets the ending time for this pass.
        /// </summary>
        public DateTime EndTime { get; } = default;

        /// <summary>
        /// Gets or sets a value indicating whether this pass has been scheduled successfully.
        /// </summary>
        public bool IsScheduledSuccessfully { get; set; } = false;

        private void InitPhases(Random random)
        {
            // TODO: Fix this to not be so hardcoded - but this will work for now to make the history graph not look bad
            this.PassPhases = new List<IPassPhase>();
            DateTime startTime = this.StartTime;
            int numPhases = Enum.GetValues(typeof(PhaseType)).Length;
            double duration = (this.EndTime - this.StartTime).TotalSeconds / numPhases;

            var enums = Enum.GetValues(typeof(PhaseType)).Cast<PhaseType>().Where(x => x != PhaseType.Encryption);
            double phaseEnergy = this.PassEnergy / numPhases;

            // Do sunlight
            var sunlight = new PassPhase(startTime, startTime.AddSeconds(duration), PhaseType.Sunlight);
            sunlight.SetRandomValues(random.NextDouble(), phaseEnergy);
            startTime = startTime.AddSeconds(duration);
            this.PassPhases.Add(sunlight);

            // Do mission
            var mission = new PassPhase(startTime, startTime.AddSeconds(duration), PhaseType.Mission);
            mission.SetRandomValues(random.NextDouble(), phaseEnergy);
            startTime = startTime.AddSeconds(duration);
            this.PassPhases.Add(mission);

            // Do encryption
            EncryptionPassPhase encryptionPhase = new EncryptionPassPhase(startTime, startTime.AddSeconds(duration), PhaseType.Encryption);
            encryptionPhase.SetRandomValues(random.NextDouble(), phaseEnergy, random.Next());
            startTime = startTime.AddSeconds(duration);
            this.PassPhases.Add(encryptionPhase);

            // Do datalink
            var datalink = new PassPhase(startTime, startTime.AddSeconds(duration), PhaseType.Datalink);
            datalink.SetRandomValues(random.NextDouble(), phaseEnergy);
            startTime = startTime.AddSeconds(duration);
            this.PassPhases.Add(datalink);
        }
    }
}
