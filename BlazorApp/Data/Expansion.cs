using System;

namespace SiRandomizer.Data
{
    public class Expansion : SelectableComponentBase<Expansion>, INamedComponent
    {
        public static Expansion BranchAndClaw = new Expansion() { Name = "Branch and Claw", Tag = "BC" };
        public static Expansion JaggedEarth = new Expansion() { Name = "Jagged Earth", Tag = "JE" };
        public static Expansion Promo1 = new Expansion() { Name = "Promo Pack 1", Tag = "P1" };
        public static Expansion Promo2 = new Expansion() { Name = "Promo Pack 2", Tag = "P2" };
        public static Expansion Apocrypha = new Expansion() { Name = "Apocrypha", Tag = "Ax" };
        public static Expansion Custom = new Expansion() { Name = "Custom", Tag = "Cx" };
        
        public string Tag {get; set;}

        private Expansion() {}
    }
}
