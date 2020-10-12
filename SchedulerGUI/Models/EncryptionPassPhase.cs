using System;
using SchedulerGUI.Enums;
using SchedulerGUI.Interfaces;

namespace SchedulerGUI.Models
{
    /// <summary>
    /// <see cref="EncryptionPassPhase"/> represents encryption phase of orbit.
    /// </summary>
    public class EncryptionPassPhase : IPassPhase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EncryptionPassPhase"/> class.
        /// </summary>
        /// <param name="startTime">Start of phase.</param>
        /// <param name="endTime">End of phase.</param>
        /// <param name="name">Name of phase.</param>
        public EncryptionPassPhase(DateTime startTime, DateTime endTime, PhaseType name)
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
        public PhaseType PhaseName { get; set; }

        /// <inheritdoc/>
        public double Power => Math.Round(Math.Abs(this.TotalEnergy) / this.EndTime.Subtract(this.StartTime).TotalSeconds, 3);

        /// <inheritdoc/>
        public double TotalEnergy { get; set; }

        /// <inheritdoc/>
        public double MaxEnergy { get; private set; }

        /// <summary>
        /// Gets max number of bytes to encrypt during phase.
        /// </summary>
        public int MaxBytes { get; private set; }

        /// <summary>
        /// Gets or sets number of bytes to encrypt during phase.
        /// </summary>
        public int BytesToEncrypt { get; set; }

        /// <inheritdoc/>
        public void SetRandomValues(Random random, double maxEnergy, int? maxBytes = null)
        {
            this.MaxBytes = (int)maxBytes;
            this.MaxEnergy = Math.Round(maxEnergy, 3);

            this.TotalEnergy = Math.Round(random.NextDouble() * this.MaxEnergy, 3);
            this.BytesToEncrypt = random.Next(0, this.MaxBytes);
        }
    }
}
