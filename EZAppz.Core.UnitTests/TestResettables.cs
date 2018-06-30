using System;
using EZAppz.Core.UnitTests.TestResettableModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EZAppz.Core.UnitTests
{
    //testing Resettable objects means basically testing the entire library, so the other classes should be ok too
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
            s.ContactInfos.Add(new ContactInfo(s) { ContactInfoType = ContactInfoTypes.PhoneNumber, Value = "0123456789", Description = "Personal Phone" });
            s.ContactInfos.Add(new ContactInfo(s) { ContactInfoType = ContactInfoTypes.PhoneNumber, Value = "0987654321", Description = "Another phone" });
            s.ContactInfos.Add(new ContactInfo(s) { ContactInfoType = ContactInfoTypes.Email, Value = "ahmednfwela@gmail.com" });
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
            for (int i = 0; i < s.ContactInfos.Count; i++)
            {
                Assert.AreEqual(s.ContactInfos[i].Value, s.GetPropertyValue($"ContactInfos[{i}].Value"));
                Assert.AreEqual(s.ContactInfos[i].ContactInfoType, s.GetPropertyValue($"ContactInfos[{i}].ContactInfoType"));
                Assert.AreEqual(s.ContactInfos[i].Description, s.GetPropertyValue($"ContactInfos[{i}].Description"));
                Assert.AreEqual(s.ContactInfos[i].OwnerPerson, s.GetPropertyValue($"ContactInfos[{i}].OwnerPerson"));                
            }
        }
        [TestMethod]
        public void TestReset()
        {
            var s = GenerateStudent();

            //saving the Student object will save all its properties + all the child objects that implement IResettable interface
            //so the ContactInfos list will also get saved
            s.SaveCurrentState();
            
            Assert.AreEqual(0, s.GetOldValues().Count);
            Assert.AreEqual(0, s.ContactInfos.GetOldValues().Count);

            var refD = DateTime.Now;
            s.DoB = refD;
            Assert.AreEqual(refD, s.DoB);

            Assert.AreEqual(1, s.GetOldValues().Count); //the old value is the DoB's old value
            Assert.AreEqual(true, s.GetOldValues().ContainsKey(nameof(Student.DoB)));




            s.ContactInfos.RemoveAt(0); //this will remove the first item (out of 3 items) only, but will change all the indices of the remaining items
            //now we have 2 contact infos
            s.ContactInfos.RemoveAt(1); //this will remove the second item (out of 2 items) only, and will NOT change any index other than itself
            //now we have 1 contact info

            
            
            Assert.AreEqual(1, s.ContactInfos.Count);
            Assert.AreEqual(4, s.ContactInfos.GetOldValues().Count); 
            //we removed two items, but one of them resulted in changing 3 indices, so the 3 old indices are stored
            //the extra entry in GetOldValues, represents the old 'ContactInfos.Count' Property which was 3 and now it's 1

            //resetting the Student object will reset all its properties to the latest state + all the child objects that implement IResettable interface
            //so the ContactInfos list will also get reset
            s.Reset();



            Assert.AreNotEqual(refD, s.DoB); //now DoB is back to its old value
            Assert.AreEqual(3, s.ContactInfos.Count); //and the 3 contact infos are back
        }

        [TestMethod]
        public void TestResetExeclusions()
        {
            var s = GenerateStudent();
            s.SaveCurrentState(); //save the state of the object, which will also save the state of the ContactInfos list

            Assert.AreEqual(0, s.GetOldValues().Count);
            Assert.AreEqual(0, s.ContactInfos.GetOldValues().Count);

            s.DoB = DateTime.Now;

            var refD = DateTime.Now;
            s.DoB = refD;
            Assert.AreEqual(refD, s.DoB);

            Assert.AreEqual(1, s.GetOldValues().Count);
            Assert.AreEqual(true, s.GetOldValues().ContainsKey(nameof(Student.DoB)));


            s.ContactInfos.RemoveAt(0);            
            s.ContactInfos.RemoveAt(1);

            Assert.AreEqual(1, s.ContactInfos.Count);
            Assert.AreEqual(4, s.ContactInfos.GetOldValues().Count);

            s.ContactInfos.Reset(); //Here we reset only the contact info list, and the student object will remain untouched

            Assert.AreEqual(refD, s.DoB); //DoB should still remain unchanged
            Assert.AreEqual(3, s.ContactInfos.Count); //the 3 contact infos are back
        }
    }
}
