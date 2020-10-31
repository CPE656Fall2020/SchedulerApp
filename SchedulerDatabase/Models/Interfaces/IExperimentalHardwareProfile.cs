using System;

namespace SchedulerDatabase.Models
{
    /// <summary>
    /// <see cref="IExperimentalHardwareProfile"/> defines a profile for device data that was collected experimentally.
    /// </summary>
    public interface IExperimentalHardwareProfile : IHardwareProfile
    {
        /// <summary>
        /// Gets a free-form description of the conditions under which this experiment was performed or any other relevant details.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets any additional information that is required to uniquely identify this profile.
        /// </summary>
        string AdditionalUniqueInfo { get; }

        /// <summary>
        /// Gets the name of the person who authored this entry.
        /// </summary>
        string Author { get; }
    }
}
