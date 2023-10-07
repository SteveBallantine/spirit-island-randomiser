using Microsoft.VisualStudio.TestTools.UnitTesting;
using SiRandomizer.Services;
using SiRandomizer.Data;
using SiRandomizer.Exceptions;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Text;
using Microsoft.Extensions.Logging;

namespace SiRandomizer.tests
{
    [TestClass]
    public class SetupGeneratorTests
    {
        private SetupGenerator _generator;
        private OverallConfiguration _config;

        [TestInitialize]
        public void Init()
        {
            var loggerFactory = new LoggerFactory();
            _generator = new SetupGenerator(loggerFactory.CreateLogger<SetupGenerator>());
            _config = new ConfigurationService().CreateConfiguration();
        }

        private void SetupMinimalOptions(bool thematic)
        {
            _config.Adversaries[Adversary.NoAdversary].Selected = true;
            _config.Scenarios[Scenario.NoScenario].Selected = true;
            _config.Spirits[Spirit.Green].Selected = true;
            if(thematic)
            {
                _config.Maps[Map.ThematicTokens].Selected = true;
                _config.Boards[Board.NEast].Selected = true;
            }
            else
            {
                _config.Maps[Map.Standard].Selected = true;
                _config.Boards[Board.A].Selected = true;
            }

            _config.MinDifficulty = 0;
            _config.MaxDifficulty = 10;
            _config.MinSpirits = 1;
        }

        private void SetupAllOptions(Func<Map, bool> mapFilter)
        {
            // Select all components
            _config.Expansions.Selected = true;
            // Must select maps before boards
            _config.Maps
                .Where(m => mapFilter(m)).ToList()
                .ForEach(m => m.Selected = true);
            _config.Boards.Selected = true;
            _config.Adversaries.Selected = true;
            _config.Scenarios.Selected = true;
            _config.Spirits.Selected = true;
        }

        /// <summary>
        /// Verify that an exception is thrown if user tries to generate 
        /// a setup using a configuration that produces no possible 
        /// difficulty combinations.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SiException))]
        public void Generate_NoneSelected()
        {
            OverallConfiguration config = new ConfigurationService().CreateConfiguration();          
            _generator.Generate(config);
        }

        /// <summary>
        /// Generate a setup where only a single result is possible.
        /// Verify that all values are as expected.
        /// </summary>
        [TestMethod]
        public void Generate_SingleOptions()
        {
            SetupMinimalOptions(false);

            var result = _generator.Generate(_config);

            Assert.AreEqual(1, result.BoardSetupOptionsConsidered);
            Assert.AreEqual(1, result.DifficultyOptionsConsidered);
            Assert.AreEqual(0, result.Setup.AdditionalBoards);
            Assert.AreEqual(Adversary.NoAdversary, result.Setup.LeadingAdversary.Parent.Name);
            Assert.AreEqual(Adversary.NoAdversary, result.Setup.SupportingAdversary.Parent.Name);
            Assert.AreEqual(Map.Standard, result.Setup.Map.Name);
            Assert.AreEqual(Scenario.NoScenario, result.Setup.Scenario.Name);

            Assert.AreEqual(1, result.Setup.BoardSetups.Count());
            Assert.AreEqual(Board.A, result.Setup.BoardSetups.First().Board.Name);
            Assert.AreEqual(Spirit.Green, result.Setup.BoardSetups.First().SpiritAspect.Parent.Name);
        }
        
