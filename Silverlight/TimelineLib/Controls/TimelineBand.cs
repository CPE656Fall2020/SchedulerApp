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
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using ColorWheel.Core;

namespace TimelineLibrary
{
    /// <summary>
    /// Timeline (which is an instance of TimelineTray class) may have several TimelineBand
    /// objects in it. Each TimelineBand may represent years, months, and so on. Events 
    /// are displayed on each timelineband</summary>
    [TemplatePart(Name = TP_CANVAS_PART, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = TP_NAVIGATE_LEFT_PART, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = TP_NAVIGATE_LEFT_BUTTON_PART, Type = typeof(Button))]
    [TemplatePart(Name = TP_NAVIGATE_RIGHT_PART, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = TP_NAVIGATE_RIGHT_BUTTON_PART, Type = typeof(Button))]
    [TemplatePart(Name = TP_VISIBLE_DATES_PART, Type = typeof(Rectangle))]
    [TemplatePart(Name = TP_MAIN_GRID_PART, Type = typeof(Grid))]
    public class TimelineBand: Control
    {
        public delegate void TimelineBandEvent(object sender, RoutedEventArgs e);
		public delegate void TimelineEventDelegate(FrameworkElement element, TimelineDisplayEvent de);
        
        public event Action<Point>                      OnCanvasDrag;
        public event Action<Point>                      OnCanvasBeginDrag;
        public event Action<Point>                      OnCanvasEndDrag;
        public event TimelineBandEvent                  OnCurrentDateChanged;
        
        private const string                            TL_TYPE_DECADES = "decades";
        private const string                            TL_TYPE_YEARS = "years";
        private const string                            TL_TYPE_MONTHS = "months";
        private const string                            TL_TYPE_DAYS = "days";
        private const string                            TL_TYPE_HOURS = "hours";
        private const string                            TL_TYPE_MINUTES = "minutes";
        private const string                            TL_TYPE_MINUTES10 = "minutes10";
        private const string                            TL_TYPE_SECONDS = "seconds";
        private const string                            TL_TYPE_MILLISECONDS100 = "milliseconds100";
        private const string                            TL_TYPE_MILLISECONDS10 = "milliseconds10";
        private const string                            TL_TYPE_MILLISECONDS = "milliseconds";
        private const string                            TP_CANVAS_PART = "CanvasPart";
        private const string                            TP_NAVIGATE_LEFT_PART = "NavigateLeft";
        private const string                            TP_NAVIGATE_LEFT_BUTTON_PART = "NavigateLeftButton";
        private const string                            TP_NAVIGATE_RIGHT_PART = "NavigateRight";
        private const string                            TP_NAVIGATE_RIGHT_BUTTON_PART = "NavigateRightButton";
        private const string                            TP_MAIN_GRID_PART = "MainGrid";
        private const string                            TP_VISIBLE_DATES_PART = "VisibleDatesPart";
        private const string                            FMT_TRACE = "Name: {0}, Type:{1}, Size:{2},{3},{4},{5}";

        // timeline columns clipping area
        private RectangleGeometry                       m_clipRect;

        // specifies if all columns already initialized
        private bool                                    m_calcInitialized;
        private string                                  m_sourceType = TL_TYPE_YEARS;
        private TimelineCalendarType                    m_timelineType;
        private string                                  m_calendarType;
        private bool                                    m_timeFormat24 = false;
        private double                                  m_offset;

        public EventHandler                             OnSelectionChanged;
        public InertialTimelineScroll                   InertialScroll;
        private TimelineTray                            m_tray;

        public static readonly DependencyProperty DefaultTextItemTemplateProperty =
            DependencyProperty.Register("DefaultTextItemTemplate", typeof(DataTemplate), 
            typeof(TimelineBand), new PropertyMetadata(DefaultTextItemTemplateChanged));

        public static void DefaultTextItemTemplateChanged(
            DependencyObject                            d, 
            DependencyPropertyChangedEventArgs          e
        )
        {
            TimelineBand                        band;
            band = (TimelineBand) d;

            if (band != null)
            {
                band.OnDefaultTextItemTemplateChanged(e); 
            }
        }

