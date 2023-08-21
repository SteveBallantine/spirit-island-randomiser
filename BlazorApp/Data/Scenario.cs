using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SiRandomizer.Data
{
    public class Scenario : SelectableExpansionComponentBase<Scenario>, IDifficultyModifier
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
        public const string SurgesOfColonization = "Surges of Colonization";
        public const string SurgesOfColonizationLarger = "Surges of Colonization (Larger)";
        public const string DestinyUnfolds = "Destiny Unfolds";
        
        [JsonIgnore]
        public int DifficultyModifier { get; set; }

        /// <summary>
        /// A list of the maps that can be used with this scenario.
        /// If this is null then the scneario can be used with any map.
        /// </summary>
        /// <value></value>
        [JsonIgnore]
        public List<Map> ValidMaps { get; set; }

        [JsonIgnore]
        /// <summary>
        /// If true then the <see cref="Weight"> can be modified by the user.
        /// </summary>
        /// <value></value>
        public override bool HasAssignableWeight { get { return true; } }

        public Scenario() {}

        public Scenario(
            string name, 
            OverallConfiguration config,
            IComponentCollection parentList,
            Expansion expansion,
            int difficultyModifier,
            List<Map> validMaps = null) 
            : base(name, config, parentList, expansion) 
        {
            DifficultyModifier = difficultyModifier;
            ValidMaps = validMaps;
        }
    }
}
