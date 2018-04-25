using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;

namespace EZAppz.Core
{
    public delegate void ItemPropertyChangingEventHandler<T>(NotifyCollection<T> SourceList, T Item, PropertyChangingEventArgs e) where T : INotifyBase;
    public delegate void ItemPropertyChangedEventHandler<T>(NotifyCollection<T> SourceList, T Item, PropertyChangedEventArgs e) where T : INotifyBase;
    public class NotifyCollection<T> : NotifyValueCollection<T>, IList<T>, INotifyCollectionChanged
        where T : INotifyBase
    {
        public NotifyCollection() : base(false)
        {

        }
        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaiseItemPropertyChanged((T)sender, e);
        }
        private void Item_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            RaiseItemPropertyChanging((T)sender, e);
        }

        protected override bool PrepareIncomingItem(T item)
        {
            if (item == null)
            {
                return false;
            }
            item.PropertyChanged += Item_PropertyChanged;
            item.PropertyChanging += Item_PropertyChanging;
            return true;
        }
        protected override void PrepareLeavingItem(T item)
        {
            item.PropertyChanged -= Item_PropertyChanged;
            item.PropertyChanging -= Item_PropertyChanging;
        }

        protected override bool IsDoPrepare => true;

        public void RaiseItemPropertyChanging(T item, PropertyChangingEventArgs args)
        {
            ItemPropertyChanging?.Invoke(this, item, args);
        }
        public event ItemPropertyChangingEventHandler<T> ItemPropertyChanging;

        public void RaiseItemPropertyChanged(T item, PropertyChangedEventArgs args)
        {
            ItemPropertyChanged?.Invoke(this, item, args);
        }
        public event ItemPropertyChangedEventHandler<T> ItemPropertyChanged;
    }
}
