using System;
using System.Collections.Generic;
using System.Linq;
using SiRandomizer.Data;
using SiRandomizer.Extensions;
using SiRandomizer.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace SiRandomizer.Services
{
    public class SetupGenerator 
    {
        private static readonly Random _rng = new Random();

        // Stores the number of possible combinations for each 
        // board count and number of picks. 
        // The first index is the number of boards available
        // to pick from (-1), the second is the number of boards
        // being picked (-1).
        // For example, THEMATIC_BOARD_COMBINATIONS[5, 1] would 
        // give the number of combinations assuming all 6 thematic 
        // boards are available and 2 of them are being picked.
        //
        // It's done this way because when getting the combinations 
        // for thematic map, we need to take account of the valid 
        // neighbours for each board.
        // This means we can't use the usual combinations 
        // calulation. It seems easier to just store the values
        // in a lookup table.
        //
        // Note that to simplify things slightly, we only consider
        // cases where the selected boards are all adjacent.
        // e.g. if won't be quite right if someone selects NE, NW and SE
        // and is picking two boards, as the only real valid option 
        // is NE + NW
        private static readonly int[,] THEMATIC_BOARD_COMBINATIONS = new int [,] 
        { 
            { 1, 0, 0, 0, 0, 0 },
            { 2, 1, 0, 0, 0, 0 },
            { 3, 2, 1, 0, 0, 0 },
            { 4, 4, 4, 1, 0, 0 },
            { 5, 5, 6, 3, 1, 0 },
            { 6, 7, 10, 10, 6, 1 }
        };

        private ILogger<SetupGenerator> _logger;

        public SetupGenerator(ILogger<SetupGenerator> logger)
        {
            _logger = logger;
        }

        public SetupResult Generate(OverallConfiguration config) 
        {
            bool foundValidSetup = false;
            int attempts = 0;
            IEnumerable<GameSetup> setups = null;
            DeterminedOptions options = null;

            // Repeat this several times if needed to try and find a valid options.
            while(foundValidSetup == false && attempts < 10)
            {
                // Determine which options to use when generating possible setups.
                options = DetermineOptions(config);

                // Get possible setups based on options and min/max difficulty.
                setups = GetSetups(config, options)                
                    .Where(c =>
                        c.GetComparitiveDifficulty(options.SpiritCount) >= config.MinDifficulty && 
                        c.GetComparitiveDifficulty(options.SpiritCount) <= config.MaxDifficulty);
                
                foundValidSetup = setups.Count() > 0;
                attempts++;
            }

            if(setups.Count() == 0)
            {
                throw new SiException($"No valid setups found for the configured options after 10 attempts");
            }

            _logger.LogDebug($"{setups.Count()} valid setups found");
            // Pick the setup to use from the available options
            var setup = setups.PickRandom(1).Single();
        
            // Get the number of possible combinations based on the number of players and
            // number of selected spirits. (Note - we can't easily use selected aspects 
            // because aspects cannot always be played together - e.g. immense lightning 
            // + wind lightning is an invalid result)
            var spirits = config.Spirits.Where(s => s.Selected);
            var spiritCombinations = spirits.GetCombinations(options.SpiritCount);

            foundValidSetup = false;
            attempts = 0;
            long boardCombinations = 0;

            while(foundValidSetup == false && attempts < 10)
            {
                attempts++;

                // Pick a random spirit and a random aspect for each selected spirit.
                var aspects = spirits.PickRandom(options.SpiritCount)
                    .Select(s => s.Aspects.Where(s => s.Selected).PickRandom(1).Single()).ToList();
                
                // Get a set of boards to match the number of spirits + any additional boards
                var boards = GetBoards(config, options, setup, out boardCombinations).ToList();
                if(boards.Count < options.SpiritCount + setup.AdditionalBoards) 
                {
                    throw new SiException("Failed to get the expected number of boards");
                }

                // Randomise spirit/board combos
                setup.BoardSetups = GetBoardSetups(boards, aspects).ToList();

                // Check if the selected setup is valid based on the final comparitive 
                // difficulty (which will now take spirit complexity into account)
                // If the 'account for cognitive load' option is not ticked, the difficulty 
                // will remain unchanged.
                foundValidSetup = 
                    setup.GetComparitiveDifficulty() >= config.MinDifficulty && 
                    setup.GetComparitiveDifficulty() <= config.MaxDifficulty;
            }
            
            if(foundValidSetup == false)
            {
                throw new SiException($"No valid setups found for the configured options after 10 attempts");
            }

            return new SetupResult() {
                Setup = setup,
                DifficultyOptionsConsidered = setups.Count(),
                BoardSetupOptionsConsidered = 
                    spiritCombinations * boardCombinations,
                ShowRandomThematicWarning = config.RandomThematicBoards == OptionChoice.Block
            };
        }

        private DeterminedOptions DetermineOptions(OverallConfiguration config)
        {
            DeterminedOptions options = new DeterminedOptions();

            // Determine spirit count.
            options.SpiritCount = config.RandomiseSpiritCount ? 
                _rng.Next(config.MinSpirits, config.MaxSpirits + 1) :
                config.MinSpirits;
            _logger.LogDebug($"Spirit count: {options.SpiritCount}");
            // Decide if we will be using an extra board or second adversary
            var useAdditionalBoard = _rng.NextDouble() <= (double)config.AdditionalBoardChance / 100;
            _logger.LogDebug($"Additional board: {useAdditionalBoard}");
            var useCombinedAdversary = _rng.NextDouble() <= (double)config.CombinedAdversariesChance / 100;
            _logger.LogDebug($"Combined adversaries: {useCombinedAdversary}");            
            if(useAdditionalBoard) 
            {
                config.AdditionalBoard = OptionChoice.Force;
                options.AdditionalBoards = 1;
            } 
            else 
            {
                config.AdditionalBoard = OptionChoice.Block;
                options.AdditionalBoards = 0;
            }
            if(useCombinedAdversary) 
            {
                config.CombinedAdversaries = OptionChoice.Force;
            } 
            else 
            {
                config.CombinedAdversaries = OptionChoice.Block;
            }

            options.Scenario = DetermineOption(config.Scenarios.Where(s => s.Selected));
            // Determine the entry to use for all options that have configurable weightings.
            options.Map = DetermineOption(config.Maps.Where(m => m.Selected && 
                // Ensure that only maps that are valid for the determined number of boards will be chosen.
                m.ValidForBoardCount(options.SpiritCount + options.AdditionalBoards) &&
                // Ensure that only maps that are valid for the determine scenario are chosen.
                (options.Scenario.ValidMaps == null || options.Scenario.ValidMaps.Any(v => v == m))));

            var possibleAdversaries = config.Adversaries.Where(a => a.Selected);
            if(useCombinedAdversary)
            {
                // If we've combining adversaries then the primary cannot be 'no adversary'
                possibleAdversaries = possibleAdversaries.Where(a => a.Name != Adversary.NoAdversary);
            }
            options.Adversary = DetermineOption(possibleAdversaries);

            if(useCombinedAdversary)
            {
                // Make sure the second adversary won't be the same as the first.
                options.SecondaryAdversary = DetermineOption(config.Adversaries.Where(a => 
                    a.Selected && a.Name != Adversary.NoAdversary && a != options.Adversary));
            }

            return options;
        }


        private T DetermineOption<T>(IEnumerable<T> options)
            where T : SelectableComponentBase<T>
        {
            if(options.Count() == 0) 
            {
                throw new SiException($"No valid '{typeof(T).Name}' to pick from");
            }

            var optionsText = string.Join(", ", options.Select(c => $"{c.Name} @ {c.Weight}"));
            _logger.LogDebug($"Selecting '{typeof(T).Name}' based on weights. Available options and weightings: {optionsText}");

            // Get the sum of all weights and pick a random value.
            var weightSum = options.Sum(c => c.Weight);
            var result = _rng.NextDouble() * weightSum;

            // Iterate through the items until we find the component corresponding to the random value.
            float weightsSoFar = 0;
            foreach(var option in options)
            {
                weightsSoFar += option.Weight;
                if(weightsSoFar > result)
                {
                    _logger.LogDebug($"Selected {typeof(T).Name}: '{option.Name}'");
                    return option;
                }    
            }

            return null;
        }

        private IEnumerable<BoardSetup> GetBoardSetups(List<Board> boards, List<SpiritAspect> spirits) 
        {
            while(boards.Count > 0) {
                var setup = new BoardSetup() {
                    Board = boards.PickRandom(1).Single(),
                    SpiritAspect = spirits.Count > 0 ? spirits.PickRandom(1).Single() : null
                };
                boards.Remove(setup.Board);
                spirits.Remove(setup.SpiritAspect);

                yield return setup;
            }
        }

        private IEnumerable<Board> GetBoards(
            OverallConfiguration config,
            DeterminedOptions options,
            GameSetup gameSetup, 
            out long boardCombinations) 
        {
            List<Board> selectedBoards = new List<Board>();
            boardCombinations = 0;

            var totalBoards = options.SpiritCount + 
                gameSetup.AdditionalBoards;

            if(gameSetup.Map.Thematic) 
            {
                var thematicBoards = config.Boards
                    .Where(b => b.Thematic && b.IsAvailable()).ToList();

                _logger.LogDebug($"Selecting thematic boards from: {string.Join(",", thematicBoards.Select(b => b.Name))}");

                Func<int> GetNonDefinitiveCombinations = () =>
                {    
                    int result = 0;
                    for(int i = config.MinAdditionalBoards; i <= config.MaxAdditionalBoards; i++)
                    {
                        // If there might be different numbers of boards (due to the 'allow additionl boards' option)
                        // We add the combinations for each possible numbers of boards in order to get the total.
                        result += THEMATIC_BOARD_COMBINATIONS[thematicBoards.Count - 1, options.SpiritCount + i - 1];
                    }
                    return result;
                };

                bool useDefinitiveBoards = false;
                switch (config.RandomThematicBoards)
                {
                    case OptionChoice.Allow:
                        useDefinitiveBoards = _rng.NextDouble() >= 0.5;
                        // Although we may or may not be using the definitive map,
                        // we use the the combinations for the non-definitive map as 
                        // that is the greatest number of options at this point.
                        boardCombinations = GetNonDefinitiveCombinations();
                        break;
                    case OptionChoice.Force:
                        useDefinitiveBoards = false;
                        boardCombinations = GetNonDefinitiveCombinations();
                        break;
                    case OptionChoice.Block:
                        useDefinitiveBoards = true;
                        // There is only 1 valid set of boards for any given number of
                        // spirits (+additional boards).
                        boardCombinations = config.MaxAdditionalBoards - config.MinAdditionalBoards + 1;
                        break;
                    default:
                        throw new Exception($"Unknown option {config.RandomThematicBoards}");
                }

                if(useDefinitiveBoards) 
                {
                    _logger.LogDebug($"Using definitive thematic boards for a count of {totalBoards}");
                    // We use the 'definitive' thematic boards based on the number of boards needed.
                    selectedBoards = thematicBoards
                        .Where(b => b.ThematicDefinitivePlayerCounts
                            .Contains(totalBoards)).ToList();
                }
                else 
                {
                    // As we're randomising the boards, only include the boards that were 
                    // actually selected.
                    thematicBoards = thematicBoards.Where(b => b.Selected).ToList();

                    _logger.LogDebug("Using random thematic boards");
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
                _logger.LogDebug("Selecting arcade boards");

                var possibleBoards = config.Boards
                    .Where(b => b.Thematic == false && b.Selected);
                // Get the total board combinations by adding the number of combinations
                // for each possible number of boards.                
                for(int i = config.MinAdditionalBoards; i <= config.MaxAdditionalBoards; i++)
                {
                    boardCombinations += possibleBoards.GetCombinations(options.SpiritCount + i);
                }

                if(totalBoards <= 4 && 
                    config.ImbalancedArcadeBoards == OptionChoice.Block)
                {
                    // If we're picking 4 or fewer boards and imbalanced arcade boards 
                    // are blocked then ensure that we do not return a set of boards that
                    // are imbalanced.
                    var boardList = possibleBoards.ToList();
                    selectedBoards = new List<Board>();
                    // Repeat until we've got the number of boards that we need.
                    while(selectedBoards.Count < totalBoards)
                    {
                        // Get a random board.
                        var nextBoard = boardList.PickRandom(1).Single();
                        boardList.Remove(nextBoard);
                        // If this board is balanced with all the existing selected boards
                        // then add it.
                        if(selectedBoards.All(b => 
                            (b.ImbalancedWith == null ||
                            b.ImbalancedWith.Contains(nextBoard) == false) &&
                            (nextBoard.ImbalancedWith == null ||
                            nextBoard.ImbalancedWith.Contains(b) == false)))
                        {
                            selectedBoards.Add(nextBoard);
                        }
                    }
                } 
                else 
                {
                    selectedBoards = possibleBoards
                        .PickRandom(totalBoards)
                        .ToList();
                }
            }

            _logger.LogDebug($"Selected boards: {string.Join(", ", selectedBoards.Select(b => b.Name))}");
            return selectedBoards;
        }

        /// <summary>
        /// Get all setup variations that affect difficulty.
        /// This will only include components that have been selected in the specified configuration.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private IEnumerable<GameSetup> GetSetups(OverallConfiguration config, DeterminedOptions options) 
        {
            var setups = options.Adversary.Levels
                .Where(a => a.Selected)
                .Select(l => new GameSetup() {
                    Scenario = options.Scenario,
                    Map = options.Map,
                    LeadingAdversary = l,
                    SupportingAdversary = config.Adversaries[Adversary.NoAdversary].Levels.Single(),
                    AdditionalBoards = options.AdditionalBoards,
                    AccountForCognitiveLoad = config.AccountForCognitiveLoad
                });
            if(options.SecondaryAdversary != null)
            {
                setups = AddSupportingAdversaryOptions(setups,
                    config.Adversaries[Adversary.NoAdversary].Levels.Single(), 
                    options.SecondaryAdversary.Levels.Where(a => a.Selected).ToList());
            }

            // Return the possible options as GameSetup instances.
            return setups;
        }

        private IEnumerable<GameSetup> AddSupportingAdversaryOptions(
            IEnumerable<GameSetup> input, 
            AdversaryLevel noAdversary,
            List<AdversaryLevel> supportingAdversaryLevels)
        {
            foreach(var entry in input)
            {
                foreach(var adversaryLevel in supportingAdversaryLevels)
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