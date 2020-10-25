using System;

namespace SchedulerGUI.Models
{
    /// <summary>
    /// <see cref="Battery"/> models an ideal battery for satellite power scheduling.
    /// </summary>
    public class Battery
    {
        /// <summary>
        /// Gets or sets the capacity of the battery in milliamp-hours.
        /// </summary>
        public double CapacitymAh { get; set; } = 20000;

        /// <summary>
        /// Gets or sets the nominal terminal voltage of the battery.
        /// </summary>
        public double Voltage { get; set; } = 5.0;

        /// <summary>
        /// Gets or sets the multiplicative scale factor used to derate the battery's capacity.
        /// </summary>
        public int DeratedPct { get; set; } = 100;

        /// <summary>
        /// Gets the max theoretical capacity of the battery in Joules.
        /// </summary>
        public double CapacityJ => this.CapacitymAh * this.Voltage;

        /// <summary>
        /// Gets the effective capacity (with derating) of the battery in milliamp-hours.
        /// </summary>
        public double EffectiveCapacitymAh => this.CapacitymAh * (this.DeratedPct / 100.0);

        /// <summary>
        /// Gets the effective capacity (with derating) of the battery in Joules.
        /// </summary>
        public double EffectiveCapacityJ => this.EffectiveCapacitymAh * this.Voltage;
    }
}
