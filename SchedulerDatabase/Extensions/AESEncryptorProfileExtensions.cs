using System;
using SchedulerDatabase.Models;

namespace SchedulerDatabase.Extensions
{
    /// <summary>
    /// Defines extension methods for the <see cref="AESEncyptorProfile"/> type.
    /// </summary>
    public static class AESEncryptorProfileExtensions
    {
        /// <summary>
        /// Returns a complete description string for the profile.
        /// </summary>
        /// <param name="profile">The profile to describe.</param>
        /// <returns>The complete description string.</returns>
        public static string ToFullDescription(this AESEncyptorProfile profile)
        {
            var additional = string.Empty;
            if (!string.IsNullOrEmpty(profile.AdditionalUniqueInfo))
            {
                additional = $@"Additional : {profile.AdditionalUniqueInfo}" + "\n";
            }

            return
                $@"Platform: {profile.PlatformName}" + "\n" +
                $@"Accelerator: {profile.PlatformAccelerator.ToFriendlyName()}" + "\n" +
                additional +
                $@"AES Mode: {profile.TestedAESMode}, {profile.TestedAESBitLength}-bit" + "\n" +
                $@"Provider: {profile.ProviderName}" + "\n" +
                $@"Tested Frequency: {profile.TestedFrequency:N0} Hz" + "\n" +
                $@"Description: {profile.Description}";
        }
    }
}
