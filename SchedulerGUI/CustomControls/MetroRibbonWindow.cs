using System;
using System.Windows;
using ControlzEx.Theming;
using Fluent;
using MahApps.Metro.Controls;

namespace SchedulerGUI.CustomControls
{
    /// <summary>
    /// <see cref="MetroRibbonWindow"/> is a MahApps Metro window with support for the Fluent Ribbon control.
    /// </summary>
    /// <remarks>
    /// Adapted from https://github.com/fluentribbon/Fluent.Ribbon/blob/develop/Fluent.Ribbon.Showcase/MahMetroWindow.xaml.cs.</remarks>
    public class MetroRibbonWindow : MetroWindow
    {
        private static readonly DependencyPropertyKey TitleBarPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(TitleBar),
                typeof(RibbonTitleBar),
                typeof(MetroWindow),
                new PropertyMetadata());

        /// <summary>
        /// A Dependency Property for the titlebar contents of this window.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:Elements should be ordered by access", Justification = "Must initialize after PropertyKey.")]
        public static readonly DependencyProperty TitleBarProperty = TitleBarPropertyKey.DependencyProperty;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetroRibbonWindow"/> class.
        /// </summary>
        public MetroRibbonWindow()
        {
            this.Loaded += this.MahMetroWindow_Loaded;
            this.Closed += this.MahMetroWindow_Closed;
        }

        /// <summary>
        /// Gets ribbon titlebar.
        /// </summary>
        public RibbonTitleBar TitleBar
        {
            get => (RibbonTitleBar)this.GetValue(TitleBarProperty);
            private set => this.SetValue(TitleBarPropertyKey, value);
        }

        private void MahMetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.TitleBar = this.FindChild<RibbonTitleBar>("RibbonTitleBar");
            this.TitleBar.InvalidateArrange();
            this.TitleBar.UpdateLayout();

            // We need this inside this window because MahApps.Metro is not loaded globally inside the Fluent.Ribbon Showcase application.
            // This code is not required in an application that loads the MahApps.Metro styles globally.
            ThemeManager.Current.ChangeTheme(this, ThemeManager.Current.DetectTheme(Application.Current));
            ThemeManager.Current.ThemeChanged += this.SyncThemes;
        }

        private void SyncThemes(object sender, ThemeChangedEventArgs e)
        {
            if (e.Target == this)
            {
                return;
            }

            ThemeManager.Current.ChangeTheme(this, e.NewTheme);
        }

        private void MahMetroWindow_Closed(object sender, EventArgs e)
        {
            ThemeManager.Current.ThemeChanged -= this.SyncThemes;
        }
    }
}
