using Microsoft.VisualStudio.TestTools.UnitTesting;
using SiRandomizer.Services;
using SiRandomizer.Data;
using SiRandomizer.Exceptions;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using System.Text;

namespace SiRandomizer.tests
{
    [TestClass]
    public class SetupGeneratorTests
    {
        private SetupGenerator _generator;

        [TestInitialize]
        public void Init()
        {
            _generator = new SetupGenerator();
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
            OverallConfiguration config = new ConfigurationService().GetConfigurationAsync().Result;          
            _generator.Generate(config);
        }

        /// <summary>
        /// Generate a setup where only a single result is possible.
        /// Verify that all values are as expected.
        /// </summary>
        [TestMethod]
        public void Generate_SingleOptions()
        {
            OverallConfiguration config = new ConfigurationService().GetConfigurationAsync().Result;
            config.Adversaries[Adversary.NoAdversary].Selected = true;
            config.Scenarios[Scenario.NoScenario].Selected = true;
            config.Maps[Map.Standard].Selected = true;
            config.Spirits[Spirit.Green].Selected = true;
            config.Boards[Board.A].Selected = true;

            config.MinDifficulty = 0;
            config.MaxDifficulty = 0;
            config.Players = 1;

            var result = _generator.Generate(config);

            Assert.AreEqual(1, result.BoardSetupOptionsConsidered);
            Assert.AreEqual(1, result.DifficultyOptionsConsidered);
            Assert.AreEqual(0, result.Setup.AdditionalBoards);
            Assert.AreEqual(Adversary.NoAdversary, result.Setup.LeadingAdversary.Adversary.Name);
            Assert.AreEqual(Adversary.NoAdversary, result.Setup.SupportingAdversary.Adversary.Name);
            Assert.AreEqual(Map.Standard, result.Setup.Map.Name);
            Assert.AreEqual(Scenario.NoScenario, result.Setup.Scenario.Name);

            Assert.AreEqual(1, result.Setup.BoardSetups.Count());
            Assert.AreEqual(Board.A, result.Setup.BoardSetups.First().Board.Name);
            Assert.AreEqual(Spirit.Green, result.Setup.BoardSetups.First().Spirit.Name);
        }

        /// <summary>
        /// Generate 1000 random setups and verify that the spread of selected
        /// options in each category is fairly smooth.
        /// </summary>
        [TestMethod]
        public void Generate_SmoothDistribution()
        {
            // This config is just used 
            OverallConfiguration config = new ConfigurationService().GetConfigurationAsync().Result;
            var selectedBoards = config.Boards
                .Where(b => b.Thematic == false)
                .ToDictionary(b => b.Name, b => 0);
            var selectedAdversaries = config.Adversaries
                .SelectMany(a => a.Levels)
                .ToDictionary(l => l.Adversary.Name + l.Level, b => 0);
            var selectedScenarios = config.Scenarios
                .ToDictionary(s => s.Name, s => 0);
            var selectedSpirits = config.Spirits
                .ToDictionary(s => s.Name, s => 0);

            for(var i = 0; i < 1000; i++)
            {
                config = new ConfigurationService().GetConfigurationAsync().Result;
                // Select all components
                config.Expansions.Selected = true;
                config.Boards.Selected = true;
                config.Adversaries.Selected = true;
                config.Scenarios.Selected = true;
                config.Spirits.Selected = true;
                // Just the standard map
                config.Maps[Map.Standard].Selected = true;

                config.MinDifficulty = 0;
                config.MaxDifficulty = 20;
                config.Players = 2;

                var result = _generator.Generate(config);

                foreach(var boardSetup in result.Setup.BoardSetups)
                {
                    selectedBoards[boardSetup.Board.Name]++;
                    selectedSpirits[boardSetup.Spirit.Name]++;
                }
                selectedScenarios[result.Setup.Scenario.Name]++;
                selectedAdversaries[result.Setup.LeadingAdversary.Adversary.Name + result.Setup.LeadingAdversary.Level]++;
            }

            VerifySelectionFrequency(selectedBoards);
            VerifySelectionFrequency(selectedSpirits);
            VerifySelectionFrequency(selectedScenarios);
            VerifySelectionFrequency(selectedAdversaries);
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
            StringBuilder message = new StringBuilder();
            foreach(var entry in frequencyTable)
            {
                message.AppendLine($"{entry.Key}: {entry.Value}");
            }

            var values = frequencyTable.Select(v => v.Value);
            var average = (int)values.Average();
            var std = StdDev(values);
            var max = average + std * 4;
            var min = average - std * 4;
            if(min < 1) { min = 1; }

            Assert.IsTrue(frequencyTable.All(v => 
                v.Value <= max && v.Value >= min),
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
