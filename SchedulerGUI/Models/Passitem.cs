using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerGUI.Models
{
    //this class may be expanded in the futre
    public class Passitem
    {
       public TimeSpan Duration
        {
            get;
            set;
        }
        public Passitem(TimeSpan duration)
        {
            this.Duration = duration;
        }
    }
}
