using CommandLine;

namespace SchedulerImportTools
{
    /// <summary>
    /// <see cref="Options"/> defines the operating parameters for the import tool.
    /// </summary>
    public class Options
    {
        /// <summary>
        /// Gets or sets the path for the database file to store imported data in.
        /// </summary>
        [Option(HelpText = "The database file to store the imported data in.", Default = "data.db")]
        public string DatabaseFile { get; set; }

        /// <summary>
        /// Gets or sets the file path for the source excel file to import from.
        /// </summary>
        [Option(HelpText = "The source Excel file to import.", Required = true)]
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets the file path for the translation map to import from.
        /// </summary>
        [Option(HelpText = "The translation map to use.", Required = true)]
        public string Map { get; set; }

        /// <summary>
        /// Gets or sets the index of the first worksheet to import from.
        /// </summary>
        [Option(HelpText = "The zero based index of the first worksheet to import.", Default = 0)]
        public int FirstWorksheet { get; set; }

        /// <summary>
        /// Gets or sets the index of the last worksheet to import from.
        /// A value of 0 indicates the index of the last worksheet present in the workbook.
        /// </summary>
        [Option(HelpText = "The zero based index of the last worksheet to import. A value of 0 will select the last worksheet present.", Default = 0)]
        public int LastWorksheet { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the program should run fully automatically
        /// without pausing for user input or feedback.
        /// </summary>
        [Option(Default = false, HelpText = "Runs without pausing and asking for user feedback.")]
        public bool AutoRun { get; set; }
    }
}
