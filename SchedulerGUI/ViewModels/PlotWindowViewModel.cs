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
using SchedulerDatabase.Models;
using SchedulerGUI.Enums;
using SchedulerGUI.Models;
using SchedulerGUI.ViewModels.Controls;
using Sdl.MultiSelectComboBox.EventArgs;
using Sdl.MultiSelectComboBox.Themes.Generic;
using TimelineLibrary;

namespace SchedulerGUI.ViewModels
{
    /// <summary>
    /// <see cref="PlotWindowViewModel"/> provides a View-Model for the <see cref="Views.PlotWindow"/> view.
    /// </summary>
    public class PlotWindowViewModel : ViewModelBase
    {
        private readonly ObservableCollection<string> aesAuthors;
        private readonly ObservableCollection<string> aesPlatforms;
        private readonly ObservableCollection<int> aesClockSpeeds;
        private readonly ObservableCollection<int> aesNumCores;

        private readonly ObservableCollection<string> compressionAuthors;
        private readonly ObservableCollection<string> compressionPlatforms;
        private readonly ObservableCollection<int> compressionClockSpeeds;
        private readonly ObservableCollection<int> compressionNumCores;

        private readonly ObservableCollection<string> allAuthors;
        private readonly ObservableCollection<string> allPlatforms;
        private readonly ObservableCollection<string> allProviders;
        private readonly ObservableCollection<string> allAccelerators;
        private readonly ObservableCollection<int> allClockSpeeds;
        private readonly ObservableCollection<int> allNumCores;
        private readonly ObservableCollection<string> allProfiles;

        private PlotOption selectedOption = PlotOption.Raw;
        private bool isAesProfileSelected;

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

                this.allAuthors = new ObservableCollection<string>(summarizer.GetAllTestAuthors());
                this.allPlatforms = new ObservableCollection<string>(summarizer.GetAllTestedPlatforms());
                this.allProviders = new ObservableCollection<string>(summarizer.GetAllTestedProviders());
                this.allAccelerators = new ObservableCollection<string>(Enum.GetNames(typeof(AESEncryptorProfile.AcceleratorType)));
                this.allClockSpeeds = new ObservableCollection<int>(summarizer.GetAllClockSpeeds().OrderBy(x => x));
                this.allNumCores = new ObservableCollection<int>(summarizer.GetAllNumCores().OrderBy(x => x));
                this.allProfiles = new ObservableCollection<string>(Enum.GetNames(typeof(ProfileType)));

                this.aesAuthors = new ObservableCollection<string>(summarizer.GetAESTestAuthors());
                this.aesPlatforms = new ObservableCollection<string>(summarizer.GetAESTestedPlatforms());
                this.aesClockSpeeds = new ObservableCollection<int>(summarizer.GetAESClockSpeeds().OrderBy(x => x));
                this.aesNumCores = new ObservableCollection<int>(summarizer.GetAESNumCores().OrderBy(x => x));

                this.compressionAuthors = new ObservableCollection<string>(summarizer.GetCompressionTestAuthors());
                this.compressionPlatforms = new ObservableCollection<string>(summarizer.GetCompressionTestedPlatforms());
                this.compressionClockSpeeds = new ObservableCollection<int>(summarizer.GetCompressionClockSpeeds().OrderBy(x => x));
                this.compressionNumCores = new ObservableCollection<int>(summarizer.GetCompressionNumCores().OrderBy(x => x));

                this.Authors = new ObservableCollection<string>();
                this.Platforms = new ObservableCollection<string>();
                this.ClockSpeeds = new ObservableCollection<int>();
                this.NumCores = new ObservableCollection<int>();

                this.allAuthors.ForEach(x => this.Authors.Add(x));
                this.allPlatforms.ForEach(x => this.Platforms.Add(x));
                this.allClockSpeeds.ForEach(x => this.ClockSpeeds.Add(x));
                this.allNumCores.ForEach(x => this.NumCores.Add(x));

                this.Providers = new ObservableCollection<string>(this.allProviders);
                this.Accelerators = new ObservableCollection<string>(this.allAccelerators);
                this.Profiles = new ObservableCollection<string>(this.allProfiles);
            }

            this.FilterSelectionChangedCommand = new RelayCommand<SelectedItemsChangedEventArgs>(this.DropdownSelectionChangedHandler);

