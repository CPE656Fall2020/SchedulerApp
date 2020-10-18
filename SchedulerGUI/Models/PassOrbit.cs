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
        public PassOrbit(string name, DateTime startTime, DateTime endTime, Random random)
        {
            this.Name = "Pass #: " + name;
            this.StartTime = startTime;
            this.EndTime = endTime;

            this.PassEnergy = random.Next();
            this.InitPhases(random);
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
            this.PassPhases = new List<IPassPhase>();
            DateTime startTime = this.StartTime;
            int numPhases = Enum.GetValues(typeof(PhaseType)).Length;
            double duration = (this.EndTime - this.StartTime).TotalSeconds / numPhases;

            var enums = Enum.GetValues(typeof(PhaseType)).Cast<PhaseType>().Where(x => x != PhaseType.Encryption);
            double phaseEnergy = this.PassEnergy / numPhases;

            foreach (PhaseType phaseType in enums)
            {
                PassPhase phase = new PassPhase(startTime, startTime.AddSeconds(duration), phaseType);
                phase.SetRandomValues(random, phaseEnergy);
                this.PassPhases.Add(phase);

                startTime = startTime.AddSeconds(duration);
            }

            EncryptionPassPhase encryptionPhase = new EncryptionPassPhase(startTime, startTime.AddSeconds(duration), PhaseType.Encryption);
            encryptionPhase.SetRandomValues(random, phaseEnergy, random.Next());
            this.PassPhases.Add(encryptionPhase);
        }
    }
}
