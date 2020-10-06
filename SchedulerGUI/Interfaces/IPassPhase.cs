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
        /// Gets or sets duration of phase.
        /// </summary>
        TimeSpan Duration { get; set; }

        /// <summary>
        /// Gets or sets start time of phase.
        /// </summary>
        DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets end time of phase.
        /// </summary>
        DateTime EndTime { get; set; }

        /// <summary>
        /// Gets or sets name of phase.
        /// </summary>
        PhaseType PhaseName { get; set; }

        /// <summary>
        /// Gets or sets total power of phase.
        /// </summary>
        double TotalPower { get; set; }

        /// <summary>
        /// Gets or sets total energy of phase.
        /// </summary>
        double TotalEnergy { get; set; }
    }
}
