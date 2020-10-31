using System;

namespace SchedulerDatabase.Models
{
    /// <summary>
    /// <see cref="IHardwareProfile"/> describes a generic profile for a component of the scheduling system that is implemented in hardware.
    /// </summary>
    public interface IHardwareProfile : IProfile
    {
        /// <summary>
        /// Gets or sets the name of the hardware platform described by this profile.
        /// </summary>
        string PlatformName { get; set; }

        /// <summary>
        /// Gets or sets the processor frequency (in Hz) during the test profile when benchmarking this platform.
        /// </summary>
        int TestedFrequency { get; set; }

        /// <summary>
        /// Gets or sets the average voltage (in Volts) used to power the device during the duration of the test.
        /// </summary>
        double AverageVoltage { get; set; }

        /// <summary>
        /// Gets or sets the average current (in Amps) consumed by the platform during the duration of the test.
        /// </summary>
        double AverageCurrent { get; set; }

        /// <summary>
        /// Gets or sets an integer value representing the number of processor cores utilized during the test.
        /// </summary>
        int NumCores { get; set;  }

        /// <summary>
        /// Gets or sets the total number of bytes that were modified when profiling this platform.
        /// </summary>
        long TotalTestedByteSize { get; set; }

        /// <summary>
        /// Gets or sets the total number of joules that were consumed when profiling this platform.
        /// </summary>
        double TotalTestedEnergyJoules { get; set; }

        /// <summary>
        /// Gets or sets the total runtime of the this profile when benchmarking this platform.
        /// </summary>
        TimeSpan TotalTestTime { get; set; }
    }
}
