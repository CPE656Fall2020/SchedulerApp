using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

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
                return HzToString(frequency);
            }

            return string.Empty;
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts a clock speed in Hz to a human-readable size representation.
        /// </summary>
        /// <param name="hz">The frequency in Hz.</param>
        /// <returns>A human-readable clockrate string.</returns>
        /// <remarks>
        /// Adapted from https://stackoverflow.com/a/4975942.
        /// </remarks>
        private static string HzToString(long hz)
        {
            string[] suf = { "Hz", "kHz", "MHz", "GHz" };
            if (hz == 0)
            {
                return "0" + suf[0];
            }

            long bytes = Math.Abs(hz);
            int place = System.Convert.ToInt32(Math.Floor(Math.Log(bytes, 1000)));
            double num = Math.Round(bytes / Math.Pow(1000, place), 1);
            return (Math.Sign(hz) * num).ToString() + " " + suf[place];
        }
    }
}
