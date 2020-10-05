using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using SchedulerDatabase;
using SchedulerDatabase.Models;
using SchedulerGUI.Models;
using SchedulerGUI.ViewModels.Controls;

namespace SchedulerGUI.ViewModels
{
    /// <summary>
    /// <see cref="PlotWindowViewModel"/> provides a View-Model for the <see cref="SchedulerGUI.Views.PlotWindow"/> view.
    /// </summary>
    public class PlotWindowViewModel : ViewModelBase
    {
        private const string AllItems = "(All)";

        private PlotOption selectedOption = PlotOption.Raw;
        private string selectedAuthor = AllItems;
        private string selectedPlatform = AllItems;
        private string selectedAccelerator = AllItems;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlotWindowViewModel"/> class.
        /// </summary>
        public PlotWindowViewModel()
        {
            this.Plot = new AESGraphViewModel();

            using (var context = SimpleIoc.Default.GetInstanceWithoutCaching<SchedulerContext>())
            {
                var summarizer = new SchedulingSummarizer(context);
                this.Authors = new ObservableCollection<string>(summarizer.GetAllTestAuthors());
                this.Platforms = new ObservableCollection<string>(summarizer.GetAllTestedPlatforms());
                this.Accelerators = new ObservableCollection<string>(Enum.GetNames(typeof(AESEncyptorProfile.AcceleratorType)));
            }

            this.Authors.Insert(0, AllItems);
            this.Platforms.Insert(0, AllItems);
            this.Accelerators.Insert(0, AllItems);

            this.GeneratePlot();
        }

        /// <summary>
        /// Gets a listing of available authors.
        /// </summary>
        public ObservableCollection<string> Authors { get; }

        /// <summary>
        /// Gets a listing of available platforms.
        /// </summary>
        public ObservableCollection<string> Platforms { get; }

        /// <summary>
        /// Gets a listing of available accelerators.
        /// </summary>
        public ObservableCollection<string> Accelerators { get; }

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
        /// Gets or sets the selected author to display data from.
        /// </summary>
        public string SelectedAuthor
        {
            get => this.selectedAuthor;
            set => this.SetAndUpdatePlot(() => this.SelectedAuthor, ref this.selectedAuthor, value);
        }

        /// <summary>
        /// Gets or sets the selected platform to display data for.
        /// </summary>
        public string SelectedPlatform
        {
            get => this.selectedPlatform;
            set => this.SetAndUpdatePlot(() => this.SelectedPlatform, ref this.selectedPlatform, value);
        }

        /// <summary>
        /// Gets or sets the selected accelerator type to display data for.
        /// </summary>
        public string SelectedAccelerator
        {
            get => this.selectedAccelerator;
            set => this.SetAndUpdatePlot(() => this.SelectedAccelerator, ref this.selectedAccelerator, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the throughput axis should be shown on a log scale.
        /// </summary>
        public bool ShowThroughputLogarithmic
        {
            get => this.Plot.ShowThroughputLogarithmic;
            set => this.Plot.ShowThroughputLogarithmic = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the energy axis should be shown on a log scale.
        /// </summary>
        public bool ShowEnergyLogarithmic
        {
            get => this.Plot.ShowEnergyLogarithmic;
            set => this.Plot.ShowEnergyLogarithmic = value;
        }

        /// <summary>
        /// Gets the generated AES plot.
        /// </summary>
        public AESGraphViewModel Plot { get; }

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
                var rawData = context.AESProfiles.AsQueryable();

                // Include platform filters
                if (this.SelectedPlatform != AllItems)
                {
                    rawData = rawData
                        .Where(a => a.PlatformName == this.SelectedPlatform);
                }

                // Include author filters
                if (this.SelectedAuthor != AllItems)
                {
                    rawData = rawData
                        .Where(a => a.Author == this.SelectedAuthor);
                }

                // Include accelerator filters
                if (this.SelectedAccelerator != AllItems)
                {
                    var accelerator = (AESEncyptorProfile.AcceleratorType)Enum.Parse(typeof(AESEncyptorProfile.AcceleratorType), this.SelectedAccelerator);
                    rawData = rawData
                        .Where(a => a.PlatformAccelerator == accelerator);
                }

                switch (this.SelectedOption)
                {
                    case PlotOption.Raw:
                        this.Plot.DisplayedData = rawData.ToList();
                        break;

                    case PlotOption.Summarized:
                        this.Plot.DisplayedData = summarizer.SummarizeResults(rawData).ToList();
                        break;
                }
            }
        }
    }
}
