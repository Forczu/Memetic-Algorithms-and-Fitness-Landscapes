using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemeticApplication.MemeticLibrary.Genetic;
using MemeticApplication.MemeticLibrary.Model;

namespace MemeticApplication.MemeticLibrary.Generators
{
    public class SolutionGenerator
    {
        public static Solution RandomFeasibleSolution(VrptwProblem problem)
        {
            Solution solution = new Solution(problem);
            var customersToAssign = new List<Customer>(problem.Customers);
            while (customersToAssign.Count != 0)
            {
                Route route = new Route(problem);
                route.AddCustomer(problem.Depot);
                while (true)
                {
                    float totalTime = route.TotalTime();
                    Customer lastOneInRoute = route.Customers.Last();
                    var feasibleCustomers = customersToAssign.Where(
                         c =>
                         c.ReadyTime >= totalTime + problem.GetDistance(c.Id, lastOneInRoute.Id) ||
                        (c.ReadyTime < totalTime + problem.GetDistance(c.Id, lastOneInRoute.Id) &&
                         c.DueDate >= totalTime + problem.GetDistance(c.Id, lastOneInRoute.Id))).ToList();
                    if (feasibleCustomers.Count > 0)
                    {
                        Customer randomCustomer = feasibleCustomers.ElementAt(RandomGenerator.NextInt(feasibleCustomers.Count()));
                        customersToAssign.Remove(randomCustomer);
                        route.AddCustomer(randomCustomer);
                    }
                    else
                    {
                        route.AddCustomer(problem.Depot);
                        solution.Routes.Add(route);
                        break;
                    }
                }
            }
            return solution;
        }

        /// <summary>
        /// Creates a new feasible solution chosen randomly from the neighbourbood
        /// of the <param name="solution">solution</param>.
        /// </summary>
        /// <param name="solution">The solution.</param>
        /// <returns></returns>
        public static Solution FeasibleNeighbourSolution(Solution solution)
        {
            Solution newSolution = (Solution)solution.Clone();
            bool isSolutionFeasible;
            do
            {
                int routeToTakeFromIndex, routeToGiveToIndex;
                RandomGenerator.NextTwoDifferentInts(newSolution.Routes.Count, out routeToTakeFromIndex, out routeToGiveToIndex);
                Route routeToTakeFrom = newSolution.Routes.ElementAt(routeToTakeFromIndex);
                Route routeToGiveTo = newSolution.Routes.ElementAt(routeToGiveToIndex);
                // backup copies in the case the solution was infeasible
                Route routeToTakeFromCopy = (Route)routeToTakeFrom.Clone();
                Route routeToGiveToCopy = (Route)routeToGiveTo.Clone();
                // move customer to another route
                int customerToMoveIndex = RandomGenerator.NextInt(routeToTakeFrom.Customers.Count - 2) + 1;
                var customerToMove = routeToTakeFrom.Customers.ElementAt(customerToMoveIndex);
                routeToTakeFrom.RemoveCustomer(customerToMove);
                int placeToMoveCustomer = RandomGenerator.NextInt(routeToGiveTo.Customers.Count - 2) + 1;
                routeToGiveTo.AddCustomer(customerToMove, placeToMoveCustomer);
                isSolutionFeasible = newSolution.IsFeasible();
                // if new solution sint feasible, bring back saved routes
                if (!isSolutionFeasible)
                {
                    newSolution.Routes[routeToTakeFromIndex] = routeToTakeFromCopy;
                    newSolution.Routes[routeToGiveToIndex] = routeToGiveToCopy;
                }
                // if route is empty, remove it from the solution
                else if (routeToTakeFrom.IsEmpty())
                {
                    newSolution.Routes.Remove(routeToTakeFrom);
                }
            } while (!isSolutionFeasible);
            return newSolution;
        }

        /// <summary>
        /// Creates a solution from the neighbourhood of the passed solution.
        /// The neighbour solution is chosen randomly.
        /// </summary>
        /// <param name="solution">The original solution.</param>
        /// <returns>The neighbour solution.</returns>
        public static Solution RandomNeighbourSolution(Solution solution)
        {
            var customerVector = solution.Genes;
            int position1, position2;
            Solution newSolution;
            do
            {
                RandomGenerator.NextTwoDifferentInts(customerVector.Count(), out position1, out position2);
                IGene swappedCustomer1 = customerVector[position1];
                IGene swappedCustomer2 = customerVector[position2];
                customerVector[position1] = swappedCustomer2;
                customerVector[position2] = swappedCustomer1;
                newSolution = new Solution((VrptwProblem)solution.Problem, customerVector);
            } while (!newSolution.IsFeasible());
            return newSolution;
        }
    }
}
