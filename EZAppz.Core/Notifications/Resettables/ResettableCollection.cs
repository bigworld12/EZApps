using System;
using System.Collections.Generic;
using System.Text;

namespace EZAppz.Core
{
   
    public class ResettableCollection<T> : NotifyCollection<T>, IResettable where T : INotifyBase
    {
        public ResettableCollection()
        {
            PropertyChanging += ResettableValueCollection_PropertyChanging;
        }       
        private void ResettableValueCollection_PropertyChanging(object sender, System.ComponentModel.PropertyChangingEventArgs e)
        {
            if (OldValues.ContainsKey(e.PropertyName) || PropertyResetExeclusions.Contains(e.PropertyName))
            {
                return;
            }            
            OldValues[e.PropertyName] = GetPropertyValue(e.PropertyName);
        }
        protected override bool PrepareIncomingItem(T item)
        {
            if (item == null)
            {
                return false;
            }
            return base.PrepareIncomingItem(item);
        }
        Dictionary<string, object> OldValues { get; } = new Dictionary<string, object>();

        public HashSet<string> PropertyResetExeclusions { get; } = new HashSet<string>();
        public HashSet<IResettable> ResetExeclusions { get; } = new HashSet<IResettable>();

        private bool IsReset = false;
        public void Reset()
        {
            if (IsReset)
            {
                return;
            }
            else
            {
                foreach (var item in OldValues)
                {
                    if (item.Key == nameof(Count))
                    {
                        continue;
                    }
                    SetPropertyValue(item.Value, item.Key);
                }
                OldValues.Clear();
                IsReset = true;
            }
            foreach (var item in Items)
            {
                if (item is IResettable r && !ResetExeclusions.Contains(r))
                {
                    r.Reset();
                }
            }
            foreach (var item in InternalDictionary)
            {
                if (!PropertyResetExeclusions.Contains(item.Key) && item.Value.Value is IResettable ro &&  !ResetExeclusions.Contains(ro))
                {
                    ro.Reset();
                }
            }
            IsReset = false;
        }


        private bool IsSaveCurrentState = false;
        public void SaveCurrentState()
        {
            if (IsSaveCurrentState)
            {
                return;
            }
            else
            {
                OldValues.Clear();
                IsSaveCurrentState = true;
            }
            foreach (var item in Items)
            {
                if (item is IResettable r && !ResetExeclusions.Contains(r))
                {
                    r.SaveCurrentState();
                }
            }
            foreach (var item in InternalDictionary)
            {
                if (!PropertyResetExeclusions.Contains(item.Key) && item.Value.Value is IResettable ro && !ResetExeclusions.Contains(ro))
                {
                    ro.SaveCurrentState();
                }
            }
            IsSaveCurrentState = false;
        }
    }
}
