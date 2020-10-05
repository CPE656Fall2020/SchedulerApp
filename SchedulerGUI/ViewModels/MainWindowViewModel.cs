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
using SchedulerGUI.Models;
using SchedulerGUI.Services;
using SchedulerGUI.Settings;
using SchedulerGUI.ViewModels.Controls;
using SchedulerGUI.Views;

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

        public MainWindowViewModel()
        {
            // Setup shared services like DbContext and SettingsManager for the entire application.
            this.Startup();

            this.Passes = new ObservableCollection<PassOrbit>();

            this.EditCommand = new RelayCommand(this.EditClickedHandler);
            this.AddCommand = new RelayCommand(this.AddClickedHandler);
            this.OpenSchedulerPlotterCommand = new RelayCommand(this.OpenSchedulerPlotterHandler);
            this.OpenImportToolGUICommand = new RelayCommand(this.OpenImportToolGUIHandler);
            this.OpenImportToolCLICommand = new RelayCommand(this.OpenImportToolCLIHandler);

            this.DialogManager = new PopupViewModel()
            {
                ClosePopup = new RelayCommand(() => this.DialogManager.PopupDialog = null, true),
                EasyClosePopup = null, // Leave EasyClose off for now,
                PopupDialog = null,
            };

            this.InitPasses();
        }

        /// <summary>
        /// Gets the passes that are currently available for scheduling or editing.
        /// </summary>
        public ObservableCollection<PassOrbit> Passes { get; }

        /// <summary>
        /// Gets or sets the pass item that is currently selected.
        /// </summary>
        public PassOrbit SelectedPass
        {
            get => this.selectedPass;
            set => this.Set(() => this.SelectedPass, ref this.selectedPass, value);
        }

        /// <summary>
        /// Gets the command to execute when the edit button is selected.
        /// </summary>
        public ICommand EditCommand { get; }

        /// <summary>
        /// Gets the command to execute when the add button is selected.
        /// </summary>
        public ICommand AddCommand { get; }

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
        /// Gets the Dialog Manager for the main window.
        /// </summary>
        public PopupViewModel DialogManager { get; }

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

        private void EditClickedHandler()
        {
            void SaveCommand(PassOrbit passData)
            {
                var currentIndex = this.Passes.IndexOf(this.SelectedPass);
                this.Passes[currentIndex] = passData;
                this.DialogManager.PopupDialog = null;

                // Restore the selected pass
                this.SelectedPass = this.Passes[currentIndex];
            }

            void CancelCommand()
            {
                this.DialogManager.PopupDialog = null;
            }

            var editDialog = new EditWindowViewModel(
                this.SelectedPass,
                SaveCommand,
                CancelCommand);

            this.DialogManager.PopupDialog = editDialog;
        }

        private void AddClickedHandler()
        {
            DateTime moment = DateTime.Now;
            this.Passes.Add(new PassOrbit((this.Passes.Count + 1).ToString(), moment, moment.AddMinutes(PASSDURATION)));
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
            var appDir = System.AppDomain.CurrentDomain.BaseDirectory;

            Process.Start(new ProcessStartInfo(
                "cmd.exe",
                $"/k \"set PATH=%PATH%;\"{appDir}\"\"; && cd %USERPROFILE%\\Desktop && SchedulerImportTools.exe"));
        }

        private void InitPasses()
        {
            DateTime startTime = DateTime.Now;

            for (int i = 0; i < NUMPASSES; i++)
            {
                this.Passes.Add(new PassOrbit((i + 1).ToString(), startTime, startTime.AddMinutes(PASSDURATION)));
                startTime = startTime.AddMinutes(PASSDURATION);
            }
        }
    }
}