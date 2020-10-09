using System;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SchedulerGUI.Interfaces;
using SchedulerGUI.Models;

namespace SchedulerGUI.ViewModels.Controls
{
    /// <summary>
    /// <see cref="EditWindowViewModel"/> provides a View-Model for the <see cref="Views.Controls.EditDialog"/>.
    /// </summary>
    public class EditWindowViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EditWindowViewModel"/> class.
        /// </summary>
        /// <param name="pass">The pass to edit.</param>
        /// <param name="editComplete">The action to execute when the edit is completed.</param>
        /// <param name="editCancel">The action to execute if the edit is cancelled.</param>
        public EditWindowViewModel(PassOrbit pass, Action<PassOrbit> editComplete, Action editCancel)
        {
            this.Pass = pass;

            this.SunlightPhase = this.Pass.PassPhases.First(x => x.PhaseName == Enums.PhaseType.Sunlight);
            this.MissionPhase = this.Pass.PassPhases.First(x => x.PhaseName == Enums.PhaseType.Mission);
            this.EncryptionPhase = this.Pass.PassPhases.First(x => x.PhaseName == Enums.PhaseType.Encryption);
            this.DatalinkPhase = this.Pass.PassPhases.First(x => x.PhaseName == Enums.PhaseType.Datalink);

            this.SaveCallback = editComplete;
            this.SaveCommand = new RelayCommand(this.SaveCommandHandler);
            this.CancelCommand = new RelayCommand(editCancel);
        }

        /// <summary>
        /// Gets the <see cref="PassOrbit"/> that is currently being edited.
        /// </summary>
        public PassOrbit Pass { get; }

        /// <summary>
        /// Gets the sunlight phase.
        /// </summary>
        public IPassPhase SunlightPhase { get; }

        /// <summary>
        /// Gets the mission phase.
        /// </summary>
        public IPassPhase MissionPhase { get; }

        /// <summary>
        /// Gets the encryption phase.
        /// </summary>
        public IPassPhase EncryptionPhase { get; }

        /// <summary>
        /// Gets the datalink phase.
        /// </summary>
        public IPassPhase DatalinkPhase { get; }

        /// <summary>
        /// Gets the command to execute to save and complete the edit operation.
        /// </summary>
        public ICommand SaveCommand { get; }

        /// <summary>
        /// Gets the command to execute to cancel the edit operation.
        /// </summary>
        public ICommand CancelCommand { get; }

        private Action<PassOrbit> SaveCallback { get; }

        private void SaveCommandHandler()
        {
            this.Pass.PassPhases.Clear();

            this.Pass.PassPhases.Add(this.SunlightPhase);
            this.Pass.PassPhases.Add(this.MissionPhase);
            this.Pass.PassPhases.Add(this.EncryptionPhase);
            this.Pass.PassPhases.Add(this.DatalinkPhase);

            this.SaveCallback(this.Pass);
        }
    }
}
