using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using SchedulerDatabase;
using SchedulerDatabase.Extensions;
using SchedulerDatabase.Models;
using SchedulerGUI.Enums;
using SchedulerGUI.Models;
using SchedulerGUI.ViewModels.Controls;
using Sdl.MultiSelectComboBox.EventArgs;
using Sdl.MultiSelectComboBox.Themes.Generic;

namespace SchedulerGUI.ViewModels
{
    /// <summary>
    /// <see cref="PlotWindowViewModel"/> provides a View-Model for the <see cref="Views.PlotWindow"/> view.
    /// </summary>
    public class PlotWindowViewModel : ViewModelBase
    {
        private PlotOption selectedOption = PlotOption.Raw;
        private IList<object> selectedAuthor;
        private IList<object> selectedPlatform;
        private IList<object> selectedProvider;
        private IList<object> selectedAccelerator;
        private IList<object> selectedClockSpeed;
        private IList<object> selectedNumCores;
        private IList<object> selectedProfile;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlotWindowViewModel"/> class.
        /// </summary>
        public PlotWindowViewModel()
        {
            this.Plot = new ProfileGraphViewModel();

            using (var context = SimpleIoc.Default.GetInstanceWithoutCaching<SchedulerContext>())
            {
                var summarizer = new SchedulingSummarizer(context);
                this.Authors = new ObservableCollection<string>(summarizer.GetAllTestAuthors());
                this.Platforms = new ObservableCollection<string>(summarizer.GetAllTestedPlatforms());
                this.Providers = new ObservableCollection<string>(summarizer.GetAllTestedProviders());
                this.Accelerators = new ObservableCollection<string>(Enum.GetNames(typeof(AESEncryptorProfile.AcceleratorType)));
                this.ClockSpeeds = new ObservableCollection<int>(summarizer.GetAllClockSpeeds().OrderBy(x => x));
                this.NumCores = new ObservableCollection<int>(summarizer.GetAllNumCores().OrderBy(x => x));
                this.Profiles = new ObservableCollection<string>(Enum.GetNames(typeof(ProfileType)));
            }

            this.FilterSelectionChangedCommand = new RelayCommand<SelectedItemsChangedEventArgs>(this.DropdownSelectionChangedHandler);

            this.GeneratePlot();
        }

        /// <summary>
        /// Gets a listing of available profile types.
        /// </summary>
        public ObservableCollection<string> Profiles { get; }

        /// <summary>
        /// Gets a listing of available authors.
        /// </summary>
        public ObservableCollection<string> Authors { get; }

        /// <summary>
        /// Gets a listing of available providers.
        /// </summary>
        public ObservableCollection<string> Providers { get; }

        /// <summary>
        /// Gets a listing of available platforms.
        /// </summary>
        public ObservableCollection<string> Platforms { get; }

        /// <summary>
        /// Gets a listing of available accelerators.
        /// </summary>
        public ObservableCollection<string> Accelerators { get; }

        /// <summary>
        /// Gets a listing of available clock speeds.
        /// </summary>
        public ObservableCollection<int> ClockSpeeds { get; }

        /// <summary>
        /// Gets a listing of available number of cores.
        /// </summary>
        public ObservableCollection<int> NumCores { get; }

        /// <summary>
        /// Gets the command to execute when the selection of options in a filter has changed.
        /// This really should not be necessary if SelectedItems was correctly bindable in
        /// sdl.MultiSelectComboBox. However, this works and is decently acceptable for now.
        /// </summary>
        public ICommand FilterSelectionChangedCommand { get; }

        /// <summary>
        /// Gets the title of the application.
        /// </summary>
        public string Title => $"CPE656 Data Plot Tool - {GlobalAssemblyInfo.InformationalVersion}";

        /// <summary>
        /// Gets or sets the selected plot option.
        /// </summary>
        public PlotOption SelectedOption
        {
            get => this.selectedOption;
            set => this.SetAndUpdatePlot(() => this.SelectedOption, ref this.selectedOption, value);
        }

        /// <summary>
        /// Gets or sets the selected profile type to display data for.
        /// </summary>
        public IList<object> SelectedProfile
        {
            get => this.selectedProfile;
            set => this.SetAndUpdatePlot(() => this.SelectedProfile, ref this.selectedProfile, value);
        }

        /// <summary>
        /// Gets or sets the selected author to display data from.
        /// </summary>
        public IList<object> SelectedAuthor
        {
            get => this.selectedAuthor;
            set => this.SetAndUpdatePlot(() => this.SelectedAuthor, ref this.selectedAuthor, value);
        }

