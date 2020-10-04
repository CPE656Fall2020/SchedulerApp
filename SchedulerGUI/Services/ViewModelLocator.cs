using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using SchedulerGUI.ViewModels;

namespace SchedulerGUI.Services
{
    /// <summary>
    /// <see cref="ViewModelLocator"/> provides a service for resolving top-level ViewModels.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelLocator"/> class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<WindowService>();

            SimpleIoc.Default.Register<MainWindowViewModel>();
            SimpleIoc.Default.Register<PlotWindowViewModel>();
        }

        /// <summary>
        /// Gets the view model for the main window in the scheduling application.
        /// </summary>
        public ViewModels.MainWindowViewModel MainWindow
        {
            get => SimpleIoc.Default.GetInstance<MainWindowViewModel>();
        }
    }
}
