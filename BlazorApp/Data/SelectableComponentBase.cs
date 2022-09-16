using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace SiRandomizer.Data
{    
    // Generic version of this base class.
    // Adds support for automatically getting all values definied against the 
    // overriding class.
    public abstract class SelectableComponentBase<T> : SelectableComponentBase
        where T : SelectableComponentBase<T>
    {
        private static List<string> _all;

        public SelectableComponentBase() {}

        public SelectableComponentBase(string name, OverallConfiguration config, IComponentCollection parentList) 
            : base(name, config, parentList)
        {
        }

        public static List<string> All 
        { 
            get 
            {
                if(_all == null) 
                {
                    _all = new List<string>();
                    // Get all static fields on the child type that return 
                    // a string.
                    var staticFields = typeof(T)
                        .GetFields(BindingFlags.Public | BindingFlags.Static)
                        .Where(m => m.FieldType == typeof(string));   
                    // Add the values from all such fields to the list                 
                    _all.AddRange(staticFields
                        // Make sure we're only taking constants.
                        .Where(f => f.IsLiteral && !f.IsInitOnly)
                        // Finally, get the string values.
                        .Select(f => f.GetValue(null) as string));
                }
                return _all;
            } 
        }
    }

    public abstract class SelectableComponentBase : 
        INotifyPropertyChanged, INamedComponent
    {
        // Use to inform watchers about this component's selected property being changed
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new 
                PropertyChangedEventArgs(propertyName));
        }

        public string Name { get; set; }

        /// <summary>
        /// Flag that controls whether this item can be deleted or not.
        /// By default, only homebrew items (which are added by the user) can be deleted.
        /// </summary>
        /// <value></value>
        public bool Deletable { get; set; } = false;

        private bool _selected = false;
        public virtual bool Selected 
        {
            get { return _selected; }
            set
            {
                if(_selected != value)
                {
                    _selected = value;
                    OnPropertyChanged(nameof(Selected));
                }
            }
        }

        /// <summary>
        /// The chance that this item will be picked out of all items in the parent's list.
        /// </summary>
        /// <value></value>
        [JsonIgnore]
        public float Weight 
        { 
            get
            {
                if(ParentList == null) { return 0; }
                return AssignedWeight.HasValue ? AssignedWeight.Value : ParentList.CalculateSelectedItemsWeight();
            }
            set 
            {
                AssignedWeight = (int)value;
                OnPropertyChanged(nameof(AssignedWeight));
            }
        }

        /// <summary>
        /// The chance that has been assigned by the user for this item to be selected.
        /// </summary>
        /// <value></value>
        public int? AssignedWeight { get; set; } = null;

        [JsonIgnore]
        /// <summary>
        /// If true then the <see cref="Weight"> can be modified by the user.
        /// </summary>
        /// <value></value>
        public virtual bool HasAssignableWeight { get { return false; } }

        /// <summary>
        /// The collection that contains this item.
        /// </summary>
        /// <value></value>
        [JsonIgnore]
        public IComponentCollection ParentList { get; set; }

        [JsonIgnore]
        public OverallConfiguration Config { get; private set; }

        public SelectableComponentBase() {}

        /// <summary>
        /// Constructor
        /// </summary>  
        /// <param name="name"></param>
        public SelectableComponentBase(
            string name,
            OverallConfiguration config,
            IComponentCollection parentList)
        {
            Name = name;
            Config = config;
            ParentList = parentList;
        }        

        public override bool Equals(object obj)
        {
            bool result = false;
            if(obj is SelectableComponentBase other)
            {
                result = Name == other.Name;
            }
            return result;
        }

        public override string ToString()
        {
            return Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public abstract bool IsVisible();
    }
}

