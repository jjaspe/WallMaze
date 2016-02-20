using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Canvas_Window_Template.Interfaces;
using Canvas_Window_Template.Basic_Drawing_Functions;
using Canvas_Window_Template.Utilities;

namespace Canvas_Window_Template_Tests
{
    [TestClass]
    public class MathTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            IPoint[] plane =
            {
                new pointObj(0,0,0),
                new pointObj(0,1,0),
                new pointObj(0,1,1),
                new pointObj(0,0,1)
            };
            IPoint[] line =
            {
                new pointObj(-0.5,0.5,0.5),
                new pointObj(0.5,0.5,0.5)
            };

            IPoint intersection=VectorMath.GetIntersection(plane, line);
            Assert.AreEqual(intersection.X, 0);
            Assert.AreEqual(intersection.Y, 0.5);
            Assert.AreEqual(intersection.Z, 0.5);
        }
    }
}
