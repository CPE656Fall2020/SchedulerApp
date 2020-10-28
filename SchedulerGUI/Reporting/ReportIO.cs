using System;
using System.IO;
using System.IO.Packaging;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;
using System.Windows.Xps.Serialization;

namespace SchedulerGUI.Reporting
{
    /// <summary>
    /// <see cref="ReportIO"/> provides file I/O functionality for generated reports.
    /// </summary>
    public class ReportIO
    {
        /// <summary>
        /// Saves a <see cref="FlowDocument"/> to an XPS file.
        /// </summary>
        /// <param name="path">The file path to save to.</param>
        /// <param name="document">The document to save.</param>
        public static void SaveAsXps(string path, FlowDocument document)
        {
            using (Package package = Package.Open(path, FileMode.Create))
            {
                using (var xpsDoc = new XpsDocument(package, CompressionOption.Maximum))
                {
                    var xpsSm = new XpsSerializationManager(new XpsPackagingPolicy(xpsDoc), false);
                    var dp = ((IDocumentPaginatorSource)document).DocumentPaginator;
                    xpsSm.SaveAsXaml(dp);
                }
            }
        }

        /// <summary>
        /// Saves a <see cref="FlowDocument"/> to an PDF file using the
        /// Windows 10 PDF Printer.
        /// </summary>
        /// <param name="document">The document to save.</param>
        public static void PrintToPdf(FlowDocument document)
        {
            try
            {
                var printDialog = new PrintDialog();
                printDialog.PrintQueue = new PrintQueue(new PrintServer(), "Microsoft Print to PDF");

                document.PageHeight = printDialog.PrintableAreaHeight;
                document.PageWidth = printDialog.PrintableAreaWidth;
                document.PagePadding = new Thickness(50);
                document.ColumnGap = 0;
                document.ColumnWidth = printDialog.PrintableAreaWidth;

                IDocumentPaginatorSource dps = document;

                printDialog.PrintDocument(dps.DocumentPaginator, "Scheduler Report");
            }
            catch (PrintQueueException)
            {
                MessageBox.Show("The Windows Print to PDF Driver could not be loaded. This function is only supported on Windows 10.", "Scheduler App", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unknown PDF error occured. {ex.Message}", "Scheduler App", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
