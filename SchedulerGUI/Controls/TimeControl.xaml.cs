using System;
using System.Windows;
using System.Windows.Controls;

namespace SchedulerGUI.Controls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class TimeControl : UserControl
    {
        public static readonly DependencyProperty HoursProperty =
            DependencyProperty.Register(
                "Hours",
                typeof(int),
                typeof(TimeControl),
                new UIPropertyMetadata(0, new PropertyChangedCallback(OnTimeChanged)));

        public static readonly DependencyProperty MinutesProperty =
            DependencyProperty.Register(
                "Minutes",
                typeof(int),
                typeof(TimeControl),
                new UIPropertyMetadata(0, new PropertyChangedCallback(OnTimeChanged)));

        public static readonly DependencyProperty SecondsProperty =
            DependencyProperty.Register(
                "Seconds",
                typeof(int),
                typeof(TimeControl),
                new UIPropertyMetadata(0, new PropertyChangedCallback(OnTimeChanged)));

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value",
                typeof(TimeSpan),
                typeof(TimeControl),
                new UIPropertyMetadata(DateTime.Now.TimeOfDay, new PropertyChangedCallback(OnValueChanged)));

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeControl"/> class.
        /// </summary>
        public TimeControl()
        {
            this.InitializeComponent();
        }

        public int Hours
        {
            get => (int)this.GetValue(HoursProperty);
            set => this.SetValue(HoursProperty, value);
        }

        public int Minutes
        {
            get => (int)this.GetValue(MinutesProperty);
            set => this.SetValue(MinutesProperty, value);
        }

        public int Seconds
        {
            get => (int)this.GetValue(SecondsProperty);
            set => this.SetValue(SecondsProperty, value);
        }

        public TimeSpan Value
        {
            get => (TimeSpan)this.GetValue(ValueProperty);
            set => this.SetValue(ValueProperty, value);
        }

        private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            TimeControl control = obj as TimeControl;
            control.Hours = ((TimeSpan)e.NewValue).Hours;
            control.Minutes = ((TimeSpan)e.NewValue).Minutes;
            control.Seconds = ((TimeSpan)e.NewValue).Seconds;
        }

        private static void OnTimeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            TimeControl control = obj as TimeControl;
            control.Value = new TimeSpan(control.Hours, control.Minutes, control.Seconds);
        }
    }
}
