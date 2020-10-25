using System;
using System.IO;
using System.IO.Packaging;
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
    }
}
