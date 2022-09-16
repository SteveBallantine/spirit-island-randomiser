using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace SiRandomizer.Data
{
    public class Map : SelectableComponentBase<Map>, INamedComponent, IDifficultyModifier
    {
        public const string Standard = "Standard";
        public const string ThematicTokens = "Thematic (+tokens)";
        public const string ThematicNoTokens = "Thematic (no tokens)";
        public const string Archipelago = "Archipelago";
        public const string Fragment = "Fragment";
        public const string OppositeShores = "Opposite Shores";
        public const string Coastline = "Coastline";
        public const string Sunrise = "Sunrise";
        public const string Leaf = "Leaf";
        public const string Snake = "Snake";
        public const string V = "V";
        public const string Snail = "Snail";
        public const string Peninsula = "Peninsula";
        public const string Star = "Star";
        public const string Flower = "Flower";
        public const string Caldera = "Caldera";

            
        [JsonIgnore]
        public int MinCount { get; private set; }
        [JsonIgnore]
        public int MaxCount { get; private set; }
        [JsonIgnore]
        public int DifficultyModifier { get; private set; }  
        [JsonIgnore]
        public bool Thematic { get; private set; } 

        [JsonIgnore]
        /// <summary>
        /// If true then the <see cref="Weight"> can be modified by the user.
        /// </summary>
        /// <value></value>
        public override bool HasAssignableWeight { get { return true; } }

        public Map() {}

        public Map(
            string name,  
            OverallConfiguration config,
            IComponentCollection parentList,
            int minPlayerCount,
            int maxPlayerCount,
            int difficultyModifier,
            bool thematic = false) 
            : base(name, config, parentList) 
        { 
            MinCount = minPlayerCount;
            MaxCount = maxPlayerCount;
            DifficultyModifier = difficultyModifier;
            Thematic = thematic;
        }     

        /// <summary>
        /// Return true if this map can be used when playing with the specified number of boards.
        /// </summary>
        /// <param name="boardCount"></param>
        /// <returns></returns>
        public bool ValidForBoardCount(int boardCount)
        {
            return boardCount >= MinCount && boardCount <= MaxCount;
        }

        public override bool IsVisible()
        {
            var visible = false;            
            var maxBoards = Config.Players + Config.MaxAdditionalBoards;
            var minBoards = Config.Players + Config.MinAdditionalBoards;
            // This map should be visible if it is valid for any possible
            // number of boards based on the current configuration.
            for(int c = minBoards; c <= maxBoards; c++)
            {
                visible |= ValidForBoardCount(c);
            }
            // If this component is not visible then also ensure it is not selected.
            if(visible == false)
            {
                Selected = false;
            }
            return visible;
        }
    }
}