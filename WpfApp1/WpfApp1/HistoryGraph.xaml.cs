using System.Collections.Generic;
using System.Windows.Controls;
using OxyPlot;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for HistoryGraph.xaml
    /// </summary>
    public partial class HistoryGraph : UserControl
    {
        public string Title { get; private set; }

        public IList<DataPoint> Points { get; private set; }

        public HistoryGraph()
        {
            InitializeComponent();
            this.Title = "Energy Consumtion Over time";
            this.Points = new List<DataPoint>
                              {
                                  new DataPoint(128,10000 ),
                                  new DataPoint(112,9000),
                                  new DataPoint(96,9000),
                                  new DataPoint( 80,6000),
                                  new DataPoint(64,5000 ),
                                  new DataPoint( 48,3000),
                                  new DataPoint(32,2000 ),
                                  new DataPoint(16,1000 ),
                                  new DataPoint(0, 0),
                              };

            //xLabels = new[] { "128", "112", "96", "80", "64", "48", "32", "16", "0" };

            //yLabels = new[] { "10000", "9000", "8000", "7000", "6000", "5000", "4000", "3000", "2000", "1000", "0" };
        }
    }
}