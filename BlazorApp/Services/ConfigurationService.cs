using System.Collections.Generic;
using System.Threading.Tasks;
using SiRandomizer.Data;
using SiRandomizer.Exceptions;

namespace SiRandomizer.Services
{
    public class ConfigurationService
    {
        private Lazy<OverallConfiguration> _current;
        public OverallConfiguration Current => _current.Value;

        public ConfigurationService()
        {
            _current = new Lazy<OverallConfiguration>(CreateConfiguration);
        }

        public OverallConfiguration CreateConfiguration()
        {
            var config = new OverallConfiguration();
            config.Expansions = CreateExpansions(config);
            config.Adversaries = CreateAdversaries(config);
            config.Boards = CreateBoards(config);
            config.Maps = CreateMaps(config);
            config.Spirits = CreateSpirits(config);
            config.Scenarios = CreateScenarios(config);
            return config;
        }

        public OptionGroup<Expansion> CreateExpansions(OverallConfiguration config)
        {
            var expansions = new OptionGroup<Expansion>()
            {
                Name = OptionGroup<Expansion>.EXPANSIONS
            };
            
            expansions.Add(new Expansion(Expansion.BranchAndClaw, config, expansions, "BC"));
            expansions.Add(new Expansion(Expansion.JaggedEarth, config, expansions, "JE"));
            expansions.Add(new Expansion(Expansion.Promo1, config, expansions, "P1"));
            expansions.Add(new Expansion(Expansion.Promo2, config, expansions, "P2"));
            expansions.Add(new Expansion(Expansion.Horizons, config, expansions, "HS"));
            //expansions.Add(new Expansion(Expansion.NatureIncarnate, config, "NI"));
            expansions.Add(new Expansion(Expansion.Apocrypha, config, expansions, "Ax"));
            expansions.Add(new Expansion(Expansion.Homebrew, config, expansions, "Hx"));

            return expansions;
        }

        public OptionGroup<Scenario> CreateScenarios(OverallConfiguration config)
        {
            if(config.Expansions == null) { throw new SiException("Must create expansions before scenarios"); }
            if(config.Maps == null) { throw new SiException("Must create maps before scenarios"); }

            var expansions = config.Expansions;
            var scenarios = new OptionGroup<Scenario>()
            {
                Name = OptionGroup<Scenario>.SCENARIOS
            };

            var noScenario = new Scenario(Scenario.NoScenario, config, scenarios, null, 0);
            noScenario.Selected = true;
            scenarios.Add(noScenario);
            scenarios.Add(new Scenario(Scenario.Blitz, config, scenarios, null, 0));
            scenarios.Add(new Scenario(Scenario.GuardTheIslesHeart, config, scenarios, null, 0));
            scenarios.Add(new Scenario(Scenario.RitualsOfTerror, config, scenarios, null, 3));
            scenarios.Add(new Scenario(Scenario.DahanInsurrection, config, scenarios, null, 4));
            scenarios.Add(new Scenario(Scenario.SecondWave, config, scenarios, expansions[Expansion.BranchAndClaw], 1));
            scenarios.Add(new Scenario(Scenario.PowersLongForgotten, config, scenarios, expansions[Expansion.BranchAndClaw], 2));
            scenarios.Add(new Scenario(Scenario.WardTheShores, config, scenarios, expansions[Expansion.BranchAndClaw], 2));
            scenarios.Add(new Scenario(Scenario.RitualsOfTheDestroyingFlame, config, scenarios, expansions[Expansion.BranchAndClaw], 3));
            scenarios.Add(new Scenario(Scenario.ElementalInvocation, config, scenarios, expansions[Expansion.JaggedEarth], 1));
            scenarios.Add(new Scenario(Scenario.DespicableTheft, config, scenarios, expansions[Expansion.JaggedEarth], 2));
            scenarios.Add(new Scenario(Scenario.TheGreatRiver, config, scenarios, expansions[Expansion.JaggedEarth], 3, new List<Map>() { config.Maps[Map.Coastline] }));
            scenarios.Add(new Scenario(Scenario.ADiversityOfSpirits, config, scenarios, expansions[Expansion.Promo2], 0));
            scenarios.Add(new Scenario(Scenario.VariedTerrains, config, scenarios, expansions[Expansion.Promo2], 2));

            return scenarios;
        }

