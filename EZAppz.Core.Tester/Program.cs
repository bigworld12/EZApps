using System;
using System.Collections.Generic;
using System.Text;

namespace EZAppz.Core.Tester
{
    class Program
    {
        static void Main(string[] args)
        {
          
            Console.ReadKey();
        }

        class A
        {
            public B MyProperty { get; set; }
        }
        class B
        {
            public C MyProperty { get; set; }
        }
        class C
        {
            public int MyProperty { get; set; }
        }
    }
}
