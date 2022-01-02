using System;

namespace SiRandomizer.Data
{
    public class Spirit : SelectableComponentBase<Spirit>, INamedComponent, IExpansionContent
    {
        public static Spirit Lightning =  new Spirit() { 
            Name = "Lightning's Swift Strike", 
            BaseComplexity = Complexity.Low
        };
        public static Spirit River =  new Spirit() { 
            Name = "River Surges in Sunlight", 
            BaseComplexity = Complexity.Low
        };
        public static Spirit Earth =  new Spirit() { 
            Name = "Vital Strength of the Earth", 
            BaseComplexity = Complexity.Low
        };
        public static Spirit Shadows =  new Spirit() { 
            Name = "Shadows Flicker Like Flame", 
            BaseComplexity = Complexity.Low
        };
        public static Spirit Thunderspeaker =  new Spirit() { 
            Name = "Thunderspeaker", 
            BaseComplexity = Complexity.Moderate
        };
        public static Spirit Green =  new Spirit() { 
            Name = "A Spread of Rampant Green", 
            BaseComplexity = Complexity.Moderate
        };
        public static Spirit Ocean =  new Spirit() { 
            Name = "Ocean's Hungry Grasp", 
            BaseComplexity = Complexity.High
        };
        public static Spirit Bringer =  new Spirit() { 
            Name = "Bringer of Dreams and Nightmares", 
            BaseComplexity = Complexity.High
        };
        public static Spirit Fangs =  new Spirit() { 
            Name = "Sharp Fangs Behind the Leaves", 
            BaseComplexity = Complexity.Moderate, 
            Expansion = Expansion.BranchAndClaw 
        };
        public static Spirit Keeper =  new Spirit() { 
            Name = "Keeper of the Forbidden Wilds", 
            BaseComplexity = Complexity.Moderate, 
            Expansion = Expansion.BranchAndClaw 
        };
        public static Spirit Wildfire =  new Spirit() { 
            Name = "Heart of the Wildfire", 
            BaseComplexity = Complexity.High, 
            Expansion = Expansion.Promo1 
        };
        public static Spirit Snek =  new Spirit() { 
            Name = "Serpent Slumbering Beneath the Island", 
            BaseComplexity = Complexity.High, 
            Expansion = Expansion.Promo1 
        };
        public static Spirit Stone =  new Spirit() { 
            Name = "Stone's Unyielding Definance", 
            BaseComplexity = Complexity.Moderate, 
            Expansion = Expansion.JaggedEarth 
        };
        public static Spirit Memory =  new Spirit() { 
            Name = "Shifting Memory of Ages", 
            BaseComplexity = Complexity.Moderate, 
            Expansion = Expansion.JaggedEarth 
        };
        public static Spirit Trickster =  new Spirit() { 
            Name = "Grinning Trickster Stirs up Trouble", 
            BaseComplexity = Complexity.Moderate, 
            Expansion = Expansion.JaggedEarth 
        };
        public static Spirit Lure =  new Spirit() { 
            Name = "Lure of the Deep Wilderness", 
            BaseComplexity = Complexity.Moderate, 
            Expansion = Expansion.JaggedEarth 
        };
        public static Spirit ManyMinds =  new Spirit() { 
            Name = "Many Minds Move as One", 
            BaseComplexity = Complexity.Moderate, 
            Expansion = Expansion.JaggedEarth 
        };
        public static Spirit Volcano =  new Spirit() { 
            Name = "Volcano Looming High", 
            BaseComplexity = Complexity.Moderate, 
            Expansion = Expansion.JaggedEarth 
        };
        public static Spirit Mist =  new Spirit() { 
            Name = "Shroud of Silent Mist", 
            BaseComplexity = Complexity.High, 
            Expansion = Expansion.JaggedEarth 
        };
        public static Spirit Vengeance =  new Spirit() { 
            Name = "Vengeance as a Burning Plague", 
            BaseComplexity = Complexity.High, 
            Expansion = Expansion.JaggedEarth 
        };
        public static Spirit Starlight =  new Spirit() { 
            Name = "Starlight Seeks its Form", 
            BaseComplexity = Complexity.VeryHigh, 
            Expansion = Expansion.JaggedEarth 
        };
        public static Spirit Fractured =  new Spirit() { 
            Name = "Fractured Days Split the Sky", 
            BaseComplexity = Complexity.VeryHigh, 
            Expansion = Expansion.JaggedEarth 
        };
        public static Spirit Downpour =  new Spirit() { 
            Name = "Downpour Drenches the World", 
            BaseComplexity = Complexity.High, 
            Expansion = Expansion.Promo2 
        };
        public static Spirit Finder =  new Spirit() { 
            Name = "Finder of Paths Unseen", 
            BaseComplexity = Complexity.VeryHigh, 
            Expansion = Expansion.Promo2 
        };
        public static Spirit Rot =  new Spirit() { 
            Name = "Spreading Rot Renews the Earth", 
            BaseComplexity = Complexity.High, 
            Expansion = Expansion.Apocrypha 
        };

        public Complexity BaseComplexity {get; set;}
        public Expansion Expansion {get; set;}

        private Spirit() {}
    }
}
