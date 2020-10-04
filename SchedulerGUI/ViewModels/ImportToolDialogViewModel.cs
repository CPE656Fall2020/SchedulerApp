using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Win32;
using SchedulerGUI.Settings;

namespace SchedulerGUI.ViewModels
{
    /// <summary>
    /// <see cref="ImportToolDialogViewModel"/> provides a View-Model for the <see cref="Views.ImportToolDialog"/> view.
    /// </summary>
    public class ImportToolDialogViewModel : ViewModelBase
    {
        private string selectedExcelFile;
        private string selectedMapFile;
        private int firstWorksheet;
        private int lastWorksheet;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportToolDialogViewModel"/> class.
        /// </summary>
        /// <param name="completionCallback">A callback to invoke when an import has successfully completed.</param>
        public ImportToolDialogViewModel(Action completionCallback)
        {
            this.ChooseFileCommand = new RelayCommand<TextBox>(this.SelectFileHandler);
            this.ImportCommand = new RelayCommand(this.DoImport);

            this.CompletionCallback = completionCallback;
        }

        /// <summary>
        /// Gets the command to execute for selecting a file path.
        /// </summary>
        public ICommand ChooseFileCommand { get; }

        /// <summary>
        /// Gets the command to execute to complete the import operation.
        /// </summary>
        public ICommand ImportCommand { get; }

        /// <summary>
        /// Gets or sets the selected Excel file path to import.
        /// </summary>
        public string SelectedExcelFile
        {
            get => this.selectedExcelFile;
            set => this.Set(() => this.SelectedExcelFile, ref this.selectedExcelFile, value);
        }

        /// <summary>
        /// Gets or sets the selected map file path to import.
        /// </summary>
        public string SelectedMapFile
        {
            get => this.selectedMapFile;
            set => this.Set(() => this.SelectedMapFile, ref this.selectedMapFile, value);
        }

        /// <summary>
        /// Gets or sets the zero-based index of the first worksheet to import.
        /// </summary>
        public int FirstWorksheet
        {
            get => this.firstWorksheet;
            set => this.Set(() => this.FirstWorksheet, ref this.firstWorksheet, value);
        }

        /// <summary>
        /// Gets or sets the zero-based index of the last worksheet to import.
        /// </summary>
        public int LastWorksheet
        {
            get => this.lastWorksheet;
            set => this.Set(() => this.LastWorksheet, ref this.lastWorksheet, value);
        }

        private Action CompletionCallback { get; }

        private void SelectFileHandler(TextBox tb)
        {
            // TODO: Alex - This should really be an attached property on textbox
            // TODO: Alex - Adding filter types would be nice :)
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                tb.Text = openFileDialog.FileName;
            }
        }

        private void DoImport()
        {
            if (!File.Exists(this.SelectedExcelFile))
            {
                MessageBox.Show("Error: Selected Excel File cannot be found!", "Import Tools", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!File.Exists(this.SelectedMapFile))
            {
                MessageBox.Show("Error: Selected Map File cannot be found!", "Import Tools", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var importer = new SchedulerImportTools.ExcelWorksheetImporter(new SchedulerImportTools.Options()
            {
                Source = this.SelectedExcelFile,
                Map = this.SelectedMapFile,
                DatabaseFile = SimpleIoc.Default.GetInstance<SettingsManager>().CoreSettings.DatabaseLocation,
                FirstWorksheet = this.FirstWorksheet,
                LastWorksheet = this.LastWorksheet,
            });

            var succeeded = importer.Import();

            MessageBox.Show($"{succeeded} entries were imported to the database!", "Import Tools", MessageBoxButton.OK, MessageBoxImage.Information);
            this.CompletionCallback();
        }
    }
}
