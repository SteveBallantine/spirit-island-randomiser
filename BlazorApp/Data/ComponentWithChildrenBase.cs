using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace SiRandomizer.Data
{
    public abstract class ComponentWithChildrenBase<TParent, TChild> : SelectableExpansionComponentBase<TParent>, IComponentCollection<TChild>
        where TChild : SelectableComponentBase<TChild>
        where TParent : SelectableComponentBase<TParent>
    {
        /// <summary>
        /// The child components that are in this group.
        /// The key is the name of the component.
        /// </summary>
        /// <value></value>
        private Dictionary<string, TChild> _children = new Dictionary<string, TChild>(); 

        [JsonIgnore]
        public TChild this[string name] 
        {
            get 
            {
                return _children[name];
            }
            set
            {
                Add(value);
            }
        }

        public ComponentWithChildrenBase() : base()
        {
            Init();
        }
        public ComponentWithChildrenBase(
            string name, 
            OverallConfiguration config,
            IComponentCollection parentList,
            Expansion expansion) : 
            base(name, config, parentList, expansion)
        {
            Init();
        }

        private void Init()
        {            
            PropertyChanged += ThisUpdated;
        }

        public void Add(TChild entry)
        {
            _children.Add(entry.Name, entry);
            if(entry is IComponentWithParent<TParent> child) 
            {
                child.Parent = this as TParent;
            }
            entry.PropertyChanged += ChildUpdated;
        }
        
        /// <summary>
        /// Remove the specified item from the list of children if it exists
        /// </summary>
        /// <param name="name"></param>
        public void Remove(string name)
        {
            if(_children.TryGetValue(name, out var entry))
            {
                entry.PropertyChanged -= ChildUpdated;  
                if(entry is IComponentWithParent<TParent> child) 
                {
                    child.Parent = null;
                } 
                _children.Remove(name);
            } 
        }

        public void ChildUpdated (object sender, PropertyChangedEventArgs args) {
            // When child is updated, trigger the property changed
            // event on the group.
            OnPropertyChanged("Child_" + args.PropertyName);
        }

        public void ThisUpdated (object sender, PropertyChangedEventArgs args) {
            // When the selection state of this group is changed, 
            // update all children accordingly.
            if(args.PropertyName == nameof(Selected)) 
            {
                foreach(var child in _children.Values.Where(c => c.IsVisible()))
                {
                    child.Selected = Selected;
                }
            }
        }

        public IEnumerator<TChild> GetEnumerator()
        {
            return _children.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _children.Values.GetEnumerator();
        }

        /// <summary>
        /// Return true if there is a child item with the specified name. False if not.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool HasChild(string name)
        {
            return _children.ContainsKey(name);
        }

    }
}