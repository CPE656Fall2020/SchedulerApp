using System;
using System.ComponentModel;
using System.Globalization;

namespace SchedulerImportTools.Converters
{
    /// <summary>
    /// <see cref="NanoSecondsToTimeSpanConverter"/> provides a type converter than can convert an integer value expressed in nanoseconds to a <see cref="TimeSpan"/>.
    /// </summary>
    public class NanoSecondsToTimeSpanConverter : TypeConverter
    {
        /// <inheritdoc/>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            throw new Exception("Only one way conversion from is supported.");
        }

        /// <inheritdoc/>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var isNumeric = long.TryParse(value.ToString(), out var numericValue);

            if (!isNumeric)
            {
                throw new InvalidCastException("Not supported type");
            }
            else
            {
                const int NanoSecondsPerTick = 100;
                return TimeSpan.FromTicks(numericValue / NanoSecondsPerTick);
            }
        }
    }
}
