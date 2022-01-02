using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SiRandomizer.Data
{
    public class OptionGroup : SelectableComponentBase, IComponentCollection<SelectableComponentBase>
    {
        protected List<SelectableComponentBase> Children {get; set;}        

        public OptionGroup(
            string name,
            List<SelectableComponentBase> children)
        {
            Name = name;
            Children = children;
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
                foreach(var child in Children) 
                {
                    child.Selected = Selected;
                }
            }
        }

        public IEnumerator<SelectableComponentBase> GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Children.GetEnumerator();
        }
    }
}
