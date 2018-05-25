using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace EZAppz.Core
{
    public delegate void ItemPropertyChangingEventHandler<T>(NotifyCollection<T> SourceList, T Item, PropertyChangingEventArgs e) where T : INotifyBase;
    public delegate void ItemPropertyChangedEventHandler<T>(NotifyCollection<T> SourceList, T Item, PropertyChangedEventArgs e) where T : INotifyBase;
    public class NotifyCollection<T> : NotifyValueCollection<T>, IList<T>, INotifyCollectionChanged
        where T : INotifyBase
    {
        public NotifyCollection() : base(false)
        {

        }
        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaiseItemPropertyChanged((T)sender, e);
            if (ItemPropertyRelations.TryGetValue(e.PropertyName, out var rel))
            {
                foreach (var item in rel.RelatedProps)
                {
                    item.RaiseChanged();
                }
                foreach (var item in rel.RelatedChangedActions)
                {
                    item();
                }
            }
        }
        private void Item_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            RaiseItemPropertyChanging((T)sender, e);
            if (ItemPropertyRelations.TryGetValue(e.PropertyName, out var rel))
            {
                foreach (var item in rel.RelatedProps)
                {
                    item.RaiseChanging();
                }
                foreach (var item in rel.RelatedChangingActions)
                {
                    item();
                }
            }
        }

        protected override bool PrepareIncomingItem(T item)
        {
            item.PropertyChanged += Item_PropertyChanged;
            item.PropertyChanging += Item_PropertyChanging;
            //notify all props when a new item is added/removed
            NotifyAllItemProps(false);
            return true;
        }
        public void NotifyAllItemProps(bool isChanging)
        {
            foreach (var rel in ItemPropertyRelations)
            {
                foreach (var item in rel.Value.RelatedProps)
                {
                    if (isChanging) item.RaiseChanging();
                    else item.RaiseChanged();
                }
                foreach (var item in (isChanging ? rel.Value.RelatedChangingActions : rel.Value.RelatedChangedActions))
                {
                    item();
                }
            }
        }
        protected override void PrepareLeavingItem(T item)
        {
            if (item == null)
            {
                return;
            }
            item.PropertyChanged -= Item_PropertyChanged;
            item.PropertyChanging -= Item_PropertyChanging;
            NotifyAllItemProps(false);
        }

        protected override bool IsDoPrepare => true;

        public void RaiseItemPropertyChanging(T item, PropertyChangingEventArgs args)
        {
            ItemPropertyChanging?.Invoke(this, item, args);
        }

        public void RegisterItemRelationProperty(string origin, NotifyBase target_owner, params string[] target_prop)
        {
            if (!ItemPropertyRelations.TryGetValue(origin, out var rel))
            {
                ItemPropertyRelations[origin] = rel = new PropertyRelation();
            }
            foreach (var prop in target_prop)
            {
                var temp = new NotifyDescriptor(target_owner, prop);
                if (rel.RelatedProps.Contains(temp))
                {
                    continue;
                }
                rel.RelatedProps.Add(temp);
            }
        }
        public void RegisterItemActionChanging(string origin, params Action[] OnChanging)
        {
            if (!ItemPropertyRelations.TryGetValue(origin, out var rel))
            {
                ItemPropertyRelations[origin] = rel = new PropertyRelation();
            }
            foreach (var act in OnChanging)
            {
                if (rel.RelatedChangingActions.Contains(act))
                {
                    continue;
                }
                rel.RelatedChangingActions.Add(act);
            }
        }
        public void RegisterItemActionChanged(string origin, params Action[] OnChanged)
        {
            if (!ItemPropertyRelations.TryGetValue(origin, out var rel))
            {
                ItemPropertyRelations[origin] = rel = new PropertyRelation();
            }
            foreach (var act in OnChanged)
            {
                if (rel.RelatedChangedActions.Contains(act))
                {
                    continue;
                }
                rel.RelatedChangedActions.Add(act);
            }
        }


        private Dictionary<string, PropertyRelation> ItemPropertyRelations { get; } = new Dictionary<string, PropertyRelation>();

        public event ItemPropertyChangingEventHandler<T> ItemPropertyChanging;

        public void RaiseItemPropertyChanged(T item, PropertyChangedEventArgs args)
        {
            ItemPropertyChanged?.Invoke(this, item, args);
        }
        public event ItemPropertyChangedEventHandler<T> ItemPropertyChanged;
    }
}
