using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{

    public class Passitem
    {
        public DateTime startTime { get; set; } = new DateTime();
        public DateTime endTime { get; set; } = new DateTime();
        public Passitem(DateTime starttime, DateTime endtime)
        {
            startTime = starttime;
            endTime = endtime;
        }
    }
}
