using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for editwindow.xaml
    /// </summary>
    public partial class editwindow : Window
    {
        public editwindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Userinterfaceviewmodel data = DataContext as Userinterfaceviewmodel;
            data.SelectedPassItem.sunlight.Duration = this.Sunlight.Value;
            data.SelectedPassItem.mission.Duration = this.Mission.Value;
            data.SelectedPassItem.encryption.Duration = this.encryption.Value;
            data.SelectedPassItem.Datalink.Duration = this.datalink.Value;
            this.Close();
        }
    }
}
