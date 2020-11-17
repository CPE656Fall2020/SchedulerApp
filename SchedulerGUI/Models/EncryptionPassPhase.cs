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

        /// <summary>
        /// Initializes a new instance of the <see cref="EncryptionPassPhase"/> class.
        /// </summary>
        /// <param name="passPhaseToCopy">Phase to copy.</param>
        public EncryptionPassPhase(EncryptionPassPhase passPhaseToCopy)
        {
            this.StartTime = passPhaseToCopy.StartTime;
            this.EndTime = passPhaseToCopy.EndTime;
            this.PhaseName = passPhaseToCopy.PhaseName;
            this.TotalEnergyUsed = passPhaseToCopy.TotalEnergyUsed;
            this.MaxEnergyUsed = passPhaseToCopy.MaxEnergyUsed;
            this.BytesToEncrypt = passPhaseToCopy.BytesToEncrypt;
            this.MaxBytes = passPhaseToCopy.MaxBytes;
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
        public double TotalEnergyUsed { get; set; }

        /// <inheritdoc/>
        public double MaxEnergyUsed { get; private set; }

        /// <summary>
        /// Gets max number of bytes to encrypt during phase.
        /// </summary>
        public long MaxBytes { get; private set; }

        /// <summary>
        /// Gets or sets number of bytes to encrypt during phase.
        /// </summary>
        public long BytesToEncrypt { get; set; }

        /// <inheritdoc/>
        public void SetRandomValues(double randomDouble, double maxEnergy, long? maxBytes = null)
        {
            this.MaxBytes = (int)maxBytes;

            this.BytesToEncrypt = (long)(randomDouble * this.MaxBytes) * 10;

            // The energy consumption will be a factor of the fake number of bytes scheduled with the fake scheduler.
            this.MaxEnergyUsed = 0;
            this.TotalEnergyUsed = 0;
        }
    }
}
