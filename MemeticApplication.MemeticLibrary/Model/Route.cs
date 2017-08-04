using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemeticApplication.MemeticLibrary.Model
{
    public class Route : ICloneable
    {
        public VrptwProblem Problem { get; protected set; }

        public List<Customer> Customers { get; protected set; }

        public uint StartTime { get; protected set; }

        public Route(VrptwProblem problem)
        {
            Problem = problem;
            Customers = new List<Customer>();
        }

        public void AddCustomer(Customer customer)
        {
            Customers.Add(customer);
        }

        public void AddCustomer(Customer customer, int index)
        {
            Customers.Insert(index, customer);
        }

        public void RemoveCustomer(Customer customer)
        {
            Customers.Remove(customer);
        }

        public float TotalDistance()
        {
            float totalDistance = 0.0f, distance;
            if (Customers.Count < 2)
                return totalDistance;
            Customer customer1, customer2;
            for (int i = 0; i < Customers.Count - 1; ++i)
            {
                customer1 = Customers[i];
                customer2 = Customers[i + 1];
                distance = Problem.GetDistance(customer1.Id, customer2.Id);
                totalDistance += distance;
            }
            return totalDistance;
        }

        public float TotalTime()
        {
            float totalTime = 0.0f, distance;
            if (Customers.Count < 2)
                return totalTime;
            Customer customer1, customer2;
            for (int i = 0; i < Customers.Count - 1; ++i)
            {
                customer1 = Customers[i];
                customer2 = Customers[i + 1];
                distance = Problem.GetDistance(customer1.Id, customer2.Id);
                totalTime += distance;
                if (totalTime < customer2.ReadyTime)
                    totalTime += customer2.ReadyTime - totalTime;
                totalTime += customer2.ServiceTime;
            }
            return totalTime;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// A copy of route gets a reference to the same problem
        /// and a new customer list instance with the same customer references.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            Route copy = new Route(Problem);
            foreach (Customer customer in Customers)
                copy.AddCustomer(customer);
            return copy;
        }

        /// <summary>
        /// Determines whether this route is feasible.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this route is feasible; otherwise, <c>false</c>.
        /// </returns>
        public bool IsFeasible()
        {
            bool isFeasible = true;
            Customer customer1, customer2;
            float distance, totalTime = 0.0f;
            for (int i = 0; i < Customers.Count - 1; ++i)
            {
                customer1 = Customers[i];
                customer2 = Customers[i + 1];
                distance = Problem.GetDistance(customer1.Id, customer2.Id);
                totalTime += distance;
                if (totalTime <= customer2.ReadyTime)
                {
                    totalTime += customer2.ReadyTime - totalTime;
                    totalTime += customer2.ServiceTime;
                }
                else if (totalTime <= customer2.DueDate)
                {
                    totalTime += customer2.ServiceTime;
                }
                else
                {
                    isFeasible = false;
                    break;
                }
            }
            return isFeasible;
        }

        /// <summary>
        /// Determines whether this route is empty, that means, if contains
        /// customers other than depot.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </returns>
        public bool IsEmpty()
        {
            if (Customers.Count < 2)
                return true;
            else
                foreach (var customer in Customers)
                    if (customer != Problem.Depot)
                        return false;
            return true;
        }

        public override string ToString()
        {
            return "Customers: " + Customers.Count + ", distance: " + TotalDistance(); 
        }
    }
}
