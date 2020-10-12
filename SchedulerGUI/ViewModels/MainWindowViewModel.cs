using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using SchedulerDatabase;
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
    /// <see cref="MainWindowViewModel"/> provides the top-level View-Model for the Scheduler application, and bound to the <see cref="Views.MainWindow"/> view.
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        private const double NUMPASSES = 11.25;
        private const double PASSDURATION = (24 / NUMPASSES) * 60;
        private PassOrbit selectedPass;
        private DateTime startTime;
        private DateTime endTime;
        private EditControlViewModel editControlVM;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {
            // Setup shared services like DbContext and SettingsManager for the entire application.
            this.Startup();

            this.TimelineEventPasses = new ObservableCollection<TimelineEvent>();
            this.Passes = new ObservableCollection<PassOrbit>();
            this.OpenSchedulerPlotterCommand = new RelayCommand(this.OpenSchedulerPlotterHandler);
            this.OpenImportToolGUICommand = new RelayCommand(this.OpenImportToolGUIHandler);
            this.OpenImportToolCLICommand = new RelayCommand(this.OpenImportToolCLIHandler);
            this.OpenAboutCommand = new RelayCommand(this.OpenAboutHandler);

            this.DialogManager = new PopupViewModel()
            {
                ClosePopup = new RelayCommand(() => this.DialogManager.PopupDialog = null, true),
                EasyClosePopup = null, // Leave EasyClose off for now,
                PopupDialog = null,
            };

            this.DevicePickerViewModel = new DevicePickerViewModel();
            this.ScheduleSolver = new GreedyOptimizedLowPowerScheduler();

            this.Init();
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
        /// Gets or sets the pass orbit that is currently selected.
        /// </summary>
        public PassOrbit SelectedPass
        {
            get => this.selectedPass;
            set => this.Set(() => this.SelectedPass, ref this.selectedPass, value);
        }

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
        /// Gets or sets the edit control view model.
        /// </summary>
        public EditControlViewModel EditControlViewModel
        {
            get => this.editControlVM;
            set => this.Set(() => this.EditControlViewModel, ref this.editControlVM, value);
        }

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
        /// Gets the device picker control ViewModel for enabling and disabling AES devices.
        /// </summary>
        public DevicePickerViewModel DevicePickerViewModel { get; }

        private IScheduleSolver ScheduleSolver { get; set; }

        /// <summary>
        /// Initializes edit control with pass informatin from the selected pass.
        /// </summary>
        public void InitEditControl()
        {
            this.EditControlViewModel = new EditControlViewModel(this.SelectedPass, this.SaveCommand);
        }

        private void Init()
        {
            this.InitPasses();
            this.InitTimelineEvents();
        }

        /// <summary>
        /// Performs application initialization.
        /// </summary>
        private void Startup()
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

        private void SaveCommand(PassOrbit passData)
        {
            var currentIndex = this.Passes.IndexOf(this.SelectedPass);
            this.Passes[currentIndex] = passData;

            this.SelectedPass = passData;
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
                this.Passes.Add(new PassOrbit((i + 1).ToString(), startTime, startTime.AddMinutes(PASSDURATION), random));
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
    }
}