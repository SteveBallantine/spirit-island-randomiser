using System;
using System.Text.Json.Serialization;

namespace SiRandomizer.Data
{
    public class AdversaryLevel : SelectableComponentBase<AdversaryLevel>, INamedComponent, IDifficultyModifier
    {
        [JsonIgnore]
        public int Level {get; private set;}
        [JsonIgnore]
        public int DifficultyModifier {get; private set;} 
        [JsonIgnore]
        public Adversary Adversary {get; set;}  

        public AdversaryLevel() {}

        public AdversaryLevel(
            string name,
            int level,
            int difficultyModifier) : base(name)
        {
            Level = level;
            DifficultyModifier = difficultyModifier;
        }    
    }
}