        protected virtual void OnDefaultTextItemTemplateChanged(
            DependencyPropertyChangedEventArgs          e
        )
        {
            DefaultTextItemTemplate = (DataTemplate) e.NewValue;
        }

        public DataTemplate DefaultTextItemTemplate
        {
            get
            {
                return (DataTemplate) GetValue(DefaultTextItemTemplateProperty);
            }
            set
            {
                SetValue(DefaultTextItemTemplateProperty, value);
            }
        }

        public static readonly DependencyProperty TextItemTemplateProperty =
            DependencyProperty.Register("TextItemTemplate", typeof(DataTemplate), 
            typeof(TimelineBand), new PropertyMetadata(TextItemTemplateChanged));

        public static void TextItemTemplateChanged(
            DependencyObject                            d, 
            DependencyPropertyChangedEventArgs          e
        )
        {
            TimelineBand                        band;
            band = (TimelineBand) d;

            if (band != null)
            {
                band.OnTextItemTemplateChanged(e); 
            }
        }

        protected virtual void OnTextItemTemplateChanged(
            DependencyPropertyChangedEventArgs          e
        )
        {
            TextItemTemplate = (DataTemplate) e.NewValue;
        }

        public DataTemplate TextItemTemplate
        {
            get
            {
                return (DataTemplate) GetValue(TextItemTemplateProperty);
            }
            set
            {
                SetValue(TextItemTemplateProperty, value);
            }
        }

        public static readonly DependencyProperty DefaultItemTemplateProperty =
            DependencyProperty.Register("DefaultItemTemplate", typeof(DataTemplate), 
            typeof(TimelineBand), new PropertyMetadata(DefaultItemTemplateChanged));

        public static void DefaultItemTemplateChanged(
            DependencyObject                            d, 
            DependencyPropertyChangedEventArgs          e
        )
        {
            TimelineBand band = (TimelineBand) d;

            if (band != null)
            {
                band.OnDefaultItemTemplateChanged(e); 
            }
        }

        protected virtual void OnDefaultItemTemplateChanged(
            DependencyPropertyChangedEventArgs          e
        )
        {
            DefaultItemTemplate = (DataTemplate) e.NewValue;
        }

        public DataTemplate DefaultItemTemplate
        {
            get
            {
                return (DataTemplate) GetValue(DefaultItemTemplateProperty);
            }
            set
            {
                SetValue(DefaultItemTemplateProperty, value);
            }
        }

        public static readonly DependencyProperty DefaultShortEventTemplateProperty =
            DependencyProperty.Register("DefaultShortEventTemplate", 
                typeof(DataTemplate), typeof(TimelineBand), 
                new PropertyMetadata(DefaultShortEventTemplateChanged));

        public static void DefaultShortEventTemplateChanged(
            DependencyObject                            d, 
            DependencyPropertyChangedEventArgs          e
        )
        {
            TimelineBand band = (TimelineBand) d;

            if (band != null)
            {
                band.OnDefaultShortEventTemplateChanged(e); 
            }
        }

        protected virtual void OnDefaultShortEventTemplateChanged(
            DependencyPropertyChangedEventArgs          e
        )
        {
            DefaultShortEventTemplate = (DataTemplate) e.NewValue;
        }
        
        public DataTemplate DefaultShortEventTemplate
        {
            get
            {
                return (DataTemplate) GetValue(DefaultShortEventTemplateProperty);
            }

            set
            {
                SetValue(DefaultShortEventTemplateProperty, value);
            }
        }

        public static readonly DependencyProperty DefaultEventTemplateProperty =
            DependencyProperty.Register("DefaultEventTemplate", 
                typeof(DataTemplate), 
                typeof(TimelineBand), 
                new PropertyMetadata(DefaultEventTemplateChanged));

        public static void DefaultEventTemplateChanged(
            DependencyObject                            d, 
            DependencyPropertyChangedEventArgs          e
        )
        {
            TimelineBand band = (TimelineBand) d;

            if (band != null)
            {
                band.OnDefaultEventTemplateChanged(e); 
            }
        }

