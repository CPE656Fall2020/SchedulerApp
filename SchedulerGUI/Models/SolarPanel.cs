using System;

namespace SchedulerGUI.Models
{
    /// <summary>
    /// <see cref="SolarPanel"/> models an ideal solar panel for satellite power scheduling.
    /// </summary>
    public class SolarPanel
    {
        /// <summary>
        /// Gets or sets the max power voltage output of the panel.
        /// </summary>
        public double Voltage { get; set; } = 5;

        /// <summary>
        /// Gets or sets the max power current output of the panel.
        /// </summary>
        public double Current { get; set; } = 3.7;

        /// <summary>
        /// Gets or sets the multiplicative scale factor used to derate the battery's capacity.
        /// </summary>
        public int DeratedPct { get; set; } = 100;

        /// <summary>
        /// Gets the output power in watts.
        /// </summary>
        public double PowerW => this.Current * this.Voltage;

        /// <summary>
        /// Gets the effective capacity (with derating) of the battery in milliamp-hours.
        /// </summary>
        public double EffectivePowerW => this.PowerW * (this.DeratedPct / 100.0);
    }
}