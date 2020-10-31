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

        /// <summary>
        /// Gets or sets the unique identifier for this profile.
        /// </summary>
        public Guid ProfileId { get; set; }

        /// <summary>
        /// Gets or sets a free-form description of the conditions under which this experiment was performed or any other relevant details.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the name of the person who authored this entry.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Gets or sets the name of the computing platform or module.
        /// </summary>
        public string PlatformName { get; set; }

        /// <summary>
        /// Gets or sets the name of the AES implementation provider.
        /// For software implementations, this likely represents the library name.
        /// For hardware-accelerated implementations, this value is typically not used.
        /// </summary>
        public string ProviderName { get; set; }

        /// <summary>
        /// Gets or sets any additional information that is required to uniquely identify this profile.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the processor frequency (in Hz) during the test profile when benchmarking this platform.
        /// </summary>
        public int TestedFrequency { get; set; }

        /// <summary>
        /// Gets or sets the average voltage (in Volts) used to power the device during the duration of the test.
        /// </summary>
        public double AverageVoltage { get; set; }

        /// <summary>
        /// Gets or sets the average current (in Amps) consumed by the platform during the duration of the test.
        /// </summary>
        public double AverageCurrent { get; set; }

        /// <summary>
        /// Gets or sets an integer value representing the number of processor cores utilized during the test.
        /// </summary>
        public int NumCores { get; set; }

        /// <summary>
        /// Gets the amount of energy, in Joules, required to encrypt 1 byte of data.
        /// </summary>
        public double JoulesPerByte => this.TotalTestedEnergyJoules / this.TotalTestedByteSize;

        /// <summary>
        /// Gets the throughput of the encryptor, in bytes per second.
        /// </summary>
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
    }
}
