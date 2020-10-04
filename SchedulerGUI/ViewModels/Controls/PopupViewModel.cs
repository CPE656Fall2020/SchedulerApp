using System.Windows.Input;
using GalaSoft.MvvmLight;

namespace SchedulerGUI.ViewModels.Controls
{
    /// <summary>
    /// <see cref="PopupViewModel"/> provides a ViewModel for the <see cref="SchedulerGUI.Views.Controls.Popup"/> control.
    /// </summary>
    /// <remarks>
    /// Adapted from https://github.com/alexdillon/GroupMeClient/blob/develop/GroupMeClient.Core/ViewModels/Controls/PopupViewModel.cs.
    /// </remarks>
    public class PopupViewModel : ViewModelBase
    {
        private ViewModelBase popupDialog;
        private ICommand closePopup;
        private ICommand easyClosePopup;
        private bool isShowingDialog;

        /// <summary>
        /// Initializes a new instance of the <see cref="PopupViewModel"/> class.
        /// </summary>
        public PopupViewModel()
        {
        }

        /// <summary>
        /// Gets or sets the Popup Dialog that should be displayed.
        /// Null specifies that no popup is shown.
        /// </summary>
        public ViewModelBase PopupDialog
        {
            get => this.popupDialog;
            set
            {
                this.Set(() => this.PopupDialog, ref this.popupDialog, value);
                this.IsShowingDialog = this.PopupDialog != null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether a dialog is being shown.
        /// </summary>
        public bool IsShowingDialog
        {
            get => this.isShowingDialog;
            private set => this.Set(() => this.IsShowingDialog, ref this.isShowingDialog, value);
        }

        /// <summary>
        /// Gets or sets the action to be be performed when the big popup has been closed.
        /// </summary>
        public ICommand ClosePopup
        {
            get => this.closePopup;
            set => this.Set(() => this.ClosePopup, ref this.closePopup, value);
        }

        /// <summary>
        /// Gets or sets the action to be be performed when the big popup has been closed indirectly.
        /// This typically is from the user clicking in the gray area around the popup to dismiss it.
        /// </summary>
        public ICommand EasyClosePopup
        {
            get => this.easyClosePopup;
            set => this.Set(() => this.EasyClosePopup, ref this.easyClosePopup, value);
        }
    }
}
