using System;
using System.Collections.Generic;
using System.Linq;
using SchedulerDatabase.Models;

namespace SchedulerDatabase
{
    /// <summary>
    /// <see cref="SchedulingSummarizer"/> provides support for retreiving summarized test results from an underlying database
    /// accessed through a <see cref="SchedulerContext"/>.
    /// </summary>
    public class SchedulingSummarizer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulingSummarizer"/> class.
        /// </summary>
        /// <param name="context">The database context to use.</param>
        public SchedulingSummarizer(SchedulerContext context)
        {
            this.Context = context;
        }

        private SchedulerContext Context { get; }

        /// <summary>
        /// Gets a listing of all the authors who have test results included in the database.
        /// </summary>
        /// <returns>An <see cref="IQueryable{T}"/> of author names.</returns>
        public IQueryable<string> GetAllTestAuthors()
        {
            return this.Context.AESProfiles
                .Select(a => a.Author)
                .Distinct();
        }

        /// <summary>
        /// Gets a listing of all the platforms which have test results included in the database.
        /// </summary>
        /// <returns>An <see cref="IQueryable{T}"/> of platform names.</returns>
        public IQueryable<string> GetAllTestedPlatforms()
        {
            return this.Context.AESProfiles
                .Select(a => a.PlatformName)
                .Distinct();
        }

        /// <summary>
        /// Gets a listing of test profiles for all experiments conducted using a specific accelerator device.
        /// </summary>
        /// <param name="acceleratorType">The specific accelerator type to query for.</param>
        /// <returns>An <see cref="IQueryable{T}"/> of test results.</returns>
        public IQueryable<AESEncyptorProfile> GetAllResultsForAccelerator(AESEncyptorProfile.AcceleratorType acceleratorType)
        {
            return this.Context.AESProfiles
                .Where(a => a.PlatformAccelerator == acceleratorType);
        }

        /// <summary>
        /// Gets a listing of the different numbers of cores available for the available platforms.
        /// </summary>
        /// <returns>An <see cref="IQueryable{T}"/> of platform core counts.</returns>
        public IQueryable<int> GetAllNumCores()
        {
            return this.Context.AESProfiles
                .Select(a => a.NumCores)
                .Distinct();
        }

        /// <summary>
        /// Computes summarized results for each unique test case provided in a collection of raw profiles.
        /// </summary>
        /// <param name="profiles">A collection of raw <see cref="AESEncyptorProfile"/>.</param>
        /// <returns>A collection of each unique test case present in the input, containing averaged results for each parameter.</returns>
        public List<AESEncyptorProfile> SummarizeResults(IEnumerable<AESEncyptorProfile> profiles)
        {
            var buckets = new Dictionary<string, List<AESEncyptorProfile>>();

            // Group all the independent test runs provided into buckets, based on which test runs are done under the same conditions.
            foreach (var profile in profiles)
            {
                // For two profiles to be considered different iterations of the same test case, they have to match on the following criteria:
                var id = $"{profile.PlatformName}{profile.PlatformAccelerator}{profile.ProviderName}{profile.TestedAESBitLength}{profile.TestedAESMode}{profile.TestedFrequency}{profile.NumCores}";

                if (!buckets.ContainsKey(id))
                {
                    buckets.Add(id, new List<AESEncyptorProfile>());
                }

                buckets[id].Add(profile);
            }

            var allSummarizedResults = new List<AESEncyptorProfile>();

            // For all related tests in each bucket, compute an average.
            foreach (var bucket in buckets)
            {
                var summation = new AESEncyptorProfile();
                var count = bucket.Value.Count;

                foreach (var result in bucket.Value)
                {
                    summation.AverageCurrent += result.AverageCurrent;
                    summation.AverageVoltage += result.AverageVoltage;
                    summation.TotalTestedByteSize += result.TotalTestedByteSize;
                    summation.TotalTestedEnergyJoules += result.TotalTestedEnergyJoules;
                    summation.TotalTestTime += result.TotalTestTime;
                }

                allSummarizedResults.Add(new AESEncyptorProfile()
                {
                    /* Static description */
                    ProfileId = Guid.NewGuid(),
                    Author = "CPE656 Authors",
                    Description = "Summarized data computed with the SchedulerDatabase tool",
                    PlatformName = bucket.Value.First().PlatformName,
                    PlatformAccelerator = bucket.Value.First().PlatformAccelerator,
                    ProviderName = bucket.Value.First().ProviderName,
                    TestedAESBitLength = bucket.Value.First().TestedAESBitLength,
                    TestedAESMode = bucket.Value.First().TestedAESMode,
                    TestedFrequency = bucket.Value.First().TestedFrequency,
                    NumCores = bucket.Value.First().NumCores,

                    /* Averaged results */
                    AverageCurrent = summation.AverageCurrent / count,
                    AverageVoltage = summation.AverageVoltage / count,
                    TotalTestedByteSize = summation.TotalTestedByteSize / count,
                    TotalTestedEnergyJoules = summation.TotalTestedEnergyJoules / count,
                    TotalTestTime = TimeSpan.FromTicks(summation.TotalTestTime.Ticks / count),
                });
            }

            return allSummarizedResults;
        }
    }
}
