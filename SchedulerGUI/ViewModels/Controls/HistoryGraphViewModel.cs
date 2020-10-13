using System;
using System.Collections.ObjectModel;
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
        /// <summary>
        /// Initializes a new instance of the <see cref="HistoryGraphViewModel"/> class.
        /// </summary>
        public HistoryGraphViewModel(ObservableCollection<PassOrbit> passes)
        {
            Random rnd = new Random();

            this.MyModel = new PlotModel { Title = "Energy Consumption Over time" };

            int i = 1;
            foreach (PassOrbit pass in passes)
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

                    var colorValue = i * 100;
                    scatterSeries.Points.Add(new DataPoint(x, y));
                }

                this.MyModel.Series.Add(scatterSeries);

                i++;
            }

            this.MyModel.LegendPosition = LegendPosition.RightMiddle;
            this.MyModel.LegendPlacement = LegendPlacement.Outside;
            this.MyModel.PlotAreaBorderColor = OxyColor.FromRgb(255, 255, 255);
            this.MyModel.TextColor = OxyColor.FromRgb(255, 255, 255);
            this.MyModel.TitleColor = OxyColor.FromRgb(255, 255, 255);
        }

        /// <summary>
        /// Gets the plot model for the history view.
        /// </summary>
        public PlotModel MyModel { get; private set; }
    }
}