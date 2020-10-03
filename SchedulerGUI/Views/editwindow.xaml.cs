using System.Windows;
using SchedulerGUI.ViewModels;

namespace SchedulerGUI.Views
{
    /// <summary>
    /// Interaction logic for EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        public EditWindow()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindowViewModel data = this.DataContext as MainWindowViewModel;
            data.SelectedPassItem.Sunlight.Duration = this.Sunlight.Value;
            data.SelectedPassItem.Mission.Duration = this.Mission.Value;
            data.SelectedPassItem.Encryption.Duration = this.encryption.Value;
            data.SelectedPassItem.Datalink.Duration = this.datalink.Value;
            this.Close();
        }
    }
}
