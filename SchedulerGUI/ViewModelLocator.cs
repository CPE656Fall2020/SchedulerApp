using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using SchedulerGUI.ViewModels;

namespace SchedulerGUI
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<MainWindowViewModel>();
            SimpleIoc.Default.Register<PlotWindowViewModel>();
        }

        public ViewModels.MainWindowViewModel MainWindow
        {
            get => SimpleIoc.Default.GetInstance<MainWindowViewModel>();
        }

        public ViewModels.PlotWindowViewModel PlotWindow
        {
            get => SimpleIoc.Default.GetInstance<PlotWindowViewModel>();
        }
    }
}
