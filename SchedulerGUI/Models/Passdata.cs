using System;

namespace SchedulerGUI.Models
{
    /// <summary>
    /// <see cref="PassData"/> represents an orbit of a satelite and the various phases that comprise that orbit.
    /// </summary>
    public class PassData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PassData"/> class.
        /// </summary>
        /// <param name="name">The pass name.</param>
        /// <param name="startTime">The beginning time of the pass.</param>
        /// <param name="endTime">The ending time of the pass.</param>
        public PassData(string name, DateTime startTime, DateTime endTime)
        {
            this.Name = name;
            this.StartTime = startTime;
            this.EndTime = endTime;

            this.Sunlight = new PassItem(new TimeSpan(0, 45, 0));
            this.Encryption = new PassItem(new TimeSpan(0, 25, 0));
            this.Mission = new PassItem(new TimeSpan(0, 30, 0));
            this.Datalink = new PassItem(new TimeSpan(0, 28, 0));
        }

        /// <summary>
        /// Gets the name of the pass.
        /// </summary>
        public string Name { get; } // TODO: Alex - removed set. Add back if needed, if not, keep immutable.

        /// <summary>
        /// Gets the starting time for this pass.
        /// </summary>
        public DateTime StartTime { get; } = default; // TODO: Alex - removed set. Add back if needed, if not, keep immutable.

        /// <summary>
        /// Gets the ending time for this pass.
        /// </summary>
        public DateTime EndTime { get; } = default; // TODO: Alex - removed set. Add back if needed, if not, keep immutable.

        /// <summary>
        /// Gets the midpoint time when this pass is half completed.
        /// </summary>
        public DateTime MidTime
        {
            get
            {
                TimeSpan ts = this.EndTime.Subtract(this.StartTime);
                return this.StartTime.AddMinutes(ts.TotalMinutes / 2);
            }
        }

        /// <summary>
        /// Gets or sets parameters about the mission phase of the pass.
        /// </summary>
        public PassItem Mission { get; set; }

        /// <summary>
        /// Gets the time when the mission phase begins.
        /// </summary>
        public DateTime MissionStart => this.SunlightEnd;

        /// <summary>
        /// Gets the time when the mission phase ends.
        /// </summary>
        public DateTime MissionEnd => this.SunlightEnd.AddMinutes(this.Mission.Duration.TotalMinutes);

        /// <summary>
        /// Gets or sets parameters about the encryption phase of the pass.
        /// </summary>
        public PassItem Encryption { get; set; }

        /// <summary>
        /// Gets the time when the encryption phase begins.
        /// </summary>
        public DateTime EncryptionStart => this.MissionEnd;

        /// <summary>
        /// Gets the time when the encryption phase ends.
        /// </summary>
        public DateTime EncryptionEnd => this.MissionEnd.AddMinutes(this.Encryption.Duration.TotalMinutes);

        /// <summary>
        /// Gets or sets parameters about the sunlight phase of the pass.
        /// </summary>
        public PassItem Sunlight { get; set; }

        /// <summary>
        /// Gets the time when the sunlight phase begins.
        /// </summary>
        public DateTime SunlightStart => this.StartTime;

        /// <summary>
        /// Gets the time when the sunlight phase ends.
        /// </summary>
        public DateTime SunlightEnd => this.StartTime.AddMinutes(this.Sunlight.Duration.TotalMinutes);

        /// <summary>
        /// Gets or sets parameters about the datalink phase of the pass.
        /// </summary>
        public PassItem Datalink { get; set; }

        /// <summary>
        /// Gets the time when the datalink phase begins.
        /// </summary>
        public DateTime DatalinkStart => this.EncryptionEnd;

        /// <summary>
        /// Gets the time when the datalink phase ends.
        /// </summary>
        public DateTime DatalinkEnd => this.EncryptionEnd.AddMinutes(this.Datalink.Duration.TotalMinutes);
    }
}
