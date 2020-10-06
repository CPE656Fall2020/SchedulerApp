/* 
 * Copyright (c) 2010, Andriy Syrov
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without modification, are permitted provided 
 * that the following conditions are met:
 * 
 * Redistributions of source code must retain the above copyright notice, this list of conditions and the 
 * following disclaimer.
 * 
 * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and 
 * the following disclaimer in the documentation and/or other materials provided with the distribution.
 *
 * Neither the name of Andriy Syrov nor the names of his contributors may be used to endorse or promote 
 * products derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED 
 * WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
 * PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY 
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED 
 * TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, 
 * OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN 
 * IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 
 *   
 */

namespace TimelineLibrary
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Xml.Linq;
    using System.Collections.ObjectModel;

    public delegate void TimelineEventDelegate(FrameworkElement element, TimelineDisplayEvent de);
    public delegate void ScrollViewpointChanged(object sender, double value);
    public delegate void DoubleClick(DateTime date, Point point);

    /// <summary>
    /// Main container class for TimelineBands. It is inherited from Grid, so that timeline band 
    /// can be places one under another and main band can be maximized.</summary>
    /// 
    /// Timeline tray is the main screen control.  Timeline bands are rows within the timeline grid
    public class TimelineTray: Grid, ITimelineToolboxTarget, INotifyPropertyChanged
    {
        // Used for tracing in debug version
        private const string                            FMT_TRACE = "Name: {0}, Size:{1},{2}";
        
        // Default 'more' link text displayed in events of main band
        private const string                            MORE_LINK_TEXT = " More...";

        // Default teaser length of events in main band
        private const int                               DEFAULT_TEASER_SIZE = 80;

        // These default constants are used for scripting
        private const int                               DEFAULT_MAIN_EVENT_SIZE = 70;
        private const int                               DEFAULT_SECONDARY_EVENT_SIZE = 4;

        // Default description width        
        private const double                            DEFAULT_DESCRIPTION_WIDTH = 226;

        private List<TimelineBand>                      m_bands;
        private TimelineEventStore                      m_eventStore;
        private TimelineEventStore                      m_eventStorePhases;
        private bool                                    m_changingDate;
        private DataControlNotifier                     m_notifier;   
        private string                                  m_cultureId = DateTimeConverter.DEFAULT_CULTUREID;    
        private DateTime                                m_currentDateTime;
        private bool                                    m_loaded;
        private double                                  m_offset = 0;

		/// <summary>
		/// Fires when the timeline control becomes ready.  This event is perfect for using 
        /// the <see cref="ResetEvents" /> method to load events into the timeline.
		/// </summary>
        public event EventHandler                       TimelineReady;
        public event EventHandler                       TimelineUpdated;
		public event EventHandler                       CurrentDateChanged;
        public event EventHandler                       SelectionChanged;
        public event DoubleClick                        TimelineDoubleClick;
        public event ScrollViewpointChanged             ScrollViewChanged;
        public event EventHandler                       EventsRefreshed;

        // This event fires every time visual element is created for TimelineEvent because
        // it is inside visible area of timeline or about to be.
        public event TimelineEventDelegate              OnEventCreated;

        // This event fires every time visual element is deleted for TimelineEvent 
        // because it is outside visible area of timeline
        public event TimelineEventDelegate              OnEventDeleted;

        // This event fires when event is in visible area of the timeline screen 
        public event TimelineEventDelegate              OnEventVisible;

        public TimelineTray()
        {
            m_bands = new List<TimelineBand>();
            m_eventStore = new TimelineEventStore(new List<TimelineEvent>());
            MoreLinkText = MORE_LINK_TEXT;

            this.Loaded += OnControlLoaded;
            this.SizeChanged += OnSizeChanged;
			this.MouseWheel += OnMouseWheel;
        }

        /// <summary>
        /// Gets or sets current date of all timeline bands. Current date is in the middle of 
        /// each timeline band. This property can be changed from code behind to programmatically 
        /// move timelines back and forth</summary>
        public static readonly DependencyProperty CurrentDateTimeProperty =
            DependencyProperty.Register("CurrentDateTime", typeof(DateTime), 
            typeof(TimelineTray), new PropertyMetadata(DateTime.Now, OnCurrentDateTimeChanged));

        [TypeConverter(typeof(DateTimeConverter))]
        public DateTime CurrentDateTime
        {
            get
            {
                return (DateTime) GetValue(CurrentDateTimeProperty);
            }
            set
            {
                SetValue(CurrentDateTimeProperty, value);
            }
        }

        public static void OnCurrentDateTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimelineTray t= d as TimelineTray;

            if (t != null && e.NewValue != e.OldValue)
            {
                if ((DateTime) e.NewValue < t.MinDateTime)
                {
                    t.SetValue(CurrentDateTimeProperty, t.MinDateTime);
                }
                else if ((DateTime) e.NewValue > t.MaxDateTime)
                {
                    t.SetValue(CurrentDateTimeProperty, t.MaxDateTime);
                }
                else 
                {
                    t.m_currentDateTime = (DateTime) e.NewValue;
                    if (t.MainBand != null)
                    {
                        t.MainBand.CurrentDateTime = t.m_currentDateTime;
                        t.m_currentDateTime = t.MainBand.CurrentDateTime;
                    }
                }
            }
        }

        public static readonly DependencyProperty EventsProperty =
            DependencyProperty.Register("Events", typeof(List<TimelineEvent>),
            typeof(TimelineTray));

        public List<TimelineEvent> Events
        {
            get
            {
                return (List<TimelineEvent>)GetValue(EventsProperty);
            }
            set
            {
                SetValue(EventsProperty, value);
                m_eventStore = new TimelineEventStore(Events);
            }
        }

        public static readonly DependencyProperty MinDateTimeProperty =
            DependencyProperty.Register("MinDateTime", typeof(DateTime), 
            typeof(TimelineTray), new PropertyMetadata(DateTime.MinValue, 
                OnMinDateTimeChanged));

        [TypeConverter(typeof(DateTimeConverter))]
        public DateTime MinDateTime
        {
            get
            {
                return (DateTime) GetValue(MinDateTimeProperty);
            }
            set
            {
                SetValue(MinDateTimeProperty, value);
            }
        }

        public static void OnMinDateTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimelineTray t= d as TimelineTray;

            if (t != null && e.NewValue != e.OldValue)
            {
                if ((DateTime) e.NewValue >= t.MaxDateTime)
                {
                    throw new ArgumentOutOfRangeException("MinDateTime cannot be more then MaxDateTime");
                }
                else if (t.CurrentDateTime < (DateTime) e.NewValue)
                {
                    t.SetValue(CurrentDateTimeProperty, e.NewValue);
                }

                foreach (TimelineBand b in t.m_bands)
                {
                    if (b.Calculator != null && b.Calculator.Calendar != null)
                    {
                        b.Calculator.Calendar.MinDateTime = ((DateTime) e.NewValue);
                    }
                }
            }
        }

        public static readonly DependencyProperty MaxDateTimeProperty =
            DependencyProperty.Register("MaxDateTime", typeof(DateTime), 
            typeof(TimelineTray), new PropertyMetadata(DateTime.MaxValue, OnMaxDateTimeChanged));

        [TypeConverter(typeof(DateTimeConverter))]
        public DateTime MaxDateTime
        {
            get
            {
                return (DateTime) GetValue(MaxDateTimeProperty);
            }
            set
            {
                SetValue(MaxDateTimeProperty, value);
            }
        }

        public static void OnMaxDateTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimelineTray t = d as TimelineTray;

            if (t != null && e.NewValue != e.OldValue)
            {
                if ((DateTime) e.NewValue < t.MinDateTime)
                {
                    throw new ArgumentOutOfRangeException("MaxDateTime cannot be less then MinDateTime");
                }
                else if (t.CurrentDateTime >= (DateTime) e.NewValue)
                {
                    t.SetValue(CurrentDateTimeProperty, e.NewValue);
                }

                foreach (TimelineBand b in t.m_bands)
                {
                    if (b.Calculator != null && b.Calculator.Calendar != null)
                    {
                        b.Calculator.Calendar.MaxDateTime = ((DateTime) e.NewValue);
                    }
                }
            }
        }

        public static readonly DependencyProperty TeaserSizeProperty =
            DependencyProperty.Register("TeaserSize", typeof(int), 
            typeof(TimelineTray), new PropertyMetadata(DEFAULT_TEASER_SIZE));

        public int TeaserSize
        {
            get
            {
                return (int) GetValue(TeaserSizeProperty);
            }
            set
            {
                SetValue(TeaserSizeProperty, value);
            }
        }

        public static readonly DependencyProperty ImmediateDisplayProperty =
            DependencyProperty.Register("ImmediateDisplay", typeof(bool), 
            typeof(TimelineTray), new PropertyMetadata(true));

        public bool ImmediateDisplay
        {
            get
            {
                return (bool) GetValue(ImmediateDisplayProperty);
            }
            set
            {
                SetValue(ImmediateDisplayProperty, value);
            }
        }

        public double EventCanvasOffset
        {
            get
            {
                return m_offset;
            }

            set
            {
                MainBand.CanvasScrollOffset = value;
                m_offset = value;
            }
        }

        /// <summary>
        /// Specifies if vertical event positions should be recalculated when 
        /// timeline is resized. If false event positions calculated only once when
        /// timeline tray is displayed</summary>
        public bool RecalculateEventTopPosition { get; set; } = true;

        public TimelineBand MainBand { get; private set; }

        /// <summary>
        /// Return list of loaded timeline events</summary>
        public List<TimelineEvent> TimelineEvents
        {
            get
            {
                return m_eventStore.All;
            }
        }

        /// <summary>
        /// Default width of of description element of main timeline band</summary>
        public double DescriptionWidth { get; set; } = DEFAULT_DESCRIPTION_WIDTH;

		/// <summary>
		/// Resets the timeline using a list of events to provide to the UI.
		/// </summary>
		/// <param name="events">The events to reset the UI with.</param>
        public void ResetEvents(List<TimelineEvent> passEvents, bool fixDates = true)
        //public void ResetEvents(List<TimelineEvent> passEvents, List<TimelineEvent> phaseEvents, bool fixDates = true)
        {
            // fix event dates
            if (fixDates)
            { 
                foreach (TimelineEvent e in passEvents)
                {
                    if (!e.IsDuration)
                    {
                        e.EndDate = e.StartDate;
                    }
                }

                //foreach (TimelineEvent e in phaseEvents)
                //{
                //    if (!e.IsDuration)
                //    {
                //        e.EndDate = e.StartDate;
                //    }
                //}
            }

            m_eventStore = new TimelineEventStore(passEvents);
            //m_eventStorePhases = new TimelineEventStore(phaseEvents);

            ClearSelection();
            RefreshEvents();
        }

        private void ClearSelection()
        {
            if (MainBand != null && MainBand.Selection != null)
            {
                MainBand.Selection.Clear();
                SelectedTimelineEvents.Clear();

                PropertyChanged(this, new PropertyChangedEventArgs("SelectedTimelineEvents"));
                SelectionChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Removes all events from all timeline bands</summary>
        public void ClearEvents()
        {
            m_eventStore = new TimelineEventStore(new List<TimelineEvent>());
            RefreshEvents();
        }

        /// <summary>
        /// This link text is displayed if description on the event is more then specified by
        /// TeaserSize property</summary>
        public string MoreLinkText { get; set; }

        /// <summary>
        /// Refresh (delete and recreate and redisplay) all events on all timeline bands.</summary>
        public void RefreshEvents()
        {
            RefreshEvents(true);
        }

        public void RefreshEvents(bool checkInit)
        {
            if (IsTimelineInitialized || !checkInit)
            {
                Utilities.Trace(this);

                foreach (TimelineBand b in m_bands)
                {
                    Debug.Assert(b.Calculator != null);

                    b.ClearEvents();

                    if (b.IsMainBand)
                        b.EventStore = m_eventStore;
                    //else
                    //    b.EventStore = m_eventStorePhases;
                }

                MainBand.CalculateEventRows();
            
                foreach (TimelineBand b in m_bands)
                {
                    b.CalculateEventPositions();
                }

                foreach (TimelineBand b in m_bands)
                {
                    b.DisplayEvents();
                }

                TimelineUpdated?.Invoke(this, EventArgs.Empty);
            }

            EventsRefreshed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// This method is used for debugging (from Utilities.Trace)</summary>
        public override string ToString()
        {
            return string.Format(FMT_TRACE, Name, ActualHeight, ActualWidth);
        }

        /// <summary>
        /// Specifies that control is initialized, which means that all bands are provided,
        /// all controls resized and xml data are read. Some of the properties and methods
        /// cannot be executed once control is initialized.</summary>
        public bool IsTimelineInitialized { get; private set; }

        /// <summary>
        /// Calendar type (see TimelineCalendar.CalendarFromString property for the list of 
        /// values). This value cannot be changed after control is initialized.</summary>
        public string CalendarType
        {
            get
            {
                return DateTimeConverter.CalendarName;
            }
            set
            {
                DateTimeConverter.CalendarName = value;
            }
        }

        /// <summary>
        /// Culture ID specified what Date format is used, by default we use 'en-US'</summary>
        public string CultureID
        {
            get
            {
                return m_cultureId;
            }
            set
            {
                Debug.Assert(value != null && value.Length > 0);
                
                m_cultureId = value;
                DateTimeConverter.CultureId = m_cultureId;
            }
        }

        /// <summary>
        /// This property is used for DateTime calculations and is based on CultureID</summary>
        public CultureInfo CultureInfo
        {
            get
            {
                return DateTimeConverter.CultureInfo;
            }
        }

        /// <summary>   		 
	    /// Returns the currently visible TimelineEvents   		 
	    /// </summary>   		 
        public IEnumerable<TimelineEvent> VisibleTimeEvents   		 
        {   		 
            get   		 
            {   		 
    			if (MainBand != null)
                {
		    		return MainBand.Calculator?.VisibleTimelineEvents;
                }   		 
                return null;   		 
            }   		 
        }

        /// <summary>   		 
        /// Returns the currently selected TimelineEvents   		 
        /// </summary>   		 
        public ObservableCollection<TimelineEvent> SelectedTimelineEvents { get; } = new ObservableCollection<TimelineEvent>();

        /// <summary>
        /// Zooms in/out all timeline bands by one column</summary>
        public void Zoom(bool zoomIn)
        {
			Zoom(zoomIn, 1);
		}

		/// <summary>
		/// Zoom in/out all timeline bands by the number of columns defined by zoomValue
		/// </summary>
		/// <param name="zoomIn">true to zoom in, false to zoom out</param>
		/// <param name="zoomValue">the number of columns to zoom</param>
		private void Zoom(bool zoomIn, int zoomValue)
		{
            int inc;
            TimeSpan visibleWindow;

            inc = zoomIn ? -1 : 1;
			inc *= zoomValue;

			if (this.IsTimelineInitialized)
            {
				foreach (TimelineBand band in this.m_bands)
                {
                    if (band.TimelineWindowSize + inc >= 2)
                    {
                        band.TimelineWindowSize += inc;
                    }
                    band.Calculator.DisplayEvents(true);
                }

				visibleWindow = this.MainBand.Calculator.MaxVisibleDateTime -
				                this.MainBand.Calculator.MinVisibleDateTime;

				foreach (TimelineBand band in this.m_bands)
                {
                    band.VisibleTimeSpan = visibleWindow;
                    band.ResetVisibleDaysHighlight();
                }
            }
        }

        /// <summary>
        /// Loads data and displays them in the control</summary>
        public void Run()
        {
            Utilities.Trace(this);
            Reload();
        }

        /// <summary>
        /// Add new timeline band</summary>
        public void AddTimelineBand(int height, bool isMain, string srcType, int columnsCount)
        {
            AddTimelineBand(height, isMain, srcType, columnsCount, 
                isMain ? DEFAULT_MAIN_EVENT_SIZE : DEFAULT_SECONDARY_EVENT_SIZE);
        }

        public void AddTimelineToolbox()
        {
            TimelineToolbox toolbox = new TimelineToolbox();
            toolbox.SetSite(this);

            RowDefinition rd = new RowDefinition();

            if (toolbox.DesiredSize.Height > 0)
            {
                rd.Height = new GridLength(toolbox.DesiredSize.Height);
            }
            else
            {
                rd.Height = new GridLength(0, GridUnitType.Auto);
            }

            this.RowDefinitions.Add(rd);
            toolbox.SetValue(RowProperty, this.RowDefinitions.Count() - 1);

            this.Children.Add(toolbox);
        }

        public void AddTimelineBand(int height, bool isMain, string srcType, int columnsCount, int eventSize)
        {
            TimelineBand band = new TimelineBand();
            RowDefinition rd;

            if (m_notifier != null)
            {
                m_notifier.AddElement(band);
            }

            band.IsMainBand = isMain;
            band.ItemSourceType = srcType;

            rd = new RowDefinition();
            
            if (height > 0)
            {
                rd.Height = new GridLength((double) height);
                band.Height = height;
            }
            else
            {
                rd.Height = new GridLength(1.0, GridUnitType.Star);
            }

            this.RowDefinitions.Add(rd);

            band.SetValue(RowProperty, this.RowDefinitions.Count() - 1);
            band.Margin = new Thickness(0.0);
            band.TimelineWindowSize = columnsCount;
            band.MaxEventHeight = eventSize;
            band.TimelineTray = this;

            if (band.IsMainBand)
            {
                MainBand = band;
            }

            m_bands.Add(band);
            this.Children.Add(band);
        }

        private void OnFullScreenChanged(object sender, EventArgs e)
        {
            Utilities.Trace(this);

            if (IsTimelineInitialized)
            {
                RefreshEvents();
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Utilities.Trace(this);

            if (IsTimelineInitialized)
            {
                RefreshEvents();
            }
        }

		private void OnMouseWheel(object sender, MouseWheelEventArgs e)
		{
			if (Keyboard.Modifiers == ModifierKeys.Control)
			{
				Zoom(e.Delta > 0, Math.Abs(e.Delta / 120));
			}
		}

        /// <summary>
        /// The user moved current datetime on one of the timeline bands, so we 
        /// sync all other bands with it.</summary>
        private void OnCurrentDateChanged(object sender, RoutedEventArgs e)
        {
            TimelineBand band;

            if (!m_changingDate)
            {
                try
                {
                    m_changingDate = true;

                    band = (TimelineBand) sender;
                    m_currentDateTime = band.CurrentDateTime;

                    m_bands.ForEach(b => 
                    {
                        if (sender != b) 
                        { 
                            b.CurrentDateTime = band.CurrentDateTime; 
                        }
                    });

                    CurrentDateChanged?.Invoke(this, EventArgs.Empty);
                }
                finally
                {
                    m_changingDate = false;
                }
            }
        }

        public virtual TimelineEvent CreateEvent()
        {
            return new TimelineEvent();
        }

        public void Reload()
        {
            if (m_bands.Count > 0 && MainBand != null)
            {
                m_notifier = new DataControlNotifier(m_bands);
                m_notifier.LoadComplete += OnControlAndDataComlete;
                m_notifier.Start();
                m_notifier.CheckCompleted();
            }
        }

        private void OnControlLoaded(object sender, RoutedEventArgs e)
        {
            Utilities.Trace(this);
            
            if (!m_loaded) 
            {
                HookChildElements(this.Children);
        
                if (m_bands.Count > 0 && MainBand != null)
                {
                    m_notifier = new DataControlNotifier(m_bands);
                    m_notifier.LoadComplete += OnControlAndDataComlete; 
                    m_notifier.Start();
                    m_notifier.CheckCompleted();
                }
                m_loaded = true;
            }

            // Do not remove copyright notice
            AddCopyrightElement();
        }

        /// <summary>
        /// Do not remove copyright notice.</summary>
        private void AddCopyrightElement()
        {}

        protected void HookChildElements(UIElementCollection col)
        {
            TimelineBand b;

            foreach (UIElement  el in col)
            {
                if (el as TimelineBand != null)
                {
                    b = (TimelineBand) el;
                    b.TimelineTray = this;

                    if (b.IsMainBand)
                    {
                        MainBand = b;
                    }

                    m_bands.Add(b);
                }
                else if (el as TimelineToolbox != null)
                {
                    ((TimelineToolbox) el).SetSite(this);
                }
                else if (el as Panel != null)
                {
                    HookChildElements(((el) as Panel).Children);
                }
            }        
        }

        /// <summary>
        /// This happens when we have all data and all timeline band controls have 
        /// completed resizing, so we ready to show content</summary>
        private void OnControlAndDataComlete(object sender, EventArgs e)
        {
            TimeSpan visibleWindow;
            List<TimelineEvent> events = new List<TimelineEvent>();

            TimelineDisplayEvent.MoreLinkText = MoreLinkText;
            TimelineDisplayEvent.TeaserSize = TeaserSize;

            m_currentDateTime = CurrentDateTime;

            if (MainBand == null)
            {
                throw new Exception("At least one main timeline band should be specified");
            }

            MainBand.CreateTimelineCalculator(CalendarType, CurrentDateTime, MinDateTime, MaxDateTime);

            m_bands.ForEach(b => b.CreateTimelineCalculator(CalendarType, CurrentDateTime, MinDateTime, MaxDateTime));

            // now we need to calculate visible timeline window and assign it to all timelineband controls
            visibleWindow = MainBand.Calculator.MaxVisibleDateTime - MainBand.Calculator.MinVisibleDateTime;

            foreach (TimelineBand band in m_bands)
            {
                band.VisibleTimeSpan = visibleWindow;
                band.ResetVisibleDaysHighlight();

                band.Calculator.BuildColumns();

                band.OnCurrentDateChanged += OnCurrentDateChanged;

                if (band.IsMainBand)
                {
                    band.OnSelectionChanged += OnSelectionChanged;
                }
            }

            m_notifier = null;
            IsTimelineInitialized = true;

            RefreshEvents(false);

            TimelineReady?.Invoke(this, new EventArgs());
        }

        protected virtual void Initialized()
        {}

        /// <summary>
        /// Attribute value reader helper.</summary>
        protected static string GetAttribute(XAttribute a)
        {
            return a == null ? string.Empty : a.Value;
        }

        /// <summary>
        /// Returns content of element as xml</summary>
        protected static string GetContent(XElement e)
        {
            return (e.FirstNode == null ? string.Empty : e.FirstNode.ToString());
        }

        /// <summary>
        /// Sort function for events by startdate</summary>
        private static int CompareEvents(TimelineEvent a, TimelineEvent b)
        {
            int ret;

            Debug.Assert(a != null);
            Debug.Assert(b != null);

            if (a.StartDate == b.StartDate)
            {
                ret = 0;
            }
            else if (a.StartDate < b.StartDate)
            {
                ret = -1;
            }
            else
            {
                ret = 1;
            }

            return ret;
        }

        void ITimelineToolboxTarget.FindMinDate()
        {
            this.CurrentDateTime = this.MinDateTime;
        }

        void ITimelineToolboxTarget.FindMaxDate()
        {
            this.CurrentDateTime = this.MaxDateTime;
        }

        void ITimelineToolboxTarget.FindDate(DateTime date)
        {
            this.CurrentDateTime = date;
        }

        void ITimelineToolboxTarget.MoveLeft()
        {
            try
            {
                this.CurrentDateTime -= MainBand.Calculator.ColumnTimeWidth;
            }
            catch (ArgumentOutOfRangeException)
            {
                this.CurrentDateTime = this.MinDateTime;
            }
        }

        void ITimelineToolboxTarget.MoveRight()
        {
            try
            {
                this.CurrentDateTime += MainBand.Calculator.ColumnTimeWidth;
            }
            catch (ArgumentOutOfRangeException)
            {
                this.CurrentDateTime = this.MaxDateTime;
            }
        }

        void ITimelineToolboxTarget.ZoomIn()
        {
            this.Zoom(true);
        }

        void ITimelineToolboxTarget.ZoomOut()
        {
            this.Zoom(false);
        }

        private void OnSelectionChanged(object sender, EventArgs e)
        {
            SelectedTimelineEvents.Clear();
            if (MainBand != null)
            {
                MainBand.Selection.ToList().ForEach((ev) => SelectedTimelineEvents.Add(ev.Event));
            }

            PropertyChanged(this,new PropertyChangedEventArgs("SelectedTimelineEvents"));
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }

        internal void FireEventCreated(FrameworkElement element, TimelineDisplayEvent de)
        {
            OnEventCreated?.Invoke(element, de);
        }

        internal void FireEventDeleted(FrameworkElement element, TimelineDisplayEvent de)
        {
            OnEventDeleted?.Invoke(element, de);
        }

        internal void FireOnEventVisible(FrameworkElement element, TimelineDisplayEvent de)
        {
            OnEventVisible?.Invoke(element, de);
        }

        internal void FireScrollViewChanged(double height)
        {
            ScrollViewChanged?.Invoke(this, height);
        }

        internal void FireTimelineDoubleClick(DateTime date, Point point)
        {
            TimelineDoubleClick?.Invoke(date, point);
        }

        public event PropertyChangedEventHandler  PropertyChanged = delegate {};
    }
}
