using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemeticApplication.MemeticLibrary.Generators;
using MemeticApplication.MemeticLibrary.Genetic;

namespace MemeticApplication.MemeticLibrary.Model
{
    /// <summary>
    /// This class represents a solution for the VRPTW problem.
    /// </summary>
    public class Solution : Chromosome, ICloneable
    {
        private static readonly object Locker = new object();

        public int CustomerNumber { get { return Genes != null ? Genes.Count() : 0; } }

        /// <summary>
        /// The vector of routes which makes for the soluton.
        /// </summary>
        public IList<Route> Routes { get; protected set; }
        
        public Solution(VrptwProblem problem) : base(problem)
        {
            for (int i = 0; i < problem.Customers.Count; ++i)
            {
                Genes[i] = new CustomerGene(problem.Customers[i]);
            }
            Routes = GenesToRoutes();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Solution"/> class.
        /// Copies the reference to the problem and creates new array with
        /// the same references to genes.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="genes">The genes.</param>
        public Solution(VrptwProblem problem, IGene[] genes) : base(problem, genes)
        {
            Routes = GenesToRoutes();
        }

        public Solution(VrptwProblem problem, IList<Customer> customers) : base(problem)
        {
            int counter = 0;
            foreach (var customer in customers)
            {
                Genes[counter++] = new CustomerGene(customer); 
            }
            Routes = GenesToRoutes();
        }

        /// <summary>
        /// Adds the route to the solution.
        /// </summary>
        /// <param name="route">The route.</param>
        public void AddRoute(Route route)
        {
            Routes.Add(route);
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// A new solution instance gets a reference to the same problem
        /// and a new route list instance with the same references.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            Solution copy = new Solution((VrptwProblem)Problem, Genes);
            return copy;
        }

        /// <summary>
        /// Determines whether this solution is feasible
        /// for a given problem instance.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this solution is feasible; otherwise, <c>false</c>.
        /// </returns>
        public bool IsFeasible()
        {
            bool isFeasible = true;
            foreach (var route in Routes)
            {
                if (!route.IsFeasible())
                {
                    isFeasible = false;
                    break;
                }
            }
            return isFeasible;
        }

        /// <summary>
        /// Returns the total the distance which vehicle travels on this route instance.
        /// </summary>
        /// <returns></returns>
        public float TotalDistance()
        {
            float distance = 0.0f;
            foreach (var route in Routes)
            {
                distance += route.TotalDistance();
            }
            return distance;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// The represenattion consists of numbers of vehicles and a sum of routes' distances.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "Routes: " + Routes.Count + ", distance: " + TotalDistance();
        }

        /// <summary>
        /// Converts the vector of Routes representation to the vector of Customers.
        /// </summary>
        /// <returns>Vector of Customers</returns>
        public List<Customer> ToVector()
        {
            var vector = new List<Customer>();
            foreach (var route in Routes)
            {
                var custromers = route.Customers;
                for (int i = 1; i < custromers.Count - 1; ++i)
                {
                    vector.Add(custromers[i]);
                }
            }
            return vector;
        }
        
        public override int CompareTo(object obj)
        {
            Solution other = (Solution)obj;
            if (Routes.Count > other.Routes.Count)
                return 1;
            else if (Routes.Count < other.Routes.Count)
                return -1;
            else
            {
                float myDistance = TotalDistance();
                float otherDistance = other.TotalDistance();
                if (myDistance > otherDistance)
                    return 1;
                else if (myDistance < otherDistance)
                    return -1;
                return 0;
            }
        }

        /// <summary>
        /// Returns the fitness of this solution for the given problem.
        /// </summary>
        /// <returns></returns>
        public override float[] FitnessValues()
        {
            int vehicleNumber = Routes.Count;
            float distance = TotalDistance();
            return new float[] { vehicleNumber, distance };
        }

        public override bool Equals(object obj)
        {
            Solution other = obj as Solution;
            if (other == null)
                return false;
            for (int i = 0; i < Genes.Count(); ++i)
                if (!Genes[i].Equals(other.Genes[i]))
                    return false;
            return true;
        }

        private IList<Route> GenesToRoutes()
        {
            VrptwProblem problem = Problem as VrptwProblem;
            IList<Route> routes = new List<Route>();
            float distance, currentVehicleDistance = 0.0f;
            uint currentCapacity = 0;
            Route currentRoute = CreateNewRoute(problem);
            routes.Add(currentRoute);
            Customer customer1 = problem.Depot, customer2;
            foreach (var gene in Genes)
            {
                lock (Locker)
                {
                    customer2 = (Customer)gene.GetValue();
                    distance = problem.GetDistance(customer1.Id, customer2.Id);
                    if (distance + currentVehicleDistance > customer2.DueDate ||
                        currentCapacity + customer2.Demand > problem.VehicleCapacity)
                    {
                        currentRoute.AddCustomer(problem.Depot);
                        currentRoute = CreateNewRoute(problem);
                        routes.Add(currentRoute);
                        customer1 = problem.Depot;
                        distance = problem.GetDistance(customer1.Id, customer2.Id);
                        currentVehicleDistance = 0.0f;
                        currentCapacity = 0;
                    }
                    currentRoute.AddCustomer(customer2);
                    currentCapacity += customer2.Demand;
                    currentVehicleDistance += distance;
                    if (currentVehicleDistance < customer2.ReadyTime)
                    {
                        currentVehicleDistance += customer2.ReadyTime - currentVehicleDistance;
                    }
                    customer1 = customer2;
                }
            }
            currentRoute.AddCustomer(problem.Depot);
            return routes;
        }

        /// <summary>
        /// Creates the new route.
        /// </summary>
        /// <returns></returns>
        private Route CreateNewRoute(VrptwProblem problem)
        {
            Route route = new Route(problem);
            route.AddCustomer(problem.Depot);
            return route;
        }
    }
}
