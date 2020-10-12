using System;
using SchedulerGUI.Enums;

namespace SchedulerGUI.Interfaces
{
    /// <summary>
    /// <see cref="IPassPhase"/> interface for passes of orbit.
    /// </summary>
    public interface IPassPhase
    {
        /// <summary>
        /// Gets duration of phase.
        /// </summary>
        TimeSpan Duration { get; }

        /// <summary>
        /// Gets start time of phase.
        /// </summary>
        DateTime StartTime { get; }

        /// <summary>
        /// Gets end time of phase.
        /// </summary>
        DateTime EndTime { get; }

        /// <summary>
        /// Gets name of phase.
        /// </summary>
        PhaseType PhaseName { get; }

        /// <summary>
        /// Gets or sets total energy of phase.
        /// </summary>
        double TotalEnergyUsed { get; set; }

        /// <summary>
        /// Gets max energy of phase.
        /// </summary>
        double MaxEnergyUsed { get; }

        /// <summary>
        /// Sets properties with randomly-generated values.
        /// </summary>
        /// <param name="random">Random number generator.</param>
        /// <param name="maxEnergy">Max energy allotted for phase.</param>
        /// <param name="maxBytes">Max bytes alloted for encryption phase.</param>
        void SetRandomValues(Random random, double maxEnergy, int? maxBytes = null);
    }
}
