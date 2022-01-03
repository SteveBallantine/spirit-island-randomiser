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
            OverallConfiguration config = new OverallConfiguration();            
            _generator.Generate(config);
        }

        /// <summary>
        /// Generate a setup where only a single result is possible.
        /// Verify that all values are as expected.
        /// </summary>
        [TestMethod]
        public void Generate_SingleOptions()
        {
            OverallConfiguration config = new OverallConfiguration();
            // The default configuration components are all singletons
            // so changes to the originals will be reflected in the 
            // configuration object as well.
            Adversary.NoAdversary.Selected = true;
            Scenario.NoScenario.Selected = true;
            Map.Standard.Selected = true;
            Spirit.Green.Selected = true;
            Board.A.Selected = true;

            config.MinDifficulty = 0;
            config.MaxDifficulty = 0;
            config.Players = 1;

            var result = _generator.Generate(config);

            Assert.AreEqual(1, result.BoardSetupOptionsConsidered);
            Assert.AreEqual(1, result.DifficultyOptionsConsidered);
            Assert.AreEqual(0, result.Setup.AdditionalBoards);
            Assert.AreEqual(Adversary.NoAdversary.Levels.First(), result.Setup.LeadingAdversary);
            Assert.AreEqual(Adversary.NoAdversary, result.Setup.LeadingAdversary.Adversary);
            Assert.IsNull(result.Setup.SupportingAdversary);
            Assert.AreEqual(Map.Standard, result.Setup.Map);
            Assert.AreEqual(Scenario.NoScenario, result.Setup.Scenario);

            Assert.AreEqual(1, result.Setup.BoardSetups.Count());
            Assert.AreEqual(Board.A, result.Setup.BoardSetups.First().Board);
            Assert.AreEqual(Spirit.Green, result.Setup.BoardSetups.First().Spirit);
        }

        /// <summary>
        /// Generate 1000 random setups and verify that the spread of selected
        /// options in each category is fairly smooth.
        /// </summary>
        [TestMethod]
        public void Generate_SmoothDistribution()
        {
            var selectedBoards = Board.All
                .Where(b => b.Thmeatic == false)
                .ToDictionary(b => b.Name, b => 0);
            var selectedAdversaries = Adversary.All
                .SelectMany(a => a.Levels)
                .ToDictionary(l => l.Adversary.Name + l.Level, b => 0);
            var selectedScenarios = Scenario.All
                .ToDictionary(s => s.Name, s => 0);
            var selectedSpirits = Spirit.All
                .ToDictionary(s => s.Name, s => 0);

            for(var i = 0; i < 1000; i++)
            {
                OverallConfiguration config = new OverallConfiguration();
                // Select all components
                config.Boards.Selected = true;
                config.Adversaries.Selected = true;
                config.Scenarios.Selected = true;
                config.Spirits.Selected = true;
                // Just the standard map
                Map.Standard.Selected = true;

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
