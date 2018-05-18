using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EZAppz.Core
{
    public class ResettableValueCollection<T> : NotifyValueCollection<T>, IResettable
    {
        public ResettableValueCollection()
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
        public IReadOnlyDictionary<string, object> GetOldValues()
        {
            return OldValues;
        }
        Dictionary<string, object> OldValues = new Dictionary<string, object>();
        public HashSet<IResettable> ResetExeclusions { get; } = new HashSet<IResettable>();
        public HashSet<string> PropertyResetExeclusions { get; } = new HashSet<string>();

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
            foreach (var item in InternalDictionary)
            {
                if (!PropertyResetExeclusions.Contains(item.Key) && item.Value.Value is IResettable ro && !ResetExeclusions.Contains(ro))
                {
                    ro.Reset();
                }
            }
            foreach (var item in Items)
            {
                if (item is IResettable r && !ResetExeclusions.Contains(r))
                {
                    r.Reset();
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

            foreach (var item in InternalDictionary)
            {
                if (!PropertyResetExeclusions.Contains(item.Key) && item.Value.Value is IResettable ro && !ResetExeclusions.Contains(ro))
                {
                    ro.SaveCurrentState();
                }
            }
            foreach (var item in Items)
            {
                if (item is IResettable r && !ResetExeclusions.Contains(r))
                {
                    r.SaveCurrentState();
                }
            }
            IsSaveCurrentState = false;
        }
    }
}
