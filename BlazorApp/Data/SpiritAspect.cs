using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace SiRandomizer.Data
{
    public class SpiritAspect : SelectableExpansionComponentBase<SpiritAspect>, IComponentWithParent<Spirit>
    {
        public const string Base = "Base";
        
        // Lightning
        public const string Immense = "Immense";
        public const string Wind = "Wind";
        public const string Pandemonium = "Pandemonium";

        // Earth
        public const string Might = "Might";
        public const string Resilience = "Resilience";

        // River
        public const string Sunshine = "Sunshine";
        public const string Travel = "Travel";

        // Shadows
        public const string Madness = "Madness";
        public const string Amorphous = "Amorphous";
        public const string Reach = "Reach";
        public const string Foreboding = "Foreboding";

        [JsonIgnore]
        public Spirit Parent { get; set; }

        public SpiritAspect() {}

        public SpiritAspect(
            string name,
            OverallConfiguration config,
            Expansion expansion) 
            : base(name, config, expansion)
        { }

        /// <summary>
        /// Return true if the parent spirit only has one aspect available for selection
        /// /// </summary>
        /// <value></value>
        public bool IsSingleAspectSpirit
        {
            get { return Parent.Aspects.Count(a => a.IsBaseAspect || a.IsVisible()) == 1; }
        }

        /// <summary>
        /// Return true if this aspect is a base aspect for the parent spirit.
        /// </summary>
        /// <value></value>
        public bool IsBaseAspect
        {
            get { return Name == Base; }
        }

        public override bool IsVisible()
        {
            // Never show the base aspect if it is the only one available.
            return IsBaseAspect && IsSingleAspectSpirit ? false : 
                Parent.IsVisible() && base.IsVisible();
        }

        public override bool Selected 
        { 
            // Base aspect is always considered selected if the parent is a 
            // single aspect spirit (and the parent is selected).
            get => IsSingleAspectSpirit == true ? Parent.Selected : base.Selected; 
            set => base.Selected = value; 
        }
        
        public override bool Equals(object obj)
        {
            bool result = base.Equals(obj);
            if(result && obj is IComponentWithParent<Spirit> other)
            {
                result = Parent.Equals(other.Parent);
            }
            return result;
        }

        public override string ToString()
        {
            return IsBaseAspect ? Parent.Name : $"{Parent.Name} ({Name})";
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ 
                Parent.GetHashCode();
        }
    }
}
