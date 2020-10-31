using System;
using GalaSoft.MvvmLight.Ioc;
using SchedulerDatabase;
using SchedulerDatabase.Models;

namespace SchedulerGUI.ViewModels.Controls
{
    /// <summary>
    /// <see cref="DevicePickerViewModelCompression"/> is a concrete implementation of the
    /// <see cref="DevicePickerViewModel{T}"/> class for binding in XAML for selecting compression devices.
    /// </summary>
    public class DevicePickerViewModelCompression : DevicePickerViewModel<CompressorProfile>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DevicePickerViewModelCompression"/> class.
        /// </summary>
        public DevicePickerViewModelCompression()
            : base()
        {
            using (var context = SimpleIoc.Default.GetInstanceWithoutCaching<SchedulerContext>())
            {
                var summarizer = new SchedulingSummarizer(context);
                var summarizedDevices = summarizer.SummarizeDeviceResults(context.CompressorProfiles);

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

        /// <inheritdoc/>
        public override string Header => "Available Compression Devices";
    }
}
