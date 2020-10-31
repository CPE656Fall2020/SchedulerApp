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

        private string SummarizedAuthor => "CPE656 Authors";

        private string SummarizedDescription => "Summarized data computed with the SchedulerDatabase tool";

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
        /// Gets a listing of all the providers which have test results included in the database.
        /// </summary>
        /// <returns>An <see cref="IQueryable{T}"/> of provider names.</returns>
        public IQueryable<string> GetAllTestedProviders()
        {
            return this.Context.AESProfiles
                .Select(a => a.ProviderName)
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
        /// Gets a listing of the different clock speeds for the available platforms.
        /// </summary>
        /// <returns>An <see cref="IQueryable{T}"/> of clock speeds, in Hz.</returns>
        public IQueryable<int> GetAllClockSpeeds()
        {
            return this.Context.AESProfiles
                .Select(a => a.TestedFrequency)
                .Distinct();
        }

        /// <summary>
        /// Computes summarized results for each unique test case provided in a collection of raw AES profiles.
        /// </summary>
        /// <param name="profiles">A collection of raw <see cref="AESEncyptorProfile"/>.</param>
        /// <returns>A collection of each unique test case present in the input, containing averaged results for each parameter.</returns>
        public List<AESEncyptorProfile> SummarizeDeviceResults(IEnumerable<AESEncyptorProfile> profiles)
        {
            var buckets = this.GroupIntoBuckets(profiles);
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
                    Author = this.SummarizedAuthor,
                    Description = this.SummarizedDescription,
                    PlatformName = bucket.Value.First().PlatformName,
                    PlatformAccelerator = bucket.Value.First().PlatformAccelerator,
                    ProviderName = bucket.Value.First().ProviderName,
                    TestedAESBitLength = bucket.Value.First().TestedAESBitLength,
                    TestedAESMode = bucket.Value.First().TestedAESMode,
                    TestedFrequency = bucket.Value.First().TestedFrequency,
                    NumCores = bucket.Value.First().NumCores,
                    AdditionalUniqueInfo = bucket.Value.First().AdditionalUniqueInfo,

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

        /// <summary>
        /// Computes summarized results for each unique test case provided in a collection of raw compression profiles.
        /// </summary>
        /// <param name="profiles">A collection of raw <see cref="CompressorProfile"/>.</param>
        /// <returns>A collection of each unique test case present in the input, containing averaged results for each parameter.</returns>
        public List<CompressorProfile> SummarizeDeviceResults(IEnumerable<CompressorProfile> profiles)
        {
            var buckets = this.GroupIntoBuckets(profiles);
            var allSummarizedResults = new List<CompressorProfile>();

            // For all related tests in each bucket, compute an average.
            foreach (var bucket in buckets)
            {
                var summation = new CompressorProfile();
                var count = bucket.Value.Count;

                foreach (var result in bucket.Value)
                {
                    summation.AverageCurrent += result.AverageCurrent;
                    summation.AverageVoltage += result.AverageVoltage;
                    summation.TotalTestedByteSize += result.TotalTestedByteSize;
                    summation.TotalTestedEnergyJoules += result.TotalTestedEnergyJoules;
                    summation.TotalTestTime += result.TotalTestTime;
                }

                allSummarizedResults.Add(new CompressorProfile()
                {
                    /* Static description */
                    ProfileId = Guid.NewGuid(),
                    Author = this.SummarizedAuthor,
                    Description = this.SummarizedDescription,
                    PlatformName = bucket.Value.First().PlatformName,
                    TestedCompressionMode = bucket.Value.First().TestedCompressionMode,
                    TestedFrequency = bucket.Value.First().TestedFrequency,
                    NumCores = bucket.Value.First().NumCores,
                    AdditionalUniqueInfo = bucket.Value.First().AdditionalUniqueInfo,

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

        private Dictionary<string, List<T>> GroupIntoBuckets<T>(IEnumerable<T> profiles)
            where T : IProfile
        {
            var buckets = new Dictionary<string, List<T>>();

            // Group all the independent test runs provided into buckets, based on which test runs are done under the same conditions.
            foreach (var profile in profiles)
            {
                var id = profile.ComparisonHashString;

                if (!buckets.ContainsKey(id))
                {
                    buckets.Add(id, new List<T>());
                }

                buckets[id].Add(profile);
            }

            return buckets;
        }
    }
}
