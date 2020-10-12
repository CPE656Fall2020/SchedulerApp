using static SchedulerDatabase.Models.AESEncyptorProfile;

namespace SchedulerDatabase.Extensions
{
    /// <summary>
    /// Defines extension methods for the <see cref="AcceleratorType"/> enum.
    /// </summary>
    public static class AcceleratorTypeExtensions
    {
        /// <summary>
        /// Returns a user-friendly name for an accelerator type.
        /// </summary>
        /// <param name="acceleratorType">The accelerator to return a name for.</param>
        /// <returns>A user friendly name.</returns>
        public static string ToFriendlyName(this AcceleratorType acceleratorType)
        {
            switch (acceleratorType)
            {
                case AcceleratorType.None:
                    return "SW-Only";

                case AcceleratorType.CpuHardware:
                    return "Hardware";

                default:
                    return "Unknown";
            }
        }
    }
}
