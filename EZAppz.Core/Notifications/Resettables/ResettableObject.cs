
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace EZAppz.Core
{
    public class ResettableObject : NotifyBase, IResettable
    {
        public ResettableObject(bool ImportFromReflection = false) : base(ImportFromReflection)
        {
            PropertyChanging += ResettableObject_PropertyChanging;
        }
        private void ResettableObject_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            if (OldValues.ContainsKey(e.PropertyName) || PropertyResetExeclusions.Contains(e.PropertyName))
            {
                return;
            }
            //store old value in OldValues
            OldValues[e.PropertyName] = GetPropertyValue(e.PropertyName);
        }
        public IReadOnlyDictionary<string, object> GetOldValues()
        {
            return OldValues;
        }
        Dictionary<string, object> OldValues { get; } = new Dictionary<string, object>();
        public HashSet<IResettable> ResetExeclusions { get; } = new HashSet<IResettable>();
        public HashSet<string> PropertyResetExeclusions { get; } = new HashSet<string>();


        private bool IsSavedState = false;
        /// <summary>
        /// Saves the current changes in the object as the current state
        /// </summary>
        public void SaveCurrentState()
        {
            if (IsSavedState)
            {
                return;
            }
            else
            {
                OldValues.Clear();
                IsSavedState = true;
            }
            foreach (var item in InternalDictionary)
            {
                if (!PropertyResetExeclusions.Contains(item.Key) && item.Value.Value is IResettable ro && !ResetExeclusions.Contains(ro))
                {
                    ro.SaveCurrentState();
                }
            }
            IsSavedState = false;
        }

        private bool IsReset = false;
        /// <summary>
        /// Reverts back all the changes that happened since the lastest save/reset
        /// </summary>
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
            IsReset = false;
        }
    }
}
