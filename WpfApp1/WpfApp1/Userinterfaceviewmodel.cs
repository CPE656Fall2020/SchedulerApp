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
        public List<Passdata> PassItems
        {
            get
            {
                return _passitems;
            }
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
            temp.sunlight.startTime = new DateTime(2016, 7, 15, 2, 7, 59);
            temp.sunlight.endTime = new DateTime(2016, 7, 15, 3, 7, 59);
            temp.mission.startTime = new DateTime(2016, 7, 15, 3, 7, 59);
            temp.mission.endTime = new DateTime(2016, 7, 15, 3, 37, 59);
            temp.encryption.startTime = new DateTime(2016, 7, 15, 3, 37, 59);
            temp.encryption.endTime = new DateTime(2016, 7, 15, 4, 7, 59);
            temp.Datalink.startTime = new DateTime(2016, 7, 15, 4, 7, 59);
            temp.Datalink.endTime = new DateTime(2016, 7, 15, 5, 7, 59);
            SelectedPassItem = _passitems.ElementAt<Passdata>(0);
        }
    }
}
