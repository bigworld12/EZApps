using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EZAppz.Core;
using System.Collections;

namespace EZAppz.Core.UnitTests
{
    [TestClass]
    public class ParserTests
    {
        public ICollection MakeCollection(params string[] p)
        {
            return p;
        }

        [TestMethod]
        public void TestGeneric()
        {
            var p = "A.B.C.D";
            CollectionAssert.AreEqual(MakeCollection("A", "B", "C", "D"), Helpers.ParsePropertyParts(p));
        }
        [TestMethod]
        public void TestIndexers()
        {
            var p = "Item[0][3][4,5]";
            CollectionAssert.AreEqual(MakeCollection("Item[0]", "Item[3]", "Item[4,5]"), Helpers.ParsePropertyParts(p));
        }
        [TestMethod]
        public void CompositeTest()
        {
            var p = "Item[0][2].A.B[3]";
            CollectionAssert.AreEqual(MakeCollection("Item[0]", "Item[2]", "A", "B", "Item[3]"), Helpers.ParsePropertyParts(p));
        }
    }
}
