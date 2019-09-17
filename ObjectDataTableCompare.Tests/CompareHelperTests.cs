
namespace ObjectDataTableCompare.Tests
{
using System;
    using System.Data;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using static ObjectDataTableCompare.CompareHelper;

    [TestClass]
    public class CompareHelperTests
    {
        [TestMethod]
        public void UpdateResultTest()
        {
            var origenalData = Enumerable.Range(0, 5).Select(index => new TestClass()
            {
                Id = index,
                Value1 = $"{index}",
                Value2 = $"{index * 2}"
            }).ToList();

            var origenalTable = origenalData.ToDataTable();
            Assert.IsNotNull(origenalData);

            Assert.AreEqual(origenalTable.Rows[0]["Value1"].ToString(), "0");
            origenalTable.Rows[0]["Value1"] = "1";
            Assert.AreEqual(origenalTable.Rows[0]["Value1"].ToString(), "1");

            var compareResult = CompareHelper.Compare(origenalData, origenalTable);
            Assert.IsNotNull(compareResult);
            Assert.AreEqual(compareResult.First(c => c.Type == CompareType.Update).Data.Count(), 1);
            Assert.AreEqual(compareResult.First(c => c.Type == CompareType.Update).Data.First().Id, 0);
        }

        [TestMethod]
        public void AddedResultTest()
        {
            var origenalData = Enumerable.Range(0, 5).Select(index => new TestClass()
            {
                Id = index,
                Value1 = $"{index}",
                Value2 = $"{index * 2}"
            }).ToList();

            var origenalTable = origenalData.ToDataTable();
            Assert.IsNotNull(origenalData);

            DataRow dr = origenalTable.NewRow();
            dr["Id"] = 5;
            dr["Value1"] = "5";
            dr["Value2"] = "10";

            origenalTable.Rows.Add(dr);

            var compareResult = CompareHelper.Compare(origenalData, origenalTable);
            Assert.IsNotNull(compareResult);
            Assert.AreEqual(compareResult.First(c => c.Type == CompareType.Insert).Data.Count(), 1);
            Assert.AreEqual(compareResult.First(c => c.Type == CompareType.Insert).Data.First().Id, 5);
        }

        [TestMethod]
        public void DeleteResultTest()
        {
            var origenalData = Enumerable.Range(0, 5).Select(index => new TestClass()
            {
                Id = index,
                Value1 = $"{index}",
                Value2 = $"{index * 2}"
            }).ToList();

            var origenalTable = origenalData.ToDataTable();
            Assert.IsNotNull(origenalData);

            origenalData.Add(new TestClass()
            {
                Id = 5,
                Value1 = "5",
                Value2 = "10"
            });

            var compareResult = CompareHelper.Compare(origenalData, origenalTable);
            Assert.IsNotNull(compareResult);
            Assert.AreEqual(compareResult.First(c => c.Type == CompareType.Delete).Data.Count(), 1);
            Assert.AreEqual(compareResult.First(c => c.Type == CompareType.Delete).Data.First().Id, 5);
        }

        [TestMethod]
        public void MixResultTest()
        {
            var origenalData = Enumerable.Range(0, 5).Select(index => new TestClass()
            {
                Id = index,
                Value1 = $"{index}",
                Value2 = $"{index * 2}"
            }).ToList();

            var origenalTable = origenalData.ToDataTable();
            Assert.IsNotNull(origenalData);

            origenalTable.Rows[0]["Value1"] = "1";

            DataRow dr1 = origenalTable.NewRow();
            dr1["Id"] = 5;
            dr1["Value1"] = "5";
            dr1["Value2"] = "10";
            origenalTable.Rows.Add(dr1);

            DataRow dr2 = origenalTable.NewRow();
            dr2["Id"] = 6;
            dr2["Value1"] = "6";
            dr2["Value2"] = "12";
            origenalTable.Rows.Add(dr2);

            origenalData.AddRange(Enumerable.Range(7, 3).Select(index => new TestClass()
            {
                Id = index,
                Value1 = $"{index}",
                Value2 = $"{index * 2}"
            }));

            var compareResult = CompareHelper.Compare(origenalData, origenalTable);
            Assert.IsNotNull(compareResult);
            Assert.AreEqual(compareResult.First(c => c.Type == CompareType.Update).Data.Count(), 1);
            Assert.AreEqual(compareResult.First(c => c.Type == CompareType.Update).Data.First().Id, 0);

            Assert.AreEqual(compareResult.First(c => c.Type == CompareType.Insert).Data.Count(), 2);
            Assert.AreEqual(compareResult.First(c => c.Type == CompareType.Delete).Data.Count(), 3);
        }
    }

    public class TestClass
    {
        public int Id { get; set; }

        public string Value1 { get; set; }

        public string Value2 { get; set; }
    }
}
