using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MemeticApplication.MemeticLibrary.Model;
using System.Text.RegularExpressions;

namespace MemeticApplication.MemeticLibrary.Readers
{
    /// <summary>
    /// This class reads the classic Solomon VRPTW problem from a file
    /// and creates a new instance of this problem.
    /// </summary>
    public class VrptwProblemReader
    {
        private static readonly byte CUST_NO_COL = 0;
        private static readonly byte XCOORD_COL = 1;
        private static readonly byte YCOORD_COL = 2;
        private static readonly byte DEMAND_COL = 3;
        private static readonly byte READY_TIME_COL = 4;
        private static readonly byte DUE_DATE_COL = 5;
        private static readonly byte SERVICE_TIME_COL = 6;

        /// <summary>
        /// The pattern for decoding values which are all non negative numbers.
        /// </summary>
        private static readonly string ROW_PATTERN = "[0-9]+";


        /// <summary>
        /// Reads the VRPTW problem instance from a text file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>A VRPTW problem instance.</returns>
        public VrptwProblem ReadFromFile(string filePath)
        {
            string name;
            uint vehicleNumber, vehicleCapacity;
            IList<Customer> customers = new List<Customer>();
            Customer depot;
            using (StreamReader sr = new StreamReader(filePath))
            {
                name = sr.ReadLine();
                sr.ReadLine(); sr.ReadLine(); sr.ReadLine();
                string vehicleData = sr.ReadLine();
                ReadVehicleNumberAndCapacity(vehicleData, out vehicleNumber, out vehicleCapacity);
                sr.ReadLine(); sr.ReadLine(); sr.ReadLine(); sr.ReadLine();
                // reading the customers data
                string customerData;
                MatchCollection data;
                Customer customer;
                Regex customerRegex = new Regex(ROW_PATTERN);
                customerData = sr.ReadLine();
                data = customerRegex.Matches(customerData);
                depot = ReadCustomer(data);
                while ((customerData = sr.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(customerData))
                        continue;
                    data = customerRegex.Matches(customerData);
                    customer = ReadCustomer(data);
                    customers.Add(customer);
                }
            }
            VrptwProblem problem = new VrptwProblem(name, vehicleNumber, vehicleCapacity, depot, customers);
            return problem;
        }

        /// <summary>
        /// Reads the vehicle number and capacity.
        /// </summary>
        /// <param name="vehicleData">The vehicle data.</param>
        /// <param name="vehicleNumber">The vehicle number.</param>
        /// <param name="vehicleCapacity">The vehicle capacity.</param>
        private void ReadVehicleNumberAndCapacity(string vehicleData, out uint vehicleNumber, out uint vehicleCapacity)
        {
            Regex regex = new Regex(ROW_PATTERN);
            var matches = regex.Matches(vehicleData);
            string vehicleNumberStr = matches[0].ToString();
            string vehicleCapacityStr = matches[1].ToString();
            vehicleNumber = Convert.ToUInt32(vehicleNumberStr);
            vehicleCapacity = Convert.ToUInt32(vehicleCapacityStr);
        }

        /// <summary>
        /// Reads the single customer from a parsed row.
        /// </summary>
        /// <param name="data">The customer data.</param>
        /// <returns>A customer instance.</returns>
        private Customer ReadCustomer(MatchCollection data)
        {
            uint customerNumber, x, y, demand, readyTime, dueTime, serviceTime;
            customerNumber = Convert.ToUInt32(data[CUST_NO_COL].ToString());
            x = Convert.ToUInt32(data[XCOORD_COL].ToString());
            y = Convert.ToUInt32(data[YCOORD_COL].ToString());
            demand = Convert.ToUInt32(data[DEMAND_COL].ToString());
            readyTime = Convert.ToUInt32(data[READY_TIME_COL].ToString());
            dueTime = Convert.ToUInt32(data[DUE_DATE_COL].ToString());
            serviceTime = Convert.ToUInt32(data[SERVICE_TIME_COL].ToString());
            Customer customer = new Customer(customerNumber, x, y, demand, readyTime, dueTime, serviceTime);
            return customer;
        }

    }
}
