using System;
using System.Windows;
using System.Windows.Controls;

namespace SchedulerGUI.Controls
{
    public class GanttRowPanel : Panel
    {
        public static readonly DependencyProperty StartDateProperty =
           DependencyProperty.RegisterAttached("StartDate", typeof(DateTime), typeof(GanttRowPanel), new FrameworkPropertyMetadata(DateTime.MinValue, FrameworkPropertyMetadataOptions.AffectsParentArrange));

        public static readonly DependencyProperty EndDateProperty =
            DependencyProperty.RegisterAttached("EndDate", typeof(DateTime), typeof(GanttRowPanel), new FrameworkPropertyMetadata(DateTime.MaxValue, FrameworkPropertyMetadataOptions.AffectsParentArrange));

        public static readonly DependencyProperty MaxDateProperty =
           DependencyProperty.Register("MaxDate", typeof(DateTime), typeof(GanttRowPanel), new FrameworkPropertyMetadata(DateTime.MaxValue, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty MinDateProperty =
            DependencyProperty.Register("MinDate", typeof(DateTime), typeof(GanttRowPanel), new FrameworkPropertyMetadata(DateTime.MaxValue, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public DateTime MaxDate
        {
            get => (DateTime)this.GetValue(MaxDateProperty);
            set => this.SetValue(MaxDateProperty, value);
        }

        public DateTime MinDate
        {
            get => (DateTime)this.GetValue(MinDateProperty);
            set => this.SetValue(MinDateProperty, value);
        }

        public static DateTime GetStartDate(DependencyObject obj)
        {
            return (DateTime)obj.GetValue(StartDateProperty);
        }

        public static void SetStartDate(DependencyObject obj, DateTime value)
        {
            obj.SetValue(StartDateProperty, value);
        }

        public static DateTime GetEndDate(DependencyObject obj)
        {
            return (DateTime)obj.GetValue(EndDateProperty);
        }

        public static void SetEndDate(DependencyObject obj, DateTime value)
        {
            obj.SetValue(EndDateProperty, value);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement child in this.Children)
            {
                child.Measure(availableSize);
            }

            return new Size(0, 0);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double range = (this.MaxDate - this.MinDate).Ticks;
            range = (range != 0) ? range : 1; // TODO: Alex - added to avoid crash, need to permafix later

            double pixelsPerTick = finalSize.Width / range;

            foreach (UIElement child in this.Children)
            {
                this.ArrangeChild(child, this.MinDate, pixelsPerTick, finalSize.Height);
            }

            return finalSize;
        }

        private void ArrangeChild(UIElement child, DateTime minDate, double pixelsPerTick, double elementHeight)
        {
            DateTime childStartDate = GetStartDate(child);
            DateTime childEndDate = GetEndDate(child);
            TimeSpan childDuration = childEndDate - childStartDate;

            double offset = (childStartDate - minDate).Ticks * pixelsPerTick;
            double width = childDuration.Ticks * pixelsPerTick;

            child.Arrange(new Rect(offset, 0, width, elementHeight));
        }
    }
}
