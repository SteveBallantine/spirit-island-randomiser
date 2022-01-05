using System;
using System.Collections.Generic;
using System.Linq;
using SiRandomizer.Data;
using SiRandomizer.Extensions;
using SiRandomizer.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace SiRandomizer.Services
{
    public class SetupGenerator {
        private static readonly Random _rng = new Random();

        // Stores the number of possible combinations for each 
        // board count. I.e. if picking 1 board then there are
        // 6 valid options. 2 boards there are 7 options, etc.
        // When getting the combinations for thematic map, we 
        // need to take account of the valid neighbours for 
        // each board.
        // This means we can't use the usual combinations 
        // calulation. It seems easier to just store the values
        // in a lookup table.
        private static readonly int[] THEMATIC_BOARD_COMBINATIONS = new int [] { 6, 7, 10, 10, 6, 1 };

        public SetupResult Generate(OverallConfiguration config) 
        {
            // Get possible setups based on included options and min/max difficulty.
            var setups = GetSetups(config)
                .Where(c => 
                    c.Difficulty >= config.MinDifficulty && 
                    c.Difficulty <= config.MaxDifficulty);

           if(setups.Count() == 0)
            {
                throw new SiException($"No setups found for difficulty " +
                    $"{config.MinDifficulty}-{config.MaxDifficulty}");
            }

            // Pick the setup to use from the avaialble options
            var setup = setups.PickRandom(1).Single();

            // Pick spirits
            var spirits = config.Spirits.Where(s => s.Selected).Cast<Spirit>().ToList();
            var spiritCombinations = spirits.GetCombinations(config.Players);
            spirits = spirits.PickRandom(config.Players).ToList();
            // Get a set of boards to match the number of spirits + any additional boards
            var boards = GetBoards(config, setup, out long boardCombinations).ToList();

            // Randomise spirit/board combos
            setup.BoardSetups = GetBoardSetups(boards, spirits).ToList();

            return new SetupResult() {
                Setup = setup,
                DifficultyOptionsConsidered = setups.Count(),
                BoardSetupOptionsConsidered = 
                    spiritCombinations * boardCombinations
            };
        }


        private IEnumerable<BoardSetup> GetBoardSetups(List<Board> boards, List<Spirit> spirits) 
        {
            while(boards.Count > 0) {
                var setup = new BoardSetup() {
                    Board = boards.PickRandom(1).Single(),
                    Spirit = spirits.Count > 0 ? spirits.PickRandom(1).Single() : null
                };
                boards.Remove(setup.Board);
                spirits.Remove(setup.Spirit);

                yield return setup;
            }
        }

        private IEnumerable<Board> GetBoards(
            OverallConfiguration config, 
            GameSetup gameSetup, 
            out long boardCombinations) 
        {
            List<Board> selectedBoards = new List<Board>();

            var totalBoards = config.Players + 
                gameSetup.AdditionalBoards;   

            if(gameSetup.Map == Map.Thematic || gameSetup.Map == Map.ThematicTokens) 
            {
                var thematicBoards = Board.All.Where(b => b.Thematic).ToList();

                bool useDefinitiveBoards = false;
                switch (config.RandomThematicBoards)
                {
                    case OptionChoice.Allow:
                        useDefinitiveBoards = _rng.NextDouble() >= 0.5;
                        // Although we may or may not be using the definitive map,
                        // we use the the combinations for the non-definitive map as 
                        // that is a possibility.
                        boardCombinations = THEMATIC_BOARD_COMBINATIONS[totalBoards + 1];
                        break;
                    case OptionChoice.Force:
                        useDefinitiveBoards = false;
                        boardCombinations = THEMATIC_BOARD_COMBINATIONS[totalBoards + 1];
                        break;
                    case OptionChoice.Block:
                        useDefinitiveBoards = true;
                        // There is only 1 valid set of boards for any given number of
                        // spirits (+additional boards).
                        boardCombinations = 1;
                        break;
                    default:
                        throw new Exception($"Unknown option {config.RandomThematicBoards}");
                }

                if(useDefinitiveBoards) 
                {
                    // We use the 'definitive' thematic boards based on the number of boards needed.
                    selectedBoards = thematicBoards
                        .Where(b => b.ThematicDefinitivePlayerCounts
                            .Contains(totalBoards)).ToList();
                }
                else 
                {
                    // We're allowed to use any thematic boards. However, the
                    // boards we pick need to be neighbouring ones. So start by 
                    // picking one at random, then repeatedly pick a random 
                    // neighbour until we have enough boards.                     
                    for(int i = 0; i < totalBoards; i++) 
                    {
                        // Set the list of possible boards to all of them.
                        var neighbourBoards = thematicBoards;
                        // If we already have 1 or more boards selected, then
                        // limit the possible boards to only those that are
                        // neighbours of the selected ones.
                        if(selectedBoards.Count > 0) 
                        {
                            neighbourBoards = thematicBoards
                                .Where(b => selectedBoards
                                    .Any(s => s.ThematicNeighbours.Contains(b)))
                                .ToList();
                        }
                        
                        // Pick a random board from the possible options
                        var board = neighbourBoards.PickRandom(1).Single();
                        thematicBoards.Remove(board);
                        selectedBoards.Add(board);
                    }                    
                } 
            } 
            else 
            {
                var possibleBoards = config.Boards
                    .Where(s => s.Selected)
                    .Cast<Board>();
                boardCombinations = possibleBoards.GetCombinations(totalBoards);

                selectedBoards = possibleBoards
                    .PickRandom(totalBoards)
                    .ToList();
            }

            return selectedBoards;
        }

        /// <summary>
        /// Get all setup variations that affect difficulty.
        /// This will only include components that have been selected in the specified configuration.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private IEnumerable<GameSetup> GetSetups(OverallConfiguration config) 
        {
            // Get the possible scenarios, maps and adversaries.
            var scenarios = config.Scenarios
                .Where(s => s.Selected).Cast<Scenario>();
            var maps = config.Maps
                .Where(s => s.Selected).Cast<Map>();
            var adversaryLevels = config.Adversaries
                .Where(s => s.Selected).Cast<Adversary>()
                .SelectMany(a => a.Levels);
            // Get the possible supporting adversaries.
            List<AdversaryLevel> supportingAdversaryLevels = adversaryLevels.ToList();
            // Get the possible numbers of additional boards.
            var additionalBoards = new List<int>();
            for(int i = 0; i <= config.MaxAdditionalBoards; i++) 
            {
                additionalBoards.Add(i);
            }

            // Join the various options so that we have all possible permutations.
            var joined1 = scenarios.Join(maps, s => true, m => true, 
                (s, m) => new { Scenario = s, Map = m });
            var joined2 = joined1.Join(adversaryLevels, j => true, a => true, 
                (j, a) => new { Scenario = j.Scenario, Map = j.Map, LeadingAdversary = a });            
            var setups = joined2.Join(additionalBoards, j => true, a => true, 
                (j, b) => new { Scenario = j.Scenario, Map = j.Map, LeadingAdversary = j.LeadingAdversary, AdditionalBoards = b })
                .Select(j => new GameSetup() {
                    Scenario = j.Scenario,
                    Map = j.Map,
                    LeadingAdversary = j.LeadingAdversary,
                    SupportingAdversary = Adversary.NoAdversary.Levels.First(),
                    AdditionalBoards = j.AdditionalBoards
                });
            // Adding a supporting adversary is a little trickier as we want some logic to ensure that:
            // 1. If leading adversary is 'no adversary' then supporting adversary MUST be 'no adversary'.
            // 2. Otherwise, supporting adversary MUST be different to leading adverary.
            // We also want to exclude options where supporting adversary is 'no adversary' if the
            // combine adversaries option is set to 'force'.
            if(config.CombinedAdversaries == OptionChoice.Allow ||
                config.CombinedAdversaries == OptionChoice.Force)
            {
                setups = AddSupportingAdversaryOptions(setups, supportingAdversaryLevels, 
                    config.CombinedAdversaries == OptionChoice.Force);
            }

            // Return the possible options as GameSetup instances.
            return setups;
        }

        private IEnumerable<GameSetup> AddSupportingAdversaryOptions(
            IEnumerable<GameSetup> input, 
            List<AdversaryLevel> supportingAdversaries,
            bool forceSupportingAdversary)
        {
            foreach(var entry in input)
            {
                // If leading adversary is 'no adversary' then supporting adversary MUST be 'no adversary'.
                if(entry.LeadingAdversary.Adversary.Name == Adversary.NoAdversary.Name)
                {
                    entry.SupportingAdversary = Adversary.NoAdversary.Levels.First();
                    // If forceSupportingAdversary flag is set then don't return values where 
                    // supporting adversary is 'NoAdversary'
                    if(forceSupportingAdversary == false)
                    {
                        yield return entry;
                    }
                }
                else 
                {
                    // Otherwise, supporting adversary MUST be different to leading adverary.
                    foreach(var adversaryLevel in supportingAdversaries
                        .Where(s => s.Adversary.Name != entry.LeadingAdversary.Adversary.Name &&
                            // If forceSupportingAdversary flag is set then don't return values where 
                            // supporting adversary is 'NoAdversary'
                            (forceSupportingAdversary == false || s.Adversary.Name != Adversary.NoAdversary.Name)))
                    {
                        yield return new GameSetup()
                        {
                            Scenario = entry.Scenario,
                            Map = entry.Map,
                            LeadingAdversary = entry.LeadingAdversary,
                            SupportingAdversary = adversaryLevel,
                            AdditionalBoards = entry.AdditionalBoards
                        };
                    }
                }
            }
        }
    }

}