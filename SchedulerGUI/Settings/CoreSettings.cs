namespace SchedulerGUI.Settings
{
    /// <summary>
    /// <see cref="CoreSettings"/> defines the settings needed for basic operation.
    /// </summary>
    public class CoreSettings
    {
        /// <summary>
        /// Gets or sets the database file that is used for scheduling data.
        /// </summary>
        public string DatabaseLocation { get; set; } = "data.db";
    }
}
