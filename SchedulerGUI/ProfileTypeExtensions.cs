using System.ComponentModel;
using System.Linq;
using System.Reflection;
using SchedulerGUI.Enums;

namespace SchedulerDatabase.Extensions
{
    /// <summary>
    /// Defines extension methods for the <see cref="ProfileType"/> enum.
    /// </summary>
    public static class ProfileTypeExtensions
    {
        /// <summary>
        /// Returns a user-friendly name for an accelerator type.
        /// </summary>
        /// <param name="profileType">The profile to return a name for.</param>
        /// <returns>A user friendly name.</returns>
        /// <remarks>
        /// Adapted from https://codereview.stackexchange.com/questions/157871/method-that-returns-description-attribute-of-enum-value.</remarks>
        public static string ToFriendlyName(this ProfileType profileType)
        {
            return
                profileType
                    .GetType()
                    .GetMember(profileType.ToString())
                    .FirstOrDefault()
                    ?.GetCustomAttribute<DescriptionAttribute>()
                    ?.Description
                ?? profileType.ToString();
        }
    }
}
