using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using SchedulerDatabase;
using SchedulerDatabase.Models;

namespace SchedulerGUI.ViewModels.Controls
{
    /// <summary>
    /// <see cref="DevicePickerViewModel"/> provides a View-Model for the <see cref="Views.Controls.DevicePickerControl"/> control.
    /// </summary>
    public abstract class DevicePickerViewModel<T> : ViewModelBase
        where T : IByteStreamProcessor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DevicePickerViewModel{T}"/> class.
        /// </summary>
        public DevicePickerViewModel()
        {
            this.Devices = new ObservableCollection<SelectableDevice<T>>();
            this.DevicesView = CollectionViewSource.GetDefaultView(this.Devices);

            this.DevicesView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(SelectableDevice<T>.DeviceName)));

            using (var context = SimpleIoc.Default.GetInstanceWithoutCaching<SchedulerContext>())
            {
                var summarizer = new SchedulingSummarizer(context);
                var summarizedDevices = summarizer.SummarizeResults(context.AESProfiles);

                foreach (var device in summarizedDevices)
                {
                    //var selectableDevice = new SelectableDevice<T>(device)
                    //{
                    //    IsSelected = true,
                    //};

                    //selectableDevice.PropertyChanged += this.Device_PropertyChanged;

                    //this.Devices.Add(selectableDevice);
                }
            }
        }

        /// <summary>
        /// Gets the title to display describing the functionality of the devices being selected.
        /// </summary>
        public abstract string Header { get; }

        /// <summary>
        /// Gets a listing of AES Profiles that are enabled.
        /// </summary>
        public IEnumerable<T> EnabledProfiles => this.Devices.Where(x => x.IsSelected).Select(x => x.Device);

        /// <summary>
        /// Gets an observable sorted view of the available AES devices.
        /// </summary>
        public ICollectionView DevicesView { get; }

        private ObservableCollection<SelectableDevice<T>> Devices { get; }

        private void Device_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.RaisePropertyChanged(nameof(this.Devices));
        }

        /// <summary>
        /// Gets or sets an AES profile that can be enabled or disabled for scheduling purposes.
        /// </summary>
        public class SelectableDevice<T> : ObservableObject
            where T : IByteStreamProcessor
        {
            private bool isSelected;

            /// <summary>
            /// Initializes a new instance of the <see cref="SelectableDevice{T}"/> class.
            /// </summary>
            /// <param name="device">The underlying AES test profile.</param>
            public SelectableDevice(T device)
            {
                this.Device = device;
            }

            /// <summary>
            /// Gets the profile for the underlying AES Device this item represents.
            /// </summary>
            public T Device { get; }

            /// <summary>
            /// Gets the name of the AES device.
            /// </summary>
            public string DeviceName => this.Device.PlatformName;

            /// <summary>
            /// Gets the short description string for this profile.
            /// </summary>
            public string DeviceDescription => this.Device.ShortProfileClassDescription;

            /// <summary>
            /// Gets the complete platform description.
            /// </summary>
            public string FullDescription => this.Device.FullProfileDescription;

            /// <summary>
            /// Gets or sets a value indicating whether this profile is enabled.
            /// </summary>
            public bool IsSelected
            {
                get => this.isSelected;
                set => this.Set(() => this.IsSelected, ref this.isSelected, value);
            }
        }
    }
}
