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
        /// Gets power of phase.
        /// </summary>
        double Power { get; }

        /// <summary>
        /// Gets or sets total energy of phase.
        /// </summary>
        double TotalEnergy { get; set; }

        /// <summary>
        /// Sets properties with randomly-generated values.
        /// </summary>
        /// <param name="random">Random number generator.</param>
        void SetRandomValues(Random random);
    }
}
