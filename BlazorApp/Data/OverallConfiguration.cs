using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using SiRandomizer.Data;

namespace SiRandomizer.Data
{
    public class OverallConfiguration : IValidatableObject
    {
        public OptionGroup Adversaries { get; private set; }
        public OptionGroup Boards { get; private set; }
        public OptionGroup Maps { get; private set; }
        public OptionGroup Expansions { get; private set; }
        public OptionGroup Spirits { get; private set; }
        public OptionGroup Scenarios { get; private set; }

        [Required]
        [Range(1, 6, ErrorMessage = "Number of players must be 1 - 6")]
        public int Players {get;set;}
        [Required]
        [Range(0, 20, ErrorMessage = "Minimum difficulty must be 0 - 20")]
        public int MinDifficulty {get;set;}
        [Required]
        [Range(0, 20, ErrorMessage = "Maximum difficulty must be 0 - 20")]
        public int MaxDifficulty {get;set;}
        [Required]
        public int MaxAdditionalBoards {get;set;}
        public bool AllowCombinedAdversaries {get;set;}
        public bool AllowRandomnThematicBoards {get;set;}

        public OverallConfiguration() {
            Init();
            Adversaries = new OptionGroup("Adversaries", GetDisplayedValues(Adversary.All));
            Boards = new OptionGroup("Boards", GetDisplayedValues(Board.All));
            Maps = new OptionGroup("Maps", GetDisplayedValues(Map.All));
            Expansions = new OptionGroup("Expansions", GetDisplayedValues(Expansion.All));
            Spirits = new OptionGroup("Spirits", GetDisplayedValues(Spirit.All));
            Scenarios = new OptionGroup("Scenarios", GetDisplayedValues(Scenario.All));
        }

        private void Init()
        {
            // Link each of the adversary levels back to thier parent adversary record.
            Adversary.All.ForEach(a => 
            {
                foreach(var level in a.Levels.Cast<AdversaryLevel>())
                {
                    level.Adversary = a;
                }
            });
        }

        private List<SelectableComponentBase> GetDisplayedValues<T>(IEnumerable<T> componenets) 
            where T : SelectableComponentBase<T>
        {
            return componenets
                .Where(m => m.Hide == false)
                .Cast<SelectableComponentBase>()
                .ToList();
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(MaxDifficulty < MinDifficulty)
            {
                yield return new ValidationResult("Maximum difficulty must be geater than or equal to minimum difficulty",
                    new[] { nameof(MaxDifficulty) });
            }
            if(Players + MaxAdditionalBoards > Boards.Count(b => b.Selected))
            {
                yield return new ValidationResult("Player count + max additional boards must by no bigger than the number of selected boards",
                    new[] { nameof(MaxAdditionalBoards) });
            }
        }
    }
}
