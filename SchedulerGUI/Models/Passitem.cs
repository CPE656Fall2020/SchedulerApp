using System;

namespace SchedulerGUI.Models
{
    // this class may be expanded in the futre
    public class Passitem
    {
        public Passitem(TimeSpan duration)
        {
            this.Duration = duration;
        }

        public TimeSpan Duration { get; set; }
    }
}
