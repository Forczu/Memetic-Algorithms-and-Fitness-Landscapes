using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MemeticApplication.MemeticLibrary.Genetic;
using MemeticApplication.MemeticLibrary.Operators.Mutation;
using MemeticApplication.MemeticLibrary.Model;

namespace MemeticApplication.Tests
{
    [TestClass]
    public class MutationTests
    {
        [TestMethod]
        public void SwapOperatorShouldReturnCorrectChildren()
        {
            IGene[] parent = new IGene[]
            {
                new UintGene(1), new UintGene(2), new UintGene(3),
                new UintGene(4), new UintGene(5), new UintGene(6),
                new UintGene(7), new UintGene(8), new UintGene(9)
            };
            IGene[] expectedChild = new IGene[]
            {
                new UintGene(1), new UintGene(2), new UintGene(3),
                new UintGene(8), new UintGene(5), new UintGene(6),
                new UintGene(7), new UintGene(4), new UintGene(9)
            };
            int poiont1 = 3, poiont2 = 7;
            var swap = new SwapOperator();
            swap.Run(parent, poiont1, poiont2);
            bool areChildrenEqual = true;
            for (int i = 0; i < expectedChild.Length; ++i)
                areChildrenEqual &= expectedChild[i].Equals(parent[i]);
            Assert.AreEqual(true, areChildrenEqual);
        }

        [TestMethod]
        public void InversionOperatorShouldReturnCorrectChildren()
        {
            IGene[] parent = new IGene[]
            {
                new UintGene(1), new UintGene(2), new UintGene(3),
                new UintGene(4), new UintGene(5), new UintGene(6),
                new UintGene(7), new UintGene(8), new UintGene(9)
            };
            IGene[] expectedChild = new IGene[]
            {
                new UintGene(1), new UintGene(2), new UintGene(3),
                new UintGene(8), new UintGene(7), new UintGene(6),
                new UintGene(5), new UintGene(4), new UintGene(9)
            };
            int startPoint = 3, length = 5;
            var swap = new InversionOperator();
            swap.Run(parent, startPoint, length);
            bool areChildrenEqual = true;
            for (int i = 0; i < expectedChild.Length; ++i)
                areChildrenEqual &= expectedChild[i].Equals(parent[i]);
            Assert.AreEqual(true, areChildrenEqual);
        }

        [TestMethod]
        public void InsertionOperatorShouldReturnCorrectChildren()
        {
            IGene[] parent = new IGene[]
            {
                new UintGene(1), new UintGene(2), new UintGene(3),
                new UintGene(4), new UintGene(5), new UintGene(6),
                new UintGene(7), new UintGene(8), new UintGene(9)
            };
            IGene[] expectedChild = new IGene[]
            {
                new UintGene(5), new UintGene(1), new UintGene(2),
                new UintGene(3), new UintGene(4), new UintGene(6),
                new UintGene(7), new UintGene(8), new UintGene(9)
            };
            int startPoint = 4, destPoint = 0;
            var swap = new InsertionOperator();
            swap.Run(parent, startPoint, destPoint);
            bool areChildrenEqual = true;
            for (int i = 0; i < expectedChild.Length; ++i)
                areChildrenEqual &= expectedChild[i].Equals(parent[i]);
            Assert.AreEqual(true, areChildrenEqual);
        }

        [TestMethod]
        public void DisplacementOperatorShouldReturnCorrectChildren()
        {
            IGene[] parent = new IGene[]
            {
                new UintGene(1), new UintGene(2), new UintGene(3),
                new UintGene(4), new UintGene(5), new UintGene(6),
                new UintGene(7), new UintGene(8), new UintGene(9)
            };
            IGene[] expectedChild = new IGene[]
            {
                new UintGene(4), new UintGene(5), new UintGene(6),
                new UintGene(1), new UintGene(2), new UintGene(3),
                new UintGene(7), new UintGene(8), new UintGene(9)
            };
            int startPoint = 3, destPoint = 0, length = 3;
            var swap = new DisplacementOperator();
            swap.Run(parent, startPoint, destPoint, length);
            bool areChildrenEqual = true;
            for (int i = 0; i < expectedChild.Length; ++i)
                areChildrenEqual &= expectedChild[i].Equals(parent[i]);
            Assert.AreEqual(true, areChildrenEqual);
        }
    }
}
