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
            expansions.Add(new Expansion(Expansion.NatureIncarnate, config, expansions, "NI"));
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
            scenarios.Add(new Scenario(Scenario.PowersLongForgotten, config, scenarios, expansions[Expansion.BranchAndClaw], 1));
            scenarios.Add(new Scenario(Scenario.WardTheShores, config, scenarios, expansions[Expansion.BranchAndClaw], 2));
            scenarios.Add(new Scenario(Scenario.RitualsOfTheDestroyingFlame, config, scenarios, expansions[Expansion.BranchAndClaw], 3));
            scenarios.Add(new Scenario(Scenario.ElementalInvocation, config, scenarios, expansions[Expansion.JaggedEarth], 1));
            scenarios.Add(new Scenario(Scenario.DespicableTheft, config, scenarios, expansions[Expansion.JaggedEarth], 2));
            scenarios.Add(new Scenario(Scenario.TheGreatRiver, config, scenarios, expansions[Expansion.JaggedEarth], 3, new List<Map>() { config.Maps[Map.Coastline] }));
            scenarios.Add(new Scenario(Scenario.ADiversityOfSpirits, config, scenarios, expansions[Expansion.Promo2], 0));
            scenarios.Add(new Scenario(Scenario.VariedTerrains, config, scenarios, expansions[Expansion.Promo2], 2));
            scenarios.Add(new Scenario(Scenario.SurgesOfColonization, config, scenarios, expansions[Expansion.NatureIncarnate], 2));
            scenarios.Add(new Scenario(Scenario.SurgesOfColonizationLarger, config, scenarios, expansions[Expansion.NatureIncarnate], 7));
            scenarios.Add(new Scenario(Scenario.DestinyUnfolds, config, scenarios, expansions[Expansion.NatureIncarnate], -1));

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
            lightning.Add(new SpiritAspect(SpiritAspect.Sparking, config, lightning, expansions[Expansion.NatureIncarnate], 0));
            spirits.Add(lightning);

            var shadows = new Spirit(Spirit.Shadows, config, spirits, null, Complexity.Low);
            shadows.Add(new SpiritAspect(SpiritAspect.Madness, config, shadows, expansions[Expansion.JaggedEarth], 1));
            shadows.Add(new SpiritAspect(SpiritAspect.Reach, config, shadows, expansions[Expansion.JaggedEarth], -1));
            shadows.Add(new SpiritAspect(SpiritAspect.Amorphous, config, shadows, expansions[Expansion.Promo2], 1));
            shadows.Add(new SpiritAspect(SpiritAspect.Foreboding, config, shadows, expansions[Expansion.Promo2], 1));
            shadows.Add(new SpiritAspect(SpiritAspect.DarkFire, config, shadows, expansions[Expansion.NatureIncarnate], 0));
            spirits.Add(shadows);

            var earth = new Spirit(Spirit.Earth, config, spirits, null, Complexity.Low);
            earth.Add(new SpiritAspect(SpiritAspect.Resilience, config, earth, expansions[Expansion.JaggedEarth], 0));
            earth.Add(new SpiritAspect(SpiritAspect.Might, config, earth, expansions[Expansion.Promo2], 1));
            earth.Add(new SpiritAspect(SpiritAspect.Nourishing, config, earth, expansions[Expansion.NatureIncarnate], 0));
            spirits.Add(earth);

            var river = new Spirit(Spirit.River, config, spirits, null, Complexity.Low);
            river.Add(new SpiritAspect(SpiritAspect.Sunshine, config, river, expansions[Expansion.JaggedEarth], 1));
            river.Add(new SpiritAspect(SpiritAspect.Travel, config, river, expansions[Expansion.Promo2], 1));
            river.Add(new SpiritAspect(SpiritAspect.Haven, config, river, expansions[Expansion.NatureIncarnate], 0));
            spirits.Add(river);

            var thunderspeaker = new Spirit(Spirit.Thunderspeaker, config, spirits, null, Complexity.Moderate);
            thunderspeaker.Add(new SpiritAspect(SpiritAspect.Warrior, config, thunderspeaker, expansions[Expansion.NatureIncarnate], 0));
            thunderspeaker.Add(new SpiritAspect(SpiritAspect.Tactician, config, thunderspeaker, expansions[Expansion.NatureIncarnate], 0));
            spirits.Add(thunderspeaker);

            var green = new Spirit(Spirit.Green, config, spirits, null, Complexity.Moderate);
            green.Add(new SpiritAspect(SpiritAspect.Regrowth, config, green, expansions[Expansion.NatureIncarnate], 0));
            green.Add(new SpiritAspect(SpiritAspect.Tangles, config, green, expansions[Expansion.NatureIncarnate], 0));
            spirits.Add(green);

            var ocean = new Spirit(Spirit.Ocean, config, spirits, null, Complexity.High);
            ocean.Add(new SpiritAspect(SpiritAspect.Deeps, config, ocean, expansions[Expansion.NatureIncarnate], 0));
            spirits.Add(ocean);

            var bringer = new Spirit(Spirit.Bringer, config, spirits, null, Complexity.High);
            bringer.Add(new SpiritAspect(SpiritAspect.Violence, config, bringer, expansions[Expansion.NatureIncarnate], 0));
            bringer.Add(new SpiritAspect(SpiritAspect.Enticing, config, bringer, expansions[Expansion.NatureIncarnate], 0));
            spirits.Add(bringer);

            // Branch and claw
            var keeper = new Spirit(Spirit.Keeper, config, spirits, expansions[Expansion.BranchAndClaw], Complexity.Moderate);
            keeper.Add(new SpiritAspect(SpiritAspect.Hostility, config, keeper, expansions[Expansion.NatureIncarnate], 0));
            spirits.Add(keeper);

            var fangs = new Spirit(Spirit.Fangs, config, spirits, expansions[Expansion.BranchAndClaw], Complexity.Moderate);
            fangs.Add(new SpiritAspect(SpiritAspect.Encircle, config, fangs, expansions[Expansion.NatureIncarnate], 0));
            fangs.Add(new SpiritAspect(SpiritAspect.Unconstrained, config, fangs, expansions[Expansion.NatureIncarnate], 0));
            spirits.Add(fangs);

            // Promo 1
            var wildfire = new Spirit(Spirit.Wildfire, config, spirits, expansions[Expansion.Promo1], Complexity.High);
            wildfire.Add(new SpiritAspect(SpiritAspect.Transforming, config, wildfire, expansions[Expansion.NatureIncarnate], 0));
            spirits.Add(wildfire);

            var snek = new Spirit(Spirit.Snek, config, spirits, expansions[Expansion.Promo1], Complexity.High);
            snek.Add(new SpiritAspect(SpiritAspect.Locus, config, snek, expansions[Expansion.NatureIncarnate], 0));
            spirits.Add(snek);

            // Promo 2
            spirits.Add(new Spirit(Spirit.Downpour, config, spirits, expansions[Expansion.Promo2], Complexity.High));
            spirits.Add(new Spirit(Spirit.Finder, config, spirits, expansions[Expansion.Promo2], Complexity.VeryHigh));
            // Jagged Earth
            spirits.Add(new Spirit(Spirit.Volcano, config, spirits, expansions[Expansion.JaggedEarth], Complexity.Moderate));

            var lure = new Spirit(Spirit.Lure, config, spirits, expansions[Expansion.JaggedEarth], Complexity.Moderate);
            lure.Add(new SpiritAspect(SpiritAspect.Stranded, config, lure, expansions[Expansion.NatureIncarnate], 0));
            spirits.Add(lure);

            spirits.Add(new Spirit(Spirit.ManyMinds, config, spirits, expansions[Expansion.JaggedEarth], Complexity.Moderate));
            
            var memory = new Spirit(Spirit.Memory, config, spirits, expansions[Expansion.JaggedEarth], Complexity.Moderate);
            memory.Add(new SpiritAspect(SpiritAspect.Mentor, config, memory, expansions[Expansion.NatureIncarnate], 0));
            memory.Add(new SpiritAspect(SpiritAspect.Intensify, config, memory, expansions[Expansion.NatureIncarnate], 0));
            spirits.Add(memory);

            spirits.Add(new Spirit(Spirit.Stone, config, spirits, expansions[Expansion.JaggedEarth], Complexity.Moderate));
            spirits.Add(new Spirit(Spirit.Trickster, config, spirits, expansions[Expansion.JaggedEarth], Complexity.Moderate));
            spirits.Add(new Spirit(Spirit.Vengeance, config, spirits, expansions[Expansion.JaggedEarth], Complexity.High));

            var mist = new Spirit(Spirit.Mist, config, spirits, expansions[Expansion.JaggedEarth], Complexity.High);
            mist.Add(new SpiritAspect(SpiritAspect.Stranded, config, mist, expansions[Expansion.NatureIncarnate], 0));
            spirits.Add(mist);

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

            // Nature Incarnate
            spirits.Add(new Spirit(Spirit.Behemoth, config, spirits, expansions[Expansion.NatureIncarnate], Complexity.Moderate));
            spirits.Add(new Spirit(Spirit.Roots, config, spirits, expansions[Expansion.NatureIncarnate], Complexity.Moderate));
            spirits.Add(new Spirit(Spirit.Vigil, config, spirits, expansions[Expansion.NatureIncarnate], Complexity.Moderate));
            spirits.Add(new Spirit(Spirit.Darkness, config, spirits, expansions[Expansion.NatureIncarnate], Complexity.High));
            spirits.Add(new Spirit(Spirit.Sun, config, spirits, expansions[Expansion.NatureIncarnate], Complexity.High));
            spirits.Add(new Spirit(Spirit.Earthquakes, config, spirits, expansions[Expansion.NatureIncarnate], Complexity.VeryHigh));
            spirits.Add(new Spirit(Spirit.Voice, config, spirits, expansions[Expansion.NatureIncarnate], Complexity.High));
            spirits.Add(new Spirit(Spirit.WoundedWaters, config, spirits, expansions[Expansion.NatureIncarnate], Complexity.High));
            
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
            adversary = new Adversary(Adversary.HapsburgMining, config, adversaries, expansions[Expansion.NatureIncarnate]);
            AddAdversaryLevels(adversary, new int[] { 1, 3, 4, 5, 7, 9, 10 }, config);
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
