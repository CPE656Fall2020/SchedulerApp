using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerDatabase.Models
{
    /// <summary>
    /// <see cref="IByteStreamProcessor"/> defines a generic device that performs a specific operation of a stream
    /// of data. Byte Processing Devices have a defined throughput rate and known energy consumption rate, varying according
    /// to the length of the stream being processed. These profiles are uniquely identifiable across the entire scheduling system.
    /// </summary>
    public interface IByteStreamProcessor
    {
         /// <summary>
        /// Gets the unique identifier for this profile.
        /// </summary>
        Guid ProfileId { get; }

        /// <summary>
        /// Gets the amount of energy, in Joules, required to process 1 byte of data.
        /// </summary>
        double JoulesPerByte { get; }

        /// <summary>
        /// Gets the throughput of the encryptor, in bytes per second.
        /// </summary>
        double BytesPerSecond { get; }

        /// <summary>
        /// Gets a complete description string for this device profile.
        /// </summary>
        string FullProfileDescription { get; }

        /// <summary>
        /// Gets a short description string for this class of device profile. This description is suitable
        /// for identifying an entire class of related experiments/profiles, when it is NOT necessary to be
        /// able to precisely identify the exact run(s) that were conducted. See also <seealso cref="ShortProfileSpecificDescription"/>.
        /// </summary>
        string ShortProfileClassDescription { get; }

        /// <summary>
        /// Gets a short description string for this precise experimental run of a device profile. This description includes
        /// metadata from the experiment such as the name of the test engineer, making it too narrow to be able to describe an entire
        /// class of related experiments. For a more general description of a device profile see also <seealso cref="ShortProfileClassDescription"/>.
        /// </summary>
        string ShortProfileSpecificDescription { get; }
    }
}
