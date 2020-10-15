using System;
using System.Collections.Generic;
using System.Text;
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
            return
                $@"Platform: {profile.PlatformName}" + "\n" +
                $@"Accelerator: {profile.PlatformAccelerator.ToFriendlyName()}" + "\n" +
                $@"AES Mode: {profile.TestedAESMode}, {profile.TestedAESBitLength}-bit" + "\n" +
                $@"Provider: {profile.ProviderName}" + "\n" +
                $@"Tested Frequency: {profile.TestedFrequency:N0} Hz" + "\n" +
                $@"Description: {profile.Description}";
        }
    }
}