        protected virtual void OnDefaultEventTemplateChanged(
            DependencyPropertyChangedEventArgs          e
        )
        {
            DefaultEventTemplate = (DataTemplate) e.NewValue;
        }

        public DataTemplate DefaultEventTemplate
        {
            get
            {
                return (DataTemplate)GetValue(DefaultEventTemplateProperty);
            }

            set
            {
                SetValue(DefaultEventTemplateProperty, value);
            }
        }

        public static readonly DependencyProperty EventTemplateProperty =
            DependencyProperty.Register("EventTemplate", 
                typeof(DataTemplate), 
                typeof(TimelineBand), 
                new PropertyMetadata(EventTemplateChanged));

        public static void EventTemplateChanged(
            DependencyObject                            d, 
            DependencyPropertyChangedEventArgs          e
        )
        {
            TimelineBand band = (TimelineBand) d;

            if (band != null)
            {
                band.OnEventTemplateChanged(e); 
            }
        }

        protected virtual void OnEventTemplateChanged(
            DependencyPropertyChangedEventArgs          e
        )
        {
            EventTemplate = (DataTemplate) e.NewValue;
        }

        public DataTemplate EventTemplate
        {
            get
            {
                return (DataTemplate) GetValue(EventTemplateProperty);
            }

            set
            {
                SetValue(EventTemplateProperty, value);
            }
        } 

        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", 
                typeof(DataTemplate), 
                typeof(TimelineBand), 
                new PropertyMetadata(ItemTemplateChanged));

        public static void ItemTemplateChanged(
            DependencyObject                            d, 
            DependencyPropertyChangedEventArgs          e
        )
        {
            TimelineBand band = (TimelineBand) d;

            if (band != null)
            {
                band.OnItemTemplateChanged(e); 
            }
        }

        protected virtual void OnItemTemplateChanged(
            DependencyPropertyChangedEventArgs          e
        )
        {
            ItemTemplate = (DataTemplate) e.NewValue;
        }

        public DataTemplate ItemTemplate
        {
            get
            {
                return (DataTemplate)GetValue(ItemTemplateProperty);
            }

            set
            {
                SetValue(ItemTemplateProperty, value);
            }
        } 

        public static readonly DependencyProperty ShortEventTemplateProperty =
            DependencyProperty.Register("ShortEventTemplate", 
                typeof(DataTemplate), 
                typeof(TimelineBand), 
                new PropertyMetadata(ShortEventTemplateChanged));

        public static void ShortEventTemplateChanged(
            DependencyObject                            d, 
            DependencyPropertyChangedEventArgs          e
        )
        {
            TimelineBand band = (TimelineBand) d;

            if (band != null)
            {
                band.OnShortEventTemplateChanged(e); 
            }
        }

        protected virtual void OnShortEventTemplateChanged(
            DependencyPropertyChangedEventArgs          e
        )
        {
            ShortEventTemplate = (DataTemplate) e.NewValue;
        }

        public DataTemplate ShortEventTemplate
        {
            get
            {
                return (DataTemplate)GetValue(ShortEventTemplateProperty);
            }

            set
            {
                SetValue(ShortEventTemplateProperty, value);
            }
        } 

        public static readonly DependencyProperty TimelineWindowSizeProperty =
            DependencyProperty.Register("TimelineWindowSize", typeof(int), 
                typeof(TimelineBand), new PropertyMetadata(
                10, TimelineWindowSizeChanged));

        public static void TimelineWindowSizeChanged(
            DependencyObject                            d, 
            DependencyPropertyChangedEventArgs          e
        )
        {
            TimelineBand band = (TimelineBand) d;

            if (band != null)
            {
                band.OnTimelineWindowSizeChanged(e); 
            }
        }

        protected virtual void OnTimelineWindowSizeChanged(
            DependencyPropertyChangedEventArgs          e
        )
        {
            int newVal = (int) e.NewValue;

            if (newVal < 2)
            {
                throw new ArgumentException("TimelineWindowSize should be greater then 2");
            }
            if (Calculator != null)
            {
                Calculator.ColumnCount = newVal;
            }

            UpdateControlSize();
        }

        public int TimelineWindowSize
        {
            get
            {
                return (int) GetValue(TimelineWindowSizeProperty);
            }
            set
            {
                SetValue(TimelineWindowSizeProperty, value);
            }
        }

