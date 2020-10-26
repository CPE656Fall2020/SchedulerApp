using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        private ObservableCollection<PassOrbit> passes;

        private SolarPanel solarPanel = new SolarPanel();

        private string selectedExamplePanelName = string.Empty;

        private ObservableCollection<SolarPanel> examplePanels = new ObservableCollection<SolarPanel>
        {
           new SolarPanel { Voltage = 16, Current = 6.2, Name = "100 WATT SOLAR PANEL" },
           new SolarPanel { Voltage = 16, Current = 6.2, DeratedPct = 85, Name = "100 WATT SOLAR PANEL (85% efficient)" },
           new SolarPanel { Voltage = 18.2, Current = 3.1, Name = "60 WATT SOLAR PANEL" },
           new SolarPanel { Voltage = 18.2, Current = 3.1, DeratedPct = 90, Name = "60 WATT SOLAR PANEL (90% efficient)" },
           new SolarPanel { Voltage = 18, Current = 0.225, Name = "18.0V 225MA SOLAR CELL" },
           new SolarPanel { Voltage = 18, Current = 0.225, DeratedPct = 95, Name = "18.0V 225MA SOLAR CELL (95% efficient)" },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="EditSolarCellControlViewModel"/> class.
        /// </summary>
        /// <param name="passes"> A reference to the list of passes in the main view model</param>
        public EditSolarCellControlViewModel(ObservableCollection<PassOrbit> passes)
        {
            this.passes = passes;
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
                this.RaisePropertyChanged("SolarPanel");
                this.UpdatePassData();
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
                this.RaisePropertyChanged("SolarPanel");
                this.UpdatePassData();
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
                this.RaisePropertyChanged("SolarPanel");
                this.UpdatePassData();
            }
        }

        /// <summary>
        /// Gets or sets the SolarPannel being modeled.
        /// </summary>
        public SolarPanel SolarPanel
        {
            get => this.solarPanel;
            set
            {
                this.Set(() => this.SolarPanel, ref this.solarPanel, value);
                this.RaisePropertyChanged("SolarPanel");
                this.UpdatePassData();
            }
        }

        /// <summary>
        /// Gets or sets the pass data that should be used to build the historical display.
        /// </summary>
        public IEnumerable<PassOrbit> Passes
        {
            get => (IEnumerable<PassOrbit>)this.passes;
            set
            {
                this.passes = (ObservableCollection<PassOrbit>)value;
                this.UpdatePassData();
            }
        }

        /// <summary>
        /// Gets or sets the example panels to be displayed in the solar panel parameters panel.
        /// </summary>
        public IEnumerable<SolarPanel> ExamplePanels
        {
            get => this.examplePanels;
            set
            {
                this.examplePanels = (ObservableCollection<SolarPanel>)value;
            }
        }

        /// <summary>
        /// Gets or sets the selected example panel name.
        /// </summary>
        public string SelectedExamplePanelName
        {
            get => this.selectedExamplePanelName;
            set
            {
                if (value != this.selectedExamplePanelName)
                {
                    this.selectedExamplePanelName = value;
                    var examplePanel = this.examplePanels.Single(i => i.Name == this.selectedExamplePanelName);
                    this.SolarPanel = examplePanel;
                }
            }
        }

        /// <summary>
        /// Modifies the passes based on the values in the solar panel class.
        /// </summary>
        private void UpdatePassData()
        {
            foreach (PassOrbit pass in this.passes)
            {
                IPassPhase sunlightPhase = pass.PassPhases[0];
                sunlightPhase.TotalEnergyUsed = -1 * this.solarPanel.EffectivePowerW * sunlightPhase.Duration.TotalSeconds;
            }

            this.RaisePropertyChanged("Passes");
        }
    }
}