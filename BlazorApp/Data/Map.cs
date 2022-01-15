using System;

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

            
        public int MinCount { get; private set; }
        public int MaxCount { get; private set; }
        public int DifficultyModifier { get; private set; }   

        public Map(
            string name, 
            int minPlayerCount,
            int maxPlayerCount,
            int difficultyModifier) 
            : base(name) 
        { 
            MinCount = minPlayerCount;
            MaxCount = maxPlayerCount;
            DifficultyModifier = difficultyModifier;
        }     
    }
}