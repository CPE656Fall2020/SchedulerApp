using System.Windows;

namespace SchedulerGUI.Services
{
    /// <summary>
    /// <see cref="WindowService"/> provides an MVVM service for launching top-level windows.
    /// </summary>
    public class WindowService
    {
        /// <summary>
        /// Opens a new window for this application.
        /// </summary>
        /// <typeparam name="T">The type of Window to launch.</typeparam>
        /// <param name="dataContext">The ViewModel containing the data to be displayed.</param>
        public void ShowWindow<T>(object dataContext)
            where T : Window, new()
        {
            var window = new T
            {
                DataContext = dataContext,
            };
            window.Show();
        }
    }
}
