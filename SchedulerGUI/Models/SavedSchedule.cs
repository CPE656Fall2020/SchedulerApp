using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SchedulerDatabase.Models;

namespace SchedulerGUI.Models
{
    /// <summary>
    /// <see cref="SavedSchedule"/> contains the data elements needed to reconstruct an entire simulation.
    /// </summary>
    public class SavedSchedule
    {
        /// <summary>
        /// A collection of pass-orbits to schedule.
        /// </summary>
        public ICollection<PassOrbit> Passes { get; set; }
    }
}
