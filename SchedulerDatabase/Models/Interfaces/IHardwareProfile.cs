namespace SchedulerDatabase.Models
{
    /// <summary>
    /// <see cref="IHardwareProfile"/> describes a generic profile for a component of the scheduling system that is implemented in hardware.
    /// </summary>
    public interface IHardwareProfile : IProfile
    {
        /// <summary>
        /// Gets the name of the hardware platform described by this profile.
        /// </summary>
        string PlatformName { get; }

        /// <summary>
        /// Gets the processor frequency (in Hz) during the test profile when benchmarking this platform.
        /// </summary>
        int TestedFrequency { get; }

        /// <summary>
        /// Gets the average voltage (in Volts) used to power the device during the duration of the test.
        /// </summary>
        double AverageVoltage { get; }

        /// <summary>
        /// Gets the average current (in Amps) consumed by the platform during the duration of the test.
        /// </summary>
        double AverageCurrent { get; }

        /// <summary>
        /// Gets an integer value representing the number of processor cores utilized during the test.
        /// </summary>
        int NumCores { get; }
    }
}
