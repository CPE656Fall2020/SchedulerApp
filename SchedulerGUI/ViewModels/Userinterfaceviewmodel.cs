using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SchedulerGUI.Models;
using SchedulerGUI.Views;

namespace SchedulerGUI.ViewModels
{
    public class Userinterfaceviewmodel : ViewModelBase
    {
        private static readonly Random random = new Random();

        private Passdata selectedPassItem;
        private EditWindow ew = new EditWindow();

        public Userinterfaceviewmodel()
        {
            this.PassItems.Add(new Passdata("pass #1", new DateTime(2016, 7, 15, 0, 0, 0), new DateTime(2016, 7, 15, 2, 7, 59)));
            this.PassItems.Add(new Passdata("pass #2", new DateTime(2016, 7, 15, 2, 7, 59), new DateTime(2016, 7, 15, 4, 15, 58)));
            Passdata temp = this.PassItems.ElementAt<Passdata>(1);
            temp.Sunlight.Duration = new TimeSpan(1, 0, 0);
            temp.Mission.Duration = new TimeSpan(0, 30, 0);
            temp.Encryption.Duration = new TimeSpan(0, 30, 0);
            temp.Datalink.Duration = new TimeSpan(0, 8, 0);
            this.SelectedPassItem = this.PassItems[0];
            this.EditClick = new RelayCommand(this.HandleEditClicked);
            this.AddClick = new RelayCommand(this.HandleAddClicked);
        }

        public ObservableCollection<Passdata> PassItems { get; } = new ObservableCollection<Passdata>();

        public ICommand EditClick { get; }

        public ICommand AddClick { get; }

        public Passdata SelectedPassItem
        {
            get => this.selectedPassItem;
            set => this.Set(() => this.SelectedPassItem, ref this.selectedPassItem, value);
        }

        private static List<int> GenerateRandomOrder()
        {
            // generate count random values.
            HashSet<int> candidates = new HashSet<int>();
            while (candidates.Count < 4)
            {
                // May strike a duplicate.
                candidates.Add(random.Next(0, 4));
            }

            // load them in to a list.
            List<int> result = new List<int>();
            result.AddRange(candidates);

            // shuffle the results:
            int i = result.Count;
            while (i > 1)
            {
                i--;
                int k = random.Next(i + 1);
                int value = result[k];
                result[k] = result[i];
                result[i] = value;
            }

            return result;
        }

        private void HandleEditClicked()
        {
            if (!this.ew.IsActive)
            {
                this.ew = new EditWindow();
                this.ew.DataContext = this;
                this.ew.Show();
            }
        }

        private void HandleAddClicked()
        {
            DateTime moment = DateTime.Now;
            DateTime startdate = new DateTime(moment.Year, moment.Month, moment.Day, moment.Hour, moment.Minute, 0);
            DateTime endDate = startdate.AddMinutes(128);
            this.PassItems.Add(new Passdata("pass #" + (this.PassItems.Count + 1), startdate, endDate));
            Passdata temp = this.PassItems[this.PassItems.Count - 1];

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
                    phaseTime = random.Next(0, 64);
                }
                else
                {
                    phaseTime = random.Next(0, timeLeftMins - i);
                }

                phases[place] = phaseTime;
                timeLeftMins -= phaseTime;
                i--;
            }

            return phases;
        }
    }
}