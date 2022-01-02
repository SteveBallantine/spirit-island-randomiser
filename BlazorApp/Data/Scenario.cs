using System;
using System.Collections.Generic;

namespace SiRandomizer.Data
{
    public class Scenario : SelectableComponentBase<Scenario>, INamedComponent, IExpansionContent, IDifficultyModifier
    {
        public static Scenario NoScenario = new Scenario() { Name = "No Scenario", DifficultyModifier = 0 };
        public static Scenario Blitz = new Scenario() { Name = "Blitz", DifficultyModifier = 0 };
        public static Scenario GuardTheIslesHeart = new Scenario() { Name = "Guard the Isle's Heart", DifficultyModifier = 0 };
        public static Scenario RitualsOfTerror = new Scenario() { Name = "Rituals of Terror", DifficultyModifier = 3 };
        public static Scenario DahanInsurrection = new Scenario() { Name = "Dahan Insurrection", DifficultyModifier = 4 };
        public static Scenario SecondWave = new Scenario() { Name = "Second Wave", DifficultyModifier = 0, Expansion = Expansion.BranchAndClaw };
        public static Scenario PowersLongForgotten = new Scenario() { Name = "Powers Long Forgotten", DifficultyModifier = 1, Expansion = Expansion.BranchAndClaw };
        public static Scenario WardTheShores = new Scenario() { Name = "Ward the Shores", DifficultyModifier = 2, Expansion = Expansion.BranchAndClaw };
        public static Scenario RitualsOfTheDestroyingFlame = new Scenario() { Name = "Rituals of the Destroying Flame", DifficultyModifier = 3, Expansion = Expansion.BranchAndClaw };
        public static Scenario ElementalInvocation = new Scenario() { Name = "Elemental Invocation", DifficultyModifier = 1, Expansion = Expansion.JaggedEarth };
        public static Scenario DespicableTheft = new Scenario() { Name = "Despicable Theft", DifficultyModifier = 2, Expansion = Expansion.JaggedEarth };
        public static Scenario TheGreatRiver = new Scenario() { Name = "The Great River", DifficultyModifier = 3, Expansion = Expansion.JaggedEarth };
        public static Scenario ADiversityOfSpirits = new Scenario() { Name = "A Diversity of Spirits", DifficultyModifier = 0, Expansion = Expansion.Promo2 };
        public static Scenario VariedTerrains = new Scenario() { Name = "Varied Terrains", DifficultyModifier = 2, Expansion = Expansion.Promo2 };


        public Expansion Expansion {get; set;}

        public int DifficultyModifier {get;set;}

        private Scenario(){} 
    }
}
