using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace EZAppz.Core
{

    public abstract class NotifyBase : DescribableObject, INotifyBase
    {

        internal protected class PropertyRelation
        {
            public HashSet<NotifyDescriptor> RelatedProps { get; } = new HashSet<NotifyDescriptor>();
            public HashSet<Action> RelatedChangedActions { get; } = new HashSet<Action>();
            public HashSet<Action> RelatedChangingActions { get; } = new HashSet<Action>();
        }
        private Dictionary<string, PropertyRelation> PropertyRelations { get; } = new Dictionary<string, PropertyRelation>();
        public NotifyBase(bool ImportFromReflection = false) : base(ImportFromReflection)
        {
            PropertyChanged += NotifyBase_PropertyChanged;
            PropertyChanging += NotifyBase_PropertyChanging;
        }

        protected override void Before_Set(string property, object NewValue)
        {
            RaisePropertyChanging(property);
        }
        protected override void After_Set(string property, object NewValue)
        {
            RaisePropertyChanged(property);
        }

        public void RegisterRelationProperty(string origin, NotifyBase target_owner, params string[] target_prop)
        {
            if (!PropertyRelations.TryGetValue(origin, out var rel))
            {
                PropertyRelations[origin] = rel = new PropertyRelation();
            }
            foreach (var prop in target_prop)
            {
                var desc = new NotifyDescriptor(target_owner, prop);
                if (!rel.RelatedProps.Contains(desc))
                {
                    rel.RelatedProps.Add(desc);
                }
            }
        }

        public void RegisterChangingAction(string origin, NotifyBase target_owner, params Action[] target_action)
        {
            if (!PropertyRelations.TryGetValue(origin, out var rel))
            {
                PropertyRelations[origin] = rel = new PropertyRelation();
            }
            foreach (var action in target_action)
            {
                if (!rel.RelatedChangingActions.Contains(action))
                {
                    rel.RelatedChangingActions.Add(action);
                }
            }
        }
        public void RegisterChangedAction(string origin, NotifyBase target_owner, params Action[] target_action)
        {
            if (!PropertyRelations.TryGetValue(origin, out var rel))
            {
                PropertyRelations[origin] = rel = new PropertyRelation();
            }
            foreach (var action in target_action)
            {
                if (!rel.RelatedChangedActions.Contains(action))
                {
                    rel.RelatedChangedActions.Add(action);
                }
            }
        }
        public void RegisterChangingAndChangedAction(string origin, NotifyBase target_owner, params Action[] target_action)
        {
            if (!PropertyRelations.TryGetValue(origin, out var rel))
            {
                PropertyRelations[origin] = rel = new PropertyRelation();
            }
            foreach (var action in target_action)
            {
                if (!rel.RelatedChangingActions.Contains(action))
                {
                    rel.RelatedChangingActions.Add(action);
                }
                if (!rel.RelatedChangedActions.Contains(action))
                {
                    rel.RelatedChangedActions.Add(action);
                }
            }
        }

        private void NotifyBase_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            if (PropertyRelations.TryGetValue(e.PropertyName, out var rel))
            {
                //notify all relations
                //execute all changing actions
                foreach (var item in rel.RelatedProps)
                {
                    item.RaiseChanging();
                }
                foreach (var item in rel.RelatedChangingActions)
                {
                    item?.Invoke();
                }
            }
        }
        private void NotifyBase_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (PropertyRelations.TryGetValue(e.PropertyName, out var rel))
            {
                //notify all relations
                //execute all changed actions
                foreach (var item in rel.RelatedProps)
                {
                    item.RaiseChanged();
                }
                foreach (var item in rel.RelatedChangedActions)
                {
                    item?.Invoke();
                }
            }
        }

        public void RaisePropertyChanged([CallerMemberName] string prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        public void RaisePropertyChanging([CallerMemberName] string prop = null)
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(prop));
        }


        [field: NonSerialized]
        public event PropertyChangingEventHandler PropertyChanging;
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
