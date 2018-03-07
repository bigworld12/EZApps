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
        static TestEvent obj = new TestEvent();
        static void Main(string[] args)
        {
            obj.SomeEvent += (s) => {
                Console.WriteLine(s);
            };
            obj.RaiseSomeEvent("ayy");

            
            obj.SomeEvent -= (GoodEventHandler)obj.SomeEvent.GetInvocationList()[0];
            obj.RaiseSomeEvent("ayy");

            Console.ReadKey();
        }

    }
    public delegate void GoodEventHandler(string s);
    public class TestEvent
    {
        public void RaiseSomeEvent(string s)
        {
            SomeEvent?.Invoke(s);            
        }
        public GoodEventHandler SomeEvent;
    }
}
