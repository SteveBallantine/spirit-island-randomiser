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
        public const string Sparking = "Sparking";

        // Earth
        public const string Might = "Might";
        public const string Resilience = "Resilience";
        public const string Nourishing = "Nourishing";

        // River
        public const string Sunshine = "Sunshine";
        public const string Travel = "Travel";
        public const string Haven = "Haven";

        // Shadows
        public const string Madness = "Madness";
        public const string Amorphous = "Amorphous";
        public const string Reach = "Reach";
        public const string Foreboding = "Foreboding";
        public const string DarkFire = "Dark Fire";

        // Green
        public const string Regrowth = "Regrowth";
        public const string Tangles = "Tangles";

        // Thunderspeaker
        public const string Tactician = "Tactician";
        public const string Warrior = "Warrior";

        // Ocean
        public const string Deeps = "Deeps";

        // Bringer
        public const string Enticing = "Enticing";
        public const string Violence = "Violence";

        // Serpent
        public const string Locus = "Locus";

        // Wildfire
        public const string Transforming = "Transforming";

        // Fangs
        public const string Encircle = "Encircle";
        public const string Unconstrained = "Unconstrained";

        // Keeper
        public const string Hostility = "Spreading Hostility";

        // Memory
        public const string Intensify = "Intensify";
        public const string Mentor = "Mentor";

        // Lure
        public const string Lair = "Lair";

        // Mist
        public const string Stranded = "Stranded";


        [JsonIgnore]
        public Spirit Parent { get; set; }

        public SpiritAspect() {}

        public SpiritAspect(
            string name,
            OverallConfiguration config,
            IComponentCollection parentList,
            Expansion expansion,
            int complexityChange) 
            : base(name, config, parentList, expansion)
        { 
            ComplexityChange = complexityChange;
        }

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
            bool result = false;
            // Never show the base aspect if it is the only one available.
            if((IsBaseAspect && IsSingleAspectSpirit) == false)
            {
                switch (Config?.Aspects ?? OptionChoice.Allow)
                {
                    case OptionChoice.Allow:
                        // Show aspect if spirit is visible and the base requirements are met
                        // (i.e. Relevant expansion is selected.)
                        result = Parent.IsVisible() && base.IsVisible();
                        break;
                    case OptionChoice.Force:
                        // Same logic as allow, except that base is never shown.
                        result = Parent.IsVisible() && base.IsVisible() && IsBaseAspect == false;
                        break;
                    case OptionChoice.Block:
                        // Aspects are blocked so never show them.
                        result = false;
                        break;
                }
            }

            return result;
        }

        public override bool Selected 
        { 
            // Base aspect is always considered selected if the parent is a 
            // single aspect spirit (and the parent is selected).
            get
            {
                bool result = false;
                if(IsBaseAspect && IsSingleAspectSpirit) 
                {
                    result = Parent.Selected;
                } 
                else 
                {
                    switch (Config?.Aspects ?? OptionChoice.Allow)
                    {
                        case OptionChoice.Allow:
                            // Aspect is only selected if both the spirit and aspect are selected.
                            result = Parent.Selected && base.Selected;
                            break;
                        case OptionChoice.Force:
                            // Same logic as 'allow', except that base aspects are never considered selected.
                            // (Remebmer that if it's a single aspect spirit, the initial 'if' statement will
                            // mean that the base aspect is selected as it's the only one available!)
                            result = Parent.Selected && base.Selected && !IsBaseAspect;
                            break;
                        case OptionChoice.Block:
                            // Aspects are blocked so only the base aspect is ever selected and it must 
                            // always be selected.
                            result = IsBaseAspect;
                            break;
                    }
                }
                return result;
            } 
            set => base.Selected = value; 
        }

        [JsonIgnore]
        public int ComplexityChange { get; set; }
        [JsonIgnore]
        public int TotalComplexity 
        { 
            get 
            {
                var totalComplexity = (int)Parent.BaseComplexity + ComplexityChange;
                // If total complexity is < 1 (which represents low complexity), just return 1.
                // This can happen when using Shadows with the Reach aspect.
                return totalComplexity < 1 ? 1 : totalComplexity;
            }
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
