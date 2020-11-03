namespace SchedulerDatabase.Models
{
    /// <summary>
    /// <see cref="IExperimentalHardwareProfile"/> defines a profile for device data that was collected experimentally.
    /// </summary>
    public interface IExperimentalHardwareProfile : IHardwareProfile
    {
        /// <summary>
        /// Gets or sets a free-form description of the conditions under which this experiment was performed or any other relevant details.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Gets or sets any additional information that is required to uniquely identify this profile.
        /// </summary>
        string AdditionalUniqueInfo { get; set; }

        /// <summary>
        /// Gets or sets the name of the person who authored this entry.
        /// </summary>
        string Author { get; set; }
    }
}
