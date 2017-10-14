using System;
using MemeticApplication.MemeticLibrary.Readers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MemeticApplication.MemeticLibrary.Model;

namespace MemeticApplication.Tests
{
    [TestClass]
    public class ReaderTests
    {
        private VrptwProblemReader reader = null;

        private static readonly string FILE_PATH = @"D:\Uczelnia\Sem 10\MGR\Dane\Solomon\solomon_100\C101.txt";

        [TestInitialize]
        public void SetUp()
        {
            reader = new VrptwProblemReader();
        }

        [TestMethod]
        public void CustomerInstanceShouldBeEqualToRowData()
        {
            Customer expectedCustomer = new Customer(1, 45, 68, 10, 912, 967, 90);
            var problem = reader.ReadFromFile(FILE_PATH);
            Customer readCustomer = problem.Customers[0];
            bool areCustomersEqual = expectedCustomer.Equals(readCustomer);
            Assert.AreEqual(true, areCustomersEqual);
        }
    }
}