        /// <summary>
        /// Gets or sets the selected platform to display data for.
        /// </summary>
        public IList<object> SelectedPlatform
        {
            get => this.selectedPlatform;
            set => this.SetAndUpdatePlot(() => this.SelectedPlatform, ref this.selectedPlatform, value);
        }

        /// <summary>
        /// Gets or sets the selected provider to display data for.
        /// </summary>
        public IList<object> SelectedProvider
        {
            get => this.selectedProvider;
            set => this.SetAndUpdatePlot(() => this.SelectedProvider, ref this.selectedProvider, value);
        }

        /// <summary>
        /// Gets or sets the selected accelerator type to display data for.
        /// </summary>
        public IList<object> SelectedAccelerator
        {
            get => this.selectedAccelerator;
            set => this.SetAndUpdatePlot(() => this.SelectedAccelerator, ref this.selectedAccelerator, value);
        }

        /// <summary>
        /// Gets or sets the selected processor clock speed to display data for.
        /// </summary>
        public IList<object> SelectedClockSpeed
        {
            get => this.selectedClockSpeed;
            set => this.SetAndUpdatePlot(() => this.SelectedClockSpeed, ref this.selectedClockSpeed, value);
        }

        /// <summary>
        /// Gets or sets the selected number of cores to display data for.
        /// </summary>
        public IList<object> SelectedNumCores
        {
            get => this.selectedNumCores;
            set => this.SetAndUpdatePlot(() => this.SelectedNumCores, ref this.selectedNumCores, value);
        }

        /// <summary>
        /// Gets the generated AES plot.
        /// </summary>
        public ProfileGraphViewModel Plot { get; }

        private void SetAndUpdatePlot<T>(Expression<Func<T>> propertyExpression, ref T field, T newValue)
        {
            this.Set(propertyExpression, ref field, newValue);
            this.GeneratePlot();
        }

        private void GeneratePlot()
        {
            using (var context = SimpleIoc.Default.GetInstanceWithoutCaching<SchedulerContext>())
            {
                var summarizer = new SchedulingSummarizer(context);

                // start with default data
                IEnumerable<IByteStreamProcessor> profileData = new List<IByteStreamProcessor>();
                var rawCompressionData = context.CompressorProfiles.AsEnumerable();
                var rawAESData = context.AESProfiles.AsEnumerable();

                // Include provider filters
                if (this.SelectedProvider?.Count > 0)
                {
                    rawAESData = rawAESData
                        .Where(a => this.SelectedProvider.Contains(a.ProviderName));
                }

                // Include accelerator filters
                if (this.SelectedAccelerator?.Count > 0)
                {
                    rawAESData = rawAESData
                        .Where(a => this.SelectedAccelerator.Contains(a.PlatformAccelerator.ToString()));
                }

                // include selected profile filters
                if (this.SelectedProfile?.Count > 1)
                {
                    profileData = rawAESData;
                    profileData = profileData.Concat(rawCompressionData);
                }
                else if (this.SelectedProfile?.Count > 0 && this.SelectedProfile.Contains(ProfileType.AES.ToString()))
                {
                    profileData = rawAESData;
                }
                else if (this.SelectedProfile?.Count == 1 && this.SelectedProfile.Contains(ProfileType.LZ4.ToString()))
                {
                    profileData = rawCompressionData;
                }

                // Include platform filters
                if (this.SelectedPlatform?.Count > 0)
                {
                    profileData = profileData
                        .Where(a => this.SelectedPlatform.Contains(a.PlatformName));
                }

                // Include author filters
                if (this.SelectedAuthor?.Count > 0)
                {
                    profileData = profileData
                        .Where(a => this.SelectedAuthor.Contains(a.Author));
                }

                // Include speed filters
                if (this.SelectedClockSpeed?.Count > 0)
                {
                    profileData = profileData
                        .Where(a => this.SelectedClockSpeed.Contains(a.TestedFrequency));
                }

                // Include core count filters
                if (this.SelectedNumCores?.Count > 0)
                {
                    profileData = profileData
                        .Where(a => this.SelectedNumCores.Contains(a.NumCores));
                }

                switch (this.SelectedOption)
                {
                    case PlotOption.Raw:
                        this.Plot.DisplayedData = profileData.ToList();
                        break;

                    case PlotOption.Summarized:
                        this.Plot.DisplayedData = summarizer.SummarizeDeviceResults(profileData).ToList();
                        break;
                }
            }
        }

        private void DropdownSelectionChangedHandler(SelectedItemsChangedEventArgs e)
        {
            // See https://github.com/sdl/Multiselect-ComboBox/issues/38
            // This control really does not work well for general cases,
            // but this work-around is close enough.
            var selected = new List<object>(e.Selected.Cast<object>());
            (e.Source as MultiSelectComboBox).SelectedItems = selected;
        }
    }
}