            this.GeneratePlot();
        }

        /// <summary>
        /// Gets a listing of available profile types.
        /// </summary>
        public ObservableCollection<string> Profiles { get; }

        /// <summary>
        /// Gets or sets a listing of available authors.
        /// </summary>
        public ObservableCollection<string> Authors { get; set; }

        /// <summary>
        /// Gets a listing of available providers.
        /// </summary>
        public ObservableCollection<string> Providers { get; }

        /// <summary>
        /// Gets or sets a listing of available platforms.
        /// </summary>
        public ObservableCollection<string> Platforms { get; set; }

        /// <summary>
        /// Gets a listing of available accelerators.
        /// </summary>
        public ObservableCollection<string> Accelerators { get; }

        /// <summary>
        /// Gets or sets a listing of available clock speeds.
        /// </summary>
        public ObservableCollection<int> ClockSpeeds { get; set; }

        /// <summary>
        /// Gets or sets a listing of available number of cores.
        /// </summary>
        public ObservableCollection<int> NumCores { get; set; }

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
        /// Gets or sets a value indicating whether the AES profile is selected.
        /// </summary>
        public bool IsAesProfileSelected
        {
            get => this.isAesProfileSelected;
            set => this.Set(() => this.IsAesProfileSelected, ref this.isAesProfileSelected, value);
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

        private IEnumerable<IByteStreamProcessor> SetProfileData(SchedulerContext context)
        {
            // start with default data
            IEnumerable<IByteStreamProcessor> profileData = new List<IByteStreamProcessor>();
            var rawCompressionData = context.CompressorProfiles.AsEnumerable();
            var rawAESData = context.AESProfiles.AsEnumerable();

            if (this.SelectedProfile?.Count > 0)
            {
                if (this.SelectedProfile?.Count > 1)
                {
                    // Both AES and LZ4 are selected profiles
                    profileData = rawAESData;
                    profileData = profileData.Concat(rawCompressionData);
                    this.IsAesProfileSelected = true;
                    this.SetAllDataCollections();
                }
                else
                {
                    // Only one profile selected
                    if (this.SelectedProfile.Contains(ProfileType.AES.ToString()))
                    {
                        // AES is selected profile
                        profileData = rawAESData;
                        this.IsAesProfileSelected = true;
                        this.SetAesCollections();
                    }
                    else
                    {
                        // LZ4 is selected profile
                        profileData = rawCompressionData;
                        this.IsAesProfileSelected = false;
                        this.SetCompressionCollections();
                    }
                }
            }
            else
            {
                // No profile selected
                this.IsAesProfileSelected = false;
            }

            if (this.IsAesProfileSelected)
            {
                // Include provider filters
                if (this.SelectedProvider?.Count > 0)
                {
                    profileData = profileData
                        .Where(a => a is AESEncryptorProfile ap && this.SelectedProvider.Contains(ap.ProviderName));
                }

                // Include accelerator filters
                if (this.SelectedAccelerator?.Count > 0)
                {
                    profileData = profileData
                        .Where(a => a is AESEncryptorProfile ap && this.SelectedAccelerator.Contains(ap.PlatformAccelerator.ToString()));
                }
            }

            return profileData;
        }

        private void GeneratePlot()
        {
            using (var context = SimpleIoc.Default.GetInstanceWithoutCaching<SchedulerContext>())
            {
                var summarizer = new SchedulingSummarizer(context);

                var profileData = this.SetProfileData(context);

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
                        var summarizedAES = summarizer.SummarizeDeviceResults(profileData.OfType<AESEncryptorProfile>());
                        var summarizedLZ4 = summarizer.SummarizeDeviceResults(profileData.OfType<CompressorProfile>());
                        this.Plot.DisplayedData = Enumerable.Concat<IByteStreamProcessor>(summarizedAES, summarizedLZ4).ToList();
                        break;
                }
            }
        }

        private void SetAllDataCollections()
        {
            this.Authors.Clear();
            this.allAuthors.ForEach(x => this.Authors.Add(x));

            this.Platforms.Clear();
            this.allPlatforms.ForEach(x => this.Platforms.Add(x));

            this.NumCores.Clear();
            this.allNumCores.ForEach(x => this.NumCores.Add(x));

            this.ClockSpeeds.Clear();
            this.allClockSpeeds.ForEach(x => this.ClockSpeeds.Add(x));
        }

        private void SetAesCollections()
        {
            this.Authors.Clear();
            this.aesAuthors.ForEach(x => this.Authors.Add(x));

            this.ClockSpeeds.Clear();
            this.aesClockSpeeds.ForEach(x => this.ClockSpeeds.Add(x));

            this.NumCores.Clear();
            this.aesNumCores.ForEach(x => this.NumCores.Add(x));

            this.Platforms.Clear();
            this.aesPlatforms.ForEach(x => this.Platforms.Add(x));
        }

        private void SetCompressionCollections()
        {
            this.Authors.Clear();
            this.compressionAuthors.ForEach(x => this.Authors.Add(x));

            this.ClockSpeeds.Clear();
            this.compressionClockSpeeds.ForEach(x => this.ClockSpeeds.Add(x));

            this.NumCores.Clear();
            this.compressionNumCores.ForEach(x => this.NumCores.Add(x));

            this.Platforms.Clear();
            this.compressionPlatforms.ForEach(x => this.Platforms.Add(x));
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
