using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using SchedulerGUI.Models;

namespace SchedulerGUI.ViewModels.Controls
{
    /// <summary>
    /// <see cref="AboutDialogViewModel"/> provides a View-Model for the <see cref="Views.HistoryGraph"/> view.
    /// </summary>
    public class HistoryGraphViewModel : ViewModelBase
    {
        private readonly Random random = new Random();

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
                //pass.PassPhases
                double passCurrentRunTime = 0;
                foreach (object phase in pass.PassPhases)
                {
                    double x = passCurrentRunTime, y = 0;
                    if (phase is PassPhase)
                    {
                        PassPhase tempPhase = (PassPhase)phase;
                        x += tempPhase.Duration.Minutes;
                        y = tempPhase.TotalEnergy;
                        passCurrentRunTime = x;
                    }
                    else if (phase is EncryptionPassPhase)
                    {
                        EncryptionPassPhase tempPhase = (EncryptionPassPhase)phase;
                        x += tempPhase.Duration.Minutes;
                        y = tempPhase.TotalEnergy;
                        passCurrentRunTime = x;
                    }
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

        public static PlotModel RandomScatter(int n, int binsize)
        {
            PlotModel model = new PlotModel();
            model.Title = string.Format("ScatterSeries (n={0})", n);
            var s1 = new ScatterSeries()
            {
                Title = "Series 1",
                MarkerType = MarkerType.Diamond,
                MarkerStrokeThickness = 2,
                BinSize = binsize,
            };
            var random = new Random();
            for (int i = 0; i < n; i++)
            {
                s1.Points.Add(new ScatterPoint(random.NextDouble(), random.NextDouble()));
            }

            model.Series.Add(s1);
            return model;
        }

        public PlotModel MyModel { get; private set; }
    }
}