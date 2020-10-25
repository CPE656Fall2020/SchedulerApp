using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using GalaSoft.MvvmLight;
using SchedulerGUI.Interfaces;
using SchedulerGUI.Models;

namespace SchedulerGUI.ViewModels.Controls
{
    /// <summary>
    /// <see cref="EditSolarCellControlViewModel"/> provides a ViewModel for the <see cref="Views.Controls.EditSolarCellControl"/> control.
    /// </summary>
    public class EditSolarCellControlViewModel : ViewModelBase
    {
        private IEnumerable<PassOrbit> passes;

        private SolarPanel solarPanel = new SolarPanel();

        /// <summary>
        /// Gets or sets the pass data that should be used to build the historical display.
        /// </summary>
        public IEnumerable<PassOrbit> Passes
        {
            get => this.passes;
            set
            {
                this.passes = value;
                this.UpdatePassData();
            }
        }

        /// <summary>
        /// Modifies the passes based on the values in the solar panel class.
        /// </summary>
        private void UpdatePassData()
        {
            foreach (PassOrbit pass in passes)
            {
                IPassPhase sunlightPhase = pass.PassPhases[0];
                sunlightPhase.TotalEnergyUsed = -1 * solarPanel.EffectivePowerW * sunlightPhase.Duration.TotalSeconds;
            }
        }

        /// <summary>
        /// Gets or sets the multiplicative derating factor to apply to the capacity.
        /// </summary>
        public int Derating
        {
            get => this.SolarPanel.DeratedPct;
            set
            {
                this.SolarPanel.DeratedPct = value;
                this.UpdatePassData();
                this.RaisePropertyChanged(string.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the current of the solar panel.
        /// </summary>
        public double Current
        {
            get => this.SolarPanel.Current;
            set
            {
                this.SolarPanel.Current = value;
                this.UpdatePassData();
                this.RaisePropertyChanged(string.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the nominal terminal voltage of the battery.
        /// </summary>
        public double Voltage
        {
            get => this.SolarPanel.Voltage;
            set
            {
                this.SolarPanel.Voltage = value;
                this.UpdatePassData();
                this.RaisePropertyChanged(string.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the SolarPannel being modeled.
        /// </summary>
        public SolarPanel SolarPanel
        {
            get => this.solarPanel;
            set => this.Set(() => this.solarPanel, ref this.solarPanel, value);
        }
    }
}