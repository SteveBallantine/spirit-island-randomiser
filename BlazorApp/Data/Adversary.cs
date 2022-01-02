using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace SiRandomizer.Data
{
    public class Adversary : SelectableComponentBase<Adversary>, IComponentCollection<INamedComponent>, IExpansionContent
    {
        public static Adversary NoAdversary = new Adversary() { 
            Name = "No Adversary", 
            Levels = new List<AdversaryLevel>() {                
                new AdversaryLevel("Level 0"){ Level = 0, DifficultyModifier = 0 },
            }};
        public static Adversary England = new Adversary() { 
            Name = "England", 
            Levels = new List<AdversaryLevel>() {
                new AdversaryLevel("Level 0"){ Level = 0, DifficultyModifier = 1 },
                new AdversaryLevel("Level 1"){ Level = 1, DifficultyModifier = 3 },
                new AdversaryLevel("Level 2"){ Level = 2, DifficultyModifier = 4 },
                new AdversaryLevel("Level 3"){ Level = 3, DifficultyModifier = 6 },
                new AdversaryLevel("Level 4"){ Level = 4, DifficultyModifier = 7 },
                new AdversaryLevel("Level 5"){ Level = 5, DifficultyModifier = 9 },
                new AdversaryLevel("Level 6"){ Level = 6, DifficultyModifier = 11 }
            }};
        public static Adversary BrandenburgPrussia = new Adversary() { 
            Name = "Brandenburg Prussia", 
            Levels = new List<AdversaryLevel>() {
                new AdversaryLevel("Level 0"){ Level = 0, DifficultyModifier = 1 },
                new AdversaryLevel("Level 1"){ Level = 1, DifficultyModifier = 2 },
                new AdversaryLevel("Level 2"){ Level = 2, DifficultyModifier = 4 },
                new AdversaryLevel("Level 3"){ Level = 3, DifficultyModifier = 6 },
                new AdversaryLevel("Level 4"){ Level = 4, DifficultyModifier = 7 },
                new AdversaryLevel("Level 5"){ Level = 5, DifficultyModifier = 9 },
                new AdversaryLevel("Level 6"){ Level = 6, DifficultyModifier = 10 }
            }};
        public static Adversary Sweden = new Adversary() { 
            Name = "Sweden", 
            Levels = new List<AdversaryLevel>() {
                new AdversaryLevel("Level 0"){ Level = 0, DifficultyModifier = 1 },
                new AdversaryLevel("Level 1"){ Level = 1, DifficultyModifier = 2 },
                new AdversaryLevel("Level 2"){ Level = 2, DifficultyModifier = 3 },
                new AdversaryLevel("Level 3"){ Level = 3, DifficultyModifier = 5 },
                new AdversaryLevel("Level 4"){ Level = 4, DifficultyModifier = 6 },
                new AdversaryLevel("Level 5"){ Level = 5, DifficultyModifier = 7 },
                new AdversaryLevel("Level 6"){ Level = 6, DifficultyModifier = 8 }
            }};
        public static Adversary France = new Adversary() { 
            Name = "France", 
            Expansion = Expansion.BranchAndClaw,
            Levels = new List<AdversaryLevel>() {
                new AdversaryLevel("Level 0"){ Level = 0, DifficultyModifier = 2 },
                new AdversaryLevel("Level 1"){ Level = 1, DifficultyModifier = 3 },
                new AdversaryLevel("Level 2"){ Level = 2, DifficultyModifier = 5 },
                new AdversaryLevel("Level 3"){ Level = 3, DifficultyModifier = 7 },
                new AdversaryLevel("Level 4"){ Level = 4, DifficultyModifier = 8 },
                new AdversaryLevel("Level 5"){ Level = 5, DifficultyModifier = 9 },
                new AdversaryLevel("Level 6"){ Level = 6, DifficultyModifier = 10 }
            }};
        public static Adversary Habsburg = new Adversary() { 
            Name = "Habsburg", 
            Expansion = Expansion.JaggedEarth,
            Levels = new List<AdversaryLevel>() {
                new AdversaryLevel("Level 0"){ Level = 0, DifficultyModifier = 2 },
                new AdversaryLevel("Level 1"){ Level = 1, DifficultyModifier = 3 },
                new AdversaryLevel("Level 2"){ Level = 2, DifficultyModifier = 5 },
                new AdversaryLevel("Level 3"){ Level = 3, DifficultyModifier = 6 },
                new AdversaryLevel("Level 4"){ Level = 4, DifficultyModifier = 8 },
                new AdversaryLevel("Level 5"){ Level = 5, DifficultyModifier = 9 },
                new AdversaryLevel("Level 6"){ Level = 6, DifficultyModifier = 10 }
            }};
        public static Adversary Russia = new Adversary() { 
            Name = "Russia", 
            Expansion = Expansion.JaggedEarth,
            Levels = new List<AdversaryLevel>() {
                new AdversaryLevel("Level 0"){ Level = 0, DifficultyModifier = 1 },
                new AdversaryLevel("Level 1"){ Level = 1, DifficultyModifier = 3 },
                new AdversaryLevel("Level 2"){ Level = 2, DifficultyModifier = 4 },
                new AdversaryLevel("Level 3"){ Level = 3, DifficultyModifier = 6 },
                new AdversaryLevel("Level 4"){ Level = 4, DifficultyModifier = 7 },
                new AdversaryLevel("Level 5"){ Level = 5, DifficultyModifier = 9 },
                new AdversaryLevel("Level 6"){ Level = 6, DifficultyModifier = 11 }
            }};
        public static Adversary Scotland = new Adversary() { 
            Name = "Scotland", 
            Expansion = Expansion.Promo2,
            Levels = new List<AdversaryLevel>() {
                new AdversaryLevel("Level 0"){ Level = 0, DifficultyModifier = 1 },
                new AdversaryLevel("Level 1"){ Level = 1, DifficultyModifier = 3 },
                new AdversaryLevel("Level 2"){ Level = 2, DifficultyModifier = 4 },
                new AdversaryLevel("Level 3"){ Level = 3, DifficultyModifier = 6 },
                new AdversaryLevel("Level 4"){ Level = 4, DifficultyModifier = 7 },
                new AdversaryLevel("Level 5"){ Level = 5, DifficultyModifier = 8 },
                new AdversaryLevel("Level 6"){ Level = 6, DifficultyModifier = 10 }
            }};

        public Expansion Expansion {get; set;}

        /// <summary>
        /// Don't need this to be public, as the class implements 
        /// IComponentCollection and exposes the levels as an IEnumerable
        /// in order to simplify the nested lists implementation in the UI.
        /// 
        /// However, it can be a little confusing working with it that
        /// way in code, so we also add this accessor for clarity.
        /// For example:
        /// `Adversary.England.Where(a => a.Selected)`
        /// is the same as
        /// `Adversary.England.Levels.Where(a => a.Selected)`
        /// But the second approach makes the intent clearer to the reader.
        /// </summary>
        /// <value></value>
        public IReadOnlyList<SelectableComponentBase> Levels { get; protected set; }

        private Adversary(){   
            if(Levels != null && Levels.Count > 0)
            {     
                foreach(var level in Levels)
                {
                    level.PropertyChanged += AdversaryLevelUpdated;
                }
            }
            PropertyChanged += ThisUpdated;
        }

        public IEnumerator<INamedComponent> GetEnumerator()
        {
            return Levels.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Levels.GetEnumerator();
        }
        
        public void AdversaryLevelUpdated (object sender, PropertyChangedEventArgs args) {
            // When child is updated, trigger the property changed
            // event on the group.
            OnPropertyChanged("Level_" + args.PropertyName);
        }

        public void ThisUpdated (object sender, PropertyChangedEventArgs args) {
            // When the selection state of this adversary is changed, 
            // update all it's levels accordingly.
            if(args.PropertyName == nameof(Selected) &&
                Levels != null && Levels.Count > 0) 
            {
                foreach(var level in Levels) 
                {
                    level.Selected = Selected;
                }
            }
        }
    }
}
