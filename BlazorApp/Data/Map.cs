using System;

namespace SiRandomizer.Data
{
    public class Map : SelectableComponentBase<Map>, INamedComponent, IDifficultyModifier
    {
        public static Map Standard = new Map() {
            Name = "Standard",
            MinCount = 1,
            MaxCount = 6,
            DifficultyModifier = 0
        };
        public static Map ThematicTokens = new Map() {
            Name = "Thematic (+Tokens)",
            MinCount = 1,
            MaxCount = 6,
            DifficultyModifier = 1
        };
        public static Map Thematic = new Map() {
            Name = "Thematic",
            MinCount = 1,
            MaxCount = 6,
            DifficultyModifier = 3
        };

        public int MinCount {get; set;}
        public int MaxCount {get; set;}
        public int DifficultyModifier {get; set;}   

        private Map(){}     
    }
}