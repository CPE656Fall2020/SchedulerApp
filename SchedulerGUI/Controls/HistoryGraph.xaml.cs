using System;
using System.Collections.Generic;
using System.Windows.Controls;
using OxyPlot;

namespace SchedulerGUI.Controls
{
    /// <summary>
    /// Interaction logic for HistoryGraph.xaml
    /// </summary>
    public partial class HistoryGraph : UserControl
    {
        private readonly Random random = new Random();

        public HistoryGraph()
        {
            this.DataContext = this;
            this.Title = "Energy Consumtion Over time";

            switch (this.random.Next(1, 4))
            {
                case 1:
                    this.Points = new List<DataPoint>
                              {
                                  new DataPoint(128, 10000),
                                  new DataPoint(112, 9000),
                                  new DataPoint(96, 9000),
                                  new DataPoint(80, 6000),
                                  new DataPoint(64, 5000),
                                  new DataPoint(48, 3000),
                                  new DataPoint(32, 2000),
                                  new DataPoint(16, 1000),
                                  new DataPoint(0, 0),
                              };
                    break;

                case 2:
                    this.Points = new List<DataPoint>
                              {
                                  new DataPoint(128, 10000),
                                  new DataPoint(112, 9500),
                                  new DataPoint(96, 9000),
                                  new DataPoint(80, 9000),
                                  new DataPoint(64, 4000),
                                  new DataPoint(48, 2000),
                                  new DataPoint(32, 1800),
                                  new DataPoint(16, 500),
                                  new DataPoint(0, 0),
                              };
                    break;

                case 3:
                    this.Points = new List<DataPoint>
                              {
                                  new DataPoint(128, 10000),
                                  new DataPoint(112, 9500),
                                  new DataPoint(96, 9000),
                                  new DataPoint(80, 8000),
                                  new DataPoint(64, 7000),
                                  new DataPoint(48, 6000),
                                  new DataPoint(32, 5800),
                                  new DataPoint(16, 500),
                                  new DataPoint(0, 0),
                              };
                    break;

                case 4:
                    this.Points = new List<DataPoint>
                              {
                                  new DataPoint(128, 8000),
                                  new DataPoint(112, 7500),
                                  new DataPoint(96, 6000),
                                  new DataPoint(80, 5000),
                                  new DataPoint(64, 4000),
                                  new DataPoint(48, 3000),
                                  new DataPoint(32, 2800),
                                  new DataPoint(16, 500),
                                  new DataPoint(0, 0),
                              };
                    break;

                default:
                    this.Points = new List<DataPoint>
                              {
                                  new DataPoint(128, 10000),
                                  new DataPoint(112, 9500),
                                  new DataPoint(96, 9000),
                                  new DataPoint(80, 9000),
                                  new DataPoint(64, 4000),
                                  new DataPoint(48, 2000),
                                  new DataPoint(32, 1800),
                                  new DataPoint(16, 500),
                                  new DataPoint(0, 0),
                              };
                    break;
            }

            this.XTitle = "Time in Mins";
            this.YTitle = "Milli-jewels Consumed";
            this.InitializeComponent();
        }

        public string Title { get; set; }

        public string YTitle { get; set; }

        public string XTitle { get; set; }

        public string DataSet { get; set; }

        public IList<DataPoint> Points { get; private set; }
    }
}