using System;
using System.Collections.Generic;
using System.Text;

namespace EZAppz.Core
{
    public struct IndexerDescriptor
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="getter">Function that takes the parameters <list type="number"><item>Source object</item><item>Indexer parameters</item> that returns an object</list></param>
        /// <param name="setter">void that takes the parameters <list type="number"><item>Source object</item><item>Indexer parameters</item><item>new value</item></list></param>
        public IndexerDescriptor(Func<object, Dictionary<MethodParameter, object>, object> getter, Action<object, Dictionary<MethodParameter, object>, object> setter, params MethodParameter[] Parameters)
        {
            if (Parameters.Length == 0)
            {
                throw new ArgumentException("indexers must have at least 1 parameter");
            }
            this.Parameters = Parameters;
            Getter = getter;
            Setter = setter;
        }
        public bool IsReadOnly => Setter == null;
        public MethodParameter[] Parameters { get; }
        private Func<object, Dictionary<MethodParameter, object>, object> Getter { get; }
        private Action<object, Dictionary<MethodParameter, object>, object> Setter { get; }
        public void SetValue(object source, Dictionary<MethodParameter, object> parameters, object newValue)
        {
            Setter?.Invoke(source, parameters, newValue);
        }
        public object GetValue(object source, Dictionary<MethodParameter, object> parameters)
        {
            return Getter?.Invoke(source, parameters);
        }
    }
}
