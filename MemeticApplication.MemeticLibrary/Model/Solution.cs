using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemeticApplication.MemeticLibrary.Generators;
using MemeticApplication.MemeticLibrary.Genetic;
using System.Runtime.Remoting.Contexts;

namespace MemeticApplication.MemeticLibrary.Model
{
    /// <summary>
    /// This class represents a solution for the VRPTW problem.
    /// </summary>
    public class Solution : Chromosome, ICloneable
    {
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
            IGene[] newGenes = new IGene[Genes.Length];
            Array.Copy(Genes, newGenes, Genes.Length);
            Solution copy = new Solution((VrptwProblem)Problem, newGenes);
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
            Customer depot = problem.Depot;
            Customer customer1 = depot, customer2;
            for (int i = 0; i < Genes.Length; ++i)
            {
                customer2 = (Customer)Genes[i].GetValue();
                distance = problem.GetDistance(customer1.Id, customer2.Id);
                if (distance + currentVehicleDistance > customer2.DueDate ||
                    currentCapacity + customer2.Demand > problem.VehicleCapacity)
                {
                    currentRoute.AddCustomer(depot);
                    currentRoute = CreateNewRoute(problem);
                    routes.Add(currentRoute);
                    customer1 = depot;
                    distance = problem.GetDistance(customer1.Id, customer2.Id);
                    currentVehicleDistance = 0.0f;
                    currentCapacity = 0;
                }
                currentRoute.AddCustomer(customer2);
                currentCapacity += customer2.Demand;
                currentVehicleDistance += distance + customer2.ServiceTime;
                if (currentVehicleDistance < customer2.ReadyTime)
                {
                    currentVehicleDistance += customer2.ReadyTime - currentVehicleDistance;
                }
                customer1 = customer2;
            }
            currentRoute.AddCustomer(depot);
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

        public override void Refresh()
        {
            Routes = GenesToRoutes();
        }

        public string ToFileText()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append((Problem as VrptwProblem).Name);
            sb.Append(Environment.NewLine);
            sb.Append("Liczba dróg: " + Routes.Count.ToString());
            sb.Append(Environment.NewLine);
            foreach (var route in Routes)
            {
                for (int i = 1; i < route.Customers.Count - 1; ++i)
                {
                    sb.Append(route.Customers[i].Id);
                    sb.Append(' ');
                }
                sb.Append(Environment.NewLine);
            }
            return sb.ToString();
        }
    }
}
