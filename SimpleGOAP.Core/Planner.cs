using System;
using System.Collections.Generic;
using System.Linq;
using Priority_Queue;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace SimpleGOAP
{
    /// <summary>The GOAP planner which runs search on possible futures to find a path to a goal state.</summary>
    /// <typeparam name="T">The type representing state.</typeparam>
    public class Planner<T>
    {
        private readonly IStateCopier<T> stateCopier;
        private readonly IEqualityComparer<T> stateComparer;
        private readonly ILogger<Planner<T>> logger;

        public Planner(IStateCopier<T> stateCopier, IEqualityComparer<T> stateComparer, ILogger<Planner<T>> logger=null)
        {
            if (logger == null)
            {
                logger = new NullLogger<Planner<T>>();
            }
            this.logger = logger;
            this.stateCopier = stateCopier;
            this.stateComparer = stateComparer;
        }

        /// <summary>Execute the plan.</summary>
        public Plan<T> Execute(PlanParameters<T> @params)
        {
            if (@params.GoalsEvaluator == null)
                throw new ArgumentOutOfRangeException(nameof(@params.GoalsEvaluator));
            if (@params.HeuristicCost == null)
                throw new ArgumentOutOfRangeException(nameof(@params.HeuristicCost));
            if (@params.GetActions == null)
                throw new ArgumentOutOfRangeException(nameof(@params.GetActions));
            if (@params.StartingState == null)
                throw new ArgumentOutOfRangeException(nameof(@params.StartingState));

            /*
             * AStar:
             *  g score: current distance from the start measured by sum of action costs
             *  h score: heuristic of how close the node's state is to goal state, supplied by caller
             *  f score: sum of (g, h), used as priority of the node
             */
            var heuristicCost = @params.HeuristicCost;
            var evalGoals = @params.GoalsEvaluator;
            var maxHScore = @params.MaxHeuristicCost;

            var start = new StateNode<T>(@params.StartingState, null, null);
            var openSet = CreateQueue(@params);
            openSet.Enqueue(start, 0);

            var distanceScores = new DefaultDictionary<T, int>(int.MaxValue, stateComparer)
            {
                [start.ResultingState] = 0
            };

            var iterations = 0;
            StateNode<T> current = null;
            while (openSet.Any() && ++iterations < @params.MaxIterations)
            {
                current = openSet.Dequeue();
                var sourceActionTitle = "(root)";
                if (current.SourceAction != null)
                {
                    sourceActionTitle = current.SourceAction.Title;
                }
                logger.LogTrace($">>> Iteration {iterations} with {openSet.Count+1} in Set after {sourceActionTitle}");
                logger.LogDebug($"{current.ResultingState.ToString()}");

                int goalIndex = 0;
                foreach(var evalGoal in evalGoals) {
                    if (evalGoal(current.ResultingState))
                    {
                        return ReconstructPath(current, @params.StartingState, iterations, goalIndex);
                    }
                    goalIndex++;
                }

                foreach (var neighbor in GetNeighbors(current, @params.GetActions(current.ResultingState)))
                {
                    logger.LogTrace($"[???] Testing Action {neighbor.SourceAction.Title}");
                    var distScore = distanceScores[current.ResultingState] + neighbor.GetActionCost(current.ResultingState);
                    if (distScore >= distanceScores[neighbor.ResultingState])
                    {
                        logger.LogDebug($"[...] Skipping due to action cost: {neighbor.SourceAction.Title}");
                        continue;
                    }

                    distanceScores[neighbor.ResultingState] = distScore;
                    var hCost = heuristicCost(neighbor.ResultingState);
                    if (hCost > maxHScore)
                    {
                        logger.LogDebug($"[...] Skipping due to heuristic cost: {neighbor.SourceAction.Title}");
                        continue;
                    }

                    if (!openSet.Contains(neighbor))
                    {
                        var finalScore = distScore + hCost;
                        logger.LogDebug($"[+++] Enqueuing action '{neighbor.SourceAction.Title}' with finalScore {finalScore} ({distScore} + {hCost})");
                        openSet.Enqueue(neighbor, finalScore);
                    }
                    else
                    {
                        logger.LogDebug($"[...] Skipping due to duplicate");
                    }
                }
            }

            if (current != null)
            {
                return ReconstructPath(current, @params.StartingState, iterations, -1);
            }

            return new Plan<T>
            {
                GoalIndex = -1,
                Steps = new List<PlanStep<T>>(),
                Iterations = iterations
            };
        }

        private static IPriorityQueue<StateNode<T>, float> CreateQueue(PlanParameters<T> args)
        {
            if (args.UseFastQueue)
                return new FastPriorityQueue<StateNode<T>>(args.QueueMaxSize);
            return new SimplePriorityQueue<StateNode<T>>();
        }

        private static Plan<T> ReconstructPath(StateNode<T> final, T startingState, int iterations, int goalIndex)
        {
            var current = final;
            var path = new List<StateNode<T>>();
            while (current.Parent != null)
            {
                path.Add(current);
                current = current.Parent;
            }

            path.Reverse();
            return new Plan<T>
            {
                GoalIndex = goalIndex,
                Steps = path.Select((step, i) => new PlanStep<T>
                {
                    Index = i,
                    Action = step.SourceAction,
                    AfterState = step.ResultingState,
                    BeforeState = step.Parent == null ? startingState : step.Parent.ResultingState
                }).ToList(),
                Iterations = iterations
            };
        }

        private IEnumerable<StateNode<T>> GetNeighbors(StateNode<T> start,
            IEnumerable<IAction<T>> actions)
        {
            var currentState = start.ResultingState;

            foreach (var action in actions)
            {
                var newState = action.TakeActionOnState(stateCopier.Copy(currentState));

                // sometimes actions have no effect on state, in which case we don't want to entertain them as nodes
                // assuming that additional actions to get to the same state is always worse
                if(!stateComparer.Equals(currentState, newState))
                    yield return new StateNode<T>(newState, start, action);
            }
        }
    }
}
