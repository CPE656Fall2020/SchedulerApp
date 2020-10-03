using System;

namespace SchedulerGUI.Models
{
    public class Passdata
    {
        public Passdata(string name, DateTime starttime, DateTime endtime)
        {
            this.Name = name;
            this.StartTime = starttime;
            this.EndTime = endtime;
            this.Sunlight = new Passitem(new TimeSpan(0, 45, 0));
            this.Encryption = new Passitem(new TimeSpan(0, 25, 0));
            this.Mission = new Passitem(new TimeSpan(0, 30, 0));
            this.Datalink = new Passitem(new TimeSpan(0, 28, 0));
        }

        public DateTime StartTime { get; set; } = default;

        public DateTime EndTime { get; set; } = default;

        public DateTime MidTime
        {
            get
            {
                TimeSpan ts = this.EndTime.Subtract(this.StartTime);
                return this.StartTime.AddMinutes(ts.TotalMinutes / 2);
            }
        }

        public Passitem Mission { get; set; }

        public DateTime MissionStart
        {
            get => this.SunlightEnd;
        }

        public DateTime MissionEnd
        {
            get => this.SunlightEnd.AddMinutes(this.Mission.Duration.TotalMinutes);
        }

        public Passitem Encryption { get; set; }

        public DateTime EncryptionStart
        {
            get => this.MissionEnd;
        }

        public DateTime EncryptionEnd
        {
            get => this.MissionEnd.AddMinutes(this.Encryption.Duration.TotalMinutes);
        }

        public Passitem Sunlight { get; set; }

        public DateTime SunlightStart
        {
            get => this.StartTime;
        }

        public DateTime SunlightEnd
        {
            get => this.StartTime.AddMinutes(this.Sunlight.Duration.TotalMinutes);
        }

        public Passitem Datalink { get; set; }

        public DateTime DatalinkStart
        {
            get => this.EncryptionEnd;
        }

        public DateTime DatalinkEnd => this.EncryptionEnd.AddMinutes(this.Datalink.Duration.TotalMinutes);

        public string Name { get; set; }
    }
}