        public OptionGroup<Spirit> CreateSpirits(OverallConfiguration config)
        {
            if(config.Expansions == null) { throw new SiException("Must create expansions before spirits"); }

            var expansions = config.Expansions;
            var spirits = new OptionGroup<Spirit>()
            {
                Name = OptionGroup<Spirit>.SPIRITS
            };

            // Base
            var lightning = new Spirit(Spirit.Lightning, config, spirits, null, Complexity.Low);
            lightning.Add(new SpiritAspect(SpiritAspect.Wind, config, lightning, expansions[Expansion.JaggedEarth], 1));
            lightning.Add(new SpiritAspect(SpiritAspect.Pandemonium, config, lightning, expansions[Expansion.JaggedEarth], 1));
            lightning.Add(new SpiritAspect(SpiritAspect.Immense, config, lightning, expansions[Expansion.Promo2], 1));
            spirits.Add(lightning);
            var shadows = new Spirit(Spirit.Shadows, config, spirits, null, Complexity.Low);
            shadows.Add(new SpiritAspect(SpiritAspect.Madness, config, shadows, expansions[Expansion.JaggedEarth], 1));
            shadows.Add(new SpiritAspect(SpiritAspect.Reach, config, shadows, expansions[Expansion.JaggedEarth], -1));
            shadows.Add(new SpiritAspect(SpiritAspect.Amorphous, config, shadows, expansions[Expansion.Promo2], 1));
            shadows.Add(new SpiritAspect(SpiritAspect.Foreboding, config, shadows, expansions[Expansion.Promo2], 1));
            spirits.Add(shadows);
            var earth = new Spirit(Spirit.Earth, config, spirits, null, Complexity.Low);
            earth.Add(new SpiritAspect(SpiritAspect.Resilience, config, earth, expansions[Expansion.JaggedEarth], 0));
            earth.Add(new SpiritAspect(SpiritAspect.Might, config, earth, expansions[Expansion.Promo2], 1));
            spirits.Add(earth);
            var river = new Spirit(Spirit.River, config, spirits, null, Complexity.Low);
            river.Add(new SpiritAspect(SpiritAspect.Sunshine, config, river, expansions[Expansion.JaggedEarth], 1));
            river.Add(new SpiritAspect(SpiritAspect.Travel, config, river, expansions[Expansion.Promo2], 1));
            spirits.Add(river);
            spirits.Add(new Spirit(Spirit.Thunderspeaker, config, spirits, null, Complexity.Moderate));
            spirits.Add(new Spirit(Spirit.Green, config, spirits, null, Complexity.Moderate));
            spirits.Add(new Spirit(Spirit.Ocean, config, spirits, null, Complexity.High));
            spirits.Add(new Spirit(Spirit.Bringer, config, spirits, null, Complexity.High));
            // Branch and claw
            spirits.Add(new Spirit(Spirit.Keeper, config, spirits, expansions[Expansion.BranchAndClaw], Complexity.Moderate));
            spirits.Add(new Spirit(Spirit.Fangs, config, spirits, expansions[Expansion.BranchAndClaw], Complexity.Moderate));
            // Promo 1
            spirits.Add(new Spirit(Spirit.Wildfire, config, spirits, expansions[Expansion.Promo1], Complexity.High));
            spirits.Add(new Spirit(Spirit.Snek, config, spirits, expansions[Expansion.Promo1], Complexity.High));
            // Promo 2
            spirits.Add(new Spirit(Spirit.Downpour, config, spirits, expansions[Expansion.Promo2], Complexity.High));
            spirits.Add(new Spirit(Spirit.Finder, config, spirits, expansions[Expansion.Promo2], Complexity.VeryHigh));
            // Jagged Earth
            spirits.Add(new Spirit(Spirit.Volcano, config, spirits, expansions[Expansion.JaggedEarth], Complexity.Moderate));
            spirits.Add(new Spirit(Spirit.Lure, config, spirits, expansions[Expansion.JaggedEarth], Complexity.Moderate));
            spirits.Add(new Spirit(Spirit.ManyMinds, config, spirits, expansions[Expansion.JaggedEarth], Complexity.Moderate));
            spirits.Add(new Spirit(Spirit.Memory, config, spirits, expansions[Expansion.JaggedEarth], Complexity.Moderate));
            spirits.Add(new Spirit(Spirit.Stone, config, spirits, expansions[Expansion.JaggedEarth], Complexity.Moderate));
            spirits.Add(new Spirit(Spirit.Trickster, config, spirits, expansions[Expansion.JaggedEarth], Complexity.Moderate));
            spirits.Add(new Spirit(Spirit.Vengeance, config, spirits, expansions[Expansion.JaggedEarth], Complexity.High));
            spirits.Add(new Spirit(Spirit.Mist, config, spirits, expansions[Expansion.JaggedEarth], Complexity.High));
            spirits.Add(new Spirit(Spirit.Starlight, config, spirits, expansions[Expansion.JaggedEarth], Complexity.VeryHigh));
            spirits.Add(new Spirit(Spirit.Fractured, config, spirits, expansions[Expansion.JaggedEarth], Complexity.VeryHigh));
            // Apocrypha
            spirits.Add(new Spirit(Spirit.Rot, config, spirits, expansions[Expansion.Apocrypha], Complexity.High));
            // Horizons
            spirits.Add(new Spirit(Spirit.Teeth, config, spirits, expansions[Expansion.Horizons], Complexity.Low));
            spirits.Add(new Spirit(Spirit.Whirlwind, config, spirits, expansions[Expansion.Horizons], Complexity.Low));
            spirits.Add(new Spirit(Spirit.Heat, config, spirits, expansions[Expansion.Horizons], Complexity.Low));
            spirits.Add(new Spirit(Spirit.Swamp, config, spirits, expansions[Expansion.Horizons], Complexity.Low));
            spirits.Add(new Spirit(Spirit.Eyes, config, spirits, expansions[Expansion.Horizons], Complexity.Low));
            
            return spirits;
        }

