using System;
using System.Collections.Generic;
using System.Text;
using EZAppz.Core;

namespace EZAppz.Core.Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            var s = new Student
            {
                Age = 6,
                GradeObj = new Grade() { Number = 8 }
            };
            Console.WriteLine(s.GetPropertyValue("Item[6]"));
            //Console.WriteLine(s.GetPropertyValue<int>($"{nameof(Student.GradeObj)}.{nameof(Grade.Number)}"));
            Console.ReadKey();
        }

        public class Student : DescribableObject
        {
            public Student()
            {
                RegisterProperty<int>(nameof(Age));
                RegisterProperty<Grade>(nameof(GradeObj));

                var para = new MethodParameter[] { new MethodParameter(typeof(double), "origin") };
                RegisterIndexer<double>(new IndexerDescriptor((x, y) => (x as Student)[(double)y[0].Value], null, para));
            }

            public virtual int Age
            {
                get => GetPropertyValue<int>();
                set => this[nameof(Age)] = value;
            }

            public virtual Grade GradeObj
            {
                get => GetPropertyValue<Grade>();
                set => this[nameof(GradeObj)] = value;
            }

            public double this[double origin]
            {
                get
                {
                    return origin * 2;
                }
            }
        }

        public class Grade : DescribableObject
        {
            public virtual int Number
            {
                get => GetPropertyValue<int>();
                set => this[nameof(Number)] = value;
            }
        }
    }
}
