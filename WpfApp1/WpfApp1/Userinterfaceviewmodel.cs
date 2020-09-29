using System;
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
    public class Userinterfaceviewmodel: INotifyPropertyChanged
    {
        private ObservableCollection<Passdata> _passitems = new ObservableCollection<Passdata>();
        private editwindow ew = new editwindow();

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
            _passitems.Add(new Passdata("pass #1",new DateTime(2016, 7, 15, 0, 0, 0), new DateTime(2016, 7, 15, 2, 7, 59)));
            _passitems.Add(new Passdata("pass #2", new DateTime(2016, 7, 15, 2, 7, 59), new DateTime(2016, 7, 15, 4, 15, 58)));
            Passdata temp = _passitems.ElementAt<Passdata>(1);
            temp.sunlight.Duration = new TimeSpan(1,0,0);
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
            _passitems.Add(new Passdata("pass #"+ (_passitems.Count+1), new DateTime(2016, 7, 15, 2, 7, 59), new DateTime(2016, 7, 15, 4, 15, 58)));
            Passdata temp = _passitems[_passitems.Count -1];
            temp.sunlight.Duration = new TimeSpan(1, 0, 0);
            temp.mission.Duration = new TimeSpan(0, 30, 0);
            temp.encryption.Duration = new TimeSpan(0, 30, 0);
            temp.Datalink.Duration = new TimeSpan(0, 8, 0);
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
    }
}
