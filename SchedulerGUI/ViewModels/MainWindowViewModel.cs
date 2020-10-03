using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Forms.Design;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SchedulerGUI.Models;
using SchedulerGUI.ViewModels.Controls;
using SchedulerGUI.Views;

namespace SchedulerGUI.ViewModels
{
    /// <summary>
    /// <see cref="MainWindowViewModel"/> provides the top-level View-Model for the Scheduler application, and is statically bound to the <see cref="Views.MainWindow"/> view.
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        private static readonly Random Random = new Random();

        private PassData selectedPass;

        public MainWindowViewModel()
        {
            this.Passes = new ObservableCollection<PassData>();

            this.EditCommand = new RelayCommand(this.EditClickedHandler);
            this.AddCommand = new RelayCommand(this.AddClickedHandler);

            this.DialogManager = new PopupViewModel()
            {
                ClosePopup = new RelayCommand(() => this.DialogManager.PopupDialog = null, true),
                EasyClosePopup = null, // Leave EasyClose off for now,
                PopupDialog = null,
            };

            // Test
            this.Passes.Add(new PassData("pass #1", new DateTime(2016, 7, 15, 0, 0, 0), new DateTime(2016, 7, 15, 2, 7, 59)));
            this.Passes.Add(new PassData("pass #2", new DateTime(2016, 7, 15, 2, 7, 59), new DateTime(2016, 7, 15, 4, 15, 58)));
            this.Passes.Add(new PassData("pass #3", new DateTime(2016, 7, 15, 2, 7, 59), new DateTime(2016, 7, 15, 6, 15, 58)));
            this.Passes.Add(new PassData("pass #4", new DateTime(2016, 7, 15, 2, 7, 59), new DateTime(2016, 7, 15, 8, 15, 58)));

            PassData temp = this.Passes.ElementAt<PassData>(1);
            temp.Sunlight.Duration = new TimeSpan(1, 0, 0);
            temp.Mission.Duration = new TimeSpan(0, 30, 0);
            temp.Encryption.Duration = new TimeSpan(0, 30, 0);
            temp.Datalink.Duration = new TimeSpan(0, 8, 0);

            this.SelectedPass = this.Passes[0];
        }

        /// <summary>
        /// Gets the passes that are currently available for scheduling or editing.
        /// </summary>
        public ObservableCollection<PassData> Passes { get; }

        /// <summary>
        /// Gets or sets the pass item that is currently selected.
        /// </summary>
        public PassData SelectedPass
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
        /// Gets the Dialog Manager for the main window.
        /// </summary>
        public PopupViewModel DialogManager { get; }

        private static List<int> GenerateRandomOrder()
        {
            // generate count random values.
            HashSet<int> candidates = new HashSet<int>();
            while (candidates.Count < 4)
            {
                // May strike a duplicate.
                candidates.Add(Random.Next(0, 4));
            }

            // load them in to a list.
            List<int> result = new List<int>();
            result.AddRange(candidates);

            // shuffle the results:
            int i = result.Count;
            while (i > 1)
            {
                i--;
                int k = Random.Next(i + 1);
                int value = result[k];
                result[k] = result[i];
                result[i] = value;
            }

            return result;
        }

        private void EditClickedHandler()
        {
            void SaveCommand(PassData passData)
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
            DateTime startdate = new DateTime(moment.Year, moment.Month, moment.Day, moment.Hour, moment.Minute, 0);
            DateTime endDate = startdate.AddMinutes(128);
            this.Passes.Add(new PassData("pass #" + (this.Passes.Count + 1), startdate, endDate));
            PassData temp = this.Passes[this.Passes.Count - 1];

            OrderedDictionary phaseTimes = this.RandomPhaseTimes();
            int totalMins = 0;
            foreach (DictionaryEntry entry in phaseTimes)
            {
                int runningMin = 0;
                int runningHour = 0;
                runningMin = (int)entry.Value;
                totalMins += runningMin;
                decimal y = runningMin / 60;
                runningHour = (int)Math.Floor(y);
                runningMin -= runningHour * 60;

                switch (entry.Key.ToString())
                {
                    case "sunlight":
                        temp.Sunlight.Duration = new TimeSpan(runningHour, runningMin, 0);
                        break;

                    case "mission":
                        temp.Mission.Duration = new TimeSpan(runningHour, runningMin, 0);
                        break;

                    case "encryption":
                        temp.Encryption.Duration = new TimeSpan(runningHour, runningMin, 0);
                        break;

                    case "Datalink":
                        temp.Datalink.Duration = new TimeSpan(runningHour, runningMin, 0);
                        break;

                    default:
                        break;
                }
            }

            // temp.sunlight.Duration = new TimeSpan(1, 0, 0);
            // temp.mission.Duration = new TimeSpan(0, 30, 0);
            // temp.encryption.Duration = new TimeSpan(0, 30, 0);
            // temp.Datalink.Duration = new TimeSpan(0, 8, 0);
        }

        private OrderedDictionary RandomPhaseTimes()
        {
            OrderedDictionary phases = new OrderedDictionary();
            phases.Add("sunlight", 0);
            phases.Add("mission", 0);
            phases.Add("encryption", 0);
            phases.Add("Datalink", 0);
            int timeLeftMins = 128;
            List<int> phaseOrder = GenerateRandomOrder();
            int i = 3;
            foreach (int place in phaseOrder)
            {
                if (i == 0)
                {
                    phases[place] = timeLeftMins;
                    break;
                }

                int phaseTime = 0;
                if (timeLeftMins == 128)
                {
                    phaseTime = Random.Next(0, 64);
                }
                else
                {
                    phaseTime = Random.Next(0, timeLeftMins - i);
                }

                phases[place] = phaseTime;
                timeLeftMins -= phaseTime;
                i--;
            }

            return phases;
        }
    }
}