        /// <summary>
        /// Verify that the 'additional board' option is behaving as expected.
        /// </summary>
        [DataTestMethod]
        // We allow the inclusion of an additional board to be determined randomly.
        // The algorithm will then consider options matching the outcome.
        // This means that there are:
        // - 3 possible board combinations considered (A, B or A + B)
        // - 2 difficulty option considerd (with additional board or without)
        [DataRow(50, 3, 2, 1, 0)]
        // We block the inclusion of an additional board.
        // This means that there are:
        // - 2 possible board combinations (A or B)
        // - 1 difficulty options (without additional board)
        [DataRow(0, 2, 1, 0, 0)]
        // We force the inclusion of an additional board.
        // This means that there are:
        // - 1 possible board combinations (A + B)
        // - 1 difficulty options (with additional board)
        [DataRow(100, 1, 1, 1, 1)]
        public void Generate_AdditionalBoard(
            int additionalBoardChance, 
            int expectedBoardOptions,
            int expectedDifficultyOptions,
            int expectedMaxAdditionalBoards,
            int expectedMinAdditionalBoards)
        {
            SetupMinimalOptions(false);
            // Need to add board B to support the additional board option.
            _config.Boards[Board.B].Selected = true;
            _config.AdditionalBoardChance = additionalBoardChance;

            List<SetupResult> setups = new List<SetupResult>();

            for(var i = 0; i < 1000; i++)
            {
                var result = _generator.Generate(_config);
                setups.Add(result);
            }

            // Verify all selected boards were only the selected ones.
            var validBoardNames = new HashSet<string>() { Board.A, Board.B };
            Assert.IsTrue(setups.All(s => s.Setup.BoardSetups.All(b => validBoardNames.Contains(b.Board.Name))));
            // Verify the number of options considered is correct.
            Assert.IsTrue(setups.All(s => s.BoardSetupOptionsConsidered == expectedBoardOptions));
            // DifficultyOptionsConsidered is no longer useful following changes to the way setups are generated.
            //Assert.IsTrue(setups.All(s => s.DifficultyOptionsConsidered == expectedDifficultyOptions));
            // Verify the number of additional boards are as expected.
            Assert.IsTrue(setups.All(s => 
                s.Setup.AdditionalBoards >= expectedMinAdditionalBoards &&
                s.Setup.AdditionalBoards <= expectedMaxAdditionalBoards));
            Assert.IsTrue(setups.All(s =>
                s.Setup.BoardSetups.Count() >= expectedMinAdditionalBoards + 1 &&
                s.Setup.BoardSetups.Count() <= expectedMaxAdditionalBoards + 1));
            // Ensure that if the number of additional boards is variable, each possible
            // number of additional boards has been selected at some point.
            if(expectedMinAdditionalBoards != expectedMaxAdditionalBoards)
            {
                var validValues = Enumerable.Range(expectedMinAdditionalBoards, expectedMaxAdditionalBoards);                
                Assert.IsTrue(
                    validValues.All(v => setups.Any(s => s.Setup.AdditionalBoards == v)));
            }
        }

        /// <summary>
        /// Verify that the 'random thematic' option is behaving as expected.
        /// </summary>
        [DataTestMethod]
        // We allow the use of random thematic boards to be determined randomly.
        // This means that there are:
        // - 4 possible board combinations (NE., E., W. or NW.)
        [DataRow(OptionChoice.Allow, 4)]
        // We block the use of random thematic boards.
        // This means that there are:
        // - 1 possible board combinations (NE.)
        [DataRow(OptionChoice.Block, 1)]
        // We force the use of random thematic boards.
        // This means that there are:
        // - 4 possible board combinations (NE., E., W. or NW.)
        [DataRow(OptionChoice.Force, 4)]
        public void Generate_RandomThematic(
            OptionChoice randomThematicChoice, 
            int expectedBoardOptions)
        {
            SetupMinimalOptions(true);
            _config.RandomThematicBoards = randomThematicChoice;

            List<SetupResult> setups = new List<SetupResult>();

            var validBoardNames = new HashSet<string>() { Board.NEast };
            if(_config.RandomThematicBoards == OptionChoice.Allow ||
                _config.RandomThematicBoards == OptionChoice.Force)
            {
                validBoardNames.Add(Board.NWest);
            }

            for(var i = 0; i < 1000; i++)
            {
                var result = _generator.Generate(_config);
                setups.Add(result);
                
                // Verify the selected boards were only the valid ones.
                Assert.IsTrue(result.Setup.BoardSetups.All(b => validBoardNames.Contains(b.Board.Name)),
                    $"Invalid board selected ({result.Setup.BoardSetups.Single().Board.Name})");
                // Verify the number of options considered is correct.
                Assert.AreEqual(expectedBoardOptions, result.BoardSetupOptionsConsidered);
                Assert.AreEqual(1, result.DifficultyOptionsConsidered);
            }
        }

