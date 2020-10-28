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
        /// <summary>
        /// Initializes a new instance of the <see cref="PassPhase"/> class.
        /// </summary>
        /// <param name="startTime">Start of phase.</param>
        /// <param name="endTime">End of phase.</param>
        /// <param name="phaseName">Name of phase.</param>
        public PassPhase(DateTime startTime, DateTime endTime, PhaseType phaseName)
        {
            this.StartTime = startTime;
            this.EndTime = endTime;
            this.PhaseName = phaseName;
        }

        /// <inheritdoc/>
        public TimeSpan Duration => this.EndTime - this.StartTime;

        /// <inheritdoc/>
        public DateTime StartTime { get; set; }

        /// <inheritdoc/>
        public DateTime EndTime { get; set; }

        /// <inheritdoc/>
        public PhaseType PhaseName { get; }

        /// <inheritdoc/>
        public double TotalEnergyUsed { get; set; }

        /// <inheritdoc/>
        public double MaxEnergyUsed { get; private set; }

        /// <inheritdoc/>
        public void SetRandomValues(Random random, double maxEnergy, int? maxBytes = null)
        {
            if (this.PhaseName == PhaseType.Sunlight)
            {
                maxEnergy *= -1.0;
            }

            this.MaxEnergyUsed = Math.Round(maxEnergy, 3);
            this.TotalEnergyUsed = Math.Round(random.NextDouble() * this.MaxEnergyUsed, 3);
        }
    }
}
