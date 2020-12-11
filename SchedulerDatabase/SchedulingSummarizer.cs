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
            return this.GetCompressionTestAuthors()
                .Concat(this.GetAESTestAuthors())
                .Distinct();
        }

        /// <summary>
        /// Gets a listing of all the authors who have compression test results included in the database.
        /// </summary>
        /// <returns>An <see cref="IQueryable{T}"/> of author names.</returns>
        public IQueryable<string> GetCompressionTestAuthors()
        {
            return this.Context.CompressorProfiles
                .Select(a => a.Author)
                .Distinct();
        }

        /// <summary>
        /// Gets a listing of all the authors who have AES test results included in the database.
        /// </summary>
        /// <returns>An <see cref="IQueryable{T}"/> of author names.</returns>
        public IQueryable<string> GetAESTestAuthors()
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
            return this.GetAESTestedPlatforms()
                .Concat(this.GetCompressionTestedPlatforms())
                .Distinct();
        }

        /// <summary>
        /// Gets a listing of all the platforms which have AES test results included in the database.
        /// </summary>
        /// <returns>An <see cref="IQueryable{T}"/> of platform names.</returns>
        public IQueryable<string> GetAESTestedPlatforms()
        {
            return this.Context.AESProfiles
                .Select(a => a.PlatformName)
                .Distinct();
        }

        /// <summary>
        /// Gets a listing of all the platforms which have compression test results included in the database.
        /// </summary>
        /// <returns>An <see cref="IQueryable{T}"/> of platform names.</returns>
        public IQueryable<string> GetCompressionTestedPlatforms()
        {
            return this.Context.CompressorProfiles
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
        public IQueryable<AESEncryptorProfile> GetAllResultsForAccelerator(AESEncryptorProfile.AcceleratorType acceleratorType)
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
            return this.GetAESNumCores()
                .Concat(this.GetCompressionNumCores())
                .Distinct();
        }

        /// <summary>
        /// Gets a listing of the different numbers of cores available for the available AES platforms.
        /// </summary>
        /// <returns>An <see cref="IQueryable{T}"/> of platform core counts.</returns>
        public IQueryable<int> GetAESNumCores()
        {
            return this.Context.AESProfiles
                .Select(a => a.NumCores)
                .Distinct();
        }

        /// <summary>
        /// Gets a listing of the different numbers of cores available for the available platforms.
        /// </summary>
        /// <returns>An <see cref="IQueryable{T}"/> of platform core counts.</returns>
        public IQueryable<int> GetCompressionNumCores()
        {
            return this.Context.CompressorProfiles
                .Select(a => a.NumCores)
                .Distinct();
        }

        /// <summary>
        /// Gets a listing of the different clock speeds for the available platforms.
        /// </summary>
        /// <returns>An <see cref="IQueryable{T}"/> of clock speeds, in Hz.</returns>
        public IQueryable<int> GetAllClockSpeeds()
        {
            return this.GetAESClockSpeeds()
                .Concat(this.GetCompressionClockSpeeds())
                .Distinct();
        }

        /// <summary>
        /// Gets a listing of the different clock speeds for the available AES platforms.
        /// </summary>
        /// <returns>An <see cref="IQueryable{T}"/> of clock speeds, in Hz.</returns>
        public IQueryable<int> GetAESClockSpeeds()
        {
            return this.Context.AESProfiles
                .Select(a => a.TestedFrequency)
                .Distinct();
        }

        /// <summary>
        /// Gets a listing of the different clock speeds for the available compression platforms.
        /// </summary>
        /// <returns>An <see cref="IQueryable{T}"/> of clock speeds, in Hz.</returns>
        public IQueryable<int> GetCompressionClockSpeeds()
        {
            return this.Context.CompressorProfiles
                .Select(a => a.TestedFrequency)
                .Distinct();
        }

        /// <summary>
        /// Computes summarized results for each unique test case provided in a collection of raw AES profiles.
        /// </summary>
        /// <param name="profiles">A collection of raw <see cref="AESEncryptorProfile"/>.</param>
        /// <returns>A collection of each unique test case present in the input, containing averaged results for each parameter.</returns>
        public List<AESEncryptorProfile> SummarizeDeviceResults(IEnumerable<AESEncryptorProfile> profiles)
        {
            var buckets = this.GroupIntoBuckets(profiles);
            var allSummarizedResults = new List<AESEncryptorProfile>();

            // For all related tests in each bucket, compute an average.
            foreach (var bucket in buckets)
            {
                var count = bucket.Value.Count;
                var summation = this.CreateSummation(bucket.Value, new AESEncryptorProfile());

                /* Static description */
                var result = new AESEncryptorProfile()
                {
                    AdditionalUniqueInfo = bucket.Value.First().AdditionalUniqueInfo,
                    PlatformAccelerator = bucket.Value.First().PlatformAccelerator,
                    ProviderName = bucket.Value.First().ProviderName,
                    TestedAESBitLength = bucket.Value.First().TestedAESBitLength,
                    TestedAESMode = bucket.Value.First().TestedAESMode,
                };
                result = (AESEncryptorProfile)this.GenerateResult(bucket.Value.First(), result);

                /* Averaged results */
                result = (AESEncryptorProfile)this.AddSummationToResult(summation, result, count);

                allSummarizedResults.Add(result);
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
                var count = bucket.Value.Count;
                var summation = this.CreateSummation(bucket.Value, new CompressorProfile());

                /* Static description */
                var result = new CompressorProfile()
                {
                    TestedCompressionMode = bucket.Value.First().TestedCompressionMode,
                };
                result = (CompressorProfile)this.GenerateResult(bucket.Value.First(), result);

                /* Averaged results */
                result = (CompressorProfile)this.AddSummationToResult(summation, result, count);

                allSummarizedResults.Add(result);
            }

            return allSummarizedResults;
        }

        private IByteStreamProcessor GenerateResult(IByteStreamProcessor bucketValue, IByteStreamProcessor result)
        {
            result.ProfileId = Guid.NewGuid();
            result.Author = this.SummarizedAuthor;
            result.Description = this.SummarizedDescription;
            result.PlatformName = bucketValue.PlatformName;
            result.TestedFrequency = bucketValue.TestedFrequency;
            result.NumCores = bucketValue.NumCores;
            result.AdditionalUniqueInfo = bucketValue.AdditionalUniqueInfo;

            return result;
        }

        private IByteStreamProcessor AddSummationToResult(IByteStreamProcessor summation, IByteStreamProcessor result, int count)
        {
            result.AverageCurrent = summation.AverageCurrent / count;
            result.AverageVoltage = summation.AverageVoltage / count;
            result.TotalTestedByteSize = summation.TotalTestedByteSize / count;
            result.TotalTestedEnergyJoules = summation.TotalTestedEnergyJoules / count;
            result.TotalTestTime = TimeSpan.FromTicks(summation.TotalTestTime.Ticks / count);

            return result;
        }

        private IByteStreamProcessor CreateSummation<T>(List<T> devices, IByteStreamProcessor summation)
            where T : IByteStreamProcessor
        {
            foreach (var device in devices)
            {
                summation.AverageCurrent += device.AverageCurrent;
                summation.AverageVoltage += device.AverageVoltage;
                summation.TotalTestedByteSize += device.TotalTestedByteSize;
                summation.TotalTestedEnergyJoules += device.TotalTestedEnergyJoules;
                summation.TotalTestTime += device.TotalTestTime;
            }

            return summation;
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
