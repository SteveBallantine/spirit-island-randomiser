using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SiRandomizer.Data
{
    public class Scenario : SelectableComponentBase<Scenario>, INamedComponent, IExpansionContent, IDifficultyModifier
    {
        public const string NoScenario = "No Scenario";
        public const string Blitz = "Blitz";
        public const string GuardTheIslesHeart = "Guard the Isle's Heart";
        public const string RitualsOfTerror = "Rituals of Terror";
        public const string DahanInsurrection = "Dahan Insurrection";
        public const string SecondWave = "Second Wave";
        public const string PowersLongForgotten = "Powers Long Forgotten";
        public const string WardTheShores = "Ward the Shores";
        public const string RitualsOfTheDestroyingFlame = "Rituals of the Destroying Flame";
        public const string ElementalInvocation = "Elemental Invocation";
        public const string DespicableTheft = "Despicable Theft";
        public const string TheGreatRiver = "The Great River";
        public const string ADiversityOfSpirits = "A Diversity of Spirits";
        public const string VariedTerrains = "Varied Terrains";
        
        [JsonIgnore]
        public Expansion Expansion { get; set; }

        [JsonIgnore]
        public int DifficultyModifier { get; set; }

        public Scenario() {}

        public Scenario(
            string name,
            Expansion expansion,
            int difficultyModifier) 
            : base(name) 
        { 
            Expansion = expansion;
            DifficultyModifier = difficultyModifier;
        }
    }
}
