using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace EZAppz.Core
{
    public class NotifyValueCollection<T> : NotifyBase, IList<T>, INotifyCollectionChanged
    {
        public NotifyValueCollection(bool ImportFromReflection = false) : base(ImportFromReflection)
        {
            RegisterIndexer(new IndexerDescriptor(
                (param) =>
                {
                    return this[(int)param[0].Value];
                },
                (param, value) =>
                {
                    this[(int)param[0].Value] = (T)value;
                },
                new MethodParameter(typeof(int), "index")));
            RegisterProperty(nameof(Count), 0, true);
            RegisterProperty(nameof(IsReadOnly), false, true);
        }
        private List<T> Items = new List<T>();
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                {
                    return default(T);
                }
                return Items[index];
            }
            set
            {
                if (index < 0 || index >= Count)
                {
                    return;
                }
                var old = Items[index];
                if (EqualityComparer<T>.Default.Equals(old, value))
                {
                    return;
                }
                if (IsDoPrepare)
                {
                    PrepareLeavingItem(old);
                    PrepareIncomingItem(value);
                }
                Items[index] = value;
                RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, old, index));
            }
        }

        public int Count => Items.Count;
        public bool IsReadOnly => false;
        public bool Contains(T item)
        {
            return Items.Contains(item);
        }
        public void CopyTo(T[] array, int arrayIndex)
        {
            Items.CopyTo(array, arrayIndex);
        }
        public IEnumerator<T> GetEnumerator()
        {
            return Items.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Items.GetEnumerator();
        }
        public int IndexOf(T item)
        {
            return Items.IndexOf(item);
        }


        public void Add(T item)
        {
            RaisePropertyChanging(nameof(Count));
            if (IsDoPrepare)
            {
                PrepareIncomingItem(item);
            }
            RaisePropertyChanging($"Item[{Count}]");
            Items.Add(item);
            RaisePropertyChanged($"Item[{Count - 1}]");
            RaisePropertyChanged(nameof(Count));
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, Count - 1));
        }
        public void Clear()
        {
            RaisePropertyChanging(nameof(Count));
            if (IsDoPrepare)
                foreach (var item in Items)
                    PrepareLeavingItem(item);

            Items.Clear();
            RaisePropertyChanged(nameof(Count));
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        public void Insert(int index, T item)
        {
            //make sure index is always within bounds
            if (index > Items.Count)
            {
                for (int i = 0; i < index - Items.Count; i++)
                {
                    Add(default(T));
                }
            }
            RaisePropertyChanging(nameof(Count));
            if (IsDoPrepare)
                PrepareIncomingItem(item);
            for (int i = index; i <= Count; i++)
            {
                RaisePropertyChanging($"Item[{i}]");
            }
            Items.Insert(index, item);
            for (int i = index; i < Count; i++)
            {
                RaisePropertyChanged($"Item[{i}]");
            }
            RaisePropertyChanged(nameof(Count));
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }
        public bool Remove(T item)
        {
            var index = Items.IndexOf(item);
            if (index < 0 || index >= Count)
            {
                return false;
            }
            RemoveAt(index);
            return true;
        }
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= Count)
            {
                return;
            }
            RaisePropertyChanging(nameof(Count));
            var item = this[index];
            if (IsDoPrepare)
                PrepareLeavingItem(item);
            for (int i = index; i < Count; i++)
            {
                RaisePropertyChanging($"Item[{i}]");
            }
            Items.RemoveAt(index);
            for (int i = index; i <= Count; i++)
            {
                RaisePropertyChanged($"Item[{i}]");
            }
            RaisePropertyChanged(nameof(Count));
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
        }

        public void RaiseCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            CollectionChanged?.Invoke(this, args);
        }
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// whether to call <see cref="PrepareIncomingItem(T)"/> and <see cref="PrepareLeavingItem(T)"/> or not
        /// </summary>
        protected virtual bool IsDoPrepare => false;

        protected virtual void PrepareIncomingItem(T item)
        { }
        protected virtual void PrepareLeavingItem(T item)
        { }
    }
}
