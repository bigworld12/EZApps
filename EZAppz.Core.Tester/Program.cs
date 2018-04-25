using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using EZAppz.Core;

namespace EZAppz.Core.Tester
{
    class Program
    {
        static ResettableCollection<Student> TestList = new ResettableCollection<Student>();
        static void Main(string[] args)
        {
           
            var Grade1 = new Grade() { GradeName = "First Grade", GradeNumber = 1 };
            var Grade2 = new Grade() { GradeName = "Second Grade", GradeNumber = 2 };

            TestList.Add(new Student() { Name = "Joe 1", DateOfBirth = new DateTime(1998, 3, 9), NationalID = "1234", CurrentGrade = Grade1 });
            TestList.Add(new Student() { Name = "Joe 2", DateOfBirth = new DateTime(1990, 5, 3), NationalID = "5678", CurrentGrade = Grade2 });
            TestList.Add(new Student() { Name = "Joe 3", DateOfBirth = new DateTime(1980, 6, 10), NationalID = "9101112", CurrentGrade = Grade1 });
            TestList.SaveCurrentState();
            TestList.Add(new Student() { Name = "Joe 4", DateOfBirth = new DateTime(2000, 1, 20), NationalID = "13141516", CurrentGrade = Grade2 });
            TestList[1].DateOfBirth = new DateTime(1900, 1, 1);
            TestList.Reset();
           

            Console.ReadKey();
        }

    }
    public class Student : ResettableObject
    {
        public Student()
        {
            RegisterProperty<int>(nameof(Age), isReadOnly: true);
            PropertyResetExeclusions.Add(nameof(CurrentGrade));
            RegisterRelationProperty(nameof(DateOfBirth), this, nameof(Age));
        }

        public string Name
        {
            get => RegisterAndGet<string>();
            set => RegisterAndSet(value);
        }
        public string NationalID
        {
            get => RegisterAndGet<string>();
            set => RegisterAndSet(value);
        }
        public DateTime DateOfBirth
        {
            get => RegisterAndGet<DateTime>();
            set => RegisterAndSet(value);
        }

        public Grade CurrentGrade
        {
            get => RegisterAndGet<Grade>();
            set => RegisterAndSet(value);
        }

        public int Age => (int)((DateTime.Now - DateOfBirth).TotalDays / 365d);
        public override string ToString()
        {
            return $"{Name} ({Age})";
        }
    }
    public class Grade : ResettableObject
    {
        public string GradeName
        {
            get => RegisterAndGet<string>();
            set => RegisterAndSet(value);
        }
        public int GradeNumber
        {
            get => RegisterAndGet<int>();
            set => RegisterAndSet(value);
        }
    }
}
