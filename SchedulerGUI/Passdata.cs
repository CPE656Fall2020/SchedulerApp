using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerGUI
{
    public class Passdata
    {
        public DateTime startTime { get; set; } = new DateTime();
        public DateTime endTime { get; set; } = new DateTime();
        public DateTime midTime {
            get
            {
                TimeSpan ts = endTime.Subtract(startTime);
                return startTime.AddMinutes(ts.TotalMinutes / 2);
            }
        } 
        public Passitem mission { get; set; }
        public DateTime missionStart
        {
            get
            {
                return sunlightEnd;
            }
        }
        public DateTime missionEnd
        {
            get
            {
                return sunlightEnd.AddMinutes(mission.Duration.TotalMinutes);
            }
        }
        public Passitem encryption { get; set; }
        public DateTime encryptionStart
        {
            get
            {
                return missionEnd;
            }
        }
        public DateTime encryptionEnd
        {
            get
            {
                return missionEnd.AddMinutes(encryption.Duration.TotalMinutes);
            }
        }
        public Passitem sunlight { get; set; }
        public DateTime sunlightStart
        {
            get
            {
                return startTime;
            }
        }
        public DateTime sunlightEnd
        {
            get
            {
                return startTime.AddMinutes(sunlight.Duration.TotalMinutes);
            }
        }
        public Passitem Datalink { get; set; }
        public DateTime DatalinkStart
        {
            get
            {
                return encryptionEnd;
            }
        }
        public DateTime DatalinkEnd
        {
            get
            {
                return encryptionEnd.AddMinutes(Datalink.Duration.TotalMinutes);
            }
        }
        public string Name { get; set; }

        public Passdata(string name,DateTime starttime,DateTime endtime)
        {
            Name = name;
            startTime = starttime;
            endTime = endtime;
            sunlight = new Passitem(new TimeSpan(0, 45, 0));
            encryption = new Passitem(new TimeSpan(0, 25, 0));
            mission = new Passitem(new TimeSpan(0,30, 0));
            Datalink = new Passitem(new TimeSpan(0, 28, 0));
        }
    }
}
