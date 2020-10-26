using System;
using GalaSoft.MvvmLight;
using SchedulerGUI.Models;

namespace SchedulerGUI.ViewModels.Controls
{
    /// <summary>
    /// <see cref="EditBatteryControlViewModel"/> provides a ViewModel for the <see cref="Views.Controls.EditBatteryControl"/> control.
    /// </summary>
    public class EditBatteryControlViewModel : ViewModelBase
    {
        private const double GramsToPounds = 0.00220462262185;
        private Battery battery = new Battery();

        /// <summary>
        /// Gets or sets the capacity of the battery in milliamp-hours.
        /// </summary>
        public double CapacitymAh
        {
            get => this.Battery.CapacitymAh;
            set
            {
                this.Battery.CapacitymAh = value;
                this.RaisePropertyChanged(nameof(this.CapacitymAh));
                this.RaisePropertyChanged(nameof(this.Battery));
                this.NotifyCalculationsChanged();
            }
        }

        /// <summary>
        /// Gets or sets the nominal terminal voltage of the battery.
        /// </summary>
        public double Voltage
        {
            get => this.Battery.Voltage;
            set
            {
                this.Battery.Voltage = value;
                this.RaisePropertyChanged(nameof(this.Voltage));
                this.RaisePropertyChanged(nameof(this.Battery));
                this.NotifyCalculationsChanged();
            }
        }

        /// <summary>
        /// Gets or sets the multiplicative derating factor to apply to the capacity.
        /// </summary>
        public int Derating
        {
            get => this.Battery.DeratedPct;
            set
            {
                this.Battery.DeratedPct = value;
                this.RaisePropertyChanged(nameof(this.Derating));
                this.RaisePropertyChanged(nameof(this.Battery));
                this.NotifyCalculationsChanged();
            }
        }

        /// <summary>
        /// Gets or sets the battery being modeled.
        /// </summary>
        public Battery Battery
        {
            get => this.battery;
            set
            {
                this.Set(() => this.Battery, ref this.battery, value);

                // Update all calculated parameters
                this.NotifyCalculationsChanged();
            }
        }

        /// <summary>
        /// Gets an estimated weight for this battery cell, in pounds, using Sealed Lead Acid.
        /// </summary>
        public double WeightSLA
        {
            get
            {
                // Based on the table at https://www.powerstream.com/Size_SLA.htm
                // A 3rd degree poly regression was calculated over the 12V SLA battery options
                // on the ratio of pounds to Amp-hours.
                // y = -3E-6x^3 + .0005x2 - 0.0178x + 0.9745
                var x = this.CapacitymAh / 1000.0; // mAh -> Ah
                var ratio = (-3e-6 * Math.Pow(x, 3)) + (0.0005 * Math.Pow(x, 2)) - (0.0178 * x) + 0.9745;

                var weight = ratio * x; // (Pounds/Ah) * Ah == lbs
                return weight;
            }
        }

        /// <summary>
        /// Gets an estimated weight of the battery cell, in pounds, using lithium cells.
        /// </summary>
        public double WeightLiion
        {
            get
            {
                // Based on combinations of 18650 Li-Ion cells to hit the needed capacity.
                // http://dalincom.ru/datasheet/SAMSUNG%20INR18650-25R.pdf
                // Weight = 45 grams
                // 2500mAh.
                var numCellsNeeded = Math.Ceiling(this.CapacitymAh / 2500.0);

                var weight = numCellsNeeded * 45;
                return weight * GramsToPounds;
            }
        }

        /// <summary>
        /// Gets an estimated weight of the battery cell, in pounds, using NiMH cells.
        /// </summary>
        public double WeightNimh
        {
            get
            {
                // Based on combinations of Panasonic BK210AH cells to hit the needed capacity.
                // https://b2b-api.panasonic.eu/file_stream/pids/fileversion/3469
                // Weight = 36 grams
                // 1900mAh.
                var numCellsNeeded = Math.Ceiling(this.CapacitymAh / 1900);

                var weight = numCellsNeeded * 36;
                return weight * GramsToPounds;
            }
        }

        private void NotifyCalculationsChanged()
        {
            this.RaisePropertyChanged(nameof(this.WeightLiion));
            this.RaisePropertyChanged(nameof(this.WeightNimh));
            this.RaisePropertyChanged(nameof(this.WeightSLA));
        }
    }
}
