using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace EZAppz.Core
{
    public struct NotifyDescriptor : IEquatable<NotifyDescriptor>
    {
        public static NotifyDescriptor[] GetDescriptors(NotifyBase Sender, params string[] props)
        {
            var f = new List<NotifyDescriptor>();
            foreach (var item in props)
            {
                f.Add(new NotifyDescriptor(Sender, item));
            }
            return f.ToArray();
        }
        public NotifyDescriptor(NotifyBase Owner, string Prop)
        {
            this.Owner = Owner;
            PropertyName = Prop;
        }
        public NotifyBase Owner { get; }
        public string PropertyName { get; }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                Owner.PropertyChanged += value;
            }
            remove
            {
                Owner.PropertyChanged -= value;
            }
        }
        public event PropertyChangingEventHandler PropertyChanging
        {
            add
            {
                Owner.PropertyChanging += value;
            }
            remove
            {
                Owner.PropertyChanging -= value;
            }
        }
        public void RaiseChanging()
        {
            Owner?.RaisePropertyChanging(PropertyName);
        }
        public void RaiseChanged()
        {
            Owner?.RaisePropertyChanged(PropertyName);
        }

        public bool Equals(NotifyDescriptor other)
        {
            return other.Owner == Owner && other.PropertyName == PropertyName;
        }
        public override int GetHashCode()
        {
            return PropertyName.GetHashCode() - Owner.GetHashCode();
        }
    }
}
