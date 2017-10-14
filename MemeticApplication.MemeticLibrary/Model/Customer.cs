using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemeticApplication.MemeticLibrary.Model
{
    /// <summary>
    /// The representation of a customer in the routing map
    /// (a node in the graph).
    /// </summary>
    public class Customer
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>
        /// The identifier of a customer, must be unique in every solution instance.
        /// </value>
        public uint Id { get; protected set; }

        /// <summary>
        /// Gets or sets the coordinates.
        /// </summary>
        /// <value>
        /// The coordinates of a customer.
        /// </value>
        public Coordinates Coord { get; set; }

        /// <summary>
        /// Gets or sets the demand.
        /// </summary>
        /// <value>
        /// The demand of a customer.
        /// </value>
        public uint Demand { get; protected set; }

        /// <summary>
        /// Gets or sets the ready time.
        /// </summary>
        /// <value>
        /// The earliest time when a customer can be serviced.
        /// </value>
        public uint ReadyTime { get; protected set; }

        /// <summary>
        /// Gets or sets the due date.
        /// </summary>
        /// <value>
        /// The latest time when a customer can be serviced.
        /// </value>
        public uint DueDate { get; protected set; }

        /// <summary>
        /// Gets or sets the service time.
        /// </summary>
        /// <value>
        /// The minimal time needed to service a customer.
        /// </value>
        public uint ServiceTime { get; protected set; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Customer"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="demand">The demand.</param>
        /// <param name="readyTime">The ready time.</param>
        /// <param name="dueDate">The due date.</param>
        /// <param name="serviceTime">The service time.</param>
        public Customer(uint id, uint x, uint y, uint demand, uint readyTime, uint dueDate, uint serviceTime)
        {
            Id = id;
            Coord = new Coordinates(x, y);
            Demand = demand;
            ReadyTime = readyTime;
            DueDate = dueDate;
            ServiceTime = serviceTime;
        }

        public Customer(Customer other) : this(other.Id, other.Coord.X, other.Coord.Y, other.Demand, other.ReadyTime, other.DueDate, other.ServiceTime)
        {
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            Customer otherCustomer = obj as Customer;
            if (otherCustomer == null)
                return false;
            if (Id != otherCustomer.Id ||
                !Coord.Equals(otherCustomer.Coord) ||
                Demand != otherCustomer.Demand ||
                ReadyTime != otherCustomer.ReadyTime ||
                DueDate != otherCustomer.DueDate ||
                ServiceTime != otherCustomer.ServiceTime)
                return false;
            return true;
        }

        public bool Equals(Customer other)
        {
            return Id == other.Id;
        }
    }
}
