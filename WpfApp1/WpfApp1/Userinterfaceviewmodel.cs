using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfApp1
{
    public class Userinterfaceviewmodel : INotifyPropertyChanged
    {
        private ObservableCollection<Passdata> _passitems = new ObservableCollection<Passdata>();
        private editwindow ew = new editwindow();
        private static Random random = new Random();

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Passdata> PassItems
        {
            get
            {
                return _passitems;
            }
        }

        public ICommand EditClick
        {
            get;
            set;
        }

        public ICommand AddClick
        {
            get;
            set;
        }

        private Passdata _SelectedPassItem;

        public Passdata SelectedPassItem
        {
            get
            {
                return _SelectedPassItem;
            }
            set
            {
                _SelectedPassItem = value;
                OnPropertyChanged("SelectedPassItem");
            }
        }

        public Userinterfaceviewmodel()
        {
            _passitems.Add(new Passdata("pass #1", new DateTime(2016, 7, 15, 0, 0, 0), new DateTime(2016, 7, 15, 2, 7, 59)));
            _passitems.Add(new Passdata("pass #2", new DateTime(2016, 7, 15, 2, 7, 59), new DateTime(2016, 7, 15, 4, 15, 58)));
            Passdata temp = _passitems.ElementAt<Passdata>(1);
            temp.sunlight.Duration = new TimeSpan(1, 0, 0);
            temp.mission.Duration = new TimeSpan(0, 30, 0);
            temp.encryption.Duration = new TimeSpan(0, 30, 0);
            temp.Datalink.Duration = new TimeSpan(0, 8, 0);
            SelectedPassItem = _passitems.ElementAt<Passdata>(0);
            PassItems.CollectionChanged += OnPassItemsCollectionChanged;
            EditClick = new RelayCommand(new Action<object>(HandleEditClicked));
            AddClick = new RelayCommand(new Action<object>(HandleAddClicked));
        }

        private void HandleEditClicked(object something)
        {
            if (!ew.IsActive)
            {
                ew = new editwindow();
                ew.DataContext = this;
                ew.Show();
            }
        }

        private void HandleAddClicked(object something)
        {
            DateTime moment = DateTime.Now;
            DateTime startdate = new DateTime(moment.Year, moment.Month, moment.Day, moment.Hour, moment.Minute, 0);
            DateTime endDate = new DateTime(moment.Year, moment.Month, moment.Day, (int)(moment.Hour) + 2, (int)(moment.Minute) + 8, 0);

            _passitems.Add(new Passdata("pass #" + (_passitems.Count + 1), new DateTime(2016, 7, 15, 2, 7, 59), new DateTime(2016, 7, 15, 4, 15, 58)));
            Passdata temp = _passitems[_passitems.Count - 1];

            OrderedDictionary phaseTimes = RandomPhaseTimes();
            int totalMins = 0;
            foreach (DictionaryEntry entry in phaseTimes)
            {
                int runningMin = 0;
                int runningHour = 0;
                runningMin = (int)entry.Value;
                totalMins += runningMin;
                decimal y = runningMin / 60;
                runningHour = (int)Math.Floor(y);
                runningMin -= ((int)runningHour * 60);

                switch (entry.Key.ToString())
                {
                    case "sunlight":
                        temp.sunlight.Duration = new TimeSpan(runningHour, runningMin, 0);
                        break;

                    case "mission":
                        temp.sunlight.Duration = new TimeSpan(runningHour, runningMin, 0);
                        break;

                    case "encryption":
                        temp.encryption.Duration = new TimeSpan(runningHour, runningMin, 0);
                        break;

                    case "Datalink":
                        temp.Datalink.Duration = new TimeSpan(runningHour, runningMin, 0);
                        break;

                    default:
                        break;
                }
            }

            //temp.sunlight.Duration = new TimeSpan(1, 0, 0);
            //temp.mission.Duration = new TimeSpan(0, 30, 0);
            //temp.encryption.Duration = new TimeSpan(0, 30, 0);
            //temp.Datalink.Duration = new TimeSpan(0, 8, 0);
        }

        private void OnPassItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged();
        }

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private DateTime dateGenerator(DateTime start, int hourOffset, int minOffset)
        {
            return new DateTime((int)(start.Year), (int)(start.Month), (int)(start.Day), (int)(start.Hour) + hourOffset, (int)(start.Minute) + minOffset, 0);
        }

        private OrderedDictionary RandomPhaseTimes()
        {
            OrderedDictionary phases = new OrderedDictionary();
            phases.Add("sunlight", 0);
            phases.Add("mission", 0);
            phases.Add("encryption", 0);
            phases.Add("Datalink", 0);
            int timeLeftMins = 128;
            List<int> randomNumbers = new List<int>(4);
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
                    phaseTime = random.Next(0, 64);
                else
                    phaseTime = random.Next(0, timeLeftMins - i);

                phases[place] = phaseTime;
                timeLeftMins -= phaseTime;
                i--;
            }
            return phases;
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
    }
}