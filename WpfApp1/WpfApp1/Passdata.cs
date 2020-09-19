using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class Passdata
    {
        public DateTime startTime { get; set; } = new DateTime();
        public DateTime endTime { get; set; } = new DateTime();
        public Passitem mission { get; set; } 
        public Passitem encryption { get; set; } 
        public Passitem sunlight { get; set; } 
        public Passitem Datalink { get; set; } 
        public string Name { get; set; }

        public Passdata(string name,DateTime starttime,DateTime endtime)
        {
            Name = name;
            startTime = starttime;
            endTime = endtime;
            sunlight = new Passitem(new DateTime(2016, 7, 15, 0, 0, 0), new DateTime(2016, 7, 15, 0, 45, 0));
            encryption = new Passitem(new DateTime(2016, 7, 15, 0, 45, 0), new DateTime(2016, 7, 15, 1, 10, 0));
            mission = new Passitem(new DateTime(2016, 7, 15, 1, 10, 0), new DateTime(2016, 7, 15, 1, 40, 0));
            Datalink = new Passitem(new DateTime(2016, 7, 15, 1, 40, 0), new DateTime(2016, 7, 15, 2, 7, 59));
        }
    }
}
