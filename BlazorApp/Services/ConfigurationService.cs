using System.Collections.Generic;
using System.Threading.Tasks;
using SiRandomizer.Data;

namespace SiRandomizer.Services
{
    public class ConfigurationService
    {
        public Task<OverallConfiguration> GetConfigurationAsync()
        {
            var config = new OverallConfiguration();
            config.Expansions = CreateExpansions(config);
            config.Adversaries = CreateAdversaries(config);
            config.Boards = CreateBoards(config);
            config.Maps = CreateMaps(config);
            config.Spirits = CreateSpirits(config);
            config.Scenarios = CreateScenarios(config);
            return Task.FromResult(config);
        }

        public OptionGroup<Expansion> CreateExpansions(OverallConfiguration config)
        {
            var expansions = new OptionGroup<Expansion>()
            {
                Name = OptionGroup<Expansion>.EXPANSIONS
            };
            
            expansions.Add(new Expansion(Expansion.BranchAndClaw, config, "BC"));
            expansions.Add(new Expansion(Expansion.JaggedEarth, config, "JE"));
            expansions.Add(new Expansion(Expansion.Promo1, config, "P1"));
            expansions.Add(new Expansion(Expansion.Promo2, config, "P2"));
            expansions.Add(new Expansion(Expansion.Apocrypha, config, "Ax"));
            expansions.Add(new Expansion(Expansion.Homebrew, config, "Hx"));

            return expansions;
        }

        public OptionGroup<Scenario> CreateScenarios(OverallConfiguration config)
        {
            var expansions = config.Expansions;
            var scenarios = new OptionGroup<Scenario>()
            {
                Name = OptionGroup<Scenario>.SPIRITS
            };

            scenarios.Add(new Scenario(Scenario.NoScenario, config, null, 0));
            scenarios.Add(new Scenario(Scenario.Blitz, config, null, 0));
            scenarios.Add(new Scenario(Scenario.GuardTheIslesHeart, config, null, 0));
            scenarios.Add(new Scenario(Scenario.RitualsOfTerror, config, null, 3));
            scenarios.Add(new Scenario(Scenario.DahanInsurrection, config, null, 4));
            scenarios.Add(new Scenario(Scenario.SecondWave, config, expansions[Expansion.BranchAndClaw], 1));
            scenarios.Add(new Scenario(Scenario.PowersLongForgotten, config, expansions[Expansion.BranchAndClaw], 2));
            scenarios.Add(new Scenario(Scenario.WardTheShores, config, expansions[Expansion.BranchAndClaw], 3));
            scenarios.Add(new Scenario(Scenario.RitualsOfTheDestroyingFlame, config, expansions[Expansion.BranchAndClaw], 3));
            scenarios.Add(new Scenario(Scenario.ElementalInvocation, config, expansions[Expansion.JaggedEarth], 1));
            scenarios.Add(new Scenario(Scenario.DespicableTheft, config, expansions[Expansion.JaggedEarth], 2));
            scenarios.Add(new Scenario(Scenario.TheGreatRiver, config, expansions[Expansion.JaggedEarth], 3));
            scenarios.Add(new Scenario(Scenario.ADiversityOfSpirits, config, expansions[Expansion.Promo2], 0));
            scenarios.Add(new Scenario(Scenario.VariedTerrains, config, expansions[Expansion.Promo2], 2));

            return scenarios;
        }

