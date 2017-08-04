using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MemeticApplication.MemeticLibrary.Operators.Crossover;
using MemeticApplication.MemeticLibrary.Model;
using MemeticApplication.MemeticLibrary.Genetic;

namespace MemeticApplication.Tests
{
    [TestClass]
    public class CrossoverTests
    {
        [TestInitialize]
        public void SetUp()
        {
        }

        [TestMethod]
        public void PmxOperatorShouldReturnCorrectChildren()
        {
            IGene[] parent1 = new IGene[]
            {
                new UintGene(1), new UintGene(2), new UintGene(3),
                new UintGene(5), new UintGene(4), new UintGene(6),
                new UintGene(7), new UintGene(8), new UintGene(9)
            };
            IGene[] parent2 = new IGene[]
            {
                new UintGene(4), new UintGene(5), new UintGene(2),
                new UintGene(1), new UintGene(8), new UintGene(7),
                new UintGene(6), new UintGene(9), new UintGene(3)
            };
            IGene[] expectedChild1 = new IGene[]
            {
                new UintGene(8), new UintGene(1), new UintGene(2),
                new UintGene(5), new UintGene(4), new UintGene(6),
                new UintGene(7), new UintGene(9), new UintGene(3)
            };
            IGene[] expectedChild2 = new IGene[]
            {
                new UintGene(5), new UintGene(2), new UintGene(3),
                new UintGene(1), new UintGene(8), new UintGene(7),
                new UintGene(6), new UintGene(4), new UintGene(9)
            };
            int cutPoint1 = 2, cutPoint2 = 6;
            IGene[] child1, child2;
            var pmx = new PartiallyMatchedCrossover();
            pmx.Run(parent1, parent2, out child1, out child2, cutPoint1, cutPoint2);
            bool areFirstChildrenEqual = true, areSecondChildrenEqual = true;
            for (int i = 0; i < expectedChild1.Length; ++i)
                areFirstChildrenEqual &= expectedChild1[i].Equals(child1[i]);
            for (int i = 0; i < expectedChild2.Length; ++i)
                areSecondChildrenEqual &= expectedChild2[i].Equals(child2[i]);
            Assert.AreEqual(true, areFirstChildrenEqual);
            Assert.AreEqual(true, areSecondChildrenEqual);
        }

        [TestMethod]
        public void UoxOperatorShouldReturnCorrectChildren()
        {
            IGene[] parent1 = new IGene[]
            {
                new UintGene(5), new UintGene(1), new UintGene(4),
                new UintGene(6), new UintGene(7), new UintGene(8),
                new UintGene(2), new UintGene(3)
            };
            IGene[] parent2 = new IGene[]
            {
                new UintGene(6), new UintGene(7), new UintGene(5),
                new UintGene(2), new UintGene(8), new UintGene(3),
                new UintGene(4), new UintGene(1)
            };
            IGene[] expectedChild1 = new IGene[]
            {
                new UintGene(5), new UintGene(1), new UintGene(2),
                new UintGene(6), new UintGene(7), new UintGene(8),
                new UintGene(4), new UintGene(3)
            };
            IGene[] expectedChild2 = new IGene[]
            {
                new UintGene(5), new UintGene(7), new UintGene(4),
                new UintGene(2), new UintGene(8), new UintGene(6),
                new UintGene(3), new UintGene(1)
            };
            var uox = new UniformBasedOrderCrossover();
            IGene[] child1, child2;
            int[] ones = new int[] { 1, 3, 4, 7 };
            int[] zeros = new int[] { 0, 2, 5, 6 };
            uox.Run(parent1, parent2, out child1, out child2, ones, zeros);
            bool areFirstChildrenEqual = AreChildrenEqual(expectedChild1, child1);
            bool areSecondChildrenEqual = AreChildrenEqual(expectedChild2, child2);
            Assert.AreEqual(true, areFirstChildrenEqual);
            Assert.AreEqual(true, areSecondChildrenEqual);
        }

        [TestMethod]
        public void CxOperatorShouldReturnCorrectChildren()
        {
            IGene[] parent1 = new IGene[]
            {
                new UintGene(1), new UintGene(2), new UintGene(3),
                new UintGene(4), new UintGene(5), new UintGene(6),
                new UintGene(7), new UintGene(8), new UintGene(9)
            };
            IGene[] parent2 = new IGene[]
            {
                new UintGene(4), new UintGene(1), new UintGene(2),
                new UintGene(8), new UintGene(7), new UintGene(6),
                new UintGene(9), new UintGene(3), new UintGene(5)
            };
            IGene[] expectedChild1 = new IGene[]
            {
                new UintGene(1), new UintGene(2), new UintGene(3),
                new UintGene(4), new UintGene(7), new UintGene(6),
                new UintGene(9), new UintGene(8), new UintGene(5)
            };
            IGene[] expectedChild2 = new IGene[]
            {
                new UintGene(4), new UintGene(1), new UintGene(2),
                new UintGene(8), new UintGene(5), new UintGene(6),
                new UintGene(7), new UintGene(3), new UintGene(9)
            };
            var cx = new CycleCrossover();
            IGene[] child1, child2;
            cx.Run(parent1, parent2, out child1, out child2);
            bool areFirstChildrenEqual = AreChildrenEqual(expectedChild1, child1);
            bool areSecondChildrenEqual = AreChildrenEqual(expectedChild2, child2);
            Assert.AreEqual(true, areFirstChildrenEqual);
            Assert.AreEqual(true, areSecondChildrenEqual);
        }

        private bool AreChildrenEqual(IGene[] expectedChild, IGene[] child)
        {
            bool areEqual = true;
            for (int i = 0; i < expectedChild.Length; ++i)
                areEqual &= expectedChild[i].Equals(child[i]);
            return areEqual;
        }
    }
}
