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
        public OptionGroup<Adversary> Adversaries { get; private set; }
        public OptionGroup<Board> Boards { get; private set; }
        public OptionGroup<Map> Maps { get; private set; }
        public OptionGroup<Expansion> Expansions { get; private set; }
        public OptionGroup<Spirit> Spirits { get; private set; }
        public OptionGroup<Scenario> Scenarios { get; private set; }

        [Required]
        [Range(1, 6, ErrorMessage = "Number of players must be 1 - 6")]
        public int Players {get;set;}
        [Required]
        [Range(0, 20, ErrorMessage = "Minimum difficulty must be 0 - 20")]
        public int MinDifficulty {get;set;}
        [Required]
        [Range(0, 20, ErrorMessage = "Maximum difficulty must be 0 - 20")]
        public int MaxDifficulty {get;set;}
        public OptionChoice AdditionalBoard {get;set;} = OptionChoice.Block;
        public OptionChoice CombinedAdversaries {get;set;} = OptionChoice.Block;
        public OptionChoice RandomThematicBoards {get;set;} = OptionChoice.Block;

        public int MaxAdditionalBoards 
        {
            get 
            {
                switch (AdditionalBoard)
                {
                    case OptionChoice.Allow:
                    case OptionChoice.Force:
                        return 1;
                    case OptionChoice.Block:
                        return 0;
                    default:
                        throw new Exception("Unexpected value");
                }
            }
        }

        public OverallConfiguration(
            OptionGroup<Adversary> adversaries,
            OptionGroup<Board> boards,
            OptionGroup<Map> maps,
            OptionGroup<Expansion> expansions,
            OptionGroup<Spirit> spirits,
            OptionGroup<Scenario> scenarios) 
        {
            Adversaries = adversaries;
            Boards = boards;
            Maps = maps;
            Expansions = expansions;
            Spirits = spirits;
            Scenarios = scenarios;
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
                yield return new ValidationResult("Player count + max additional boards must be no bigger than the number of selected boards",
                    new[] { nameof(Boards) });
            }
            if(Scenarios.All(s => s.Selected) == false)
            {
                yield return new ValidationResult("Must pick at least one scenario", new[] { nameof(Scenarios) });
            }
            if(Adversaries.All(s => s.Selected) == false)
            {
                yield return new ValidationResult("Must pick at least one adversary", new[] { nameof(Adversaries) });
            }
            if(Maps.All(s => s.Selected) == false)
            {
                yield return new ValidationResult("Must pick at least one maps", new[] { nameof(Maps) });
            }
            if(Spirits.Count(s => s.Selected) < Players)
            {
                yield return new ValidationResult($"Must pick at least {Players} spirits for {Players} players", new[] { nameof(Spirits) });
            }
        }
    }
}
