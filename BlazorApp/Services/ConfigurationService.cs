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
            
            var jaggedEarth = new Expansion[] { expansions[Expansion.JaggedEarth] };
            var promo2 = new Expansion[] { expansions[Expansion.Promo2] };
            var branchAndClaw = new Expansion[] { expansions[Expansion.BranchAndClaw] };
            var natureIncarnate = new Expansion[] { expansions[Expansion.NatureIncarnate] };

            var noScenario = new Scenario(Scenario.NoScenario, config, scenarios, null, 0);
            noScenario.Selected = true;
            scenarios.Add(noScenario);
            scenarios.Add(new Scenario(Scenario.Blitz, config, scenarios, null, 0));
            scenarios.Add(new Scenario(Scenario.GuardTheIslesHeart, config, scenarios, null, 0));
            scenarios.Add(new Scenario(Scenario.RitualsOfTerror, config, scenarios, null, 3));
            scenarios.Add(new Scenario(Scenario.DahanInsurrection, config, scenarios, null, 4));
            scenarios.Add(new Scenario(Scenario.SecondWave, config, scenarios, branchAndClaw, 1));
            scenarios.Add(new Scenario(Scenario.PowersLongForgotten, config, scenarios, branchAndClaw, 1));
            scenarios.Add(new Scenario(Scenario.WardTheShores, config, scenarios, branchAndClaw, 2));
            scenarios.Add(new Scenario(Scenario.RitualsOfTheDestroyingFlame, config, scenarios, branchAndClaw, 3));
            scenarios.Add(new Scenario(Scenario.ElementalInvocation, config, scenarios, jaggedEarth, 1));
            scenarios.Add(new Scenario(Scenario.DespicableTheft, config, scenarios, jaggedEarth, 2));
            scenarios.Add(new Scenario(Scenario.TheGreatRiver, config, scenarios, jaggedEarth, 3, new List<Map>() { config.Maps[Map.Coastline] }));
            scenarios.Add(new Scenario(Scenario.ADiversityOfSpirits, config, scenarios, promo2, 0));
            scenarios.Add(new Scenario(Scenario.VariedTerrains, config, scenarios, promo2, 2));
            scenarios.Add(new Scenario(Scenario.SurgesOfColonization, config, scenarios, natureIncarnate, 2));
            scenarios.Add(new Scenario(Scenario.SurgesOfColonizationLarger, config, scenarios, natureIncarnate, 7));
            scenarios.Add(new Scenario(Scenario.DestinyUnfolds, config, scenarios, natureIncarnate, -1));

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

            var jaggedEarth = new Expansion[] { expansions[Expansion.JaggedEarth] };
            var promo1 = new Expansion[] { expansions[Expansion.Promo1] };
            var promo2 = new Expansion[] { expansions[Expansion.Promo2] };
            var branchAndClaw = new Expansion[] { expansions[Expansion.BranchAndClaw] };
            var horizons = new Expansion[] { expansions[Expansion.Horizons] };
            var natureIncarnate = new Expansion[] { expansions[Expansion.NatureIncarnate] };

            // Base
            var lightning = new Spirit(Spirit.Lightning, config, spirits, null, Complexity.Low);
            lightning.Add(new SpiritAspect(SpiritAspect.Wind, config, lightning, jaggedEarth, 1));
            lightning.Add(new SpiritAspect(SpiritAspect.Pandemonium, config, lightning, jaggedEarth, 1));
            lightning.Add(new SpiritAspect(SpiritAspect.Immense, config, lightning, promo2, 1));
            lightning.Add(new SpiritAspect(SpiritAspect.Sparking, config, lightning, natureIncarnate, 0));
            spirits.Add(lightning);

            var shadows = new Spirit(Spirit.Shadows, config, spirits, null, Complexity.Low);
            shadows.Add(new SpiritAspect(SpiritAspect.Madness, config, shadows, jaggedEarth, 1));
            shadows.Add(new SpiritAspect(SpiritAspect.Reach, config, shadows, jaggedEarth, -1));
            shadows.Add(new SpiritAspect(SpiritAspect.Amorphous, config, shadows, promo2, 1));
            shadows.Add(new SpiritAspect(SpiritAspect.Foreboding, config, shadows, promo2, 1));
            shadows.Add(new SpiritAspect(SpiritAspect.DarkFire, config, shadows, natureIncarnate, 0));
            spirits.Add(shadows);

            var earth = new Spirit(Spirit.Earth, config, spirits, null, Complexity.Low);
            earth.Add(new SpiritAspect(SpiritAspect.Resilience, config, earth, jaggedEarth, 0));
            earth.Add(new SpiritAspect(SpiritAspect.Might, config, earth, promo2, 1));
            earth.Add(new SpiritAspect(SpiritAspect.Nourishing, config, earth, natureIncarnate, 0));
            spirits.Add(earth);

            var river = new Spirit(Spirit.River, config, spirits, null, Complexity.Low);
            river.Add(new SpiritAspect(SpiritAspect.Sunshine, config, river, jaggedEarth, 1));
            river.Add(new SpiritAspect(SpiritAspect.Travel, config, river, promo2, 1));
            river.Add(new SpiritAspect(SpiritAspect.Haven, config, river, natureIncarnate, 0));
            spirits.Add(river);

            var thunderspeaker = new Spirit(Spirit.Thunderspeaker, config, spirits, null, Complexity.Moderate);
            thunderspeaker.Add(new SpiritAspect(SpiritAspect.Warrior, config, thunderspeaker, natureIncarnate, 0));
            thunderspeaker.Add(new SpiritAspect(SpiritAspect.Tactician, config, thunderspeaker, natureIncarnate, 0));
            spirits.Add(thunderspeaker);

            var green = new Spirit(Spirit.Green, config, spirits, null, Complexity.Moderate);
            green.Add(new SpiritAspect(SpiritAspect.Regrowth, config, green, natureIncarnate, 0));
            green.Add(new SpiritAspect(SpiritAspect.Tangles, config, green, natureIncarnate, 0));
            spirits.Add(green);

            var ocean = new Spirit(Spirit.Ocean, config, spirits, null, Complexity.High);
            ocean.Add(new SpiritAspect(SpiritAspect.Deeps, config, ocean, natureIncarnate, 0));
            spirits.Add(ocean);

            var bringer = new Spirit(Spirit.Bringer, config, spirits, null, Complexity.High);
            bringer.Add(new SpiritAspect(SpiritAspect.Violence, config, bringer, natureIncarnate, 0));
            bringer.Add(new SpiritAspect(SpiritAspect.Enticing, config, bringer, natureIncarnate, 0));
            spirits.Add(bringer);

            // Branch and claw
            var keeper = new Spirit(Spirit.Keeper, config, spirits, branchAndClaw, Complexity.Moderate);
            keeper.Add(new SpiritAspect(SpiritAspect.Hostility, config, keeper, natureIncarnate, 0));
            spirits.Add(keeper);

            var fangs = new Spirit(Spirit.Fangs, config, spirits, branchAndClaw, Complexity.Moderate);
            fangs.Add(new SpiritAspect(SpiritAspect.Encircle, config, fangs, natureIncarnate, 0));
            fangs.Add(new SpiritAspect(SpiritAspect.Unconstrained, config, fangs, natureIncarnate, 0));
            spirits.Add(fangs);

            // Promo 1
            var wildfire = new Spirit(Spirit.Wildfire, config, spirits, promo1, Complexity.High);
            wildfire.Add(new SpiritAspect(SpiritAspect.Transforming, config, wildfire, natureIncarnate, 0));
            spirits.Add(wildfire);

            var snek = new Spirit(Spirit.Snek, config, spirits, promo1, Complexity.High);
            snek.Add(new SpiritAspect(SpiritAspect.Locus, config, snek, natureIncarnate, 0));
            spirits.Add(snek);

            // Promo 2
            spirits.Add(new Spirit(Spirit.Downpour, config, spirits, promo2, Complexity.High));
            spirits.Add(new Spirit(Spirit.Finder, config, spirits, promo2, Complexity.VeryHigh));
            // Jagged Earth
            spirits.Add(new Spirit(Spirit.Volcano, config, spirits, jaggedEarth, Complexity.Moderate));

            var lure = new Spirit(Spirit.Lure, config, spirits, jaggedEarth, Complexity.Moderate);
            lure.Add(new SpiritAspect(SpiritAspect.Stranded, config, lure, natureIncarnate, 0));
            spirits.Add(lure);

            spirits.Add(new Spirit(Spirit.ManyMinds, config, spirits, jaggedEarth, Complexity.Moderate));
            
            var memory = new Spirit(Spirit.Memory, config, spirits, jaggedEarth, Complexity.Moderate);
            memory.Add(new SpiritAspect(SpiritAspect.Mentor, config, memory, natureIncarnate, 0));
            memory.Add(new SpiritAspect(SpiritAspect.Intensify, config, memory, natureIncarnate, 0));
            spirits.Add(memory);

            spirits.Add(new Spirit(Spirit.Stone, config, spirits, jaggedEarth, Complexity.Moderate));
            spirits.Add(new Spirit(Spirit.Trickster, config, spirits, jaggedEarth, Complexity.Moderate));
            spirits.Add(new Spirit(Spirit.Vengeance, config, spirits, jaggedEarth, Complexity.High));

            var mist = new Spirit(Spirit.Mist, config, spirits, jaggedEarth, Complexity.High);
            mist.Add(new SpiritAspect(SpiritAspect.Stranded, config, mist, natureIncarnate, 0));
            spirits.Add(mist);

            spirits.Add(new Spirit(Spirit.Starlight, config, spirits, jaggedEarth, Complexity.VeryHigh));
            spirits.Add(new Spirit(Spirit.Fractured, config, spirits, jaggedEarth, Complexity.VeryHigh));
            // Apocrypha
            spirits.Add(new Spirit(Spirit.Rot, config, spirits, new Expansion[] { expansions[Expansion.Apocrypha] }, Complexity.High));
            // Horizons
            spirits.Add(new Spirit(Spirit.Teeth, config, spirits, horizons, Complexity.Low));
            spirits.Add(new Spirit(Spirit.Whirlwind, config, spirits, horizons, Complexity.Low));
            spirits.Add(new Spirit(Spirit.Heat, config, spirits, horizons, Complexity.Low));
            spirits.Add(new Spirit(Spirit.Swamp, config, spirits, horizons, Complexity.Low));
            spirits.Add(new Spirit(Spirit.Eyes, config, spirits, horizons, Complexity.Low));

            // Nature Incarnate
            spirits.Add(new Spirit(Spirit.Behemoth, config, spirits, natureIncarnate, Complexity.Moderate));
            spirits.Add(new Spirit(Spirit.Roots, config, spirits, natureIncarnate, Complexity.Moderate));
            spirits.Add(new Spirit(Spirit.Vigil, config, spirits, natureIncarnate, Complexity.Moderate));
            spirits.Add(new Spirit(Spirit.Darkness, config, spirits, natureIncarnate, Complexity.High));
            spirits.Add(new Spirit(Spirit.Sun, config, spirits, natureIncarnate, Complexity.High));
            spirits.Add(new Spirit(Spirit.Earthquakes, config, spirits, natureIncarnate, Complexity.VeryHigh));
            spirits.Add(new Spirit(Spirit.Voice, config, spirits, natureIncarnate, Complexity.High));
            spirits.Add(new Spirit(Spirit.WoundedWaters, config, spirits, natureIncarnate, Complexity.High));
            
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
            adversary = new Adversary(Adversary.France, config, adversaries, new Expansion[] { expansions[Expansion.BranchAndClaw] });
            AddAdversaryLevels(adversary, new int[] { 2, 3, 5, 7, 8, 9, 10 }, config);
            adversaries.Add(adversary);
            adversary = new Adversary(Adversary.Habsburg, config, adversaries, new Expansion[] { expansions[Expansion.JaggedEarth] });
            AddAdversaryLevels(adversary, new int[] { 2, 3, 5, 6, 8, 9, 10 }, config);
            adversaries.Add(adversary);
            adversary = new Adversary(Adversary.Russia, config, adversaries, new Expansion[] { expansions[Expansion.JaggedEarth] });
            AddAdversaryLevels(adversary, new int[] { 1, 3, 4, 6, 7, 9, 11 }, config);
            adversaries.Add(adversary);
            adversary = new Adversary(Adversary.Scotland, config, adversaries,new Expansion[] { expansions[Expansion.Promo2] });
            AddAdversaryLevels(adversary, new int[] { 1, 3, 4, 6, 7, 8, 10 }, config);
            adversaries.Add(adversary);
            adversary = new Adversary(Adversary.HapsburgMining, config, adversaries, new Expansion[] { expansions[Expansion.NatureIncarnate] });
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

            var jaggedEarth = new Expansion[] { expansions[Expansion.JaggedEarth] };
            var horizons = new Expansion[] { expansions[Expansion.Horizons] };

            var a = new Board(Board.A, config, boards, null, false);
            var b = new Board(Board.B, config, boards, null, false);
            var c = new Board(Board.C, config, boards, null, false);
            var d = new Board(Board.D, config, boards, null, false);
            var e = new Board(Board.E, config, boards, jaggedEarth.Union(horizons).ToArray(), false) 
            {
                ImbalancedWith = new List<Board>() { b }
            };
            var f = new Board(Board.F, config, boards, jaggedEarth, false)
            {
                ImbalancedWith = new List<Board>() { d }
            };
            var g = new Board(Board.G, config, boards, horizons, false)
            {
                ImbalancedWith = new List<Board>() { c }
            };
            var h = new Board(Board.H, config, boards, horizons, false)
            {
                ImbalancedWith = new List<Board>() { a }
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
            boards.Add(g);
            boards.Add(h);

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
