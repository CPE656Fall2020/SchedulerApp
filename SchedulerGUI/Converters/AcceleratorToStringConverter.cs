using System;
using System.Globalization;
using System.Windows.Data;
using SchedulerDatabase.Extensions;
using SchedulerDatabase.Models;

namespace SchedulerGUI.Converters
{
    /// <summary>
    /// <see cref="AcceleratorToStringConverter"/> provides a converter to display <see cref="AESEncyptorProfile.AcceleratorType"/>s
    /// in a user-friendly string format.
    /// </summary>
    public sealed class AcceleratorToStringConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string enumStr && Enum.TryParse<AESEncyptorProfile.AcceleratorType>(enumStr, out var acc))
            {
                return acc.ToFriendlyName();
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
