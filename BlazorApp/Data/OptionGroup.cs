using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SiRandomizer.Data
{
    public class OptionGroup<T> : SelectableComponentBase<T>, IComponentCollection<T>
        where T : SelectableComponentBase<T>
    {
        /// <summary>
        /// The child components that are in this group.
        /// The key is the name of the component.
        /// </summary>
        /// <value></value>
        protected IReadOnlyDictionary<string, T> Children { get; set; }    

        public T this[string name] 
        {
            get 
            {
                return Children[name];
            }
        }

        public OptionGroup(
            string name,
            List<T> children)
            : base(name)
        {
            Name = name;
            Children = children.ToDictionary(c => c.Name, c => c);
            foreach(var child in children)
            {
                child.PropertyChanged += ChildUpdated;
            }
            PropertyChanged += ThisUpdated;
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
                foreach(var child in Children.Values
                    .Where((c) => 
                    {
                        // Make sure we're only selecting children that are
                        // actually available based on the selected expansions.
                        bool available = true;
                        if(c is IExpansionContent item) {
                            available = item.Expansion == null ||
                                item.Expansion.Selected;
                        }
                        return available;
                    })) 
                {
                    child.Selected = Selected;
                }
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Children.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Children.Values.GetEnumerator();
        }
    }
}
