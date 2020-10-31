using System;
using System.ComponentModel;
using SchedulerDatabase.Extensions;
using SchedulerDatabase.Helpers;

namespace SchedulerDatabase.Models
{
    /// <summary>
    /// <see cref="AESEncyptorProfile"/> describes the characteristics of a computing module and methodology for performing AES encryption.
    /// </summary>
    public class AESEncyptorProfile : IByteStreamProcessor
    {
        /// <summary>
        /// <see cref="AESMode"/> defines the different modes in which the AES encryption can be performed.
        /// </summary>
        public enum AESMode
        {
            /// <summary>
            /// Counter Mode.
            /// </summary>
            CTR,

            /// <summary>
            /// Chained Block Cipher Mode.
            /// </summary>
            CBC,

            /// <summary>
            /// Embedded Codeblock Mode.
            /// </summary>
            ECB,
        }

        /// <summary>
        /// <see cref="AcceleratorType"/> defines different types of AES accelerators that can be used.
        /// </summary>
        public enum AcceleratorType
        {
            /// <summary>
            /// No accelerator is used.
            /// </summary>
            [Description("SW-Only")]
            None,

            /// <summary>
            /// CPU-provided Hardware acceleration is used.
            /// </summary>
            [Description("Hardware")]
            CpuHardware,
        }

        /// <inheritdoc/>
        public Guid ProfileId { get; set; }

        /// <inheritdoc/>
        public string Description { get; set; }

        /// <inheritdoc/>
        public string Author { get; set; }

        /// <inheritdoc/>
        public string PlatformName { get; set; }

        /// <summary>
        /// Gets or sets the name of the AES implementation provider.
        /// For software implementations, this likely represents the library name.
        /// For hardware-accelerated implementations, this value is typically not used.
        /// </summary>
        public string ProviderName { get; set; }

        /// <inheritdoc/>
        public string AdditionalUniqueInfo { get; set; }

        /// <summary>
        ///  Gets or sets the type of AES Accelerator that is used on this platform.
        /// </summary>
        public AcceleratorType PlatformAccelerator { get; set; }

        /// <summary>
        /// Gets or sets the AES encryption mode that was tested under this profile.
        /// </summary>
        public AESMode TestedAESMode { get; set; }

        /// <summary>
        /// Gets or sets the length of the AES encryption key (in bits) that was tested under this profile.
        /// </summary>
        public int TestedAESBitLength { get; set; }

        /// <summary>
        /// Gets or sets the total number of bytes that were encrypted when profiling this platform.
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
                    $@"Accelerator: {this.PlatformAccelerator.ToFriendlyName()}" + "\n" +
                    additional +
                    $@"AES Mode: {this.TestedAESMode}, {this.TestedAESBitLength}-bit" + "\n" +
                    $@"Provider: {this.ProviderName}" + "\n" +
                    $@"Tested Frequency: {this.TestedFrequency:N0} Hz" + "\n" +
                    $@"Description: {this.Description}";
            }
        }

        /// <inheritdoc/>
        public string ShortProfileClassDescription =>
            $"{this.PlatformAccelerator.ToFriendlyName()}, {this.AdditionalUniqueInfo} {MetricUtils.HzToString(this.TestedFrequency)} {this.NumCores} Cores, {this.ProviderName}";

        /// <inheritdoc/>
        public string ShortProfileSpecificDescription =>
            $"{this.PlatformName} {this.ProviderName} {this.AdditionalUniqueInfo}\n{MetricUtils.HzToString(this.TestedFrequency)} {this.NumCores} core(s) {this.Author}";

        /// <inheritdoc/>
        public string ComparisonHashString =>
            $"{this.PlatformName}{this.PlatformAccelerator}{this.ProviderName}{this.TestedAESBitLength}{this.TestedAESMode}{this.TestedFrequency}{this.NumCores}{this.AdditionalUniqueInfo}";
    }
}
