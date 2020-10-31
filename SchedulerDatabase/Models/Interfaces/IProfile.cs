using System;

namespace SchedulerDatabase.Models
{
    /// <summary>
    /// <see cref="IProfile"/> describes a generic profile for a component of the scheduling system.
    /// </summary>
    public interface IProfile
    {
        /// <summary>
        /// Gets or sets the unique identifier for this profile.
        /// </summary>
        Guid ProfileId { get; set; }

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

        /// <summary>
        /// Gets a hash string that can be used to compare different <see cref="IProfile"/>s
        /// to determine if they both describe the same device, setup, experiment, and intentions.
        /// Equivalent profiles may be averaged or interpolated together to accomidate for incomplete data sets.
        /// </summary>
        /// <remarks>
        /// Formatting is not relevant here, this value will never be displayed to end users.
        /// However, it should be treated as a hash value. Identical profiles must always
        /// produce the same hash, and the likelihood of collisions (identical hashes being produced by
        /// differing profiles) must be minimized.
        /// </remarks>
        string ComparisonHashString { get; }
    }
}
