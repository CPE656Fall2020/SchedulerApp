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
    /// <see cref="AboutDialogViewModel"/> provides a View-Model for the <see cref="Views.HistoryGraph"/> view.
    /// </summary>
    public class HistoryGraphViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HistoryGraphViewModel"/> class.
        /// </summary>
        public HistoryGraphViewModel(ObservableCollection<PassOrbit> passes)
        {
            Random rnd = new Random();

            MyModel = new PlotModel { Title = "Energy Consumtion Over time" };

            int i = 1;
            foreach (PassOrbit pass in passes)
            {
                LineSeries scatterSeries = new LineSeries { MarkerType = MarkerType.Circle, MarkerSize = 5, Title = pass.Name, Color = OxyColor.FromRgb((byte)rnd.Next(256), (byte)rnd.Next(256), (byte)rnd.Next(256)) };
                double passCurrentRunTime = 0;
                foreach (IPassPhase phase in pass.PassPhases)
                {
                    double x = passCurrentRunTime, y = 0;
                    x += phase.Duration.Minutes;
                    y = phase.TotalEnergy;
                    passCurrentRunTime = x;

                    var size = 5;
                    var colorValue = i * 100;
                    scatterSeries.Points.Add(new DataPoint(x, y));
                }
                MyModel.Series.Add(scatterSeries);

                i++;
            }

            MyModel.LegendPosition = LegendPosition.RightMiddle;
            MyModel.LegendPlacement = LegendPlacement.Outside;
            MyModel.PlotAreaBorderColor = OxyColor.FromRgb(255, 255, 255);
            MyModel.TextColor = OxyColor.FromRgb(255, 255, 255);
            MyModel.TitleColor = OxyColor.FromRgb(255, 255, 255);
        }

        public PlotModel MyModel { get; private set; }
    }
}