using System;
using System.Text.Json.Serialization;

namespace SiRandomizer.Data
{
    public class Spirit : SelectableExpansionComponentBase<Spirit>
    {
        public const string Lightning = "Lightning's Swift Strike";
        public const string River = "River Surges in Sunlight";
        public const string Earth = "Vital Strength of the Earth";
        public const string Shadows = "Shadows Flicker Like Flame";
        public const string Thunderspeaker = "Thunderspeaker";
        public const string Green = "A Spread of Rampant Green";
        public const string Ocean = "Ocean's Hungry Grasp";
        public const string Bringer = "Bringer of Dreams and Nightmares";
        public const string Fangs = "Sharp Fangs Behind the Leaves";
        public const string Keeper = "Keeper of the Forbidden Wilds";
        public const string Wildfire = "Heart of the Wildfire";
        public const string Snek = "Serpent Slumbering Beneath the Island";
        public const string Stone = "Stone's Unyielding Definance";
        public const string Memory = "Shifting Memory of Ages";
        public const string Trickster = "Grinning Trickster Stirs up Trouble";
        public const string Lure = "Lure of the Deep Wilderness";
        public const string ManyMinds = "Many Minds Move as One";
        public const string Volcano = "Volcano Looming High";
        public const string Mist = "Shroud of Silent Mist";
        public const string Vengeance = "Vengeance as a Burning Plague";
        public const string Starlight = "Starlight Seeks its Form";
        public const string Fractured = "Fractured Days Split the Sky";
        public const string Downpour = "Downpour Drenches the World";
        public const string Finder = "Finder of Paths Unseen";
        public const string Rot = "Spreading Rot Renews the Earth";        

        [JsonIgnore]
        public Complexity BaseComplexity {get; set;}

        public Spirit() {}

        public Spirit(
            string name,
            Expansion expansion,
            Complexity baseComplexity) 
            : base(name, expansion) 
        { 
            Expansion = expansion;
            BaseComplexity = baseComplexity;
        }
    }
}
