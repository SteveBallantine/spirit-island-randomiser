using System;
using System.Text.Json.Serialization;

namespace SiRandomizer.Data
{
    public class AdversaryLevel : SelectableComponentBase<AdversaryLevel>, IDifficultyModifier, IComponentWithParent<Adversary>
    {
        [JsonIgnore]
        public int Level { get; private set; }
        [JsonIgnore]
        public int DifficultyModifier { get; private set; } 
        [JsonIgnore]
        public Adversary Parent { get; set; }

        public AdversaryLevel() {}

        public AdversaryLevel(
            string name, 
            OverallConfiguration config,
            IComponentCollection parentList,
            int level,
            int difficultyModifier) 
            : base(name, config, parentList)
        {
            Level = level;
            DifficultyModifier = difficultyModifier;
        }

        public override bool IsVisible()
        {
            // Levels are always visible if the parent adversary is visible.
            return Parent.IsVisible();
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
            return $"{Parent.Name} - {Name}";
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ 
                Parent.GetHashCode();
        }
    }
}
