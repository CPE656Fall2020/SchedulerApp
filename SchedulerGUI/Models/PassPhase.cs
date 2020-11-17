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

        /// <summary>
        /// Initializes a new instance of the <see cref="PassPhase"/> class.
        /// </summary>
        /// <param name="passPhaseToCopy">Phase to copy.</param>
        public PassPhase(IPassPhase passPhaseToCopy)
        {
            this.StartTime = passPhaseToCopy.StartTime;
            this.EndTime = passPhaseToCopy.EndTime;
            this.PhaseName = passPhaseToCopy.PhaseName;
            this.TotalEnergyUsed = passPhaseToCopy.TotalEnergyUsed;
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
        public void SetRandomValues(double randomDouble, double maxEnergy, long? maxBytes = null)
        {
            if (this.PhaseName == PhaseType.Sunlight)
            {
                maxEnergy *= -1.0;
            }

            maxEnergy = Math.Round(maxEnergy, 3);
            this.TotalEnergyUsed = Math.Round(randomDouble * maxEnergy, 3);
        }
    }
}
