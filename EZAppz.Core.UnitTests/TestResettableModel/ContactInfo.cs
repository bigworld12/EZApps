using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZAppz.Core.UnitTests.TestResettableModel
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
        public ContactInfo(Person OwnerPerson)
        {
            //when the contact info is reset, the owner person shouldn't be reset too
            PropertyResetExeclusions.Add(nameof(OwnerPerson));

            this.OwnerPerson = OwnerPerson;
        }
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

        public Person OwnerPerson
        {
            get => RegisterAndGet<Person>();
            private set => RegisterAndSet(value);
        }
    }
}