        /// <summary>
        /// Verify that the 'combined adversaries' option is behaving as expected.
        /// </summary>
        [DataTestMethod]
        // We allow the use of an additional adversary to be determined randomly.
        // Notation: E = Enagland, B = BrandenburgPrussia, N = No Adversary.
        // This means that there are:
        // - 2 possible difficulty combinations (N, B, E or B + E, E + B)
        [DataRow(50, new int[] { 2, 3 })]
        // We block the use of an additional adversary.
        // This means that there are:
        // - 3 possible difficulty combinations (N, B, E)
        [DataRow(0, new int[] { 3 })]
        // We force the use of an additional adversary.
        // This means that there are:
        // - 2 possible difficulty combinations (B + E, E + B)
        [DataRow(100, new int[] { 2 })]
        public void Generate_CombinedAdversaries(
            int combinedAdversariesChance,
            int[] expectedDifficultyOptions)
        {
            SetupMinimalOptions(true);
            // Slect the first level for two adversaries to allow them to be combined.
            _config.Adversaries[Adversary.BrandenburgPrussia].Selected = true;
            _config.Adversaries[Adversary.England].Selected = true;
            _config.CombinedAdversariesChance = combinedAdversariesChance;

            List<SetupResult> setups = new List<SetupResult>();

            var validAdversaries = new HashSet<string>() { Adversary.BrandenburgPrussia, Adversary.England };
            if(_config.RandomThematicBoards != OptionChoice.Force)
            {
                validAdversaries.Add(Adversary.NoAdversary);
            }

            for(var i = 0; i < 1000; i++)
            {
                var result = _generator.Generate(_config);
                setups.Add(result);
                
                // Verify the selected boards were only the valid ones.
                Assert.IsTrue(validAdversaries.Contains(result.Setup.LeadingAdversary.Parent.Name), 
                    $"Invalid adversary - {result.Setup.LeadingAdversary.Parent.Name}");
                Assert.IsTrue(validAdversaries.Contains(result.Setup.SupportingAdversary.Parent.Name),
                    $"Invalid adversary - {result.Setup.SupportingAdversary.Parent.Name}");
                // Verify the number of options considered is correct.
                Assert.AreEqual(1, result.BoardSetupOptionsConsidered);
                // DifficultyOptionsConsidered is no longer useful following changes to the way setups are generated.
                //Assert.IsTrue(expectedDifficultyOptions.Contains((int)result.DifficultyOptionsConsidered));
            }
        }

        /// <summary>
        /// Generate 1000 random setups and verify that the spread of selected
        /// options in each category is fairly smooth.
        /// </summary>
        [TestMethod]
        public void Generate_SmoothDistribution()
        {
            _config.MinDifficulty = 0;
            _config.MaxDifficulty = 20;
            _config.MinSpirits = 2;

            // Limit this test to non-thematic maps as they can skew the selected boards.
            Func<Map, bool> mapFilter = m => m.Name.Contains("Thematic") == false && 
                    _config.MinSpirits <= m.MaxCount && _config.MinSpirits >= m.MinCount;
            SetupAllOptions(mapFilter);

            // Build some dictionaries to keep counts of options that are picked.
            var selectedBoards = _config.Boards
                .Where(b => b.Thematic == false)
                .ToDictionary(b => b.Name, b => 0);
            var selectedAdversaries = _config.Adversaries
                .ToDictionary(a => a.Name, b => 0);
            var selectedScenarios = _config.Scenarios
                .ToDictionary(s => s.Name, s => 0);
            var selectedSpirits = _config.Spirits
                .ToDictionary(s => s.Name, s => 0);
            var selectedMaps = _config.Maps
                .Where(m => mapFilter(m))
                .ToDictionary(s => s.Name, s => 0);

            // Generate 1000 setups and store the results in the dictionaries.
            for(var i = 0; i < 1000; i++)
            {
                var result = _generator.Generate(_config);

                foreach(var boardSetup in result.Setup.BoardSetups)
                {
                    selectedBoards[boardSetup.Board.Name]++;
                    selectedSpirits[boardSetup.SpiritAspect.Parent.Name]++;
                }
                selectedScenarios[result.Setup.Scenario.Name]++;
                selectedAdversaries[result.Setup.LeadingAdversary.Parent.Name]++;
                selectedMaps[result.Setup.Map.Name]++;
            }
            
            // Verify that the frequency of each option being selected is within the 
            // expected range.
            VerifySelectionFrequency(selectedBoards);
            VerifySelectionFrequency(selectedSpirits);
            VerifySelectionFrequency(selectedScenarios);
            VerifySelectionFrequency(selectedAdversaries);
            VerifySelectionFrequency(selectedMaps);
        }

