using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace SchedulerGUI.Reporting
{
    /// <summary>
    /// <see cref="ReportTheme"/> provides support for generating thematically styled elements for a report.
    /// </summary>
    public class ReportTheme
    {
        /// <summary>
        /// Generates an icon to indicate whether an operation has succeeded or not.
        /// </summary>
        /// <param name="succeeded">The status of the operation.</param>
        /// <returns>An inline containing a success icon if the operation succeeded, a failure icon otherwise.</returns>
        public static Inline GetSuccessIcon(bool succeeded)
        {
            ImageSource icon;
            if (succeeded)
            {
                icon = (ImageSource)App.Current.Resources["VS2017Icons.TestCoveringPassed"];
            }
            else
            {
                icon = (ImageSource)App.Current.Resources["VS2017Icons.TestCoveringFailed"];
            }

            return new InlineUIContainer(new Image() { Source = icon, Width = 16, Height = 16 });
        }

        /// <summary>
        /// Generated a Title element for a document.
        /// </summary>
        /// <param name="text">The contents of the title.</param>
        /// <returns>A title section.</returns>
        public static Section MakeTitle(string text)
        {
            var title = new Section();
            var runText = new Run(text)
            {
                FontFamily = new FontFamily("Calibri Light"),
                FontSize = 36,
            };

            title.Blocks.Add(new Paragraph() { Inlines = { runText } });
            return title;
        }

        /// <summary>
        /// Generated a Level-1 Header element for a document.
        /// </summary>
        /// <param name="text">The contents of the header.</param>
        /// <param name="extraContent">Additional inlines to include in the header.</param>
        /// <returns>A header section.</returns>
        public static Section MakeHeader1(string text, IEnumerable<Inline> extraContent = null)
        {
            return MakeHeader(
                new FontFamily("Calibri Light"),
                new SolidColorBrush(Color.FromRgb(47, 84, 150)),
                22,
                text,
                extraContent);
        }

        /// <summary>
        /// Generated a Level-2 Header element for a document.
        /// </summary>
        /// <param name="text">The contents of the header.</param>
        /// <param name="extraContent">Additional inlines to include in the header.</param>
        /// <returns>A header section.</returns>
        public static Section MakeHeader2(string text, IEnumerable<Inline> extraContent = null)
        {
            return MakeHeader(
              new FontFamily("Calibri Light"),
              new SolidColorBrush(Color.FromRgb(47, 84, 150)),
              18,
              text,
              extraContent);
        }

        private static Section MakeHeader(FontFamily font, Brush color, double size, string text, IEnumerable<Inline> extraContent = null)
        {
            var title = new Section()
            {
                FontFamily = font,
                Foreground = color,
                FontSize = size,
            };

            var paragraph = new Paragraph() { Inlines = { new Run(text) } };

            if (extraContent != null)
            {
                paragraph.Inlines.AddRange(extraContent);
            }

            title.Blocks.Add(paragraph);

            return title;
        }
    }
}
