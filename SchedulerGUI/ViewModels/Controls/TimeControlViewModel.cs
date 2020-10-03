using System;
using GalaSoft.MvvmLight;

namespace SchedulerGUI.ViewModels.Controls
{
    /// <summary>
    /// <see cref="TimeControlViewModel"/> provides a View-Model for the <see cref="Views.Controls.TimeControl"/> editor control.
    /// </summary>
    public class TimeControlViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeControlViewModel"/> class.
        /// </summary>
        /// <param name="initialValue">The initial time value to display in the editor.</param>
        public TimeControlViewModel(TimeSpan initialValue)
        {
            this.Hours = initialValue.Hours;
            this.Minutes = initialValue.Minutes;
            this.Seconds = initialValue.Seconds;
        }

        /// <summary>
        /// Gets or sets the number of hours.
        /// </summary>
        public int Hours { get; set; }

        /// <summary>
        /// Gets or sets the number of minutes.
        /// </summary>
        public int Minutes { get; set; }

        /// <summary>
        /// Gets or sets the number of seconds.
        /// </summary>
        public int Seconds { get; set; }

        /// <summary>
        /// Gets the duration that has been entered into the control.
        /// </summary>
        public TimeSpan SelectedDuration => new TimeSpan(this.Hours, this.Minutes, this.Seconds);

    }
}
