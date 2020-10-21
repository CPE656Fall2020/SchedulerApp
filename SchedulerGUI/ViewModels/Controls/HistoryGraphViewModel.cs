using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
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
                Title = "Battery Capacity",
                IsLegendVisible = false,
                TextColor = OxyColors.White,
                TitleColor = OxyColors.White,
                PlotAreaBorderColor = OxyColors.White,
            };

            this.PlotModel.Axes.Add(new DateTimeAxis()
            {
                StringFormat = "hh:mm:ss tt",
            });

            this.PlotModel.Annotations.Add(new RectangleAnnotation
            {
                MinimumY = -1e10,
                MaximumY = 0,
                ClipByXAxis = false,
                ClipByYAxis = true,
                Fill = OxyColor.FromAColor(50, OxyColors.Red),
            });
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
            OxyColor[] colors = { OxyColors.Yellow, OxyColors.Blue, OxyColors.Green, OxyColors.Orange };
            int colorIndex = 0;

            this.PlotModel.Series.Clear();

            var cumulativeEnergy = 0D;

            foreach (var pass in this.Passes)
            {
                foreach (var phase in pass.PassPhases)
                {
                    var scatterSeries = new LineSeries
                    {
                        MarkerType = MarkerType.Circle,
                        MarkerSize = 5,
                        Title = pass.Name,
                        Color = colors[colorIndex],
                        TrackerFormatString = $"{{0}}\n{{2}}\n{phase.PhaseName} Phase\n{{4}} Joules",
                    };

                    scatterSeries.Points.Add(new DataPoint(
                        DateTimeAxis.ToDouble(phase.StartTime),
                        cumulativeEnergy));

                    cumulativeEnergy += -1 * phase.TotalEnergyUsed;

                    scatterSeries.Points.Add(new DataPoint(
                        DateTimeAxis.ToDouble(phase.EndTime),
                        cumulativeEnergy));

                    this.PlotModel.Series.Add(scatterSeries);

                    colorIndex = (colorIndex + 1) % colors.Length;
                }
            }

            this.PlotModel.InvalidatePlot(true);
        }
    }
}