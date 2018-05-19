using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZAppz.Core.UnitTests.TestResettableModel
{
    public class Student : Person
    {
        public Student()
        {

        }
        public int Grade
        {
            get => RegisterAndGet<int>();
            set => RegisterAndSet(value);
        }

    }
}
