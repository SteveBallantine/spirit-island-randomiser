using System;
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

        public Map() {}

        public Map(
            string name, 
            int minPlayerCount,
            int maxPlayerCount,
            int difficultyModifier,
            bool thematic = false) 
            : base(name) 
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
    }
}