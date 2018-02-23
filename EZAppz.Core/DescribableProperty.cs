using System;
using System.Collections.Generic;
using System.Text;

namespace EZAppz.Core
{
    public class DescribableProperty
    {
        public DescribableProperty(string Name, bool readOnly, object defaultValue = null)
        {
            this.Name = Name;
            Value = defaultValue;
            DefaultValue = defaultValue;
        }
        public virtual bool IsReadOnly { get; }
  
        /// <summary>
        /// the property name
        /// </summary>
        public virtual string Name { get;  }
     
        /// <summary>
        /// the default value to reverse the property back to
        /// </summary>
        public virtual object DefaultValue { get; protected set; }

        public virtual object Value { get; set; }

        public void ResetToDefault()
        {
            Value = DefaultValue;
        }
    }
}
