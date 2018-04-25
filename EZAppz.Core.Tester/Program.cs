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
            //TestList.CollectionChanged += TestList_CollectionChanged;
            //TestList.PropertyChanged += TestList_PropertyChanged;
            //TestList.PropertyChanging += TestList_PropertyChanging;
            //TestList.ItemPropertyChanging += TestList_ItemPropertyChanging;
            //TestList.ItemPropertyChanged += TestList_ItemPropertyChanged;
            //
            var Grade1 = new Grade() { GradeName = "First Grade", GradeNumber = 1 };
            var Grade2 = new Grade() { GradeName = "Second Grade", GradeNumber = 2 };

            TestList.Add(new Student() { Name = "Ahmed Fwela", DateOfBirth = new DateTime(1998, 3, 9), NationalID = "1234", CurrentGrade = Grade1 });
            TestList.Add(new Student() { Name = "Ahmed Saber", DateOfBirth = new DateTime(1990, 5, 3), NationalID = "5678", CurrentGrade = Grade2 });
            TestList.Add(new Student() { Name = "Mohammed Ali", DateOfBirth = new DateTime(1980, 6, 10), NationalID = "9101112", CurrentGrade = Grade1 });
            TestList.SaveCurrentState();
            TestList.Add(new Student() { Name = "Joe", DateOfBirth = new DateTime(2000, 1, 20), NationalID = "13141516", CurrentGrade = Grade2 });
            TestList[1].DateOfBirth = new DateTime(1900, 1, 1);
            TestList.Reset();
            //
            //TestList[0].DateOfBirth = DateTime.Now;

            //var s = new Student()
            //{
            //Name = "Ahmed Fwela",
            //DateOfBirth = new DateTime(1998, 3, 9),
            //NationalID = "123456789"
            //};
            //s.CurrentGrade.GradeName = "Second Grade";
            //s.CurrentGrade.GradeValue = 87.6d;
            //s.PropertyChanged += S_PropertyChanged;
            //s.PropertyChanging += S_PropertyChanging;
            //s.SaveCurrentState();
            //s.NationalID = "987654321";
            //s.CurrentGrade.GradeValue = 36.1d;
            //s.Reset();


            Console.ReadKey();
        }

        private static void S_PropertyChanging(object sender, System.ComponentModel.PropertyChangingEventArgs e)
        {
            Console.WriteLine($"Student Property changing : {e.PropertyName}");
        }

        private static void S_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Console.WriteLine($"Student Property changed : {e.PropertyName}");
        }

        private static void TestList_ItemPropertyChanging(NotifyCollection<Student> SourceList, Student Item, System.ComponentModel.PropertyChangingEventArgs e)
        {
            Console.WriteLine($"TestList.ItemPropertyChanging Called,PropertyName : {e.PropertyName}, Student : {Item.ToString()}");
        }
        private static void TestList_ItemPropertyChanged(NotifyCollection<Student> SourceList, Student Item, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Console.WriteLine($"TestList.ItemPropertyChanged Called,PropertyName : {e.PropertyName}, Student : {Item.ToString()}");
        }
        private static void TestList_PropertyChanging(object sender, System.ComponentModel.PropertyChangingEventArgs e)
        {
            Console.WriteLine($"TestList.PropertyChanging Called, PropertyName : {e.PropertyName}");
        }
        private static void TestList_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Console.WriteLine($"TestList.PropertyChanged Called, PropertyName : {e.PropertyName}");
        }
        private static void TestList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var newItemsText = e.NewItems == null ? "(Null)" : string.Join(';', e.NewItems.Cast<Student>());
            var oldItemsText = e.OldItems == null ? "(Null)" : string.Join(';', e.OldItems.Cast<Student>());
            Console.WriteLine($"TestList.CollectionChanged Called, Action : {e.Action}, New items : {newItemsText}, NewStartingIndex : {e.NewStartingIndex}, Old items : {oldItemsText}, OldStartingIndex : {e.OldStartingIndex}");
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
