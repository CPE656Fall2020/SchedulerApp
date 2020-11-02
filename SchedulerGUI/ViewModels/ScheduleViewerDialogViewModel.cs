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
            this.Warnings = new ObservableCollection<ScheduleSolution.SchedulerProblem>(
                solution.Problems.Where(p => p.Level == ScheduleSolution.SchedulerProblem.SeverityLevel.Warning));

            this.Errors = new ObservableCollection<ScheduleSolution.SchedulerProblem>(
                solution.Problems.Where(p => p.Level == ScheduleSolution.SchedulerProblem.SeverityLevel.Error));

            this.Fatal = new ObservableCollection<ScheduleSolution.SchedulerProblem>(
                solution.Problems.Where(p => p.Level == ScheduleSolution.SchedulerProblem.SeverityLevel.Fatal));

            this.SolutionPerPass = new Dictionary<PassOrbit, string>();
            foreach (var passSln in solution.ViableProfiles)
            {
                // TODO - this is a placeholder just to show both enc and compression in the same place
                var description = string.Empty;

                var phasesForPass = passSln.Value;

                if (phasesForPass.ContainsKey(Enums.PhaseType.Encryption))
                {
                    description += passSln.Value[Enums.PhaseType.Encryption]?.FullProfileDescription;
                    description += "\n\n";
                }

                if (phasesForPass.ContainsKey(Enums.PhaseType.Datalink))
                {
                    description += passSln.Value[Enums.PhaseType.Datalink]?.FullProfileDescription;
                }

                this.SolutionPerPass.Add(passSln.Key, description);
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
