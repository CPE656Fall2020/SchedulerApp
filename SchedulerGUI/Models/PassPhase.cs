using System;
using SchedulerGUI.Enums;
using SchedulerGUI.Interfaces;

namespace SchedulerGUI.Models
{
    /// <summary>
    /// <see cref="PassPhase"/> represents phase of orbit.
    /// </summary>
    public class PassPhase : IPassPhase
    {
        private double maxEnergy;

        /// <summary>
        /// Initializes a new instance of the <see cref="PassPhase"/> class.
        /// </summary>
        /// <param name="startTime">Start of phase.</param>
        /// <param name="endTime">End of phase.</param>
        /// <param name="name">Name of phase.</param>
        /// <param name="maxEnergy">Max energy alloted for phase.</param>
        public PassPhase(DateTime startTime, DateTime endTime, PhaseType name, double maxEnergy)
        {
            this.StartTime = startTime;
            this.EndTime = endTime;
            this.PhaseName = name;
            this.Duration = endTime - startTime;

            if (name == PhaseType.Sunlight)
            {
                maxEnergy *= -1.0;
            }

            this.maxEnergy = maxEnergy;
        }

        /// <inheritdoc/>
        public TimeSpan Duration { get; set; }

        /// <inheritdoc/>
        public DateTime StartTime { get; set; }

        /// <inheritdoc/>
        public DateTime EndTime { get; set; }

        /// <inheritdoc/>
        public PhaseType PhaseName { get; }

        /// <inheritdoc/>
        public double Power => this.TotalEnergy / this.EndTime.Subtract(this.StartTime).TotalSeconds;

        /// <inheritdoc/>
        public double TotalEnergy { get; set; }

        /// <inheritdoc/>
        public void SetRandomValues(Random random)
        {
            this.TotalEnergy = random.NextDouble() * this.maxEnergy;
        }
    }
}
