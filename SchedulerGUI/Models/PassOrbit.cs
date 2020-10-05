using System;
using System.Collections.Generic;
using System.Linq;
using SchedulerGUI.Enums;
using SchedulerGUI.Interfaces;

namespace SchedulerGUI.Models
{
    /// <summary>
    /// <see cref="PassOrbit"/> represents an orbit of a satelite and the various phases that comprise that orbit.
    /// </summary>
    public class PassOrbit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PassOrbit"/> class.
        /// </summary>
        /// <param name="name">The pass name.</param>
        /// <param name="startTime">The beginning time of the pass.</param>
        /// <param name="endTime">The ending time of the pass.</param>
        public PassOrbit(string name, DateTime startTime, DateTime endTime)
        {
            this.Name = "Pass #: " + name;
            this.StartTime = startTime;
            this.EndTime = endTime;

            this.InitPhases();
        }

        /// <summary>
        /// Gets the name of the pass.
        /// </summary>
        public string Name { get; } // TODO: Alex - removed set. Add back if needed, if not, keep immutable.

        /// <summary>
        /// Gets or sets the phases of the pass orbit.
        /// </summary>
        public List<IPassPhase> PassPhases { get; set; }

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

        private void InitPhases()
        {
            this.PassPhases = new List<IPassPhase>();
            DateTime startTime = this.StartTime;
            double duration = (this.EndTime - this.StartTime).TotalSeconds / Enum.GetValues(typeof(PhaseType)).Length;

            var enums = Enum.GetValues(typeof(PhaseType)).Cast<PhaseType>().Where(x => x != PhaseType.Encryption);
            foreach (PhaseType phaseType in enums)
            {
                this.PassPhases.Add(new PassPhase(startTime, startTime.AddSeconds(duration), phaseType));
                startTime = startTime.AddSeconds(duration);
            }

            this.PassPhases.Add(new EncryptionPassPhase(startTime, startTime.AddSeconds(duration), PhaseType.Encryption));

            // randomize list order
            Random rnd = new Random();
            this.PassPhases = this.PassPhases.Select(x => new { value = x, order = rnd.Next() })
                            .OrderBy(x => x.order).Select(x => x.value).ToList();
        }
    }
}
