using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using GalaSoft.MvvmLight;
using SchedulerGUI.Models;

namespace SchedulerGUI.ViewModels.Controls
{
    /// <summary>
    /// <see cref="EditSolarCellControlViewModel"/> provides a ViewModel for the <see cref="Views.Controls.EditSolarCellControl"/> control.
    /// </summary>
    public class EditSolarCellControlViewModel : ViewModelBase
    {
        private IEnumerable<PassOrbit> passes;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditSolarCellControlViewModel"/> class.
        /// </summary>
        public EditSolarCellControlViewModel()
        {
        }

        /// <summary>
        /// Gets or sets the pass data that should be used to build the historical display.
        /// </summary>
        public IEnumerable<PassOrbit> Passes
        {
            get => this.passes;
            set
            {
                this.passes = value;
                this.InitDefaultSolarData();
            }
        }

        /// <summary>
        /// Modifies the initial passes based on the defaults of the edit solar control vm.
        /// </summary>
        private void InitDefaultSolarData()
        {
        }
    }
}