using System;
using System.Globalization;
using System.Windows.Data;
using SchedulerDatabase.Helpers;

namespace SchedulerGUI.Converters
{
    /// <summary>
    /// <see cref="HzToStringConverter"/> provides a converter to display frequencies in Hz as a user-friendly string.
    /// </summary>
    public sealed class HzToStringConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int frequency)
            {
                return MetricUtils.HzToString(frequency);
            }

            return string.Empty;
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
