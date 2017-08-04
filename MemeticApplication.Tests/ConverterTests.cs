using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MemeticApplication.MemeticLibrary.Readers;
using MemeticApplication.MemeticLibrary.Model;

namespace MemeticApplication.Tests
{
    [TestClass]
    public class ConverterTests
    {
        private static readonly string FILE_PATH = @"D:\Uczelnia\Sem 10\MGR\Dane\Solomon\solomon_100\C101.txt";

        private SolomonProblemReader reader = null;

        private VrptwProblem problem = null;

        [TestInitialize]
        public void Setup()
        {
            reader = new SolomonProblemReader();
        }

        [TestMethod]
        public void TwoConversionShouldReturnTheSameVectorOfCustomers()
        {
            problem = reader.ReadFromFile(FILE_PATH);
            var expectedVectorOfCustomers = problem.Customers;
            Solution solution = new Solution(problem, expectedVectorOfCustomers);
            List<Customer> customers = solution.ToVector();
            bool areVectorsEqual = true, areCustomersEqual;
            for (int i = 0; i < customers.Count; ++i)
            {
                areCustomersEqual = customers[i].Equals(expectedVectorOfCustomers[i]);
                if (!areCustomersEqual)
                {
                    areVectorsEqual = false;
                    break;
                }
            }
            Assert.AreEqual(true, areVectorsEqual);
        }
    }
}
