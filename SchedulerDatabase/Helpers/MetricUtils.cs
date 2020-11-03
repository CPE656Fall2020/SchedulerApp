using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerDatabase.Helpers
{
    /// <summary>
    /// <see cref="MetricUtils"/> provides helper methods for converting metric values.
    /// </summary>
    public class MetricUtils
    {
        /// <summary>
        /// Converts a clock speed in Hz to a human-readable size representation.
        /// </summary>
        /// <param name="hz">The frequency in Hz.</param>
        /// <returns>A human-readable clockrate string.</returns>
        /// <remarks>
        /// Adapted from https://stackoverflow.com/a/4975942.
        /// </remarks>
        public static string HzToString(long hz)
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

        /// <summary>
        /// Formats a value into a label with the most appropriate SI prefix.
        /// </summary>
        /// <param name="input">The numerical value to format.</param>
        /// <param name="unit">The base SI unit. Examples include byte, Joule, or Volt.</param>
        /// <param name="binary">True to use base 1024 for digital storage formats, false to use standard SI base_10.</param>
        /// <returns>A formatted string.</returns>
        /// <remarks>
        /// // Adapted from https://stackoverflow.com/a/40266660.
        /// </remarks>
        public static string MetricValueAxisLabelFormatter(double input, string unit, bool binary = false)
        {
            double res = double.NaN;
            string suffix = string.Empty;
            var powBase = binary ? 2 : 10;

            // Smaller than Base unit
            if (Math.Abs(input) <= 0.001)
            {
                var siLow = new Dictionary<int, string> { };

                if (!binary)
                {
                    siLow = new Dictionary<int, string>
                    {
                        [-12] = "p",
                        [-9] = "n",
                        [-6] = "μ",
                        [-3] = "m",
                    };
                }

                foreach (var v in siLow.Keys)
                {
                    if (input != 0 && Math.Abs(input) <= Math.Pow(powBase, v))
                    {
                        res = input * Math.Pow(powBase, Math.Abs(v));
                        suffix = siLow[v];
                        break;
                    }
                }
            }

            // Greater than Base Unit
            if (Math.Abs(input) >= 1000)
            {
                Dictionary<int, string> siHigh;

                if (!binary)
                {
                    // Powers of 10
                    siHigh = new Dictionary<int, string>
                    {
                        [12] = "T",
                        [9] = "G",
                        [6] = "M",
                        [3] = "k",
                    };
                }
                else
                {
                    // As defined in IEEE 1541, expect without the trailing "i" since that's not commonly used in written text.
                    siHigh = new Dictionary<int, string>
                    {
                        [40] = "T",
                        [30] = "G",
                        [20] = "M",
                        [10] = "k",
                    };
                }

                foreach (var v in siHigh.Keys)
                {
                    if (input != 0 && Math.Abs(input) >= Math.Pow(powBase, v))
                    {
                        res = input / Math.Pow(powBase, Math.Abs(v));
                        suffix = siHigh[v];
                        break;
                    }
                }
            }

            return double.IsNaN(res) ? $"{input:0.000}{unit}" : $"{res:0.000}{suffix}{unit}";
        }
    }
}
