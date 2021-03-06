﻿using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Win32;
using Newtonsoft.Json;
using SchedulerDatabase;
using SchedulerDatabase.Models;
using SchedulerGUI.Interfaces;
using SchedulerGUI.Models;
using SchedulerGUI.Services;
using SchedulerGUI.Settings;
using SchedulerGUI.Solver;
using SchedulerGUI.Solver.Algorithms;
using SchedulerGUI.ViewModels.Controls;
using SchedulerGUI.Views;
using TimelineLibrary;

namespace SchedulerGUI.ViewModels
{
    /// <summary>
    /// <see cref="MainWindowViewModel"/> provides the top-level View-Model for the Scheduler application, and bound to the <see cref="MainWindow"/> view.
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        private const double NUMPASSES = 11.25;
        private const double PASSDURATION = (24 / NUMPASSES) * 60;
        private PassOrbit selectedPass;
        private DateTime startTime;
        private DateTime endTime;
        private EditControlViewModel editControlVM;
        private IScheduleSolver selectedAlgorithm;
        private ScheduleSolution lastSolution;
        private object scheduleStatusIcon;
        private bool isAESDeviceSelectionVisible;
        private bool isCompressionDeviceSelectionVisible;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {
            // Setup shared services like DbContext and SettingsManager for the entire application.
            this.StartupAppServices();

            this.TimelineEventPasses = new ObservableCollection<TimelineEvent>();
            this.Passes = new ObservableCollection<PassOrbit>();

            this.SaveScheduleCommand = new RelayCommand(this.SaveScheduleHandler);
            this.OpenScheduleCommand = new RelayCommand(this.OpenScheduleHandler);
            this.ExportReportXPSCommand = new RelayCommand(this.ExportReportXPSHandler);
            this.ExportReportPDFCommand = new RelayCommand(this.ExportReportPDFHandler);
            this.ImportDatabaseCommand = new RelayCommand(this.ImportDatabaseHandler);
            this.ExportDatabaseCommand = new RelayCommand(this.ExportDatabaseHandler);
            this.ToggleAESDeviceSelectionVisibilityCommand = new RelayCommand(() => this.IsAESDeviceSelectionVisible = !this.IsAESDeviceSelectionVisible, true);
            this.ToggleCompressionDeviceSelectionVisibilityCommand = new RelayCommand(() => this.IsCompressionDeviceSelectionVisible = !this.IsCompressionDeviceSelectionVisible, true);
            this.OpenBatteryEditorCommand = new RelayCommand(this.OpenBatteryEditorHandler);
            this.OpenSolarCellEditorCommand = new RelayCommand(this.OpenSolarCellEditorHandler);
            this.OpenScheduleStatusCommand = new RelayCommand(this.OpenScheduleStatusHandler);
            this.OpenSchedulerPlotterCommand = new RelayCommand(this.OpenSchedulerPlotterHandler);
            this.OpenImportToolGUICommand = new RelayCommand(this.OpenImportToolGUIHandler);
            this.OpenImportToolCLICommand = new RelayCommand(this.OpenImportToolCLIHandler);
            this.OpenAboutCommand = new RelayCommand(this.OpenAboutHandler);

            this.DialogManager = new PopupViewModel()
            {
                ClosePopup = new RelayCommand(() => this.DialogManager.PopupDialog = null, true),
                EasyClosePopup = null,
                PopupDialog = null,
            };

            this.HistoryGraphViewModel = new HistoryGraphViewModel();
            this.AESDevicePickerViewModel = new DevicePickerViewModelAES();
            this.CompressionDevicePickerViewModel = new DevicePickerViewModelCompression();
            this.BatteryEditorViewModel = new EditBatteryControlViewModel();
            this.SolarCellEditorViewModel = new EditSolarCellControlViewModel(this.Passes);

            // Update the schedules when the parameters are changed
            this.AESDevicePickerViewModel.PropertyChanged += (s, e) => this.RunSchedule();
            this.CompressionDevicePickerViewModel.PropertyChanged += (s, e) => this.RunSchedule();
            this.BatteryEditorViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(this.BatteryEditorViewModel.Battery))
                {
                    this.RunSchedule();
                }
            };

            this.SolarCellEditorViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(this.SolarCellEditorViewModel.Passes))
                {
                    this.RunSchedule();
                }
            };

            // TODO: Finding a way to have icons in XAML and algorithms in CS and not having to manually map them by index
            // would be nice as opposed to providing the icon from ViewModel.
            this.AvailableAlgorithms = new ObservableCollection<IScheduleSolver>()
            {
                new GreedyOptimizedLowPowerScheduler() { Tag = Application.Current.Resources["VS2017Icons.VBPowerPack"] },
            };

            this.SelectedAlgorithm = this.AvailableAlgorithms.First();

            this.Init();
            this.SolarCellEditorViewModel.UpdatePassData();
            this.RunSchedule();
        }

        /// <summary>
        /// Gets the passes that are currently available for scheduling or editing.
        /// </summary>
        public ObservableCollection<PassOrbit> Passes { get; }

        /// <summary>
        /// Gets the passes as timeline events for viewing.
        /// </summary>
        public ObservableCollection<TimelineEvent> TimelineEventPasses { get; }

        /// <summary>
        /// Gets a collection of algorithms that are available for scheduling.
        /// </summary>
        public ObservableCollection<IScheduleSolver> AvailableAlgorithms { get; }

        /// <summary>
        /// Gets or sets the pass orbit that is currently selected.
        /// </summary>
        public PassOrbit SelectedPass
        {
            get => this.selectedPass;
            set
            {
                this.Set(() => this.SelectedPass, ref this.selectedPass, value);

                // Update the editor pane to show information for this pass.
                if (this.SelectedPass != null)
                {
                    this.InitEditControl();
                }
            }
        }

        /// <summary>
        /// Gets or sets the schedule solving algorithm that is currently selected.
        /// </summary>
        public IScheduleSolver SelectedAlgorithm
        {
            get => this.selectedAlgorithm;
            set
            {
                this.Set(() => this.SelectedAlgorithm, ref this.selectedAlgorithm, value);
                this.RunSchedule();
            }
        }

        /// <summary>
        /// Gets the command to execute to save the current set of scheduling parameters to a file.
        /// </summary>
        public ICommand SaveScheduleCommand { get; }

        /// <summary>
        /// Gets the command to execute to load scheduling parameters from a file.
        /// </summary>
        public ICommand OpenScheduleCommand { get; }

        /// <summary>
        /// Gets the command to export a scheduling report to a Microsoft XPS document.
        /// </summary>
        public ICommand ExportReportXPSCommand { get; }

        /// <summary>
        /// Gets the command to export a scheduling report to a Adobe PDF document.
        /// </summary>
        public ICommand ExportReportPDFCommand { get; }

        /// <summary>
        /// Gets the command to execute to import a database of AES devices.
        /// </summary>
        public ICommand ImportDatabaseCommand { get; }

        /// <summary>
        /// Gets the command to execute to export the current database of AES devices.
        /// </summary>
        public ICommand ExportDatabaseCommand { get; }

        /// <summary>
        /// Gets the command to execute to toggle the visibilty of the AES device selection flyout.
        /// </summary>
        public ICommand ToggleAESDeviceSelectionVisibilityCommand { get; }

        /// <summary>
        /// Gets the command to execute to toggle the visibilty of the compression device selection flyout.
        /// </summary>
        public ICommand ToggleCompressionDeviceSelectionVisibilityCommand { get; }

        /// <summary>
        /// Gets the command to execute to open the battery parameters editor.
        /// </summary>
        public ICommand OpenBatteryEditorCommand { get; }

        /// <summary>
        /// Gets the command to execute to open the solar cell parameters editor.
        /// </summary>
        public ICommand OpenSolarCellEditorCommand { get; }

        /// <summary>
        /// Gets the command to execute to view the status of a schedule.
        /// </summary>
        public ICommand OpenScheduleStatusCommand { get; }

        /// <summary>
        /// Gets the command to execute to open the Scheduler Plotter tool.
        /// </summary>
        public ICommand OpenSchedulerPlotterCommand { get; }

        /// <summary>
        /// Gets the command to execute to open the GUI import tools.
        /// </summary>
        public ICommand OpenImportToolGUICommand { get; }

        /// <summary>
        /// Gets the command to execute to open the CLI import tools.
        /// </summary>
        public ICommand OpenImportToolCLICommand { get; }

        /// <summary>
        /// Gets the command to execute to show the about screen.
        /// </summary>
        public ICommand OpenAboutCommand { get; }

        /// <summary>
        /// Gets the Dialog Manager for the main window.
        /// </summary>
        public PopupViewModel DialogManager { get; }

        /// <summary>
        /// Gets the edit control view model.
        /// </summary>
        public EditControlViewModel EditControlViewModel
        {
            get => this.editControlVM;
            private set => this.Set(() => this.EditControlViewModel, ref this.editControlVM, value);
        }

        /// <summary>
        /// Gets or sets the history graph for the scheduled passes.
        /// </summary>
        public HistoryGraphViewModel HistoryGraphViewModel { get; set; }

        /// <summary>
        /// Gets or sets the start time of the 24-hr period in which orbits will occur.
        /// </summary>
        public DateTime StartTime
        {
            get => this.startTime;
            set => this.Set(() => this.StartTime, ref this.startTime, value);
        }

        /// <summary>
        /// Gets or sets the end time of the 24-hr period in which orbits will occur.
        /// </summary>
        public DateTime EndTime
        {
            get => this.endTime;
            set => this.Set(() => this.EndTime, ref this.endTime, value);
        }

        /// <summary>
        /// Gets the icon to represent the current schedule status.
        /// </summary>
        public object ScheduleStatusIcon
        {
            get => this.scheduleStatusIcon;
            private set => this.Set(() => this.ScheduleStatusIcon, ref this.scheduleStatusIcon, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the device selection flyout is visible.
        /// </summary>
        public bool IsAESDeviceSelectionVisible
        {
            get => this.isAESDeviceSelectionVisible;
            set => this.Set(() => this.IsAESDeviceSelectionVisible, ref this.isAESDeviceSelectionVisible, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the AES device selection flyout is visible.
        /// </summary>
        public bool IsCompressionDeviceSelectionVisible
        {
            get => this.isCompressionDeviceSelectionVisible;
            set => this.Set(() => this.IsCompressionDeviceSelectionVisible, ref this.isCompressionDeviceSelectionVisible, value);
        }

        /// <summary>
        /// Gets the device picker control ViewModel for enabling and disabling AES devices.
        /// </summary>
        public DevicePickerViewModelAES AESDevicePickerViewModel { get; }

        /// <summary>
        /// Gets the device picker control ViewModel for enabling and disabling compression devices.
        /// </summary>
        public DevicePickerViewModelCompression CompressionDevicePickerViewModel { get; }

        /// <summary>
        /// Gets the battery editor ViewModel.
        /// </summary>
        public EditBatteryControlViewModel BatteryEditorViewModel { get; }

        /// <summary>
        /// Gets the solar cell editor ViewModel.
        /// </summary>
        public EditSolarCellControlViewModel SolarCellEditorViewModel { get; }

        /// <summary>
        /// Gets the last solved scheduling solution.
        /// </summary>
        public ScheduleSolution LastSolution
        {
            get => this.lastSolution;
            private set => this.Set(() => this.LastSolution, ref this.lastSolution, value);
        }

        /// <summary>
        /// Initializes edit control with pass information from the selected pass.
        /// </summary>
        public void InitEditControl()
        {
            if (this.SelectedPass != null)
            {
                this.EditControlViewModel = new EditControlViewModel(this.SelectedPass, this.SaveCommand);
            }
        }

        private void Init()
        {
            this.InitPasses();
            this.InitTimelineEvents();
        }

        /// <summary>
        /// Performs application initialization.
        /// </summary>
        private void StartupAppServices()
        {
            // Setup the SettingsManager
            const string SettingsFileName = "settings.json";
            SimpleIoc.Default.Register(() => new SettingsManager(SettingsFileName));

            // Load the user's settings file
            var settingsManager = SimpleIoc.Default.GetInstance<SettingsManager>();
            settingsManager.LoadSettings();

            // Didn't have a settings file? Make one now
            settingsManager.SaveSettings();

            // Register the DBContext with DI based on the provided path in Settings.
            SimpleIoc.Default.Register(() => new SchedulerContext(settingsManager.CoreSettings.DatabaseLocation));

            // EF Core kinda lags on the first time it opens a new context
            // Make a new instance int the background and just toss it to allow most of the startup cost
            // to be done ahead-of-time and allow for building it's internal cache.
            Task.Run(() => SimpleIoc.Default.GetInstanceWithoutCaching<SchedulerContext>().AESProfiles.Count());
        }

        private void UpdateEditControlVM(PassOrbit passData)
        {
            var currentIndex = this.Passes.IndexOf(this.SelectedPass);
            this.Passes[currentIndex] = passData;

            this.SelectedPass = passData;
        }

        private void SaveCommand(PassOrbit passData)
        {
            this.RunSchedule();
            this.UpdateEditControlVM(passData);
        }

        private void OpenBatteryEditorHandler()
        {
            this.DialogManager.PopupDialog = this.BatteryEditorViewModel;
        }

        private void OpenSolarCellEditorHandler()
        {
            this.DialogManager.PopupDialog = this.SolarCellEditorViewModel;
        }

        private void OpenScheduleStatusHandler()
        {
            var dialog = new ScheduleViewerDialogViewModel(this.LastSolution);
            this.DialogManager.PopupDialog = dialog;
        }

        private void OpenSchedulerPlotterHandler()
        {
            var instance = SimpleIoc.Default.GetInstanceWithoutCaching<PlotWindowViewModel>();
            var dialogService = SimpleIoc.Default.GetInstance<WindowService>();
            dialogService.ShowWindow<PlotWindow>(instance);
        }

        private void OpenImportToolGUIHandler()
        {
            var editDialog = new ImportToolDialogViewModel(
                () => this.DialogManager.PopupDialog = null);

            this.DialogManager.PopupDialog = editDialog;
        }

        private void OpenImportToolCLIHandler()
        {
            var appDir = AppDomain.CurrentDomain.BaseDirectory;

            Process.Start(new ProcessStartInfo(
                "cmd.exe",
                $"/k \"set PATH=%PATH%;\"{appDir}\"\"; && cd %USERPROFILE%\\Desktop && SchedulerImportTools.exe"));
        }

        private void InitPasses()
        {
            DateTime startTime = DateTime.Now;
            this.StartTime = startTime;
            Random random = new Random();

            for (int i = 0; i < NUMPASSES; i++)
            {
                this.Passes.Add(new PassOrbit($"Pass #{i + 1}", startTime, startTime.AddMinutes(PASSDURATION), random));
                startTime = startTime.AddMinutes(PASSDURATION);
            }

            this.EndTime = startTime;
        }

        private void InitTimelineEvents()
        {
            string[] colors = { "Yellow", "Blue", "Green", "Orange" };
            int colorIndex;

            foreach (PassOrbit pass in this.Passes.ToList())
            {
                this.TimelineEventPasses.Add(new TimelineEvent
                {
                    EndDate = pass.EndTime,
                    StartDate = pass.StartTime,
                    Title = pass.Name,
                    EventColor = "CornflowerBlue",
                    IsDuration = true,
                    PassParentName = pass.Name,
                });

                colorIndex = 0;
                foreach (IPassPhase phase in pass.PassPhases.ToList())
                {
                    this.TimelineEventPasses.Add(new TimelineEvent
                    {
                        EndDate = phase.EndTime,
                        StartDate = phase.StartTime,
                        EventColor = colors[colorIndex],
                        IsDuration = true,
                        Title = phase.PhaseName.ToString(),
                        RowOverride = 2,
                        PassParentName = pass.Name,
                    });

                    colorIndex++;
                }
            }

            foreach (TimelineEvent e in this.TimelineEventPasses.ToList())
            {
                e.EventClicked += this.OnEventClicked;
            }
        }

        private void OpenAboutHandler()
        {
            this.DialogManager.PopupDialog = new AboutDialogViewModel();
        }

        private void OnEventClicked(object sender, EventArgs e)
        {
            var timelineEvent = (TimelineEvent)sender;
            this.SelectedPass = this.Passes.ToList().Find(x => x.Name == timelineEvent.PassParentName);
        }

        private void RunSchedule()
        {
            var aesSummary = new SchedulingSummarizer(null).SummarizeDeviceResults(this.AESDevicePickerViewModel.EnabledProfiles);
            var compressionSummary = new SchedulingSummarizer(null).SummarizeDeviceResults(this.CompressionDevicePickerViewModel.EnabledProfiles);

            this.LastSolution = this.SelectedAlgorithm.Solve(this.Passes, aesSummary, compressionSummary, this.BatteryEditorViewModel.Battery);

            // Make sure the sidebar updates with new status icons
            // PassOrbit and its phases don't use INotifyPropChanged to bubble up notifications
            // and since this is the only place it will ever change, just re-render the entire
            // list at once instead of piecemeal anyways.
            CollectionViewSource.GetDefaultView(this.Passes).Refresh();

            this.InitEditControl();

            var hasWarnings = this.LastSolution.Problems.Exists(x => x.Level == ScheduleSolution.SchedulerProblem.SeverityLevel.Warning);
            var hasError = this.LastSolution.Problems.Exists(x => x.Level == ScheduleSolution.SchedulerProblem.SeverityLevel.Error);
            var hasFatal = this.LastSolution.Problems.Exists(x => x.Level == ScheduleSolution.SchedulerProblem.SeverityLevel.Fatal);

            var warningIcon = Application.Current.Resources["VS2017Icons.StatusWarning"];
            var failedIcon = Application.Current.Resources["VS2017Icons.TestCoveringFailed"];
            var successIcon = Application.Current.Resources["VS2017Icons.TestCoveringPassed"];

            if (hasError)
            {
                this.ScheduleStatusIcon = failedIcon;
            }
            else if (hasFatal)
            {
                this.ScheduleStatusIcon = failedIcon;
            }
            else if (!this.LastSolution.IsSolvable)
            {
                // ??? not solvable but no errors?
                this.ScheduleStatusIcon = failedIcon;
            }
            else if (hasWarnings)
            {
                this.ScheduleStatusIcon = warningIcon;
            }
            else
            {
                // Worked okay
                this.ScheduleStatusIcon = successIcon;
            }

            // Update the History graph with the new data, even if the schedule failed
            this.HistoryGraphViewModel.Battery = this.BatteryEditorViewModel.Battery;
            this.HistoryGraphViewModel.Passes = this.Passes;
        }

        private void SaveScheduleHandler()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Scheduler Json Documents|*.sjn",
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                var data = new SavedSchedule()
                {
                    Passes = this.Passes,
                    Battery = this.BatteryEditorViewModel.Battery,
                    SolarPanel = this.SolarCellEditorViewModel.SolarPanel,
                };

                var settings = new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                };

                var result = JsonConvert.SerializeObject(data, settings);
                File.WriteAllText(saveFileDialog.FileName, result);
            }
        }

        private void OpenScheduleHandler()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Scheduler Json Documents|*.sjn",
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var settings = new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                };

                var result = JsonConvert.DeserializeObject<SavedSchedule>(File.ReadAllText(openFileDialog.FileName), settings);

                this.Passes.Clear();
                foreach (var pass in result.Passes)
                {
                    this.Passes.Add(pass);
                }

                this.BatteryEditorViewModel.Battery = result.Battery;
                this.SolarCellEditorViewModel.SolarPanel = result.SolarPanel;

                this.RunSchedule();
            }
        }

        private FlowDocument GenerateReport()
        {
            return Reporting.ReportGenerator.GenerateReport(this.Passes, this.BatteryEditorViewModel.Battery, this.SolarCellEditorViewModel.SolarPanel, this.LastSolution);
        }

        private void ExportReportXPSHandler()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Microsoft XPS Document|*.xps",
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                Reporting.ReportIO.SaveAsXps(saveFileDialog.FileName, this.GenerateReport());
            }
        }

        private void ExportReportPDFHandler()
        {
            Reporting.ReportIO.PrintToPdf(this.GenerateReport());
        }

        private void ExportDatabaseHandler()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Database|*.db",
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                var settings = SimpleIoc.Default.GetInstance<SettingsManager>();
                File.Copy(settings.CoreSettings.DatabaseLocation, saveFileDialog.FileName);
            }
        }

        private void ImportDatabaseHandler()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Database|*.db",
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var settings = SimpleIoc.Default.GetInstance<SettingsManager>();
                File.Delete(settings.CoreSettings.DatabaseLocation);
                File.Copy(openFileDialog.FileName, settings.CoreSettings.DatabaseLocation);

                MessageBox.Show("Database Import Completed. Please Reboot To Reload New Data!", "Scheduler Application", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}