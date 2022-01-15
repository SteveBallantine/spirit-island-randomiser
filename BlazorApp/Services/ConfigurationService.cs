using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SiRandomizer.Data;

namespace SiRandomizer.Services
{
    public class ConfigurationService
    {
        public Task<OverallConfiguration> GetConfigurationAsync()
        {
            var expansions = CreateExpansions();
            var config = new OverallConfiguration(
                CreateAdversaries(expansions),                
                CreateBoards(expansions),
                CreateMaps(),
                expansions,
                CreateSpirits(expansions),
                CreateScenarios(expansions)
            );            
            return Task.FromResult(config);
        }

        public OptionGroup<Expansion> CreateExpansions()
        {
            var expansions = new List<Expansion>();
            expansions.Add(new Expansion(Expansion.BranchAndClaw, "BC"));
            expansions.Add(new Expansion(Expansion.JaggedEarth, "JE"));
            expansions.Add(new Expansion(Expansion.Promo1, "P1"));
            expansions.Add(new Expansion(Expansion.Promo2, "P2"));
            expansions.Add(new Expansion(Expansion.Apocrypha, "Ax"));
            expansions.Add(new Expansion(Expansion.Custom, "Cx"));

            return new OptionGroup<Expansion>("Expansions", expansions);
        }

        public OptionGroup<Scenario> CreateScenarios(OptionGroup<Expansion> expansions)
        {
            var scenarios = new List<Scenario>();

            scenarios.Add(new Scenario(Scenario.NoScenario, null, 0));
            scenarios.Add(new Scenario(Scenario.Blitz, null, 0));
            scenarios.Add(new Scenario(Scenario.GuardTheIslesHeart, null, 0));
            scenarios.Add(new Scenario(Scenario.RitualsOfTerror, null, 3));
            scenarios.Add(new Scenario(Scenario.DahanInsurrection, null, 4));
            scenarios.Add(new Scenario(Scenario.SecondWave, expansions[Expansion.BranchAndClaw], 1));
            scenarios.Add(new Scenario(Scenario.PowersLongForgotten, expansions[Expansion.BranchAndClaw], 2));
            scenarios.Add(new Scenario(Scenario.WardTheShores, expansions[Expansion.BranchAndClaw], 3));
            scenarios.Add(new Scenario(Scenario.RitualsOfTheDestroyingFlame, expansions[Expansion.BranchAndClaw], 3));
            scenarios.Add(new Scenario(Scenario.ElementalInvocation, expansions[Expansion.JaggedEarth], 1));
            scenarios.Add(new Scenario(Scenario.DespicableTheft, expansions[Expansion.JaggedEarth], 2));
            scenarios.Add(new Scenario(Scenario.TheGreatRiver, expansions[Expansion.JaggedEarth], 3));
            scenarios.Add(new Scenario(Scenario.ADiversityOfSpirits, expansions[Expansion.Promo2], 0));
            scenarios.Add(new Scenario(Scenario.VariedTerrains, expansions[Expansion.Promo2], 2));

            return new OptionGroup<Scenario>("Scenarios", scenarios);
        }

        public OptionGroup<Spirit> CreateSpirits(OptionGroup<Expansion> expansions)
        {
            var spirits = new List<Spirit>();
            spirits.Add(new Spirit(Spirit.Lightning, null, Complexity.Low));
            spirits.Add(new Spirit(Spirit.Shadows, null, Complexity.Low));
            spirits.Add(new Spirit(Spirit.Earth, null, Complexity.Low));
            spirits.Add(new Spirit(Spirit.River, null, Complexity.Low));
            spirits.Add(new Spirit(Spirit.Thunderspeaker, null, Complexity.Moderate));
            spirits.Add(new Spirit(Spirit.Green, null, Complexity.Moderate));
            spirits.Add(new Spirit(Spirit.Ocean, null, Complexity.High));
            spirits.Add(new Spirit(Spirit.Bringer, null, Complexity.High));
            spirits.Add(new Spirit(Spirit.Keeper, expansions[Expansion.BranchAndClaw], Complexity.Moderate));
            spirits.Add(new Spirit(Spirit.Fangs, expansions[Expansion.BranchAndClaw], Complexity.Moderate));
            spirits.Add(new Spirit(Spirit.Wildfire, expansions[Expansion.Promo1], Complexity.High));
            spirits.Add(new Spirit(Spirit.Volcano, expansions[Expansion.JaggedEarth], Complexity.Moderate));
            spirits.Add(new Spirit(Spirit.Snek, expansions[Expansion.Promo1], Complexity.High));
            spirits.Add(new Spirit(Spirit.Lure, expansions[Expansion.JaggedEarth], Complexity.Moderate));
            spirits.Add(new Spirit(Spirit.ManyMinds, expansions[Expansion.JaggedEarth], Complexity.Moderate));
            spirits.Add(new Spirit(Spirit.Memory, expansions[Expansion.JaggedEarth], Complexity.Moderate));
            spirits.Add(new Spirit(Spirit.Stone, expansions[Expansion.JaggedEarth], Complexity.Moderate));
            spirits.Add(new Spirit(Spirit.Trickster, expansions[Expansion.JaggedEarth], Complexity.Moderate));
            spirits.Add(new Spirit(Spirit.Vengeance, expansions[Expansion.JaggedEarth], Complexity.High));
            spirits.Add(new Spirit(Spirit.Mist, expansions[Expansion.JaggedEarth], Complexity.High));
            spirits.Add(new Spirit(Spirit.Starlight, expansions[Expansion.JaggedEarth], Complexity.VeryHigh));
            spirits.Add(new Spirit(Spirit.Fractured, expansions[Expansion.JaggedEarth], Complexity.VeryHigh));
            spirits.Add(new Spirit(Spirit.Downpour, expansions[Expansion.Promo2], Complexity.High));
            spirits.Add(new Spirit(Spirit.Finder, expansions[Expansion.Promo2], Complexity.VeryHigh));
            return new OptionGroup<Spirit>("Spirits", spirits);
        }

