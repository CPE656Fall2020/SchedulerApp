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
        /// <param name="name">Name of phase.</param>
        public PassPhase(DateTime startTime, DateTime endTime, PhaseType name)
        {
            this.StartTime = startTime;
            this.EndTime = endTime;
            this.PhaseName = name;
            this.Duration = endTime - startTime;
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
        public double Power => Math.Round(Math.Abs(this.TotalEnergy) / this.EndTime.Subtract(this.StartTime).TotalSeconds, 3);

        /// <inheritdoc/>
        public double TotalEnergy { get; set; }

        /// <inheritdoc/>
        public double MaxEnergy { get; private set; }

        /// <inheritdoc/>
        public void SetRandomValues(Random random, double maxEnergy, int? maxBytes = null)
        {
            if (this.PhaseName == PhaseType.Sunlight)
            {
                maxEnergy *= -1.0;
            }

            this.MaxEnergy = Math.Round(maxEnergy, 3);
            this.TotalEnergy = Math.Round(random.NextDouble() * this.MaxEnergy, 3);
        }
    }
}
