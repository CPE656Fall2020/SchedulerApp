using System.Collections.Generic;
using SchedulerDatabase.Models;
using SchedulerGUI.Models;

namespace SchedulerGUI.Solver
{
    /// <summary>
    /// <see cref="IScheduleSolver"/> defines a generic schedule solving algorithm.
    /// </summary>
    public interface IScheduleSolver
    {
        /// <summary>
        /// Gets the name of this solver.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets or sets freeform user content used to tag this algorithm.
        /// </summary>
        object Tag { get; set; }

        /// <summary>
        /// Computes a solution to the scheduling problem, and stores the final energy values in <see cref="PassOrbit"/>.
        /// </summary>
        /// <param name="passes">The orbital passes and associated constraints that need scheduled.</param>
        /// <param name="availableProfiles">The AES profiles describing the devices that are available to schedule for.</param>
        /// <param name="battery">The specification for the battery powering the satellite.</param>
        /// <returns>A <see cref="ScheduleSolution"/> describing if the scenerio is possible and which device are required.</returns>
        ScheduleSolution Solve(IEnumerable<PassOrbit> passes, IEnumerable<AESEncyptorProfile> availableProfiles, Battery battery);
    }
}
