using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfApp1
{
    public class Userinterfaceviewmodel
    {
        private List<Passdata> _passitems = new List<Passdata>();
        private editwindow ew = new editwindow();
        public List<Passdata> PassItems
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

        public Passdata SelectedPassItem
        {
            get;set;
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
            EditClick = new RelayCommand(new Action<object>(HandleEditClicked));
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
    }
}
