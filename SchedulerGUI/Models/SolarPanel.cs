using System;

namespace SchedulerGUI.Models
{
    /// <summary>
    /// <see cref="SolarPanel"/> models an ideal solar panel for satellite power scheduling.
    /// </summary>
    public class SolarPanel
    {
        /// <summary>
        /// Gets or sets the name of the panel.
        /// </summary>
        public string Name { get; set; } = "Solar Panel";

        /// <summary>
        /// Gets or sets the max power voltage output of the panel.
        /// </summary>
        public double Voltage { get; set; } = 5;

        /// <summary>
        /// Gets or sets the max power current output of the panel.
        /// </summary>
        public double Current { get; set; } = 3.7;

        /// <summary>
        /// Gets or sets the multiplicative scale factor used to derate the solar panel's power output.
        /// </summary>
        public int DeratedPct { get; set; } = 75;

        /// <summary>
        /// Gets the output power in watts.
        /// </summary>
        public double PowerW => this.Current * this.Voltage;

        /// <summary>
        /// Gets the effective power output (with derating) of the solar panel in watts.
        /// </summary>
        public double EffectivePowerW => this.PowerW * (this.DeratedPct / 100.0);
    }
}