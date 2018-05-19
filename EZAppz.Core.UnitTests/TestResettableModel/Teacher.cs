using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZAppz.Core.UnitTests.TestResettableModel
{
    public class Teacher : Person
    {
        public int Salary
        {
            get => RegisterAndGet<int>();
            set => RegisterAndSet(value);
        }
        

    }
}