        public static readonly DependencyProperty MaxEventHeightProperty =
            DependencyProperty.Register("MaxEventHeight", 
                typeof(double), typeof(TimelineBand), 
                new PropertyMetadata(50.0, MaxEventHeightChanged));

        public static void MaxEventHeightChanged(
            DependencyObject                            d, 
            DependencyPropertyChangedEventArgs          e
        )
        {
            ((TimelineBand) d).MaxEventHeight = (double) e.NewValue;
        }

        public double MaxEventHeight
        { 
            get
            {
                return (double) GetValue(MaxEventHeightProperty);
            }

            set
            {
                Debug.Assert(value > 1);
                SetValue(MaxEventHeightProperty, value);
            }
        }

        public static readonly DependencyProperty IsMainBandProperty =
            DependencyProperty.Register("IsMainBand", 
                typeof(bool), typeof(TimelineBand), 
                new PropertyMetadata(false, IsMainBandChanged));

        public static void IsMainBandChanged(
            DependencyObject                            d, 
            DependencyPropertyChangedEventArgs          e
        )
        {
            ((TimelineBand) d).IsMainBand = (bool) e.NewValue;
        }

        public bool IsMainBand
        { 
            get
            {
                return (bool) GetValue(IsMainBandProperty);
            }

            set
            {
                SetValue(IsMainBandProperty, value);
            }
        }

        public Grid MainGridPart
        {
            get;
            set;
        }

        public Canvas CanvasPart
        {
            get
            {
                return Canvas;
            }

            set
            {
                if (Canvas != null)
                {
                    if (InertialScroll != null)
                    {
                        InertialScroll.OnDragStart -= OnCanvasMouseLeftButtonDown;
                        InertialScroll.OnDragStop  -= OnCanvasMouseLeftButtonUp;
                        InertialScroll.OnDragMove  -= OnCanvasMouseMove;
                        InertialScroll = null;
                    }

                    Canvas.MouseWheel  -= OnCanvasMouseWheel;
                    Canvas.SizeChanged -= OnSizeChanged;
                }

                Canvas = value;

                if (value != null)
                {
                    InertialScroll = new InertialTimelineScroll(Canvas);

                    InertialScroll.OnDragStart   += OnCanvasMouseLeftButtonDown;
                    InertialScroll.OnDragStop    += OnCanvasMouseLeftButtonUp;
                    InertialScroll.OnDragMove    += OnCanvasMouseMove;
                    InertialScroll.OnDoubleClick += OnCanvasDoubleClick;

                    Canvas.MouseWheel  += OnCanvasMouseWheel;
                    Canvas.SizeChanged += OnSizeChanged;

                    Canvas.DataContext = this;
                }
            }
        }

        void OnCanvasDoubleClick(
            Point                                       point
        )
        {
            DateTime date = CurrentDateTime + 
                Calculator.PixelsToTimeSpan(point.X - Canvas.ActualWidth / 2);

            TimelineTray.FireTimelineDoubleClick(date, point);
        }

        void OnSizeChanged(
            object                                      sender, 
            SizeChangedEventArgs                        e
        )
        {
            this.ClipRect.Rect = new Rect(new Point(0, 0), 
                new Size(e.NewSize.Width, e.NewSize.Height));
        }

        void OnCanvasMouseWheel(
            object                                      sender, 
            MouseWheelEventArgs                         e
        )
        {
		    if (Keyboard.Modifiers != ModifierKeys.Control)
		    {
			    TimeSpan span;

			    if (Calculator != null)
			    {
				    span = Calculator.PixelsToTimeSpan(e.Delta / 5);
				    SafeDateChange(span, true);
			    }
		    }
        }

        public FrameworkElement VisibleDatesPart
        {
            get;
            set;
        }
        
        private Button m_navigateLeftButton;
        public Button NavigateLeftButton
        {
            get
            {
                return m_navigateLeftButton;
            }

            set
            {
                if (m_navigateLeftButton != null)
                {
                    m_navigateLeftButton.Click -= OnNavigateLeftClick;
                }

                m_navigateLeftButton = value;

                if (m_navigateLeftButton != null)
                {
                    m_navigateLeftButton.Click += OnNavigateLeftClick;
                }
            }
        }