        public OptionGroup<Spirit> CreateSpirits(OverallConfiguration config)
        {
            var expansions = config.Expansions;
            var spirits = new OptionGroup<Spirit>()
            {
                Name = OptionGroup<Spirit>.SPIRITS
            };

            // Base
            var lightning = new Spirit(Spirit.Lightning, config, null, Complexity.Low);
            lightning.Add(new SpiritAspect(SpiritAspect.Wind, config, expansions[Expansion.JaggedEarth]));
            lightning.Add(new SpiritAspect(SpiritAspect.Pandemonium, config, expansions[Expansion.JaggedEarth]));
            lightning.Add(new SpiritAspect(SpiritAspect.Immense, config, expansions[Expansion.Promo2]));
            spirits.Add(lightning);
            var shadows = new Spirit(Spirit.Shadows, config, null, Complexity.Low);
            shadows.Add(new SpiritAspect(SpiritAspect.Madness, config, expansions[Expansion.JaggedEarth]));
            shadows.Add(new SpiritAspect(SpiritAspect.Reach, config, expansions[Expansion.JaggedEarth]));
            shadows.Add(new SpiritAspect(SpiritAspect.Amorphous, config, expansions[Expansion.Promo2]));
            shadows.Add(new SpiritAspect(SpiritAspect.Foreboding, config, expansions[Expansion.Promo2]));
            spirits.Add(shadows);
            var earth = new Spirit(Spirit.Earth, config, null, Complexity.Low);
            earth.Add(new SpiritAspect(SpiritAspect.Resilience, config, expansions[Expansion.JaggedEarth]));
            earth.Add(new SpiritAspect(SpiritAspect.Might, config, expansions[Expansion.Promo2]));
            spirits.Add(earth);
            var river = new Spirit(Spirit.River, config, null, Complexity.Low);
            river.Add(new SpiritAspect(SpiritAspect.Sunshine, config, expansions[Expansion.JaggedEarth]));
            river.Add(new SpiritAspect(SpiritAspect.Travel, config, expansions[Expansion.Promo2]));
            spirits.Add(river);
            spirits.Add(new Spirit(Spirit.Thunderspeaker, config, null, Complexity.Moderate));
            spirits.Add(new Spirit(Spirit.Green, config, null, Complexity.Moderate));
            spirits.Add(new Spirit(Spirit.Ocean, config, null, Complexity.High));
            spirits.Add(new Spirit(Spirit.Bringer, config, null, Complexity.High));
            // Branch and claw
            spirits.Add(new Spirit(Spirit.Keeper, config, expansions[Expansion.BranchAndClaw], Complexity.Moderate));
            spirits.Add(new Spirit(Spirit.Fangs, config, expansions[Expansion.BranchAndClaw], Complexity.Moderate));
            // Promo 1
            spirits.Add(new Spirit(Spirit.Wildfire, config, expansions[Expansion.Promo1], Complexity.High));
            spirits.Add(new Spirit(Spirit.Snek, config, expansions[Expansion.Promo1], Complexity.High));
            // Promo 2
            spirits.Add(new Spirit(Spirit.Downpour, config, expansions[Expansion.Promo2], Complexity.High));
            spirits.Add(new Spirit(Spirit.Finder, config, expansions[Expansion.Promo2], Complexity.VeryHigh));
            // Jagged Earth
            spirits.Add(new Spirit(Spirit.Volcano, config, expansions[Expansion.JaggedEarth], Complexity.Moderate));
            spirits.Add(new Spirit(Spirit.Lure, config, expansions[Expansion.JaggedEarth], Complexity.Moderate));
            spirits.Add(new Spirit(Spirit.ManyMinds, config, expansions[Expansion.JaggedEarth], Complexity.Moderate));
            spirits.Add(new Spirit(Spirit.Memory, config, expansions[Expansion.JaggedEarth], Complexity.Moderate));
            spirits.Add(new Spirit(Spirit.Stone, config, expansions[Expansion.JaggedEarth], Complexity.Moderate));
            spirits.Add(new Spirit(Spirit.Trickster, config, expansions[Expansion.JaggedEarth], Complexity.Moderate));
            spirits.Add(new Spirit(Spirit.Vengeance, config, expansions[Expansion.JaggedEarth], Complexity.High));
            spirits.Add(new Spirit(Spirit.Mist, config, expansions[Expansion.JaggedEarth], Complexity.High));
            spirits.Add(new Spirit(Spirit.Starlight, config, expansions[Expansion.JaggedEarth], Complexity.VeryHigh));
            spirits.Add(new Spirit(Spirit.Fractured, config, expansions[Expansion.JaggedEarth], Complexity.VeryHigh));
            // Apocrypha
            spirits.Add(new Spirit(Spirit.Rot, config, expansions[Expansion.Apocrypha], Complexity.High));
            
            return spirits;
        }

        public OptionGroup<Adversary> CreateAdversaries(OverallConfiguration config)
        {
            var expansions = config.Expansions;
            var adversaries = new OptionGroup<Adversary>()
            {
                Name = OptionGroup<Adversary>.ADVERSARIES
            };

            var adversary = new Adversary(Adversary.NoAdversary, config, null);
            adversary.Add(new AdversaryLevel("Level 0", config, 0, 0));
            adversaries.Add(adversary);
            adversary = new Adversary(Adversary.England, config, null);
            AddAdversaryLevels(adversary, new int[] { 1, 3, 4, 6, 7, 9, 11 }, config);
            adversaries.Add(adversary);
            adversary = new Adversary(Adversary.BrandenburgPrussia, config, null);
            AddAdversaryLevels(adversary, new int[] { 1, 2, 4, 6, 7, 9, 10 }, config);
            adversaries.Add(adversary);
            adversary = new Adversary(Adversary.Sweden, config, null);
            AddAdversaryLevels(adversary, new int[] { 1, 2, 3, 5, 6, 7, 8 }, config);
            adversaries.Add(adversary);
            adversary = new Adversary(Adversary.France, config, expansions[Expansion.BranchAndClaw]);
            AddAdversaryLevels(adversary, new int[] { 2, 3, 5, 7, 8, 9, 10 }, config);
            adversaries.Add(adversary);
            adversary = new Adversary(Adversary.Habsburg, config, expansions[Expansion.JaggedEarth]);
            AddAdversaryLevels(adversary, new int[] { 2, 3, 5, 6, 8, 9, 10 }, config);
            adversaries.Add(adversary);
            adversary = new Adversary(Adversary.Russia, config, expansions[Expansion.JaggedEarth]);
            AddAdversaryLevels(adversary, new int[] { 1, 3, 4, 6, 7, 9, 11 }, config);
            adversaries.Add(adversary);
            adversary = new Adversary(Adversary.Scotland, config, expansions[Expansion.Promo2]);
            AddAdversaryLevels(adversary, new int[] { 1, 3, 4, 6, 7, 8, 10 }, config);
            adversaries.Add(adversary);
            
            return adversaries;
        }

