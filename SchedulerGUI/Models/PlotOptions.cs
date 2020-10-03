namespace SchedulerGUI.Models
{
    /// <summary>
    /// <see cref="PlotOption"/> describes the modes for the AES performance plots that can be generated.
    /// </summary>
    public enum PlotOption
    {
        /// <summary>
        /// All input data is grouped into like categories and summarized to produce a single result per experiment.
        /// </summary>
        Summarized,

        /// <summary>
        /// All raw data points are displayed with no processing applied.
        /// </summary>
        Raw,
    }
}
