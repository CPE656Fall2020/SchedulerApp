using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight;
using OxyPlot;
using OxyPlot.Series;
using SchedulerGUI.Interfaces;
using SchedulerGUI.Models;

namespace SchedulerGUI.ViewModels.Controls
{
    /// <summary>
    /// <see cref="HistoryGraphViewModel"/> provides a View-Model for the <see cref="Views.HistoryGraph"/> view.
    /// </summary>
    public class HistoryGraphViewModel : ViewModelBase
    {
        private IEnumerable<PassOrbit> passes;

        /// <summary>
        /// Initializes a new instance of the <see cref="HistoryGraphViewModel"/> class.
        /// </summary>
        public HistoryGraphViewModel()
        {
            this.PlotModel = new PlotModel
            {
                Title = "Energy Consumption Over Time",
            };
        }

        /// <summary>
        /// Gets the plot model for the history view.
        /// </summary>
        public PlotModel PlotModel { get; }

        /// <summary>
        /// Gets or sets the pass data that should be used to build the historical display.
        /// </summary>
        public IEnumerable<PassOrbit> Passes
        {
            get => this.passes;
            set
            {
                this.passes = value;
                this.GeneratePlot();
            }
        }

        private void GeneratePlot()
        {
            Random rnd = new Random();

            this.PlotModel.Series.Clear();

            foreach (PassOrbit pass in this.Passes)
            {
                LineSeries scatterSeries = new LineSeries
                {
                    MarkerType = MarkerType.Circle,
                    MarkerSize = 5,
                    Title = pass.Name,
                    Color = OxyColor.FromRgb((byte)rnd.Next(256), (byte)rnd.Next(256), (byte)rnd.Next(256)),
                };

                double passCurrentRunTime = 0;
                foreach (IPassPhase phase in pass.PassPhases)
                {
                    double x = passCurrentRunTime, y = 0;
                    x += phase.Duration.Minutes;
                    y = phase.TotalEnergyUsed;
                    passCurrentRunTime = x;

                    scatterSeries.Points.Add(new DataPoint(x, y));
                }

                this.PlotModel.Series.Add(scatterSeries);
            }

            this.PlotModel.LegendPosition = LegendPosition.RightMiddle;
            this.PlotModel.LegendPlacement = LegendPlacement.Outside;
            this.PlotModel.PlotAreaBorderColor = OxyColor.FromRgb(255, 255, 255);
            this.PlotModel.TextColor = OxyColor.FromRgb(255, 255, 255);
            this.PlotModel.TitleColor = OxyColor.FromRgb(255, 255, 255);

            this.PlotModel.InvalidatePlot(true);
        }
    }
}