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
        public void SegmentTests()
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

        [Test]
        public void ShapeIntersectsTest()
        {
            var squareIntersectsOne = new Shape(new List<IntVector2>()
            {
                new IntVector2(-2, -2),
                new IntVector2(-2, 2),
                new IntVector2(2, 2),
                new IntVector2(2, -2)
            });

            var squareIntersectsTwo = new Shape(new List<IntVector2>()
            {
                new IntVector2(-1, -3),
                new IntVector2(-1, 3),
                new IntVector2(1, 3),
                new IntVector2(1, -3)
            });

            var squaresIntersectTest = Shape.Intersect(squareIntersectsOne, squareIntersectsTwo);

            var squareIntersectsExpected = new Shape(new List<IntVector2>()
            {
                new IntVector2(-2, -2),
                new IntVector2(-2, 2),
                new IntVector2(-1, 2),
                new IntVector2(-1, 3),
                new IntVector2(1, 3),
                new IntVector2(1, 2),
                new IntVector2(2, 2),
                new IntVector2(2, -2),
                new IntVector2(1, -2),
                new IntVector2(1, -3),
                new IntVector2(-1, -3),
                new IntVector2(-1, -2)
            });

            Assert.AreEqual(squareIntersectsExpected, squaresIntersectTest);
        }
    }
}
