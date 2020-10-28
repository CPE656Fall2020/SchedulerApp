using System;
using System.Collections.Generic;

namespace SchedulerGUI.Models
{
    /// <summary>
    /// <see cref="SavedSchedule"/> contains the data elements needed to reconstruct an entire simulation.
    /// </summary>
    public class SavedSchedule
    {
        /// <summary>
        /// Gets or sets a collection of pass-orbits to schedule.
        /// </summary>
        public ICollection<PassOrbit> Passes { get; set; }

        /// <summary>
        /// Gets or sets the battery specifications for the satellite.
        /// </summary>
        public Battery Battery { get; set; }

        /// <summary>
        /// Gets or sets the solar specifications for the satellite.
        /// </summary>
        public SolarPanel SolarPanel { get; set; }
    }
}