        public OptionGroup<Adversary> CreateAdversaries(OverallConfiguration config)
        {
            if(config.Expansions == null) { throw new SiException("Must create expansions before adversaries"); }

            var expansions = config.Expansions;
            var adversaries = new OptionGroup<Adversary>()
            {
                Name = OptionGroup<Adversary>.ADVERSARIES
            };

            var adversary = new Adversary(Adversary.NoAdversary, config, adversaries, null);
            adversary.Add(new AdversaryLevel("Level 0", config, adversary, 0, 0));
            adversaries.Add(adversary);
            adversary = new Adversary(Adversary.England, config, adversaries, null);
            AddAdversaryLevels(adversary, new int[] { 1, 3, 4, 6, 7, 9, 11 }, config);
            adversaries.Add(adversary);
            adversary = new Adversary(Adversary.BrandenburgPrussia, config, adversaries, null);
            AddAdversaryLevels(adversary, new int[] { 1, 2, 4, 6, 7, 9, 10 }, config);
            adversaries.Add(adversary);
            adversary = new Adversary(Adversary.Sweden, config, adversaries, null);
            AddAdversaryLevels(adversary, new int[] { 1, 2, 3, 5, 6, 7, 8 }, config);
            adversaries.Add(adversary);
            adversary = new Adversary(Adversary.France, config, adversaries, expansions[Expansion.BranchAndClaw]);
            AddAdversaryLevels(adversary, new int[] { 2, 3, 5, 7, 8, 9, 10 }, config);
            adversaries.Add(adversary);
            adversary = new Adversary(Adversary.Habsburg, config, adversaries, expansions[Expansion.JaggedEarth]);
            AddAdversaryLevels(adversary, new int[] { 2, 3, 5, 6, 8, 9, 10 }, config);
            adversaries.Add(adversary);
            adversary = new Adversary(Adversary.Russia, config, adversaries, expansions[Expansion.JaggedEarth]);
            AddAdversaryLevels(adversary, new int[] { 1, 3, 4, 6, 7, 9, 11 }, config);
            adversaries.Add(adversary);
            adversary = new Adversary(Adversary.Scotland, config, adversaries, expansions[Expansion.Promo2]);
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
                adversary.Add(new AdversaryLevel($"Level {level}", config, adversary, level, difficulties[level]));
            }        
        }

