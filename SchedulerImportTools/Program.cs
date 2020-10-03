using System;
using CommandLine;

namespace SchedulerImportTools
{
    /// <summary>
    /// <see cref="Program"/> is the entry-point for the Scheduler Import Tool CLI application.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Entry point for the Scheduler Import Tool CLI application.
        /// </summary>
        /// <param name="args">The Command Line arguments provided from the shell.</param>
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
               .WithParsed(o =>
               {
                   var importer = new ExcelWorksheetImporter(o);

                   var success = 0;
                   try
                   {
                       success = importer.Import();
                   }
                   catch (Exception ex)
                   {
                       Console.WriteLine($"An error occured during the import process: {ex.Message}");
                   }

                   Console.WriteLine($"{success} records imported successfully to the database!");

                   if (!o.AutoRun)
                   {
                       Console.WriteLine("Press enter to exit.");
                       Console.ReadLine();
                   }
               });
        }
    }
}
