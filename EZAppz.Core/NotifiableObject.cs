using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace EZAppz.Core
{
    public class NotifiableObject : DescribableObject,INotifyPropertyChanged,INotifyPropertyChanging
    {
        public NotifiableObject()
        {

        }

        /*
        copy stuff from NotifyBase
        */

        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
