using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
                GradeObj = new Grade() { SomeBiskyBoii = 30 }
            };
            s.SetPropertyValue(20M, "GradeObj.Item[2]");
            Console.WriteLine(s.GetPropertyValue<decimal>("GradeObj.Item[2]"));
            Console.ReadKey();
        }

        public class Student : DescribableObject
        {
            public Student()
            {
                RegisterProperty<int>(nameof(Age));
                RegisterProperty<Grade>(nameof(GradeObj));
            }

            public virtual int Age
            {
                get => GetPropertyValue<int>();
                set => SetPropertyValue(value);
            }

            public virtual Grade GradeObj
            {
                get => GetPropertyValue<Grade>();
                set => SetPropertyValue(value);
            }


        }

        public class Grade : DescribableObject
        {
            public Grade()
            {
                var para = new MethodParameter[] { new MethodParameter(typeof(int), "index") };
                RegisterIndexer(new IndexerDescriptor(
                    (x, y) => (x as Grade)[(int)y[0].Value],
                    (x, y, v) =>
                    {
                        var sourceObj = x as Grade;
                        sourceObj[(int)y[0].Value] = (decimal)v;
                    }, para));
            }


            public decimal SomeBiskyBoii
            {
                get => GetPropertyValue<decimal>();
                set => SetPropertyValue(value);
            }

            private decimal[] lastVal = new decimal[10];
            public decimal this[int index]
            {
                get { return lastVal[index]; }
                set { lastVal[index] = value; }
            }
        }
    }
}
