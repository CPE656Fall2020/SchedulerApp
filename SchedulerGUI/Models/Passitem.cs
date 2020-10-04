using System;

namespace SchedulerGUI.Models
{
    // this class may be expanded in the futre
    public class PassItem
    {
        public PassItem(TimeSpan duration)
        {
            this.Duration = duration;
        }

        public TimeSpan Duration { get; set; }
    }
}
