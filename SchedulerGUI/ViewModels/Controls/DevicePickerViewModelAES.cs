using System;
using SchedulerDatabase.Models;

namespace SchedulerGUI.ViewModels.Controls
{
    /// <summary>
    /// <see cref="DevicePickerViewModelAES"/> is a concrete implementation of the
    /// <see cref="DevicePickerViewModel{T}"/> class for binding in XAML for selecting AES devices.
    /// </summary>
    public class DevicePickerViewModelAES : DevicePickerViewModel<AESEncyptorProfile>
    {
        /// <inheritdoc/>
        public override string Header => "Available AES Devices";
    }
}
