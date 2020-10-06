using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace SchedulerGUI.ViewModels
{
    /// <summary>
    /// <see cref="AboutDialogViewModel"/> provides a View-Model for the <see cref="Views.AboutDialog"/> view.
    /// </summary>
    public class AboutDialogViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AboutDialogViewModel"/> class.
        /// </summary>
        public AboutDialogViewModel()
        {
        }

        /// <summary>
        /// Gets the short application version number.
        /// </summary>
        public string ShortVersion => GlobalAssemblyInfo.SimpleVersion;

        /// <summary>
        /// Gets the complete application build description string.
        /// </summary>
        public string FullVersion => GlobalAssemblyInfo.InformationalVersion;
    }
}
