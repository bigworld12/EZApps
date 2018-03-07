using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace EZAppz.Core
{
    public class ResettableObject : NotifyBase
    {
        public ResettableObject()
        {
            PropertyChanging += ResettableObject_PropertyChanging;
        }
        private void ResettableObject_PropertyChanging(object sender, System.ComponentModel.PropertyChangingEventArgs e)
        {
            //store old value in OldValues
            OldValues[e.PropertyName] = GetPropertyValue(e.PropertyName);
        }


        /// <summary>
        /// first index 
        /// </summary>
        Dictionary<string, (PropertyChangingEventHandler changing, PropertyChangedEventHandler changed)> PropertyListenerDelegateLocation = new Dictionary<string, (PropertyChangingEventHandler changing, PropertyChangedEventHandler changed)>();

        protected override void Before_Set(string property, object NewValue)
        {
            //if value was a notifiable object, listen to it and bubble its events to current object
            var oldValue = GetPropertyValue(property);

            if (oldValue is NotifyBase nbOld)
            {
                var (changing, changed) = PropertyListenerDelegateLocation[property];
                nbOld.PropertyChanging -= changing;
                nbOld.PropertyChanged -= changed;
            }

            if (NewValue is NotifyBase nbNew)
            {
                (PropertyChangingEventHandler changing, PropertyChangedEventHandler changed) propPair = ((s, e) =>
                {
                    RaisePropertyChanging(property + "." + e.PropertyName);
                }
                , (s, e) =>
                {
                    RaisePropertyChanged(property + "." + e.PropertyName);
                });

                PropertyListenerDelegateLocation[property] = propPair;
                nbNew.PropertyChanging += propPair.changing;
                nbNew.PropertyChanged += propPair.changed;
            }

            base.Before_Set(property, NewValue);
        }


        Dictionary<string, object> OldValues = new Dictionary<string, object>();


        /// <summary>
        /// Saves the current changes in the object as the current state
        /// </summary>
        public void SaveCurrentState()
        {
            OldValues.Clear();
        }
        /// <summary>
        /// Reverts back all the changes that happened since the lastest save/reset
        /// </summary>
        public void Reset()
        {
            foreach (var item in OldValues)
            {
                SetPropertyValue(item.Value, item.Key);
            }
            OldValues.Clear();
        }
    }
}
