using System;
using System.Collections.Generic;
using System.Text;

namespace EZAppz.Core
{
    public struct MethodParameter
    {
        public MethodParameter(Type t,string Name)
        {
            Type = t;
            this.Name = Name;
        }
        public Type Type { get; }
        public string Name { get; }
    }
    public struct MethodParameterValue
    {
        public MethodParameterValue(MethodParameter Parameter,object val)
        {
            this.Parameter = Parameter;
            Value = val;
        }
        public MethodParameter Parameter { get; }
        public object Value { get; }
    }
}
