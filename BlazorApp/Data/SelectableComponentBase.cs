using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SiRandomizer.Data
{
    // Generic version of this base class.
    // Adds support for automatically getting all values definied against the 
    // overriding class.
    public abstract class SelectableComponentBase<T> : SelectableComponentBase
        where T : SelectableComponentBase<T>
    {
        private static List<T> _all;

        public static List<T> All { get {
            if(_all == null) {
                _all = new List<T>();
                // Get all static fields on the child type that return 
                // an instance of the child type.
                var staticFields = typeof(T)
                    .GetFields(BindingFlags.Public | BindingFlags.Static)
                    .Where(m => m.FieldType == typeof(T));   
                // Add the values from all such fields to the list                 
                _all.AddRange(staticFields.Select(f => f.GetValue(null) as T));
            }
            return _all;
        } }
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

        public string Name { get; protected set; }

        /// <summary>
        /// If set to true, this component will never be displayed in the UI
        /// </summary>
        /// <value></value>
        public bool Hide { get; protected set; }
        
        private bool _selected = false;
        public bool Selected 
        {
            get { return _selected; }
            set
            {
                _selected = value;
                OnPropertyChanged(nameof(Selected));
            }
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
    }
}

