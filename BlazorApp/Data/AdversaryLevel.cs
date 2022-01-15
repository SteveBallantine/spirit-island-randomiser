using System;

namespace SiRandomizer.Data
{
    public class AdversaryLevel : SelectableComponentBase<AdversaryLevel>, INamedComponent, IDifficultyModifier
    {
        public int Level {get; private set;}
        public int DifficultyModifier {get; private set;} 
        public Adversary Adversary {get; set;}  

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
