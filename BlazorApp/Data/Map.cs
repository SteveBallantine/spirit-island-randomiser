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
        public static Map Archipelago = new Map() {
            Name = "Archipelago",
            MinCount = 2,
            MaxCount = 6,
            DifficultyModifier = 1
        };
        public static Map Fragment = new Map() {
            Name = "Fragment",
            MinCount = 2,
            MaxCount = 2,
            DifficultyModifier = 0
        };
        public static Map OppositeShores = new Map() {
            Name = "Opposite Shores",
            MinCount = 2,
            MaxCount = 2,
            DifficultyModifier = 0
        };
        public static Map Coastline = new Map() {
            Name = "Coastline",
            MinCount = 2,
            MaxCount = 6,
            DifficultyModifier = 0
        };
        public static Map Sunrise = new Map() {
            Name = "Sunrise",
            MinCount = 3,
            MaxCount = 3,
            DifficultyModifier = 0
        };
        public static Map Leaf = new Map() {
            Name = "Leaf",
            MinCount = 4,
            MaxCount = 4,
            DifficultyModifier = 0
        };
        public static Map Snake = new Map() {
            Name = "Snake",
            MinCount = 3,
            MaxCount = 6,
            DifficultyModifier = 0
        };
        public static Map V = new Map() {
            Name = "V",
            MinCount = 5,
            MaxCount = 5,
            DifficultyModifier = 0
        };
        public static Map Snail = new Map() {
            Name = "Snail",
            MinCount = 5,
            MaxCount = 5,
            DifficultyModifier = 0
        };
        public static Map Peninsula = new Map() {
            Name = "Peninsula",
            MinCount = 5,
            MaxCount = 5,
            DifficultyModifier = 0
        };
        public static Map Star = new Map() {
            Name = "Star",
            MinCount = 6,
            MaxCount = 6,
            DifficultyModifier = 0
        };
        public static Map Flower = new Map() {
            Name = "Flower",
            MinCount = 6,
            MaxCount = 6,
            DifficultyModifier = 0
        };
        public static Map Caldera = new Map() {
            Name = "Caldera",
            MinCount = 6,
            MaxCount = 6,
            DifficultyModifier = 0
        };

        public int MinCount {get; set;}
        public int MaxCount {get; set;}
        public int DifficultyModifier {get; set;}   

        private Map(){}     
    }
}