using System;
using System.Collections.Generic;
using System.Text;

namespace EZAppz.Core
{
    public struct IndexerDescriptor : IEquatable<IndexerDescriptor>
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="getter">Function that takes the parameters <list type="number"><item>Source object</item><item>Indexer parameters</item> that returns an object</list></param>
        /// <param name="setter">void that takes the parameters <list type="number"><item>Source object</item><item>Indexer parameters</item><item>new value</item></list></param>
        public IndexerDescriptor(Func<object, MethodParameterValue[], object> getter, Action<object, MethodParameterValue[], object> setter, params MethodParameter[] Parameters)
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
        private Func<object, MethodParameterValue[], object> Getter { get; }
        private Action<object, MethodParameterValue[], object> Setter { get; }
        public void SetValue<TSource>(DescribableObject source, MethodParameterValue[] parameters, object newValue) where TSource : DescribableObject
        {
            Setter?.Invoke(source as TSource, parameters, newValue);
        }
        public TValue GetValue<TSource,TValue>(DescribableObject source, MethodParameterValue[] parameters) where TSource : DescribableObject
        {
            return (TValue)Getter?.Invoke(source as TSource, parameters);
        }

        public bool Equals(IndexerDescriptor other)
        {
            return other.Getter == Getter && other.Setter == Setter;
        }

        public override int GetHashCode()
        {
            return Parameters.GetHashCode();
        }
    }
}
