using System;
using System.Collections.Generic;
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
            if (OldValues.ContainsKey(e.PropertyName))
            {
                return;
            }            
            OldValues[e.PropertyName] = GetPropertyValue(e.PropertyName);
        }
        
        Dictionary<string, object> OldValues = new Dictionary<string, object>();
        public void Reset()
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
        }

        public void SaveCurrentState()
        {
            OldValues.Clear();
        }
    }
}
