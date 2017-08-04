using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemeticApplication.MemeticLibrary.Genetic;

namespace MemeticApplication.MemeticLibrary.Model
{
    /// <summary>
    /// Represents a VRPTW problem instance.
    /// </summary>
    public class VrptwProblem : IProblem
    {
        public string Name { get; protected set; }

        public uint VehicleNumber { get; protected set; }

        public uint VehicleCapacity { get; protected set; }

        public Customer Depot { get; protected set; }

        public IList<Customer> Customers { get; protected set; }

        protected float[,] Distances { set; get; }

        public VrptwProblem(string name, uint vehicleNumber, uint vehicleCapacity, Customer depot, IList<Customer> customers)
        {
            Name = name;
            VehicleNumber = vehicleNumber;
            VehicleCapacity = vehicleCapacity;
            Depot = depot;
            Customers = customers;
            CalculateDistances();
        }

        /// <summary>
        /// Calculates the distances between all the customers and a depot.
        /// </summary>
        private void CalculateDistances()
        {
            int size = Customers.Count + 1;
            Distances = new float[size, size];
            uint deltaX, deltaY;
            float distance;
            // distance between a depot and customers
            for (int i = 0; i < Customers.Count; ++i)
            {
                deltaX = Depot.Coord.X - Customers[i].Coord.X;
                deltaY = Depot.Coord.Y - Customers[i].Coord.Y;
                distance = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
                Distances[0, i + 1] = Distances[i + 1, 0] = distance;
            }
            // distance between the customers
            for (int i = 0; i < Customers.Count; ++i)
            {
                for (int j = 0; j < Customers.Count; ++j)
                {
                    deltaX = Customers[i].Coord.X - Customers[j].Coord.X;
                    deltaY = Customers[i].Coord.Y - Customers[j].Coord.Y;
                    distance = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
                    Distances[i + 1, j + 1] = distance;
                }
            }
        }

        /// <summary>
        /// Gets the distance between two customers.
        /// </summary>
        /// <param name="customer1">The first customer.</param>
        /// <param name="customer2">The second customer.</param>
        /// <returns></returns>
        public float GetDistance(uint customer1, uint customer2)
        {
            return Distances[customer1, customer2];
        }

        public int GeneCount()
        {
            return Customers.Count;
        }
    }
}
