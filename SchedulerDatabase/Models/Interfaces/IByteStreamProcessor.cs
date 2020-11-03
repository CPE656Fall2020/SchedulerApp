namespace SchedulerDatabase.Models
{
    /// <summary>
    /// <see cref="IByteStreamProcessor"/> defines a generic device that performs a specific operation of a stream
    /// of data. Byte Processing Devices have a defined throughput rate and known energy consumption rate, varying according
    /// to the length of the stream being processed. These profiles are uniquely identifiable across the entire scheduling system.
    /// </summary>
    public interface IByteStreamProcessor : IExperimentalHardwareProfile
    {
        /// <summary>
        /// Gets the amount of energy, in Joules, required to process 1 byte of data.
        /// </summary>
        double JoulesPerByte { get; }

        /// <summary>
        /// Gets the throughput of the encryptor, in bytes per second.
        /// </summary>
        double BytesPerSecond { get; }
    }
}
