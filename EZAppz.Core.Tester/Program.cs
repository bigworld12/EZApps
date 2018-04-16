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
        static NotifyCollection<Student> TestList = new NotifyCollection<Student>();
        static void Main(string[] args)
        {
            TestList.CollectionChanged += TestList_CollectionChanged;
            TestList.PropertyChanged += TestList_PropertyChanged;
            TestList.PropertyChanging += TestList_PropertyChanging;
            TestList.ItemPropertyChanging += TestList_ItemPropertyChanging; 
            TestList.ItemPropertyChanged += TestList_ItemPropertyChanged;

            TestList.Add(new Student() { Name = "Ahmed Fwela" });
            TestList.Add(new Student() { Name = "Ahmed Saber", DateOfBirth = new DateTime(1990, 5, 3) });
            TestList.Add(new Student() { Name = "Mohammed Ali", DateOfBirth = new DateTime(1980, 6, 10) });

            TestList[0].DateOfBirth = DateTime.Now;

            Console.ReadKey();
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
            var newItemsText = e.NewItems == null ? "(Null)" : string.Join(';', e.NewItems.OfType<int>());
            var oldItemsText = e.OldItems == null ? "(Null)" : string.Join(';', e.OldItems.OfType<int>());
            Console.WriteLine($"TestList.CollectionChanged Called, Action : {e.Action}, New items : {newItemsText}, NewStartingIndex : {e.NewStartingIndex}, Old items : {oldItemsText}, OldStartingIndex : {e.OldStartingIndex}");
        }
    }
    public class Student : NotifyBase
    {
        public Student()
        {
            RegisterProperty<string>(nameof(Name));
            RegisterProperty(nameof(DateOfBirth), new DateTime(1998, 3, 9));
            RegisterProperty<int>(nameof(Age), isReadOnly: true);
        }
        public string Name
        {
            get { return GetPropertyValue<string>(); }
            set { SetPropertyValue(value); }
        }

        public DateTime DateOfBirth
        {
            get { return GetPropertyValue<DateTime>(); }
            set { SetPropertyValue(value); }
        }

        public int Age => (int)((DateTime.Now - DateOfBirth).TotalDays / 365d);

        public override string ToString()
        {
            return $"{Name} ({Age})";
        }
    }
}
