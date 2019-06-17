using MathConcepts;
using MathConcepts.Geometry;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Tests.SpaceTests
{
    public class SpaceGeometryTest
    {
        [Test]
        public void SegmentIntersectionTests()
        {
            var segmentIntersectsMidpointOne = new Segment(new IntVector2(-5, 0), new IntVector2(5, 0));
            var segmentIntersectsMidpointTwo = new Segment(new IntVector2(0, -5), new IntVector2(0, 5));

            var intersectsMidpointTest = Segment.Intersect(segmentIntersectsMidpointOne, segmentIntersectsMidpointTwo);

            Assert.AreEqual(new IntVector2(0, 0), intersectsMidpointTest);

            var segmentIntersectsVertexOne = new Segment(new IntVector2(-5, 0), new IntVector2(5, 0));
            var segmentIntersectsVertexTwo = new Segment(new IntVector2(-5, -5), new IntVector2(-5, 5));

            var intersectsVertexTest = Segment.Intersect(segmentIntersectsVertexOne, segmentIntersectsVertexTwo);

            Assert.AreEqual(new IntVector2(-5, 0), intersectsVertexTest);

            var areParallelOne = new Segment(new IntVector2(-5, 0), new IntVector2(5, 0));
            var areParallelTwo = new Segment(new IntVector2(-5, 1), new IntVector2(5, 1));

            var areParallelTest = Segment.Intersect(areParallelOne, areParallelTwo);

            Assert.AreEqual(null, areParallelTest);

            var doesNotIntersectOne = new Segment(new IntVector2(-2, -2), new IntVector2(-2, 2));
            var doesNotIntersectTwo = new Segment(new IntVector2(-1, 3), new IntVector2(1, 3));

            var doesNotIntersectTest = Segment.Intersect(doesNotIntersectOne, doesNotIntersectTwo);

            Assert.AreEqual(null, doesNotIntersectTest);
        }

        [Test]
        public void SegmentContainsTest()
        {
            var testSegment = new Segment(new IntVector2(-2, -2), new IntVector2(2, 2));

            Assert.True(testSegment.ContainsX(0), $"Test segment did not reach expected X in center.");
            Assert.True(testSegment.ContainsY(0), $"Test segment did not reach expected Y in center.");

            Assert.True(testSegment.ContainsX(-2), $"Test segment did not reach expected X at edge.");
            Assert.True(testSegment.ContainsY(-2), $"Test segment did not reach expected Y at edge.");

            Assert.False(testSegment.ContainsX(-3), $"Test segment reached X beyond edge.");
            Assert.False(testSegment.ContainsY(-3), $"Test segment reached Y beyond edge.");
        }

        [Test]
        public void SegmentTrimTest()
        {
            var testSegment = new Segment(new IntVector2(-2, -2), new IntVector2(2, 2));
            testSegment.Trim(Directions.Right, 1);

            Assert.AreEqual(-2, testSegment.Start.X, $"Test segment trimmed wrong side X.");
            Assert.AreEqual(1, testSegment.End.X, $"Test segment did not trim X correctly.");
            Assert.AreEqual(-2, testSegment.Start.Y, $"Test segment trimmed wrong side Y.");
            Assert.AreEqual(1, testSegment.End.X, $"Test segment did not trim Y correctly.");

            testSegment = new Segment(new IntVector2(0, 0), new IntVector2(-10, -2));
            testSegment.Trim(Directions.Up, -1);

            Assert.AreEqual(-5, testSegment.Start.X, $"Test segment did not trim X correctly.");
            Assert.AreEqual(-10, testSegment.End.X, $"Test segment trimmed wrong side X.");
            Assert.AreEqual(-1, testSegment.Start.Y, $"Test segment did not trim Y correctly.");
            Assert.AreEqual(-2, testSegment.End.Y, $"Test segment trimmed wrong side Y.");

            Assert.Throws<ArgumentException>(() => testSegment.Trim(Directions.Right + Directions.Up, -4));

            testSegment = new Segment(new IntVector2(0, 0), new IntVector2(10, 0));
            testSegment.Trim(Directions.Left, -1);

            Assert.AreEqual(new IntVector2(-1, 0), testSegment.Start, $"Test segment did not collapse to the right value.");
            Assert.AreEqual(new IntVector2(-1, 0), testSegment.End, $"Test segment did not collapse to a single point.");
        }

        [Test]
        public void InvalidShapesTest()
        {
            Assert.Throws<ArgumentException>(() => new Shape(null));
            Assert.Throws<ArgumentException>(() => new Shape(new List<IntVector2>() { }));
            Assert.Throws<ArgumentException>(() => new Shape(new List<IntVector2>() { new IntVector2(0, 0) }));
        }

        [Test]
        public void ShapeContainsTest()
        {
            var lineContainsOne = new Shape(new List<IntVector2>()
            {
                new IntVector2(-5, 0),
                new IntVector2(5, 0)
            });

            Assert.True(lineContainsOne.Contains(new IntVector2(0, 0)));
            Assert.False(lineContainsOne.Contains(new IntVector2(0, 2)));

            var triangleContainsOne = new Shape(new List<IntVector2>()
            {
                new IntVector2(-3, 0),
                new IntVector2(0, 3),
                new IntVector2(3, 0)
            });

            Assert.True(triangleContainsOne.Contains(new IntVector2(0, 2)));
            Assert.True(triangleContainsOne.Contains(new IntVector2(2, 1)));
            Assert.True(triangleContainsOne.Contains(new IntVector2(0, 0)));
            Assert.True(triangleContainsOne.Contains(new IntVector2(3, 0)));

            Assert.False(triangleContainsOne.Contains(new IntVector2(0, 4)));
            Assert.False(triangleContainsOne.Contains(new IntVector2(2, 2)));
            Assert.False(triangleContainsOne.Contains(new IntVector2(0, -1)));
            Assert.False(triangleContainsOne.Contains(new IntVector2(0, -4)));
        }
    }
}
