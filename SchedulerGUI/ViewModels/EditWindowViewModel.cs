using System;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SchedulerGUI.Models;
using SchedulerGUI.ViewModels.Controls;

namespace SchedulerGUI.ViewModels
{
    /// <summary>
    /// <see cref="EditWindowViewModel"/> provides a View-Model for the <see cref="Views.EditDialog"/>.
    /// </summary>
    public class EditWindowViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EditWindowViewModel"/> class.
        /// </summary>
        /// <param name="pass">The pass to edit.</param>
        /// <param name="editComplete">The action to execute when the edit is completed.</param>
        /// <param name="editCancel">The action to execute if the edit is cancelled.</param>
        public EditWindowViewModel(PassData pass, Action<PassData> editComplete, Action editCancel)
        {
            this.Pass = pass;

            this.SunlightEditor = new TimeControlViewModel(this.Pass.Sunlight.Duration);
            this.MissionEditor = new TimeControlViewModel(this.Pass.Mission.Duration);
            this.EncryptionEditor = new TimeControlViewModel(this.Pass.Encryption.Duration);
            this.DatalinkEditor = new TimeControlViewModel(this.Pass.Datalink.Duration);

            this.SaveCallback = editComplete;
            this.SaveCommand = new RelayCommand(this.SaveCommandHandler);
            this.CancelCommand = new RelayCommand(editCancel);
        }

        /// <summary>
        /// Gets the <see cref="PassData"/> that is currently being edited.
        /// </summary>
        public PassData Pass { get; }

        /// <summary>
        /// Gets the editor control for the sunlight phase duration.
        /// </summary>
        public TimeControlViewModel SunlightEditor { get; }

        /// <summary>
        /// Gets the editor control for the mission phase duration.
        /// </summary>
        public TimeControlViewModel MissionEditor { get; }

        /// <summary>
        /// Gets the editor control for the encryption phase duration.
        /// </summary>
        public TimeControlViewModel EncryptionEditor { get; }

        /// <summary>
        /// Gets the editor control for the datalink phase duration.
        /// </summary>
        public TimeControlViewModel DatalinkEditor { get; }

        /// <summary>
        /// Gets the command to execute to save and complete the edit operation.
        /// </summary>
        public ICommand SaveCommand { get; }

        /// <summary>
        /// Gets the command to execute to cancel the edit operation.
        /// </summary>
        public ICommand CancelCommand { get; }

        private Action<PassData> SaveCallback { get; }

        private void SaveCommandHandler()
        {
            var newPassData = new PassData(this.Pass.Name, this.Pass.StartTime, this.Pass.EndTime);
            newPassData.Sunlight.Duration = this.SunlightEditor.SelectedDuration;
            newPassData.Mission.Duration = this.MissionEditor.SelectedDuration;
            newPassData.Encryption.Duration = this.EncryptionEditor.SelectedDuration;
            newPassData.Datalink.Duration = this.DatalinkEditor.SelectedDuration;

            this.SaveCallback(newPassData);
        }
    }
}
