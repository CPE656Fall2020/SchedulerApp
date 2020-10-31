using System.ComponentModel;
using System.Linq;
using System.Reflection;
using static SchedulerDatabase.Models.AESEncryptorProfile;

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
        /// <remarks>
        /// Adapted from https://codereview.stackexchange.com/questions/157871/method-that-returns-description-attribute-of-enum-value.</remarks>
        public static string ToFriendlyName(this AcceleratorType acceleratorType)
        {
            return
                acceleratorType
                    .GetType()
                    .GetMember(acceleratorType.ToString())
                    .FirstOrDefault()
                    ?.GetCustomAttribute<DescriptionAttribute>()
                    ?.Description
                ?? acceleratorType.ToString();
        }
    }
}
