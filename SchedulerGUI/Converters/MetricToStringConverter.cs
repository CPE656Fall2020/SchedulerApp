using System;
using System.Globalization;
using System.Windows.Data;
using SchedulerDatabase.Helpers;

namespace SchedulerGUI.Converters
{
    /// <summary>
    /// <see cref="MetricToStringConverter"/> provides a converter to display metric values as a user-friendly string.
    /// </summary>
    public sealed class MetricToStringConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double v)
            {
                return MetricUtils.MetricValueAxisLabelFormatter(v, parameter?.ToString(), false);
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
