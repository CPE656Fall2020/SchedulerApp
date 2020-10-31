using System;
using SchedulerDatabase.Helpers;

namespace SchedulerDatabase.Models
{
    /// <summary>
    /// <see cref="CompressorProfile"/>  describes the characteristics of a computing module and methodology for performing data compression.
    /// </summary>
    public class CompressorProfile : IByteStreamProcessor
    {
        /// <summary>
        /// Different compression modes that can applied to data.
        /// </summary>
        public enum CompressionMode
        {
            /// <summary>
            /// LZ4 Algorithm.
            /// </summary>
            Lz4,
        }

        /// <inheritdoc/>
        public Guid ProfileId { get; set; }

        /// <inheritdoc/>
        public string Description { get; set; }

        /// <inheritdoc/>
        public string Author { get; set; }

        /// <inheritdoc/>
        public string PlatformName { get; set; }

        /// <inheritdoc/>
        public string AdditionalUniqueInfo { get; set; }

        /// <summary>
        /// Gets or sets the AES encryption mode that was tested under this profile.
        /// </summary>
        public CompressionMode TestedCompressionMode { get; set; }

        /// <summary>
        /// Gets or sets the total number of bytes that were compressed when profiling this platform.
        /// </summary>
        public long TotalTestedByteSize { get; set; }

        /// <summary>
        /// Gets or sets the total number of joules that were consumed when profiling this platform.
        /// </summary>
        public double TotalTestedEnergyJoules { get; set; }

        /// <summary>
        /// Gets or sets the total runtime of the this profile when benchmarking this platform.
        /// </summary>
        public TimeSpan TotalTestTime { get; set; }

        /// <inheritdoc/>
        public int TestedFrequency { get; set; }

        /// <inheritdoc/>
        public double AverageVoltage { get; set; }

        /// <inheritdoc/>
        public double AverageCurrent { get; set; }

        /// <inheritdoc/>
        public int NumCores { get; set; }

        /// <inheritdoc/>
        public double JoulesPerByte => this.TotalTestedEnergyJoules / this.TotalTestedByteSize;

        /// <inheritdoc/>
        public double BytesPerSecond => this.TotalTestedByteSize / this.TotalTestTime.TotalSeconds;

        /// <inheritdoc/>
        public string FullProfileDescription
        {
            get
            {
                var additional = string.Empty;
                if (!string.IsNullOrEmpty(this.AdditionalUniqueInfo))
                {
                    additional = $@"Additional : {this.AdditionalUniqueInfo}" + "\n";
                }

                return
                    $@"Platform: {this.PlatformName}" + "\n" +
                    additional +
                    $@"Compression Mode: {this.TestedCompressionMode}" + "\n" +
                    $@"Tested Frequency: {this.TestedFrequency:N0} Hz" + "\n" +
                    $@"Description: {this.Description}";
            }
        }

        /// <inheritdoc/>
        public string ShortProfileClassDescription =>
            $"{this.TestedCompressionMode} - {this.AdditionalUniqueInfo} {MetricUtils.HzToString(this.TestedFrequency)} {this.NumCores} Cores";

        /// <inheritdoc/>
        public string ShortProfileSpecificDescription =>
            $"{this.PlatformName} {this.TestedCompressionMode} {this.AdditionalUniqueInfo}\n{MetricUtils.HzToString(this.TestedFrequency)} {this.NumCores} core(s) {this.Author}";

        /// <inheritdoc/>
        public string ComparisonHashString =>
            $"{this.PlatformName}{this.TestedCompressionMode}{this.TestedFrequency}{this.NumCores}{this.AdditionalUniqueInfo}";
    }
}
