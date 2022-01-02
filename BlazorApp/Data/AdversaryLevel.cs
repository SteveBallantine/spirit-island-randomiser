using System;

namespace SiRandomizer.Data
{
    public class AdversaryLevel : SelectableComponentBase<AdversaryLevel>, INamedComponent, IDifficultyModifier
    {
        public int Level {get; set;}
        public int DifficultyModifier {get; set;} 
        public Adversary Adversary {get; set;}  

        public AdversaryLevel(string name){
            Name = name;
        }    
    }
}
