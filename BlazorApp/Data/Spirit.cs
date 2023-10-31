using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SiRandomizer.Data
{
    [JsonConverter(typeof(SpiritConverter))]
    public class Spirit : ComponentWithChildrenBase<Spirit, SpiritAspect>
    {
        // Base game
        public const string Lightning = "Lightning's Swift Strike";
        public const string River = "River Surges in Sunlight";
        public const string Earth = "Vital Strength of the Earth";
        public const string Shadows = "Shadows Flicker Like Flame";
        public const string Thunderspeaker = "Thunderspeaker";
        public const string Green = "A Spread of Rampant Green";
        public const string Ocean = "Ocean's Hungry Grasp";
        public const string Bringer = "Bringer of Dreams and Nightmares";

        // B&C
        public const string Fangs = "Sharp Fangs Behind the Leaves";
        public const string Keeper = "Keeper of the Forbidden Wilds";

        // Promo 1
        public const string Wildfire = "Heart of the Wildfire";
        public const string Snek = "Serpent Slumbering Beneath the Island";

        // JE
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

        // Promo 2
        public const string Downpour = "Downpour Drenches the World";
        public const string Finder = "Finder of Paths Unseen";

        // Apoc
        public const string Rot = "Spreading Rot Renews the Earth";
        
        // Horizons
        public const string Teeth = "Devouring Teeth Lurk Underfoot";
        public const string Whirlwind = "Sun-Bright Whirlwind";
        public const string Eyes = "Eyes Watch From the Trees";
        public const string Heat = "Rising Heat of Stone and Sand";
        public const string Swamp = "Fathomless Mud of the Swamp";

        // NI
        public const string Behemoth = "Ember-Eyed Behemoth";
        public const string Roots = "Towering Roots of the Jungle";
        public const string Vigil = "Hearth-Vigil";
        public const string Darkness = "Breath of Darkness Down Your Spine";
        public const string Sun = "Relentless Gaze of the Sun";
        public const string Earthquakes = "Dances Up Earthquakes";
        public const string Voice = "Wandering Voice Keens Delirium";
        public const string WoundedWaters = "Wounded Waters Bleeding";



        [JsonIgnore]
        public Complexity BaseComplexity {get; set;}

        public Spirit() {}

        public Spirit(
            string name,
            OverallConfiguration config,
            IComponentCollection parentList,
            Expansion[] expansions,
            Complexity baseComplexity) 
            : base(name, config, parentList, expansions) 
        { 
            BaseComplexity = baseComplexity;
            // Add a 'base' aspect for all spirits.
            // This allows us to keep the setup generation logic nice and general, rather
            // than having to deal with Aspects as a specical case.
            Add(new SpiritAspect(SpiritAspect.Base, config, this, expansions, 0));
        }

        [JsonIgnore]
        /// <summary>
        /// Convenience property for accessing the SpiritAspect child elements
        /// </summary>
        /// <value></value>
        public IEnumerable<SpiritAspect> Aspects 
        {
            get 
            {
                return this;
            }
        }
    }
}
