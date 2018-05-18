using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZAppz.Core.UnitTests.TestModel
{
    public enum ContactInfoTypes
    {
        PhoneNumber,
        Email,
        Twitter,
        Youtube
    }
    public class ContactInfo : ResettableObject
    {
        public string Description
        {
            get => RegisterAndGet<string>();
            set => RegisterAndSet(value);
        }
        public ContactInfoTypes ContactInfoType
        {
            get => RegisterAndGet<ContactInfoTypes>();
            set => RegisterAndSet(value);
        }
        public string Value
        {
            get => RegisterAndGet<string>();
            set => RegisterAndSet(value);
        }
    }
}
