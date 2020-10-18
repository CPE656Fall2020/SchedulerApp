using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using SchedulerDatabase.Extensions;
using SchedulerGUI.Models;
using SchedulerGUI.Solver;

namespace SchedulerGUI.ViewModels.Controls
{
    /// <summary>
    /// <see cref="ScheduleViewerViewModel"/> provides a View-Model for the <see cref="Views.ScheduleViewer"/> control.
    /// </summary>
    public class ScheduleViewerViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleViewerViewModel"/> class.
        /// </summary>
        /// <param name="solution">The solution to visualize.</param>
        public ScheduleViewerViewModel(ScheduleSolution solution)
        {
            this.Warnings = new ObservableCollection<ScheduleSolution.SchedulerProblem>(
                solution.Problems.Where(p => p.Level == ScheduleSolution.SchedulerProblem.SeverityLevel.Warning));

            this.Errors = new ObservableCollection<ScheduleSolution.SchedulerProblem>(
                solution.Problems.Where(p => p.Level == ScheduleSolution.SchedulerProblem.SeverityLevel.Error));

            this.Fatal = new ObservableCollection<ScheduleSolution.SchedulerProblem>(
                solution.Problems.Where(p => p.Level == ScheduleSolution.SchedulerProblem.SeverityLevel.Fatal));

            this.SolutionPerPass = new Dictionary<PassOrbit, string>();
            foreach (var passSln in solution.ViableProfiles)
            {
                this.SolutionPerPass.Add(passSln.Key, passSln.Value?.ToFullDescription());
            }
        }

        /// <summary>
        /// Gets a collection of scheduling warnings.
        /// </summary>
        public ObservableCollection<ScheduleSolution.SchedulerProblem> Warnings { get; }

        /// <summary>
        /// Gets a collection of scheduling errors.
        /// </summary>
        public ObservableCollection<ScheduleSolution.SchedulerProblem> Errors { get; }

        /// <summary>
        /// Gets a collection of scheduling fatal errors.
        /// </summary>
        public ObservableCollection<ScheduleSolution.SchedulerProblem> Fatal { get; }

        /// <summary>
        /// Gets a mapping of solutions for each pass.
        /// </summary>
        public Dictionary<PassOrbit, string> SolutionPerPass { get; }
    }
}
