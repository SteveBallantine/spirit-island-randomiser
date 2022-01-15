using System;

namespace SiRandomizer.Data
{
    public class Expansion : SelectableComponentBase<Expansion>, INamedComponent
    {
        public const string BranchAndClaw = "Branch and Claw";
        public const string JaggedEarth = "Jagged Earth";
        public const string Promo1 = "Promo Pack 1";
        public const string Promo2 = "Promo Pack 2";
        public const string Apocrypha = "Apocrypha";
        public const string Custom = "Custom";        
        
        public string Tag { get; private set; }

        public Expansion(
            string name, 
            string tag) 
            : base(name) 
        {
            Tag = tag;
        }
    }
}