        /// <summary>
        /// Verify that only the selected adversary levels can be picked
        /// </summary>
        [TestMethod]
        public void Generate_SelectedAdversaryLevels()
        {
            SetupMinimalOptions(true);
            _config.Adversaries[Adversary.NoAdversary].Selected = false;
            // Slect the middle levels for BP.
            _config.Adversaries[Adversary.BrandenburgPrussia].Selected = true;
            _config.Adversaries[Adversary.BrandenburgPrussia].Levels.ToList().ForEach(l => l.Selected = false);
            var validLevels = new HashSet<int>() { 3, 4 };
            _config.Adversaries[Adversary.BrandenburgPrussia].Levels.Where(l => validLevels.Contains(l.Level)).ToList().ForEach(l => l.Selected = true);

            List<SetupResult> setups = new List<SetupResult>();
            var validAdversaries = new HashSet<string>() { Adversary.BrandenburgPrussia };

            for(var i = 0; i < 1000; i++)
            {
                var result = _generator.Generate(_config);
                setups.Add(result);
                
                // Verify the selected boards were only the valid ones.
                Assert.IsTrue(validAdversaries.Contains(result.Setup.LeadingAdversary.Parent.Name), 
                    $"Invalid adversary - {result.Setup.LeadingAdversary.Parent.Name}");
                Assert.IsTrue(validLevels.Contains(result.Setup.LeadingAdversary.Level), 
                    $"Invalid adversary level - {result.Setup.LeadingAdversary.Level}");
            }
        }

        /// <summary>
        /// Check that each of the options in the table
        /// was selected a similar number of times.
        /// I've gone for fairly loose limits here as the process is random and
        /// we don't want it failing half the time just by chance.
        /// Instead, the purpose is to detect glaring errors.
        /// /// </summary>
        /// <param name="frequencyTable"></param>
        private void VerifySelectionFrequency(Dictionary<string, int> frequencyTable)
        {
            var values = frequencyTable.Select(v => v.Value);
            var average = (int)values.Average();
            var std = StdDev(values);
            var max = average + std * 4;
            var min = average - std * 4;
            if(min < 1) { min = 1; }

            /*foreach(var entry in frequencyTable)
            {
                Console.WriteLine($"{entry.Key}: {entry.Value}");
            }*/

            var componentsOusideRange = frequencyTable.Where(v => 
                v.Value > max || v.Value < min);
            StringBuilder message = new StringBuilder();
            foreach(var entry in componentsOusideRange)
            {
                message.AppendLine($"{entry.Key}: {entry.Value}");
            }
            Assert.IsFalse(componentsOusideRange.Any(),
                $"Number of selections is outside expected range " +
                $"({min}-{max}).{Environment.NewLine}" + message.ToString());
        }

        private int StdDev(IEnumerable<int> values)
        {
            double ret = 0;
            int count = values.Count();
            if (count  > 1)
            {
                //Compute the Average
                double avg = values.Average();
                //Perform the Sum of (value-avg)^2
                double sum = values.Sum(d => (d - avg) * (d - avg));
                //Put it all together
                ret = Math.Sqrt(sum / count);
            }
            return (int)ret;
        }
    }
}