        private void AddAdversaryLevels(Adversary adversary,
            int[] difficulties,
            OverallConfiguration config)
        {
            for(int level = 0; level < difficulties.Length; level++)
            {
                adversary.Add(new AdversaryLevel($"Level {level}", config, level, difficulties[level]));
            }        
        }

        public OptionGroup<Map> CreateMaps(OverallConfiguration config)
        {
            var expansions = config.Expansions;
            var maps = new OptionGroup<Map>()
            {
                Name = OptionGroup<Map>.MAPS
            };

            maps.Add(new Map(Map.Standard, config, 1, 6, 0));
            maps.Add(new Map(Map.ThematicTokens, config, 1, 6, 1, true));
            maps.Add(new Map(Map.ThematicNoTokens, config, 1, 6, 3, true));
            maps.Add(new Map(Map.Archipelago, config, 2, 6, 1));
            maps.Add(new Map(Map.Fragment, config, 2, 2, 0));
            maps.Add(new Map(Map.OppositeShores, config, 2, 2, 0));
            maps.Add(new Map(Map.Coastline, config, 2, 6, 0));
            maps.Add(new Map(Map.Sunrise, config, 3, 3, 0));
            maps.Add(new Map(Map.Leaf, config, 4, 4, 0));
            maps.Add(new Map(Map.Snake, config, 3, 6, 0));
            maps.Add(new Map(Map.V, config, 5, 5, 0));
            maps.Add(new Map(Map.Snail, config, 5, 5, 0));
            maps.Add(new Map(Map.Peninsula, config, 5, 5, 0));
            maps.Add(new Map(Map.Star, config, 6, 6, 0));
            maps.Add(new Map(Map.Flower, config, 6, 6, 0));
            maps.Add(new Map(Map.Caldera, config, 6, 6, 0));  

            return maps;
        }

        public OptionGroup<Board> CreateBoards(OverallConfiguration config)
        {
            var expansions = config.Expansions;
            var boards = new OptionGroup<Board>()
            {
                Name = OptionGroup<Board>.BOARDS
            };

            var jaggedEarth = expansions[Expansion.JaggedEarth];

            var a = new Board(Board.A, config, null, false);
            var b = new Board(Board.B, config, null, false);
            var c = new Board(Board.C, config, null, false);
            var d = new Board(Board.D, config, null, false);
            var e = new Board(Board.E, config, jaggedEarth, false) 
            {
                ImbalancedWith = new List<Board>() { b }
            };
            var f = new Board(Board.F, config, jaggedEarth, false)
            {
                ImbalancedWith = new List<Board>() { d }
            };


            var ne = new Board(Board.NEast, config, null, true) 
            {
                ThematicDefinitivePlayerCounts = new List<int>() { 1, 3, 4, 5, 6 }
            };
            var nw = new Board(Board.NWest, config, null, true) 
            {
                ThematicDefinitivePlayerCounts = new List<int>() { 4, 5, 6 }
            };
            var east = new Board(Board.East, config, null, true) 
            {
                ThematicDefinitivePlayerCounts = new List<int>() { 2, 3, 4, 5, 6 }
            };
            var west = new Board(Board.West, config, null, true) 
            {
                ThematicDefinitivePlayerCounts = new List<int>() { 2, 3, 4, 5, 6 }
            };
            var se = new Board(Board.SEast, config, jaggedEarth, true) 
            {
                ThematicDefinitivePlayerCounts = new List<int>() { 5, 6 }
            };
            var sw = new Board(Board.SWest, config, jaggedEarth, true) 
            {
                ThematicDefinitivePlayerCounts = new List<int>() { 6 }
            };

            ne.ThematicNeighbours = new List<Board>() { nw, east };
            nw.ThematicNeighbours = new List<Board>() { ne, west };
            west.ThematicNeighbours = new List<Board>() { east, nw, sw };
            east.ThematicNeighbours = new List<Board>() { west, ne, se };
            se.ThematicNeighbours = new List<Board>() { east, sw };
            sw.ThematicNeighbours = new List<Board>() { west, se };

            boards.Add(a);
            boards.Add(b);
            boards.Add(c);
            boards.Add(d);
            boards.Add(e);
            boards.Add(f);

            boards.Add(ne);
            boards.Add(nw);
            boards.Add(east);
            boards.Add(west);
            boards.Add(se);
            boards.Add(sw);

            return boards;
        }

    }
}
