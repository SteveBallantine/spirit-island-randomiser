using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace SiRandomizer.Data
{
    public class Presets
    {
        private string _current;
        public string Current 
        {
            get { return _current; }
            set 
            {
                if(Available != null &&
                    Available.Contains(value) == false)
                {
                    throw new ArgumentException($"'{value}' is not an available preset");
                }
                OnPropertyChanging();
                _current = value;
                OnPropertyChanged();
            }
        }

        public List<string> Available { get; set; }
        
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanging;
        protected void OnPropertyChanging([CallerMemberName] string propertyName = null)
        {
            PropertyChanging?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}