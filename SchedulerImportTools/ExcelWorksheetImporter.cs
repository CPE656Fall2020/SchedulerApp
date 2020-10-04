using System;
using System.ComponentModel;
using System.IO;
using ExcelDataReader;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Newtonsoft.Json.Linq;
using SchedulerDatabase;
using SchedulerDatabase.Models;
using SchedulerImportTools.Converters;

namespace SchedulerImportTools
{
    /// <summary>
    /// <see cref="ExcelWorksheetImporter"/> provides support for importing data contained in individual Excel worksheet into the database.
    /// </summary>
    public class ExcelWorksheetImporter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelWorksheetImporter"/> class.
        /// </summary>
        /// <param name="options">The import options to use.</param>
        public ExcelWorksheetImporter(Options options)
        {
            this.Options = options;
            this.Context = new SchedulerContext(this.Options.DatabaseFile);

            // TODO: If needed, make the time scale unit (nano, milli, etc.) a parameter in Options, and pass it to the Converter.
            TypeDescriptor.AddAttributes(
                typeof(TimeSpan),
                new TypeConverterAttribute(typeof(NanoSecondsToTimeSpanConverter)));
        }

        private Options Options { get; }

        private SchedulerContext Context { get; }

        /// <summary>
        /// Executes the import operation.
        /// </summary>
        /// <returns>The number of records successfully imported.</returns>
        public int Import()
        {
            var map = JObject.Parse(File.ReadAllText(this.Options.Map));

            using (var stream = File.Open(this.Options.Source, FileMode.Open, FileAccess.Read))
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                var dataset = reader.AsDataSet();

                int firstTableIndex = this.Options.FirstWorksheet;
                int lastTableIndex = this.Options.LastWorksheet == 0 ? dataset.Tables.Count - 1 : this.Options.LastWorksheet;

                int successfulEntryCount = 0;

                for (int index = firstTableIndex; index <= lastTableIndex; index++)
                {
                    var worksheet = dataset.Tables[index];
                    Console.WriteLine($"Beginning import on worksheet \"{worksheet.TableName}\"");

                    AESEncyptorProfile profile;

                    var guidLocation = map[nameof(AESEncyptorProfile.ProfileId)];
                    if (guidLocation != null && IsExcelFieldRef(guidLocation.ToString(), out var guidColumnIndex, out var guidRowIndex))
                    {
                        // A valid Excel cell-ref is provided for the GUID. The GUID may not exist in the DB yet though.
                        var targetGUID = Guid.Parse(worksheet.Rows[guidRowIndex][guidColumnIndex].ToString());
                        profile = this.Context.AESProfiles.Find(targetGUID);

                        if (profile == null)
                        {
                            // Make a new DB entry with this GUID
                            profile = new AESEncyptorProfile()
                            {
                                ProfileId = targetGUID,
                            };

                            // Since this is a new entry with a static ID, add it to the DB to begin tracking changes.
                            this.Context.AESProfiles.Add(profile);
                        }
                    }
                    else
                    {
                        profile = new AESEncyptorProfile()
                        {
                            ProfileId = Guid.NewGuid(),
                        };

                        // Since this is a new entry with a random ID, add it to the DB to begin tracking changes.
                        this.Context.AESProfiles.Add(profile);

                        Console.WriteLine($"Entry missing ID, assigned new random ID ({profile.ProfileId})");
                    }

                    foreach (var param in map)
                    {
                        var databaseProperty = profile.GetType().GetProperty(param.Key);
                        var converter = TypeDescriptor.GetConverter(databaseProperty.PropertyType);

                        var scale = 1.0d;
                        var mappedValue = string.Empty;
                        if (param.Value.HasValues)
                        {
                            // If yes, this means a scale factor is included (like to convert from millivolt to volt
                            scale = (double)param.Value[1];
                            mappedValue = param.Value[0].ToString();
                        }
                        else
                        {
                            mappedValue = param.Value.ToString();
                        }

                        if (IsExcelFieldRef(mappedValue, out var columnIndex, out var rowIndex))
                        {
                            // Lookup value from Excel worksheet and insert into Model
                            var excelValue = worksheet.Rows[rowIndex][columnIndex].ToString();
                            var parsedValue = converter.ConvertFromString(excelValue);

                            if (scale != 1.0)
                            {
                                // Apply scale factor
                                parsedValue = (double)parsedValue * scale;
                            }

                            databaseProperty.SetValue(profile, parsedValue);
                        }
                        else
                        {
                            // Use the static value provided in the Map instead
                            var parsedValue = converter.ConvertFromString(param.Value.ToString());
                            databaseProperty.SetValue(profile, parsedValue);
                        }
                    }

                    successfulEntryCount++;
                }

                this.Context.SaveChanges();
                return successfulEntryCount;
            }
        }

        private static bool IsExcelFieldRef(string value, out int columnIndex, out int rowIndex)
        {
            columnIndex = 0;
            rowIndex = 0;

            // Known limitation: valuable data must be stored in the first 26 columns. Otherwise, strings like MSP430 could be interpreted as a cell ref.
            var columnRef = value[0];
            if (!char.IsLetter(columnRef) || !char.IsUpper(columnRef))
            {
                return false;
            }
            else
            {
                columnIndex = (int)columnRef - (int)'A';
            }

            var rowRef = value.Substring(1);

            if (!int.TryParse(rowRef, out var excelRowIndex))
            {
                return false;
            }
            else
            {
                rowIndex = excelRowIndex - 1; // convert to zero based
            }

            return true;
        }
    }
}
