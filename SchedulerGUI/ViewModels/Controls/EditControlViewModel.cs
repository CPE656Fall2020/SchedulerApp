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
    /// <see cref="EditControlViewModel"/> provides a View-Model for the <see cref="Views.Controls.EditControl"/>.
    /// </summary>
    public class EditControlViewModel : ViewModelBase
    {
        private IPassPhase sunlightPhase;
        private IPassPhase missionPhase;
        private IPassPhase datalinkPhase;
        private EncryptionPassPhase encryptionPhase;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditControlViewModel"/> class.
        /// </summary>
        /// <param name="pass">The pass to edit.</param>
        /// <param name="editComplete">The action to execute when the edit is completed.</param>
        public EditControlViewModel(PassOrbit pass, Action<PassOrbit> editComplete)
        {
            this.Pass = pass;
            this.InitPhases();

            this.SaveCallback = editComplete;
            this.SaveCommand = new RelayCommand(this.SaveCommandHandler);
            this.CancelCommand = new RelayCommand(this.CancelCommandHandler);
        }

        /// <summary>
        /// Gets the <see cref="PassOrbit"/> that is currently being edited.
        /// </summary>
        public PassOrbit Pass { get; }

        /// <summary>
        /// Gets or sets the sunlight phase.
        /// </summary>
        public IPassPhase SunlightPhase
        {
            get => this.sunlightPhase;
            set => this.Set(() => this.SunlightPhase, ref this.sunlightPhase, value);
        }

        /// <summary>
        /// Gets or sets the mission phase.
        /// </summary>
        public IPassPhase MissionPhase
        {
            get => this.missionPhase;
            set => this.Set(() => this.MissionPhase, ref this.missionPhase, value);
        }

        /// <summary>
        /// Gets or sets the encryption phase.
        /// </summary>
        public EncryptionPassPhase EncryptionPhase
        {
            get => this.encryptionPhase;
            set => this.Set(() => this.EncryptionPhase, ref this.encryptionPhase, value);
        }

        /// <summary>
        /// Gets or sets the datalink phase.
        /// </summary>
        public IPassPhase DatalinkPhase
        {
            get => this.datalinkPhase;
            set => this.Set(() => this.DatalinkPhase, ref this.datalinkPhase, value);
        }

        /// <summary>
        /// Gets the command to execute to save and complete the edit operation.
        /// </summary>
        public ICommand SaveCommand { get; }

        /// <summary>
        /// Gets the command to execute to cancel the edit operation.
        /// </summary>
        public ICommand CancelCommand { get; }

        private Action<PassOrbit> SaveCallback { get; }

        private void CancelCommandHandler()
        {
            this.InitPhases();
        }

        private void InitPhases()
        {
            this.SunlightPhase = this.Pass.PassPhases.First(x => x.PhaseName == Enums.PhaseType.Sunlight);
            this.MissionPhase = this.Pass.PassPhases.First(x => x.PhaseName == Enums.PhaseType.Mission);
            this.EncryptionPhase = (EncryptionPassPhase)this.Pass.PassPhases.First(x => x.PhaseName == Enums.PhaseType.Encryption);
            this.DatalinkPhase = this.Pass.PassPhases.First(x => x.PhaseName == Enums.PhaseType.Datalink);
        }

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
