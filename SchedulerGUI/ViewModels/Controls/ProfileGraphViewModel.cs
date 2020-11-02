using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using SchedulerDatabase.Helpers;
using SchedulerDatabase.Models;

namespace SchedulerGUI.ViewModels.Controls
{
    /// <summary>
    /// <see cref="ProfileGraphViewModel"/> provides a View-Model for the <see cref="Views.Controls.AESGraphControl"/> control.
    /// </summary>
    public class ProfileGraphViewModel : ViewModelBase
    {
        private IEnumerable<IByteStreamProcessor> displayedData;
        private double joulesPerByteStdDev;
        private double bytesPerSecondStdDev;
        private bool showBytesLog;
        private bool showEnergyLog;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileGraphViewModel"/> class.
        /// </summary>
        public ProfileGraphViewModel()
        {
            this.Plot = new PlotModel()
            {
                Title = "Profile Comparison",
            };
        }

        /// <summary>
        /// Gets or sets the profile data to visually display in the graph.
        /// </summary>
        public IEnumerable<IByteStreamProcessor> DisplayedData
        {
            get => this.displayedData;
            set
            {
                this.displayedData = value;
                this.GeneratePlot();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether throughput is displayed on a log scale.
        /// </summary>
        public bool ShowThroughputLogarithmic
        {
            get => this.showBytesLog;
            set
            {
                this.showBytesLog = value;
                this.GeneratePlot();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether energy is displayed on a log scale.
        /// </summary>
        public bool ShowEnergyLogarithmic
        {
            get => this.showEnergyLog;
            set
            {
                this.showEnergyLog = value;
                this.GeneratePlot();
            }
        }

        /// <summary>
        /// Gets the standard deviation of the bytes/second values that are plotted.
        /// </summary>
        public double BytesPerSecondStdDev
        {
            get => this.bytesPerSecondStdDev;
            private set => this.Set(() => this.BytesPerSecondStdDev, ref this.bytesPerSecondStdDev, value);
        }

        /// <summary>
        /// Gets the standard deviation of the joules/byte values that are plotted.
        /// </summary>
        public double JoulesPerByteStdDev
        {
            get => this.joulesPerByteStdDev;
            private set => this.Set(() => this.JoulesPerByteStdDev, ref this.joulesPerByteStdDev, value);
        }

        /// <summary>
        /// Gets the processed data that is being directly plotted.
        /// </summary>
        public PlotModel Plot { get; }

        // Adapted from https://stackoverflow.com/a/3141731.
        private static double CalculateStandardDeviation(IEnumerable<double> values)
        {
            double standardDeviation = 0;

            if (values.Any())
            {
                // Compute the average.
                double avg = values.Average();

                // Perform the Sum of (value-avg)_2_2.
                double sum = values.Sum(d => Math.Pow(d - avg, 2));

                // Put it all together.
                standardDeviation = Math.Sqrt(sum / (values.Count() - 1));
            }

            return standardDeviation;
        }

        /// <summary>
        /// Converts a number of bytes to a human-readable size representation.
        /// </summary>
        /// <param name="byteCount">The number of bytes.</param>
        /// <returns>A human-readable size string.</returns>
        /// <remarks>
        /// Adapted from https://stackoverflow.com/a/4975942.
        /// Copied from https://github.com/alexdillon/GroupMeClient/blob/develop/GroupMeClient.Core/ViewModels/Controls/Attachments/FileAttachmentControlViewModel.cs.
        /// </remarks>
        private static string BytesToString(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; // Longs run out around EB
            if (byteCount == 0)
            {
                return "0" + suf[0];
            }

            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + " " + suf[place];
        }

        private void GeneratePlot()
        {
            var joulesPerByteSeriesData = new List<ColumnItem>();
            var bytesPerSecondSeriesData = new List<ThroughputColumnItem>();
            var categoryAxisData = new List<string>();

            foreach (var aesProfile in this.DisplayedData)
            {
                joulesPerByteSeriesData.Add(new ColumnItem() { Value = aesProfile.JoulesPerByte });
                bytesPerSecondSeriesData.Add(new ThroughputColumnItem() { Value = aesProfile.BytesPerSecond });

                categoryAxisData.Add(aesProfile.ShortProfileSpecificDescription);
            }

            this.Plot.Axes.Clear();
            this.Plot.Axes.Add(new CategoryAxis
            {
                Position = AxisPosition.Bottom,
                Angle = 45,
                Key = "Profile Axis",
                ItemsSource = categoryAxisData,
                IsPanEnabled = false,
                IsZoomEnabled = false,
            });

            // Add the Energy Axis (either log or linear)
            const string EnergyAxisKey = "JoulesPerByteAxis";
            const string energyAxisTitle = "Joules / byte";
            if (this.ShowEnergyLogarithmic)
            {
                this.Plot.Axes.Add(new FixedCenterLogAxis(1e-10)
                {
                    Position = AxisPosition.Left,
                    Key = EnergyAxisKey,
                    Title = energyAxisTitle,
                    Base = 10,
                    Minimum = 1e-10,
                    LabelFormatter = (v) => MetricUtils.MetricValueAxisLabelFormatter(v, "J/b", binary: false),
                    IsPanEnabled = false,
                });
            }
            else
            {
                this.Plot.Axes.Add(new FixedCenterLinearAxis(0)
                {
                    Position = AxisPosition.Left,
                    Key = EnergyAxisKey,
                    Title = energyAxisTitle,
                    AbsoluteMinimum = 0,
                    Minimum = 0,
                    LabelFormatter = (v) => MetricUtils.MetricValueAxisLabelFormatter(v, "J/b", binary: false),
                    IsPanEnabled = false,
                });
            }

            // Add the Throughput Axis (either log or linear)
            const string ThroughputAxisKey = "BytesPerSecondAxis";
            const string throughputAxisTitle = "bytes / sec";
            if (this.ShowThroughputLogarithmic)
            {
                this.Plot.Axes.Add(new FixedCenterLogAxis(1)
                {
                    Position = AxisPosition.Right,
                    Key = ThroughputAxisKey,
                    Title = throughputAxisTitle,
                    Base = 1024,
                    Minimum = 0,
                    LabelFormatter = (v) => MetricUtils.MetricValueAxisLabelFormatter(v, "B/s", binary: true),
                    IsPanEnabled = false,
                });
            }
            else
            {
                this.Plot.Axes.Add(new FixedCenterLinearAxis(0)
                {
                    Position = AxisPosition.Right,
                    Key = ThroughputAxisKey,
                    Title = throughputAxisTitle,
                    AbsoluteMinimum = 0,
                    Minimum = 0,
                    LabelFormatter = (v) => MetricUtils.MetricValueAxisLabelFormatter(v, "B/s", binary: true),
                    IsPanEnabled = false,
                });
            }

            // From https://stackoverflow.com/questions/47887366/oxyplot-logarithmic-stem-plot-is-upside-down
            var baseValueThroughput = this.ShowThroughputLogarithmic ? 1 : 0;
            var baseValueEnergy = this.ShowEnergyLogarithmic ? 1e-10 : 0;

            this.Plot.Series.Clear();
            this.Plot.Series.Add(new ColumnSeries()
            {
                ItemsSource = joulesPerByteSeriesData,
                YAxisKey = EnergyAxisKey,
                BaseValue = baseValueEnergy,
                FillColor = OxyColors.Red,
            });

            this.Plot.Series.Add(new ColumnSeries()
            {
                ItemsSource = bytesPerSecondSeriesData,
                YAxisKey = ThroughputAxisKey,
                FillColor = OxyColors.Green,
                BaseValue = baseValueThroughput,
                TrackerFormatString = "{0}\n{1}: {2}\n{SpeedString}",
            });

            this.Plot.InvalidatePlot(true);

            this.JoulesPerByteStdDev = CalculateStandardDeviation(joulesPerByteSeriesData.Select(s => s.Value));
            this.BytesPerSecondStdDev = CalculateStandardDeviation(bytesPerSecondSeriesData.Select(s => s.Value));
        }

        private class ThroughputColumnItem : ColumnItem
        {
            public string SpeedString
            {
                get => $"{ProfileGraphViewModel.BytesToString((long)this.Value)}/sec";
            }
        }

        // Adapted from https://stackoverflow.com/questions/41565360/oxyplot-axis-locking-center-when-mouse-wheel.
        private class FixedCenterLinearAxis : LinearAxis
        {
            public FixedCenterLinearAxis()
                : base()
            {
            }

            public FixedCenterLinearAxis(double center)
                : base()
            {
                this.Center = center;
            }

            private double Center { get; } = 0;

            public override void ZoomAt(double factor, double x)
            {
                base.ZoomAt(factor, this.Center);
            }
        }

        // Adapted from https://stackoverflow.com/questions/41565360/oxyplot-axis-locking-center-when-mouse-wheel.
        private class FixedCenterLogAxis : LogarithmicAxis
        {
            public FixedCenterLogAxis()
                : base()
            {
            }

            public FixedCenterLogAxis(double center)
                : base()
            {
                this.Center = center;
            }

            private double Center { get; } = 1;

            public override void ZoomAt(double factor, double x)
            {
                base.ZoomAt(factor, this.Center);
            }
        }
    }
}
