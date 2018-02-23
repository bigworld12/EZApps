using System.Collections;
using System.Collections.Generic;

namespace EZAppz.Core
{
    public class DescribableList<T> : DescribableObject, IList<T>
    {
        //a collection represented in a dictionary
        public DescribableList(List<T> initialList = null, bool CloneToNewList = true)
        {
            if (initialList != null)
            {
                if (CloneToNewList)
                {
                    BaseList = new List<T>(initialList);
                }
                else
                {
                    BaseList = initialList;
                }
            }

            RegisterIndexer(new IndexerDescriptor(
                (source, param) =>
                {
                    return (source as DescribableList<T>)[(int)param[0].Value];
                },
                (source, param, value) =>
                {
                    (source as DescribableList<T>)[(int)param[0].Value] = (T)value;
                },
                new MethodParameter(typeof(int), "index")));

            RegisterProperty(nameof(Count), 0, true);
            RegisterProperty(nameof(IsReadOnly), false, true);
        }
        protected List<T> BaseList { get; }
        public virtual T this[int index]
        {
            get => BaseList[index];
            set => BaseList[index] = value;
        }

        public int Count => BaseList.Count;

        public bool IsReadOnly => false;

        public virtual void Add(T item)
        {
            BaseList.Add(item);
        }

        public virtual void Clear()
        {
            BaseList.Clear();
        }

        public virtual bool Contains(T item)
        {
            return BaseList.Contains(item);
        }

        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            BaseList.CopyTo(array, arrayIndex);
        }

        public virtual IEnumerator<T> GetEnumerator()
        {
            return BaseList.GetEnumerator();
        }

        public virtual int IndexOf(T item)
        {
            return BaseList.IndexOf(item);
        }

        public virtual void Insert(int index, T item)
        {
            BaseList.Insert(index, item);
        }

        public virtual bool Remove(T item)
        {
            return BaseList.Remove(item);
        }

        public virtual void RemoveAt(int index)
        {
            BaseList.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return BaseList.GetEnumerator();
        }

        public override string ToString()
        {
            return BaseList.ToString();
        }
    }
}
