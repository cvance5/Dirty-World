using MathConcepts;
using MathConcepts.Comparisons;
using NUnit.Framework;

namespace Tests.MathTests
{
    public class IntVector2GridComparerTests
    {
        [Test]
        public void TestComparisons()
        {
            var comparer = new IntVector2GridComparer();

            var smallerXSameY = new IntVector2(-1, 0);
            var largerXSameY = new IntVector2(1, 0);

            Assert.AreEqual(-2, comparer.Compare(smallerXSameY, largerXSameY));
            Assert.AreEqual(2, comparer.Compare(largerXSameY, smallerXSameY));

            var smallerXLargerY = new IntVector2(-1, 1);
            var largerXSmallerY = new IntVector2(1, -1);

            Assert.AreEqual(-2, comparer.Compare(smallerXLargerY, largerXSmallerY));
            Assert.AreEqual(2, comparer.Compare(largerXSmallerY, smallerXLargerY));

            var smallerXSmallerY = new IntVector2(-1, -1);
            var largerXLargerY = new IntVector2(1, 1);

            Assert.AreEqual(-2, comparer.Compare(smallerXSmallerY, largerXLargerY));
            Assert.AreEqual(2, comparer.Compare(largerXLargerY, smallerXSmallerY));

            var sameXSmallerY = new IntVector2(0, -1);
            var sameXLargerY = new IntVector2(0, 1);

            Assert.AreEqual(-2, comparer.Compare(sameXSmallerY, sameXLargerY));
            Assert.AreEqual(2, comparer.Compare(sameXLargerY, sameXSmallerY));

            var sameXsameY = new IntVector2(0, 0);

            Assert.AreEqual(0, comparer.Compare(sameXsameY, sameXsameY));

            var baseline = new IntVector2(0, 0);
            var largerByOne = new IntVector2(1, 0);
            var smallerByOne = new IntVector2(-1, 0);
            var largerByThree = new IntVector2(3, 2);

            Assert.AreEqual(-1, comparer.Compare(baseline, largerByOne));
            Assert.AreEqual(1, comparer.Compare(baseline, smallerByOne));
            Assert.AreEqual(-3, comparer.Compare(baseline, largerByThree));
        }
    }
}
