using System;
using EZAppz.Core.UnitTests.TestModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EZAppz.Core.UnitTests
{
    [TestClass]
    public class TestDescribableObject
    {
        public Student GenerateStudent()
        {
            var s = new Student
            {
                DoB = DateTime.Now.Date.Subtract(TimeSpan.FromDays(365 * 17)),
                Grade = 2,
                ID = 10,
                Name = "some name ?",
                NationalID = "123whocares321"
            };
            s.ContactInfos.Add(new ContactInfo() { ContactInfoType = ContactInfoTypes.PhoneNumber, Value = "0123456789", Description = "Personal Phone" });
            s.ContactInfos.Add(new ContactInfo() { ContactInfoType = ContactInfoTypes.PhoneNumber, Value = "0987654321", Description = "Another phone" });
            s.ContactInfos.Add(new ContactInfo() { ContactInfoType = ContactInfoTypes.Email, Value = "ahmednfwela@gmail.com" });
            return s;
        }
        [TestMethod]
        public void TestCreation()
        {
            var s = GenerateStudent();
            Assert.AreEqual(DateTime.Now.Date.Subtract(TimeSpan.FromDays(365 * 17)), s.DoB);
            Assert.AreEqual(2, s.Grade);
            Assert.AreEqual(10, s.ID);
            Assert.AreEqual("some name ?", s.Name);
            Assert.AreEqual("123whocares321", s.NationalID);
            Assert.AreEqual(17, s.Age);
            Assert.AreEqual(3, s.ContactInfos.Count);

        }
        [TestMethod]
        public void TestReset()
        {
            var s = GenerateStudent();
            s.SaveCurrentState();

            Assert.AreEqual(0, s.GetOldValues().Count);
            Assert.AreEqual(0, s.ContactInfos.GetOldValues().Count);

            var refD = DateTime.Now;
            s.DoB = refD;
            s.ContactInfos.RemoveAt(1);
            s.ContactInfos.RemoveAt(1);


            Assert.AreEqual(refD, s.DoB);
            Assert.AreEqual(1, s.ContactInfos.Count);
            Assert.AreEqual(1, s.GetOldValues().Count);
            //Count/Item[1]/Item[2]
            var oldContacts = s.ContactInfos.GetOldValues();
            Assert.AreEqual(3, oldContacts.Count, "Save doesn't work ?");


            s.Reset();
            Assert.AreNotEqual(refD, s.DoB);
            Assert.AreEqual(3, s.ContactInfos.Count);
        }
    }
}
