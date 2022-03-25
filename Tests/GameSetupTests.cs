using Microsoft.VisualStudio.TestTools.UnitTesting;
using SiRandomizer.Services;
using SiRandomizer.Data;
using SiRandomizer.Exceptions;
using System.Linq;
using System;
using System.Collections.Generic;

namespace SiRandomizer.tests
{
    [TestClass]
    public class GameSetupTests
    {
        private static Adversary _adversaryA;
        private static AdversaryLevel _levelA1;
        private static AdversaryLevel _levelA2;

        private static Adversary _adversaryB;
        private static AdversaryLevel _levelB1;
        private static AdversaryLevel _levelB2;
        private static AdversaryLevel _levelB3;

        private static Map _map0;
        private static Map _map1;
        private static Scenario _scenario0;
        private static Scenario _scenario1;

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            _adversaryA = new Adversary("Test A", null, null);
            _levelA1 = new AdversaryLevel("A1", null, 1, 2);
            _levelA2 = new AdversaryLevel("A2", null, 2, 4);
            _adversaryA.Add(_levelA1);
            _adversaryA.Add(_levelA2);

            _adversaryB = new Adversary("Test B", null, null);
            _levelB1 = new AdversaryLevel("B1", null, 1, 1);
            _levelB2 = new AdversaryLevel("B2", null, 2, 3);
            _levelB3 = new AdversaryLevel("B3", null, 2, 4);
            _adversaryB.Add(_levelB1);
            _adversaryB.Add(_levelB2);
            _adversaryB.Add(_levelB3);

            _map0 = new Map("TestM0", null, 1, 6, 0);
            _map1 = new Map("TestM1", null, 1, 6, 1);

            _scenario0 = new Scenario("TestS0", null, null, 0);
            _scenario1 = new Scenario("TestS1", null, null, 1);
        }

        [DataTestMethod]
        [DynamicData("Difficulty_TestData", DynamicDataSourceType.Method)]
        public void Difficulty(string testDescription,
            GameSetup setup, 
            bool hasSupportingAdversary,
            int expectedDifficulty, 
            int leadingDifficulty, 
            int supportingDifficulty)
        {
            Assert.AreEqual(hasSupportingAdversary, setup.HasSupportingAdversary, "HasSupportingAdversary is incorrect");
            Assert.AreEqual(expectedDifficulty, setup.Difficulty, "Difficulty is incorrect");
            Assert.AreEqual(leadingDifficulty, setup.LeadingAdversaryDifficultyModifier, "LeadingAdversaryDifficultyModifier is incorrect");
            Assert.AreEqual(supportingDifficulty, setup.SupportingAdversaryDifficultyModifier, "SupportingAdversaryDifficultyModifier is incorrect");
        }

        public static IEnumerable<object[]> Difficulty_TestData()
        {
            // Single adversary
            GameSetup setup = new GameSetup()
            {
                LeadingAdversary = _levelA1,
                Map = _map0,
                Scenario = _scenario0
            };
            var lDiff = setup.LeadingAdversary.DifficultyModifier;
            yield return new object[] { "Single adversary", setup, false, lDiff, lDiff, 0 };

            // Leading adversary is higher difficulty than supporting adversary.
            // Multiplier does not affect overall difficulty.
            setup = new GameSetup()
            {
                LeadingAdversary = _levelA1,
                SupportingAdversary = _levelB1,
                Map = _map0,
                Scenario = _scenario0
            };
            lDiff = setup.LeadingAdversary.DifficultyModifier;
            var sDiff = (int)Math.Round(setup.SupportingAdversary.DifficultyModifier * 0.6);
            yield return new object[] { "Leading adversary higher", setup, true, lDiff + sDiff, lDiff, sDiff };

            // Leading adversary is higher difficulty than supporting adversary.
            // Multiplier does affect overall difficulty.
            setup = new GameSetup()
            {
                LeadingAdversary = _levelA2, 
                SupportingAdversary = _levelB2,
                Map = _map0,
                Scenario = _scenario0
            };
            lDiff = setup.LeadingAdversary.DifficultyModifier;
            sDiff = (int)Math.Round(setup.SupportingAdversary.DifficultyModifier * 0.6);
            yield return new object[] { "Leading adversary higher 2", setup, true, lDiff + sDiff, lDiff, sDiff };

            // Supporting adversary is higher difficulty than leading adversary.
            setup = new GameSetup()
            {
                LeadingAdversary = _levelA1, 
                SupportingAdversary = _levelB2,
                Map = _map0,
                Scenario = _scenario0
            };
            lDiff = (int)Math.Round(setup.LeadingAdversary.DifficultyModifier * 0.6);
            sDiff = setup.SupportingAdversary.DifficultyModifier;
            yield return new object[] { "Supporting adversary higher", setup, true, lDiff + sDiff, lDiff, sDiff };
            
            // Supporting adversary and leading adversary are same difficulty
            setup = new GameSetup()
            {
                LeadingAdversary = _levelA2, 
                SupportingAdversary = _levelB3,
                Map = _map0,
                Scenario = _scenario0
            };
            lDiff = setup.LeadingAdversary.DifficultyModifier;
            sDiff = (int)Math.Round(setup.SupportingAdversary.DifficultyModifier * 0.6);
            yield return new object[] { "Leading and supporting same difficulty",  setup, true, lDiff + sDiff, lDiff, sDiff };

            // Map and scenario difficulty included
            setup = new GameSetup()
            {
                LeadingAdversary = _levelA1,
                Map = _map1,
                Scenario = _scenario1
            };
            lDiff = setup.LeadingAdversary.DifficultyModifier;
            yield return new object[] { "Map and Scenario", setup, false, lDiff + _map1.DifficultyModifier + _scenario1.DifficultyModifier, lDiff, 0 };
        }

    }
}