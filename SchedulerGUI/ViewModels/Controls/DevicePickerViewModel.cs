using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using SchedulerDatabase;
using SchedulerDatabase.Extensions;
using SchedulerDatabase.Models;

namespace SchedulerGUI.ViewModels.Controls
{
    /// <summary>
    /// <see cref="DevicePickerViewModel"/> provides a View-Model for the <see cref="Views.Controls.DevicePickerControl"/> control.
    /// </summary>
    public class DevicePickerViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DevicePickerViewModel"/> class.
        /// </summary>
        public DevicePickerViewModel()
        {
            this.Devices = new ObservableCollection<SelectableDevice>();
            this.DevicesView = CollectionViewSource.GetDefaultView(this.Devices);

            this.DevicesView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(SelectableDevice.DeviceName)));

            using (var context = SimpleIoc.Default.GetInstanceWithoutCaching<SchedulerContext>())
            {
                var summarizer = new SchedulingSummarizer(context);
                var summarizedDevices = summarizer.SummarizeResults(context.AESProfiles);

                foreach (var device in summarizedDevices)
                {
                    var selectableDevice = new SelectableDevice(device)
                    {
                        IsSelected = true,
                    };

                    selectableDevice.PropertyChanged += this.Device_PropertyChanged;

                    this.Devices.Add(selectableDevice);
                }
            }
        }

        /// <summary>
        /// Gets a listing of AES Profiles that are enabled.
        /// </summary>
        public IEnumerable<AESEncyptorProfile> EnabledProfiles => this.Devices.Where(x => x.IsSelected).Select(x => x.Device);

        /// <summary>
        /// Gets an observable sorted view of the available AES devices.
        /// </summary>
        public ICollectionView DevicesView { get; }

        private ObservableCollection<SelectableDevice> Devices { get; }

        private void Device_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.RaisePropertyChanged(nameof(this.Devices));
        }

        /// <summary>
        /// Gets or sets an AES profile that can be enabled or disabled for scheduling purposes.
        /// </summary>
        public class SelectableDevice : ObservableObject
        {
            private bool isSelected;

            /// <summary>
            /// Initializes a new instance of the <see cref="SelectableDevice"/> class.
            /// </summary>
            /// <param name="device">The underlying AES test profile.</param>
            public SelectableDevice(AESEncyptorProfile device)
            {
                this.Device = device;
            }

            /// <summary>
            /// Gets the profile for the underlying AES Device this item represents.
            /// </summary>
            public AESEncyptorProfile Device { get; }

            /// <summary>
            /// Gets the name of the AES device.
            /// </summary>
            public string DeviceName => this.Device.PlatformName;

            /// <summary>
            /// Gets the short description string for this profile.
            /// </summary>
            public string DeviceDescription => $"{this.Device.PlatformAccelerator.ToFriendlyName()}, {Converters.HzToStringConverter.HzToString(this.Device.TestedFrequency)} {this.Device.NumCores} Cores, {this.Device.ProviderName}";

            /// <summary>
            /// Gets the complete platform description.
            /// </summary>
            public string FullDescription => this.Device.ToFullDescription();

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