        public OptionGroup<Adversary> CreateAdversaries(OptionGroup<Expansion> expansions)
        {
            var adversaries = new List<Adversary>();

            var adversary = new Adversary(Adversary.NoAdversary, null);
            adversary.AddLevel(new AdversaryLevel("Level 0", 0, 0));
            adversaries.Add(adversary);
            adversary = new Adversary(Adversary.England, null);
            AddAdversaryLevels(adversary, new int[] { 1, 3, 4, 6, 7, 9, 11 });
            adversaries.Add(adversary);
            adversary = new Adversary(Adversary.BrandenburgPrussia, null);
            AddAdversaryLevels(adversary, new int[] { 1, 2, 4, 6, 7, 9, 10 });
            adversaries.Add(adversary);
            adversary = new Adversary(Adversary.Sweden, null);
            AddAdversaryLevels(adversary, new int[] { 1, 2, 3, 5, 6, 7, 8 });
            adversaries.Add(adversary);
            adversary = new Adversary(Adversary.France, expansions[Expansion.BranchAndClaw]);
            AddAdversaryLevels(adversary, new int[] { 2, 3, 5, 7, 8, 9, 10 });
            adversaries.Add(adversary);
            adversary = new Adversary(Adversary.Habsburg, expansions[Expansion.JaggedEarth]);
            AddAdversaryLevels(adversary, new int[] { 2, 3, 5, 6, 8, 9, 10 });
            adversaries.Add(adversary);
            adversary = new Adversary(Adversary.Russia, expansions[Expansion.JaggedEarth]);
            AddAdversaryLevels(adversary, new int[] { 1, 3, 4, 6, 7, 9, 11 });
            adversaries.Add(adversary);
            adversary = new Adversary(Adversary.Scotland, expansions[Expansion.Promo2]);
            AddAdversaryLevels(adversary, new int[] { 1, 3, 4, 6, 7, 8, 10 });
            adversaries.Add(adversary);
            
            return new OptionGroup<Adversary>("Adversaries", adversaries);
        }

        private void AddAdversaryLevels(Adversary adversary,
            int[] difficulties)
        {
            for(int level = 0; level < difficulties.Length; level++)
            {
                adversary.AddLevel(new AdversaryLevel($"Level {level}", level, difficulties[level]));
            }        
        }

        public OptionGroup<Map> CreateMaps()
        {        
            var maps = new List<Map>();

            maps.Add(new Map(Map.Standard, 1, 6, 0));
            maps.Add(new Map("Thematic (+tokens)", 1, 6, 1));
            maps.Add(new Map("Thematic (no tokens)", 1, 6, 3));
            maps.Add(new Map("Archipelago", 2, 6, 1));
            maps.Add(new Map("Fragment", 2, 2, 0));
            maps.Add(new Map("Opposite Shores", 2, 2, 0));
            maps.Add(new Map("Coastline", 2, 6, 0));
            maps.Add(new Map("Sunrise", 3, 3, 0));
            maps.Add(new Map("Leaf", 4, 4, 0));
            maps.Add(new Map("Snake", 3, 6, 0));
            maps.Add(new Map("V", 5, 5, 0));
            maps.Add(new Map("Snail", 5, 5, 0));
            maps.Add(new Map("Peninsula", 5, 5, 0));
            maps.Add(new Map("Star", 6, 6, 0));
            maps.Add(new Map("Flower", 6, 6, 0));
            maps.Add(new Map("Caldera", 6, 6, 0));  

            return new OptionGroup<Map>("Maps", maps);          
        }

        public OptionGroup<Board> CreateBoards(OptionGroup<Expansion> expansions)
        {
            var boards = new List<Board>();
            var jaggedEarth = expansions[Expansion.JaggedEarth];

            var a = new Board(Board.A, null, false);
            var b = new Board(Board.B, null, false);
            var c = new Board(Board.C, null, false);
            var d = new Board(Board.D, null, false);
            var e = new Board(Board.E, jaggedEarth, false);
            var f = new Board(Board.F, jaggedEarth, false);

            var ne = new Board(Board.NEast, null, true, true) 
            {
                ThematicDefinitivePlayerCounts = new List<int>() { 1, 3, 4, 5, 6 }
            };
            var nw = new Board(Board.NWest, null, true, true) 
            {
                ThematicDefinitivePlayerCounts = new List<int>() { 4, 5, 6 }
            };
            var east = new Board(Board.East, null, true, true) 
            {
                ThematicDefinitivePlayerCounts = new List<int>() { 2, 3, 4, 5, 6 }
            };
            var west = new Board(Board.West, null, true, true) 
            {
                ThematicDefinitivePlayerCounts = new List<int>() { 2, 3, 4, 5, 6 }
            };
            var se = new Board(Board.SEast, jaggedEarth, true, true) 
            {
                ThematicDefinitivePlayerCounts = new List<int>() { 5, 6 }
            };
            var sw = new Board(Board.SWest, jaggedEarth, true, true) 
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

            return new OptionGroup<Board>("Boards", boards);
        }

    }
}