        void OnNavigateLeftClick(object sender, RoutedEventArgs e)
        {
            SafeDateChange(Calculator.ColumnTimeWidth, true);
        }
        
        private Button m_navigateRightButton;
        public Button NavigateRightButton
        {
            get
            {
                return m_navigateRightButton;
            }

            set
            {
                if (m_navigateRightButton != null)
                {
                    m_navigateRightButton.Click -= OnNavigateRightClick;
                }

                m_navigateRightButton = value;

                if (m_navigateRightButton != null)
                {
                    m_navigateRightButton.Click += OnNavigateRightClick;
                }
            }
        }

        void OnNavigateRightClick(object sender, RoutedEventArgs e)
        {
            SafeDateChange(Calculator.ColumnTimeWidth, false);
        }

        private void OnCanvasMouseLeftButtonDown(
            Point                                       p
        )
        {
            if (TooltipServiceEx.LastTooltip != null)
            {
                TooltipServiceEx.LastTooltip.Hide();
                TooltipServiceEx.LastTooltip = null;
            }

            Canvas.Cursor = Cursors.Hand;
            OnCanvasBeginDrag?.Invoke(p);
        }

        private void OnCanvasMouseLeftButtonUp(
            Point                                       p
        )
        {
            StopDragging();
            if (TooltipServiceEx.LastTooltip != null)
            {
                TooltipServiceEx.LastTooltip.Hide();
                TooltipServiceEx.LastTooltip = null;
            }

            OnCanvasEndDrag?.Invoke(p);
        }

        public virtual double VerticalScrollableSize
        {
            get
            {
                if (Calculator != null && Calculator.VerticalViewSize > this.ActualHeight)
                {
                    return Calculator.VerticalViewSize;
                }
                else
                { 
                    return this.ActualHeight;
                }
            }
        }

        protected virtual void OnCanvasMouseMove(
            Point                                       pPrev,
            Point                                       pNew
        )
        {
            if (Calculator != null)
            {
                Dispatcher.BeginInvoke(new Action(() => 
                {
                    MoveScale(pPrev, pNew);

                    if (IsMainBand && OnCanvasDrag != null)
                    {
                        OnCanvasDrag(pNew);
                    }

                    if (TooltipServiceEx.LastTooltip != null)
                    {
                        TooltipServiceEx.LastTooltip.Hide();
                        TooltipServiceEx.LastTooltip = null;
                    }
                }));
            }
        }

        private void StopDragging()
        {
            Canvas.Cursor = Cursors.Arrow;
        }

        public double CanvasScrollOffset
        {
            get
            {
                return m_offset;
            }
            set
            {
                // we do not do display events here any more, it should be done manually
                m_offset = value;
                OnScrollPositionChanged();
            }
        }

        public virtual void OnScrollPositionChanged()
        {}

        public Canvas Canvas { get; private set; }

        public bool TimeFormat24
        {
            get
            {
                return m_timeFormat24;
            }
            set
            {
                m_timeFormat24 = value;
                if (Calculator != null && Calculator.Calendar != null)
                {
                    Calculator.Calendar.TimeFormat24 = m_timeFormat24;
                }
            }
        }

        public TimelineTray TimelineTray
        {
            get
            {
                if (m_tray == null && Parent as TimelineTray != null)
                {
                    m_tray = Parent as TimelineTray;
                }
                return m_tray;
            }

            set
            {
                TimelineTray old = m_tray;
                m_tray = value;
                OnTimelineTrayChanged(old, m_tray);
            }
        }

        protected virtual void OnTimelineTrayChanged(
            TimelineTray                                old,
            TimelineTray                                newTray
        )
        {}

        public string CalendarType
        {
            get;
            set;
        }