        public OptionGroup<Map> CreateMaps(OverallConfiguration config)
        {
            if(config.Expansions == null) { throw new SiException("Must create expansions before maps"); }

            var expansions = config.Expansions;
            var maps = new OptionGroup<Map>()
            {
                Name = OptionGroup<Map>.MAPS
            };

            maps.Add(new Map(Map.Standard, config, maps, 1, 6, 0));
            maps.Add(new Map(Map.ThematicTokens, config, maps, 1, 6, 1, true));
            maps.Add(new Map(Map.ThematicNoTokens, config, maps, 1, 6, 3, true));
            maps.Add(new Map(Map.Archipelago, config, maps, 2, 6, 1));
            maps.Add(new Map(Map.Fragment, config, maps, 2, 2, 0));
            maps.Add(new Map(Map.OppositeShores, config, maps, 2, 2, 0));
            maps.Add(new Map(Map.Coastline, config, maps, 2, 6, 0));
            maps.Add(new Map(Map.Sunrise, config, maps, 3, 3, 0));
            maps.Add(new Map(Map.Leaf, config, maps, 4, 4, 0));
            maps.Add(new Map(Map.Snake, config, maps, 3, 6, 0));
            maps.Add(new Map(Map.V, config, maps, 5, 5, 0));
            maps.Add(new Map(Map.Snail, config, maps, 5, 5, 0));
            maps.Add(new Map(Map.Peninsula, config, maps, 5, 5, 0));
            maps.Add(new Map(Map.Star, config, maps, 6, 6, 0));
            maps.Add(new Map(Map.Flower, config, maps, 6, 6, 0));
            maps.Add(new Map(Map.Caldera, config, maps, 6, 6, 0));  

            return maps;
        }

        public OptionGroup<Board> CreateBoards(OverallConfiguration config)
        {
            if(config.Expansions == null) { throw new SiException("Must create expansions before boards"); }

            var expansions = config.Expansions;
            var boards = new OptionGroup<Board>()
            {
                Name = OptionGroup<Board>.BOARDS
            };

            var jaggedEarth = expansions[Expansion.JaggedEarth];

            var a = new Board(Board.A, config, boards, null, false);
            var b = new Board(Board.B, config, boards, null, false);
            var c = new Board(Board.C, config, boards, null, false);
            var d = new Board(Board.D, config, boards, null, false);
            var e = new Board(Board.E, config, boards, jaggedEarth, false) 
            {
                ImbalancedWith = new List<Board>() { b }
            };
            var f = new Board(Board.F, config, boards, jaggedEarth, false)
            {
                ImbalancedWith = new List<Board>() { d }
            };


            var ne = new Board(Board.NEast, config, boards, null, true) 
            {
                ThematicDefinitivePlayerCounts = new List<int>() { 1, 3, 4, 5, 6 }
            };
            var nw = new Board(Board.NWest, config, boards, null, true) 
            {
                ThematicDefinitivePlayerCounts = new List<int>() { 4, 5, 6 }
            };
            var east = new Board(Board.East, config, boards, null, true) 
            {
                ThematicDefinitivePlayerCounts = new List<int>() { 2, 3, 4, 5, 6 }
            };
            var west = new Board(Board.West, config, boards, null, true) 
            {
                ThematicDefinitivePlayerCounts = new List<int>() { 2, 3, 4, 5, 6 }
            };
            var se = new Board(Board.SEast, config, boards, jaggedEarth, true) 
            {
                ThematicDefinitivePlayerCounts = new List<int>() { 5, 6 }
            };
            var sw = new Board(Board.SWest, config, boards, jaggedEarth, true) 
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
