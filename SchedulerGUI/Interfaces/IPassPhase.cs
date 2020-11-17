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
        /// Sets properties with randomly-generated values.
        /// </summary>
        /// <param name="randomDouble">Random double.</param>
        /// <param name="maxEnergy">Max energy allotted for phase.</param>
        /// <param name="maxBytes">Max bytes alloted for encryption phase.</param>
        void SetRandomValues(double randomDouble, double maxEnergy, long? maxBytes = null);
    }
}
