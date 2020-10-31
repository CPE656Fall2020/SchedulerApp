using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using SchedulerGUI.Models;
using SchedulerGUI.Solver;

namespace SchedulerGUI.ViewModels
{
    /// <summary>
    /// <see cref="ScheduleViewerDialogViewModel"/> provides a View-Model for the <see cref="Views.ScheduleViewerDialog"/> control.
    /// </summary>
    public class ScheduleViewerDialogViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleViewerDialogViewModel"/> class.
        /// </summary>
        /// <param name="solution">The solution to visualize.</param>
        public ScheduleViewerDialogViewModel(ScheduleSolution solution)
        {
            this.Warnings = new ObservableCollection<Solver.ScheduleSolution.SchedulerProblem>(
                solution.Problems.Where(p => p.Level == Solver.ScheduleSolution.SchedulerProblem.SeverityLevel.Warning));

            this.Errors = new ObservableCollection<Solver.ScheduleSolution.SchedulerProblem>(
                solution.Problems.Where(p => p.Level == Solver.ScheduleSolution.SchedulerProblem.SeverityLevel.Error));

            this.Fatal = new ObservableCollection<Solver.ScheduleSolution.SchedulerProblem>(
                solution.Problems.Where(p => p.Level == Solver.ScheduleSolution.SchedulerProblem.SeverityLevel.Fatal));

            this.SolutionPerPass = new Dictionary<PassOrbit, string>();
            foreach (var passSln in solution.ViableProfiles)
            {
                this.SolutionPerPass.Add(passSln.Key, passSln.Value?.FullProfileDescription);
            }
        }

        /// <summary>
        /// Gets a collection of scheduling warnings.
        /// </summary>
        public ObservableCollection<Solver.ScheduleSolution.SchedulerProblem> Warnings { get; }

        /// <summary>
        /// Gets a collection of scheduling errors.
        /// </summary>
        public ObservableCollection<Solver.ScheduleSolution.SchedulerProblem> Errors { get; }

        /// <summary>
        /// Gets a collection of scheduling fatal errors.
        /// </summary>
        public ObservableCollection<Solver.ScheduleSolution.SchedulerProblem> Fatal { get; }

        /// <summary>
        /// Gets a mapping of solutions for each pass.
        /// </summary>
        public Dictionary<PassOrbit, string> SolutionPerPass { get; }
    }
}
