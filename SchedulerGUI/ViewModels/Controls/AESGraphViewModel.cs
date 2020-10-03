using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using SchedulerDatabase.Models;

namespace SchedulerGUI.ViewModels.Controls
{
    /// <summary>
    /// <see cref="AESGraphViewModel"/> provides a View-Model for the <see cref="Views.Controls.AESGraphControl"/> control.
    /// </summary>
    public class AESGraphViewModel : ViewModelBase
    {
        private IEnumerable<AESEncyptorProfile> displayedData;
        private double joulesPerByteStdDev;
        private double bytesPerSecondStdDev;

        /// <summary>
        /// Initializes a new instance of the <see cref="AESGraphViewModel"/> class.
        /// </summary>
        public AESGraphViewModel()
        {
            this.Plot = new PlotModel()
            {
                Title = "AES Profile Comparison",
            };
        }

        /// <summary>
        /// Gets or sets the AES profile data to visually display in the graph.
        /// </summary>
        public IEnumerable<AESEncyptorProfile> DisplayedData
        {
            get => this.displayedData;
            set
            {
                this.displayedData = value;
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
                joulesPerByteSeriesData.Add(new ColumnItem() { Value = aesProfile.TotalTestedEnergyJoules / aesProfile.TotalTestedByteSize });
                bytesPerSecondSeriesData.Add(new ThroughputColumnItem() { Value = aesProfile.TotalTestedByteSize / aesProfile.TotalTestTime.TotalSeconds });
                categoryAxisData.Add($"{aesProfile.PlatformName} {aesProfile.ProviderName}\n{aesProfile.Author}");
            }

            this.Plot.Axes.Clear();
            this.Plot.Axes.Add(new CategoryAxis
            {
                Position = AxisPosition.Bottom,
                Angle = 45,
                Key = "AESAxis",
                ItemsSource = categoryAxisData,
                IsPanEnabled = false,
                IsZoomEnabled = false,
            });

            this.Plot.Axes.Add(new FixedCenterLinearAxis(0)
            {
                Position = AxisPosition.Left,
                Key = "JoulesPerByteAxis",
                Title = "Joules / byte",
                AbsoluteMinimum = 0,
                Minimum = 0,
                IsPanEnabled = false,
            });

            this.Plot.Axes.Add(new FixedCenterLinearAxis(0)
            {
                Position = AxisPosition.Right,
                Key = "BytesPerSecondAxis",
                Title = "Bytes / sec",
                AbsoluteMinimum = 0,
                Minimum = 0,
                IsPanEnabled = false,
            });

            this.Plot.Series.Clear();
            this.Plot.Series.Add(new ColumnSeries()
            {
                ItemsSource = joulesPerByteSeriesData,
                YAxisKey = "JoulesPerByteAxis",
                FillColor = OxyColors.Red,
            });

            this.Plot.Series.Add(new ColumnSeries()
            {
                ItemsSource = bytesPerSecondSeriesData,
                YAxisKey = "BytesPerSecondAxis",
                FillColor = OxyColors.Green,
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
                get => $"{AESGraphViewModel.BytesToString((long)this.Value)}/sec";
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
    }
}
