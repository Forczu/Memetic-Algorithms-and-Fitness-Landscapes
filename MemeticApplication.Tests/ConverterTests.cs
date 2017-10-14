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

        private VrptwProblemReader reader = null;

        private VrptwProblem problem = null;

        [TestInitialize]
        public void Setup()
        {
            reader = new VrptwProblemReader();
        }
    }
}
