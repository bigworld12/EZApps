using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZAppz.Core.UnitTests.TestModel
{
    public abstract class Person : ResettableObject
    {
        public Person()
        {
            RegisterProperty(nameof(ContactInfos), new ResettableCollection<ContactInfo>(), true);
        }
        public int ID
        {
            get => RegisterAndGet<int>();
            set => RegisterAndSet(value);
        }

        public string Name
        {
            get => RegisterAndGet<string>();
            set => RegisterAndSet(value);
        }
        public DateTime DoB
        {
            get => RegisterAndGet<DateTime>();
            set => RegisterAndSet(value);
        }
        public string NationalID
        {
            get => RegisterAndGet<string>();
            set => RegisterAndSet(value);
        }

        //Properties with no backing field shouldn't be registered ?
        public int Age => (int)((DateTime.Now - DoB).TotalDays / 30d / 12d);

        public ResettableCollection<ContactInfo> ContactInfos
        {
            get => GetPropertyValue<ResettableCollection<ContactInfo>>();
        }
    }
}
