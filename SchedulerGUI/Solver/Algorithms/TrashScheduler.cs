using System.Collections.Generic;
using System.Linq;
using SchedulerDatabase;
using SchedulerDatabase.Models;
using SchedulerGUI.Models;

namespace SchedulerGUI.Solver.Algorithms
{
    /// <summary>
    /// <see cref="TrashScheduler"/> attempts to solve a scheduling problem by never actually solving it.
    /// This scheduler is a placeholder for testing the error handling capabilities.
    /// </summary>
    public class TrashScheduler : IScheduleSolver
    {
        /// <inheritdoc/>
        public string Name => "Trash Scheduler";

        /// <inheritdoc/>
        public object Tag { get; set; }

        /// <inheritdoc/>
        public ScheduleSolution Solve(IEnumerable<PassOrbit> passes, IEnumerable<AESEncyptorProfile> availableProfiles)
        {
            var solution = new ScheduleSolution()
            {
                IsSolvable = false,
                ViableProfiles = new Dictionary<PassOrbit, AESEncyptorProfile>(),
                Problems = new List<ScheduleSolution.SchedulerProblem>()
                {
                    new ScheduleSolution.SchedulerProblem(ScheduleSolution.SchedulerProblem.SeverityLevel.Fatal, "This scheduler will never work."),
                },
            };

            return solution;
        }
    }
}