        public string ItemSourceType
        {
            get
            {
                return m_sourceType;
            }

            set
            {
                string itemsType = value.ToLower();
                m_sourceType = value;

                switch (itemsType)
                {
                    case TL_TYPE_DECADES:
                        m_timelineType = TimelineCalendarType.Decades;
                        break;

                    case TL_TYPE_YEARS:
                        m_timelineType = TimelineCalendarType.Years;
                        break;

                    case TL_TYPE_MONTHS:
                        m_timelineType = TimelineCalendarType.Months;
                        break;

                    case TL_TYPE_DAYS:
                        m_timelineType = TimelineCalendarType.Days;
                        break;

                    case TL_TYPE_HOURS:
                        m_timelineType = TimelineCalendarType.Hours;
                        break;

                    case TL_TYPE_MINUTES10:
                        m_timelineType = TimelineCalendarType.Minutes10;
                        break;

                    case TL_TYPE_MINUTES:
                        m_timelineType = TimelineCalendarType.Minutes;
                        break;

                    case TL_TYPE_SECONDS:
                        m_timelineType = TimelineCalendarType.Seconds;
                        break;

                    case TL_TYPE_MILLISECONDS100:
                        m_timelineType = TimelineCalendarType.Milliseconds100;
                        break;

                    case TL_TYPE_MILLISECONDS10:
                        m_timelineType = TimelineCalendarType.Milliseconds10;
                        break;

                    case TL_TYPE_MILLISECONDS:
                        m_timelineType = TimelineCalendarType.Milliseconds;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// This sets/gets both dependency property and internal current datetime</summary>
        internal DateTime CurrentDateTime
        {
            get
            {
                return Calculator == null ? new DateTime() : Calculator.CurrentDateTime;
            }
            set
            {
                if (Calculator != null && Calculator.CurrentDateTime != value)
                {
                    Calculator.TimeMove(Calculator.CurrentDateTime - value);
                    OnCurrentDateChanged?.Invoke(this, new RoutedEventArgs());
                }
            }
        }

        public string DisplayField
        { 
            get; 
            set;
        }

        public TimelineCalendar ItemsSource 
        { 
            get; 
            set; 
        }

        public TimelineEventStore EventStore
        {
            get
            {
                return Calculator.EventStore;
            }
            set
            {
                Debug.Assert(value != null);
                Calculator.EventStore = value;
            }
        }

        public double VisibleDatesAreaWidth
        {
            get
            {
                double width;
                TimeSpan visTimeSpan;

                Debug.Assert(Calculator != null);

                if (IsMainBand)
                {
                    width = 0;
                }
                else 
                {
                    visTimeSpan = VisibleTimeSpan;

                    if (visTimeSpan.Ticks < 0)
                    {
                        visTimeSpan = new TimeSpan(0L);
                    }

                    width = Calculator.TimeSpanToPixels(visTimeSpan);
                }

                return width;
            }
        }

        public double VisibleDatesAreaHeight
        {
            get
            {
                return CanvasPart.ActualHeight - 1;
            }
        }

        public TimeSpan VisibleTimeSpan
        {
            get
            {
                return (Calculator == null ? new TimeSpan(0L) : Calculator.VisibleWindowSize);
            }

            internal set
            {
                Debug.Assert(Calculator != null);
                Calculator.VisibleWindowSize = value;
            }
        }

        public TimelineBuilder Calculator { get; private set; }

        public ObservableCollection<TimelineDisplayEvent> Selection { get; } = new ObservableCollection<TimelineDisplayEvent>();

        public void OnMoreInfoClick(
            object                                      sender, 
            RoutedEventArgs                             args
        )
        {
            FrameworkElement                            element;

            //details = new EventDetailsWindow();
            //details.DataContext = ((FrameworkElement) sender).DataContext;

            element = (FrameworkElement) sender;
            TooltipServiceEx.HideLastTooltip();
        }

        public override void OnApplyTemplate()
        {
            Utilities.Trace(this);

            base.OnApplyTemplate();

            MainGridPart = (Grid) GetTemplateChild(TP_MAIN_GRID_PART);
            CanvasPart = (Canvas) GetTemplateChild(TP_CANVAS_PART);

            NavigateLeftButton = (Button) GetTemplateChild(TP_NAVIGATE_LEFT_BUTTON_PART);
            NavigateRightButton = (Button) GetTemplateChild(TP_NAVIGATE_RIGHT_BUTTON_PART);

            VisibleDatesPart = (FrameworkElement) GetTemplateChild(TP_VISIBLE_DATES_PART);

            if (VisibleDatesPart != null)
            {
                VisibleDatesPart.SetValue(Panel.ZIndexProperty, TimelineBuilder.MIN_EVENT_ZINDEX - 1);
            }
        }

        static TimelineBand()
        {
#if !SILVERLIGHT
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimelineBand), 
                new FrameworkPropertyMetadata(typeof(TimelineBand)));
#endif

            if (!ColorManager.Templates.ContainsKey("TimelineEventLine"))
            {
                var line = new TemplateColor()
                {
                    Name = "TimelineEventLine", Color = Colors.LightGray, Category = "Essential"
                };
            }
        }

        public TimelineBand()
        {
            Utilities.Trace(this);
            this.Loaded += OnControlLoaded;
            this.SizeChanged += OnControlSizeChanged;
        }

        void OnControlSizeChanged(
            object                                      sender, 
            SizeChangedEventArgs                        e
        )
        {
            Utilities.Trace(this);
            UpdateControlSize(false, e.NewSize);
        }

        void UpdateControlSize(
            bool                                        animate = true,
            Size?                                       size = null                                          
        )
        {
            Utilities.Trace(this);

            if (size == null)
            {
                size = new Size(this.ActualWidth, this.ActualHeight);
            }

            if (m_calcInitialized)
            {
                Calculator.BuildColumns(animate, false, size);
                this.ResetVisibleDaysHighlight();
            }
        }

        
        void OnControlLoaded(
            object                                      sender, 
            RoutedEventArgs                             e
        )
        {
            Utilities.Trace(this);
        }

        public void CreateTimelineCalculator(
            string                                      calendarType,
            DateTime                                    currentDateTime,
            DateTime                                    minDateTime,
            DateTime                                    maxDateTime
        )
        {
            Debug.Assert(this.TimelineTray != null);

            ItemsSource = new TimelineCalendar(calendarType, m_timelineType,
                minDateTime, maxDateTime)
            {
                TimeFormat24 = TimeFormat24
            };

            m_calendarType = calendarType;

            if (ItemTemplate == null)
            {
                ItemTemplate = DefaultItemTemplate;
            }

            if (TextItemTemplate == null)
            {
                TextItemTemplate = DefaultTextItemTemplate;
            }
            
            if (ShortEventTemplate == null)
            {
                ShortEventTemplate = DefaultShortEventTemplate;
            }

            if (EventTemplate == null)
            {
                EventTemplate = DefaultEventTemplate;
            }

            if (Calculator != null)
            {
                Calculator.ClearEvents();
                Calculator.ClearColumns();
            }

            Calculator = new TimelineBuilder(
                this,
                CanvasPart, 
                ItemTemplate, 
                TextItemTemplate,
                TimelineWindowSize, 
                ItemsSource, 
                !IsMainBand ? ShortEventTemplate : EventTemplate,
                MaxEventHeight,
                IsMainBand,
                currentDateTime);

            Calculator.BuildColumns();
            m_calcInitialized = true;
        }

        public void CalculateEventRows()
        {
            // it only makes sence to calculate row positions for main band becuase
            // other bands should use rows from main band to look similar with main.
            Debug.Assert(IsMainBand);
            Debug.Assert(Calculator != null);

            Calculator.CalculateEventRows();
        }

        /// <summary>
        /// Calculates event positions (should be called after CalculateEventRows for main (see IsMainBand)
        /// timelineband)</summary>
        public void CalculateEventPositions()
        {
            Debug.Assert(Calculator != null);
            Calculator.CalculateEventPositions();
        }

        /// <summary>
        /// Clear all events from timelineband screen</summary>
        public void ClearEvents()
        {
            if (Calculator != null)
            {
                Calculator.ClearEvents();
            }
        }

        /// <summary>
        /// Display all events which should be visible in current timelineband window</summary>
        public void DisplayEvents()
        {
            Debug.Assert(Calculator != null);
            Calculator.DisplayEvents();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResetVisibleDaysHighlight()
        {
            if (VisibleDatesPart != null)
            {
                if (VisibleTimeSpan.Ticks == 0 || IsMainBand)
                {
                    VisibleDatesPart.Visibility = Visibility.Collapsed;
                }
                else if (VisibleDatesAreaWidth != 0.0 && VisibleDatesAreaHeight != 0.0)
                {
                    VisibleDatesPart.Visibility = Visibility.Visible;
                    VisibleDatesPart.Width = VisibleDatesAreaWidth;
                    VisibleDatesPart.Height = VisibleDatesAreaHeight;

                    VisibleDatesPart.SetValue(Canvas.LeftProperty, 
                        (CanvasPart.ActualWidth - VisibleDatesAreaWidth) / 2 + 1);

                    VisibleDatesPart.SetValue(Canvas.ZIndexProperty, TimelineBuilder.MIN_EVENT_ZINDEX - 1);
                }
            }
        }

        public override string ToString()
        {
            return string.Format(FMT_TRACE, Name, ItemSourceType, GetValue(Canvas.LeftProperty),
                GetValue(Canvas.TopProperty), ActualHeight, ActualWidth);
        }

		public event TimelineEventDelegate OnEventCreated;

		internal void FireEventCreated(
			FrameworkElement element,
			TimelineDisplayEvent de
		)
		{
            OnEventCreated?.Invoke(element, de);
        }

        private RectangleGeometry ClipRect
        {
            get
            {
                if (m_clipRect == null)
                {
                    m_clipRect = (RectangleGeometry) GetTemplateChild("ClipRect");
                }
                return m_clipRect;
            }
        }

        private string GetDataContext(
            int                                         index
        )
        {
            return TimelineCalendar.ItemToString(ItemsSource, ItemsSource[index]);
        }

        /// <summary>
        /// Moves timeline according to mouse move during drag-drop</summary>
        private void MoveScale(
            Point                                       prevPos,
            Point                                       newPos
        )
        {
            TimeSpan                                    span;
            double                                      vertOffset;
            bool                                        display = false;

            Debug.Assert(Calculator != null);

            if (newPos.Y - prevPos.Y != 0 && IsMainBand)
            { 
                vertOffset = TimelineTray.EventCanvasOffset - (newPos.Y - prevPos.Y);

                if (vertOffset != 0 && vertOffset <= VerticalScrollableSize)
                {
                    if (vertOffset < 0)
                    {
                        vertOffset = 0;
                    }

                    vertOffset = Math.Min(
                        VerticalScrollableSize - this.ActualHeight, vertOffset);

                    TimelineTray.EventCanvasOffset = vertOffset;
                    display = true;
                }
            }

            if (newPos.X != prevPos.X)
            {
                span = Calculator.PixelsToTimeSpan(newPos.X - prevPos.X);

                SafeDateChange(span, true, newPos.X < prevPos.X);
            }
            else if (display)
            {
                DisplayEvents();
            }
        }

        protected virtual void OnMoveScale()
        {}

        private void SafeDateChange(
            TimeSpan                                    span,
            bool                                        subtract
        )
        {
              SafeDateChange(span, subtract, false);
        }

        private void SafeDateChange(
            TimeSpan                                    span,
            bool                                        subtract,
            bool                                        fixAsMaxDate
        )
        {
            bool fixDate = true;

            try
            {
                if (subtract)
                {
                    if (CurrentDateTime - span > Calculator.Calendar.MinDateTime)  
                    {
                        TimelineTray.CurrentDateTime -= span;
                        fixDate = false;
                    }
                }
                else
                {
                    if (CurrentDateTime + span < Calculator.Calendar.MaxDateTime)  
                    {
                        TimelineTray.CurrentDateTime += span;
                        fixDate = false;
                    }
                }
            }
            catch(ArgumentOutOfRangeException)
            {
                fixDate = true;
            }
            
            if (fixDate)
            {
                if (!fixAsMaxDate)
                {
                    TimelineTray.CurrentDateTime = Calculator.Calendar.MinDateTime;
                }
                else
                {
                    TimelineTray.CurrentDateTime = Calculator.Calendar.MaxDateTime;
                }
            }
        }
    }
}
