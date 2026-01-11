using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using SiRandomizer.Data;
using SiRandomizer.Exceptions;

namespace SiRandomizer.Services
{
    /// <summary>
    /// Used to launch the handelabra app with the current configuration.
    /// See https://www.handelabra.com/spirit-island-launch-url-specification
    /// </summary>
    public class HandelabraLaunchService
    {
        private readonly static Dictionary<string, string> SpiritNameMappings = new Dictionary<string, string>
        {
            { Spirit.Bringer, "BringerOfDreamsAndNightmares" },
            { Spirit.Teeth, "DevouringTeethLurkUnderfoot" },
            { Spirit.Downpour, "DownpourDrenchesTheWorld" },
            { Spirit.Behemoth, "EmberEyedBehemoth" },
            { Spirit.Eyes, "EyesWatchFromTheTrees" },
            { Spirit.Swamp, "FathomlessMudOfTheSwamp" },
            { Spirit.Finder, "FinderOfPathsUnseen" },
            { Spirit.Fractured, "FracturedDaysSplitTheSky" },
            { Spirit.Trickster, "GrinningTricksterStirsUpTrouble" },
            { Spirit.Wildfire, "HeartOfTheWildfire" },
            { Spirit.Keeper, "KeeperOfTheForbiddenWilds" },
            { Spirit.Lightning, "LightningsSwiftStrike" },
            { Spirit.Lure, "LureOfTheDeepWilderness" },
            { Spirit.ManyMinds, "ManyMindsMoveAsOne" },
            { Spirit.Ocean, "OceansHungryGrasp" },
            { Spirit.Heat, "RisingHeatOfStoneAndSand" },
            { Spirit.River, "RiverSurgesInSunlight" },
            { Spirit.Snek, "SerpentSlumberingBeneathTheIsland" },
            { Spirit.Shadows, "ShadowsFlickerLikeFlame" },
            { Spirit.Fangs, "SharpFangsBehindTheLeaves" },
            { Spirit.Memory, "ShiftingMemoryOfAges" },
            { Spirit.Mist, "ShroudOfSilentMist" },
            { Spirit.Green, "ASpreadOfRampantGreen" },
            { Spirit.Starlight, "StarlightSeeksItsForm" },
            { Spirit.Stone, "StonesUnyieldingDefiance" },
            { Spirit.Whirlwind, "SunBrightWhirlwind" },
            { Spirit.Thunderspeaker, "Thunderspeaker" },
            { Spirit.Vengeance, "VengeanceAsABurningPlague" },
            { Spirit.Earth, "VitalStrengthOfTheEarth" },
            { Spirit.Volcano, "VolcanoLoomingHigh" },
            { Spirit.Voice, "WanderingVoiceKeensDelirium" }
        };

        private readonly static Dictionary<string, string> SpiritAspectNameMappings = new Dictionary<string, string>
        {
            { SpiritAspect.Violence, "Violence" },
            { SpiritAspect.Hostility, "SpreadingHostility" },
            { SpiritAspect.Pandemonium, "Pandemonium" },
            { SpiritAspect.Wind, "Wind" },
            { SpiritAspect.Immense, "Immense" },
            { SpiritAspect.Sunshine, "Sunshine" },
            { SpiritAspect.Travel, "Travel" },
            { SpiritAspect.Madness, "Madness" },
            { SpiritAspect.Reach, "Reach" },
            { SpiritAspect.Amorphous, "Amorphous" },
            { SpiritAspect.Foreboding, "Foreboding" },
            { SpiritAspect.DarkFire, "DarkFire" },
            { SpiritAspect.Unconstrained, "Unconstrained" },
            { SpiritAspect.Tactician, "Tactician" },
            { SpiritAspect.Resilience, "Resilience" },
            { SpiritAspect.Might, "Might" },
        };

        private readonly static Dictionary<string, string> AdversaryNameMappings = new Dictionary<string, string>
        {
            { Adversary.BrandenburgPrussia, "TheKingdomOfBrandenburgPrussia" },
            { Adversary.England, "TheKingdomOfEngland" },
            { Adversary.Sweden, "TheKingdomOfSweden" },
            { Adversary.France, "TheKingdomOfFrance" },
            { Adversary.Habsburg, "TheHabsburgMonarchy" },
            { Adversary.Russia, "TheTsardomOfRussia" },
            { Adversary.Scotland, "TheKingdomOfScotland" },
        };

        private readonly static Dictionary<string, string> ScenarioNameMappings = new Dictionary<string, string>
        {
            { Scenario.Blitz, "Blitz" },
            { Scenario.DahanInsurrection, "DahanInsurrection" },
            { Scenario.GuardTheIslesHeart, "GuardTheIslesHeart" },
            { Scenario.RitualsOfTerror, "RitualsOfTerror" },
            { Scenario.PowersLongForgotten, "PowersLongForgotten" },
            { Scenario.RitualsOfTheDestroyingFlame, "RitualsOfTheDestroyingFlame" },
            { Scenario.SecondWave, "SecondWave" },
            { Scenario.WardTheShores, "WardTheShores" },
            { Scenario.DespicableTheft, "DespicableTheft" },
            { Scenario.ElementalInvocation, "ElementalInvocation" },
            { Scenario.TheGreatRiver, "TheGreatRiver" },
            { Scenario.ADiversityOfSpirits, "ADiversityOfSpirits" },
            { Scenario.VariedTerrains, "VariedTerrains" },
        };

        private readonly static Dictionary<string, string> BoardNameMappings = new Dictionary<string, string>
        {
            { Board.A, "A" },
            { Board.B, "B" },
            { Board.C, "C" },
            { Board.D, "D" },
            { Board.E, "E" },
            { Board.F, "F" },
            { Board.G, "G" },
            { Board.H, "H" },
            { Board.East, "East" },
            { Board.West, "West" },
            { Board.NEast, "NorthEast" },
            { Board.NWest, "NorthWest" },
            { Board.SEast, "SouthEast" },
            { Board.SWest, "SouthWest" }            
        };

        private ILogger<HandelabraLaunchService> _logger;

        public HandelabraLaunchService(ILogger<HandelabraLaunchService> logger)
        {
            _logger = logger;
        }

        public string BuildLaunchUrl(GameSetup setup)
        {            
            _logger.LogDebug("Generating handelabra url..");
            var unsupported = GetUnsupportedSetupItems(setup);
            if(unsupported.Any())
            {
                var ex = new SiException("One or more setup options not supported by Handelaba app. Call GetUnsupportedSetupItems for more details.");
                throw ex;
            }

            StringBuilder result = new StringBuilder("http://play.spiritislanddigital.com/screen/NewGame?");
            
            result.Append($"spirits={string.Join(",", setup.BoardSetups.Where(s => s.SpiritAspect != null).Select(s => SpiritNameMappings[s.SpiritAspect.Parent.Name]))}");
            result.Append($"&boards={string.Join(",", setup.BoardSetups.Select(s => BoardNameMappings[s.Board.Name]))}");
            result.Append($"&layout={BuildLayoutString(setup)}");
            if(setup.LeadingAdversary.Parent.Name != Adversary.NoAdversary) 
            {
                result.Append($"&adversary={AdversaryNameMappings[setup.LeadingAdversary.Parent.Name]}");
            }
            if(setup.LeadingAdversary.Level > 0)
            {
                result.Append($"&adversaryLevel={setup.LeadingAdversary.Level}");
            }
            if(setup.LeadingAdversary.Parent.Name != Adversary.NoAdversary) 
            {
                result.Append($"&supportingAdversary={AdversaryNameMappings[setup.SupportingAdversary.Parent.Name]}");
            }
            if(setup.SupportingAdversary.Level > 0)
            {
                result.Append($"&supportingAdversaryLevel={setup.SupportingAdversary.Level}");
            }
            if(setup.Scenario.Name != Scenario.NoScenario)
            {
                result.Append($"&scenario={ScenarioNameMappings[setup.Scenario.Name]}");
            }
            result.Append($"&useExpansions={BuildExpansionString(setup)}");
            result.Append($"&useTokens={(setup.Map.Name == Map.ThematicNoTokens ? "0" : "1")}");
            result.Append($"&useEvents=1");
            result.Append($"&aspects={string.Join(",", setup.BoardSetups.Where(s => s.SpiritAspect != null && s.SpiritAspect.Name != SpiritAspect.Base).Select(s => SpiritAspectNameMappings[s.SpiritAspect.Name]))}");
            _logger.LogInformation($"Handelabra url: {result}");

            return result.ToString();
        }

        public IReadOnlyList<string> GetUnsupportedSetupItems(GameSetup setup)
        {
            var unsupportedItems = new List<string>();

            if(setup.BoardSetups.Count() > 4)
            {
                unsupportedItems.Add("More than 4 boards/spirits");
            }

            var unsupportedSpirits = setup.BoardSetups.Where(s => s.SpiritAspect != null && !SpiritNameMappings.ContainsKey(s.SpiritAspect.Parent.Name)).ToList();
            unsupportedSpirits.ForEach(s => unsupportedItems.Add($"Spirit - {s.SpiritAspect.Parent.Name}"));

            var unsupportedAspects = setup.BoardSetups.Where(s => s.SpiritAspect != null && 
                s.SpiritAspect.Name != SpiritAspect.Base && 
                !SpiritAspectNameMappings.ContainsKey(s.SpiritAspect.Name)).ToList();
            unsupportedAspects.ForEach(a => unsupportedItems.Add($"Aspect - {a.SpiritAspect.Name}"));

            if(setup.LeadingAdversary.Parent.Name != Adversary.NoAdversary &&
                !AdversaryNameMappings.ContainsKey(setup.LeadingAdversary.Parent.Name))
            {
                unsupportedItems.Add($"Adversary - {setup.LeadingAdversary.Parent.Name}");
            }

            if(setup.SupportingAdversary.Parent.Name != Adversary.NoAdversary &&
                !AdversaryNameMappings.ContainsKey(setup.SupportingAdversary.Parent.Name))
            {
                unsupportedItems.Add($"Adversary - {setup.SupportingAdversary.Parent.Name}");
            }

            var unsupportedBoards = setup.BoardSetups.Where(s => !BoardNameMappings.ContainsKey(s.Board.Name)).ToList();            
            unsupportedBoards.ForEach(s => unsupportedItems.Add($"Board - {s.Board.Name}"));

            if(setup.Scenario.Name != Scenario.NoScenario &&
                !ScenarioNameMappings.ContainsKey(setup.Scenario.Name))
            {
                unsupportedItems.Add($"Scenario - {setup.Scenario.Name}");
            }


            return unsupportedItems;
        }

        private string BuildLayoutString(GameSetup setup)
        {
            var boards = setup.BoardSetups.ToArray();
            var boardCount = boards.Count();
            if(boardCount == 1) return "";

            if(setup.Map.Name == Map.Standard)
            {
                switch (boardCount)
                {
                    case 2:
                        return $"{boards[0].Board.Name}2:{boards[1].Board.Name}2";
                    case 3:
                        return $"{boards[0].Board.Name}1:{boards[1].Board.Name}2," +
                            $"{boards[1].Board.Name}1:{boards[2].Board.Name}2," +
                            $"{boards[2].Board.Name}1:{boards[0].Board.Name}2";
                    case 4:
                        return $"{boards[0].Board.Name}1:{boards[1].Board.Name}1," +
                            $"{boards[1].Board.Name}0:{boards[2].Board.Name}2," +
                            $"{boards[2].Board.Name}1:{boards[3].Board.Name}1," +
                            $"{boards[3].Board.Name}0:{boards[0].Board.Name}2";
                    default:
                        return "";
                }
            }
            else if(setup.Map.Name == Map.Coastline)
            {
                switch (boardCount)
                {
                    case 2:
                        return $"{boards[0].Board.Name}0:{boards[1].Board.Name}2";
                    case 3:
                        return $"{boards[0].Board.Name}0:{boards[1].Board.Name}2," +
                            $"{boards[1].Board.Name}0:{boards[2].Board.Name}2";
                    case 4:
                        return $"{boards[0].Board.Name}0:{boards[1].Board.Name}2," +
                            $"{boards[1].Board.Name}0:{boards[2].Board.Name}2," +
                            $"{boards[2].Board.Name}0:{boards[3].Board.Name}2";
                    default:
                        return "";
                }
            }
            else if(setup.Map.Name == Map.Snake)
            {
                switch (boardCount)
                {
                    case 3:
                        return $"{boards[0].Board.Name}2:{boards[1].Board.Name}2," +
                            $"{boards[1].Board.Name}1:{boards[2].Board.Name}1";
                    case 4:
                        return $"{boards[0].Board.Name}2:{boards[1].Board.Name}2," +
                            $"{boards[1].Board.Name}1:{boards[2].Board.Name}1," +
                            $"{boards[2].Board.Name}2:{boards[3].Board.Name}2";
                    default:
                        return "";
                }
            }
            else if(setup.Map.Name == Map.Fragment)
            {
                if(boardCount != 2) return "";
                return $"{boards[0].Board.Name}2:{boards[1].Board.Name}1";
            }
            else if(setup.Map.Name == Map.OppositeShores)
            {
                if(boardCount != 2) return "";
                return $"{boards[0].Board.Name}1:{boards[1].Board.Name}1";
            }
            else if(setup.Map.Name == Map.Sunrise)
            {
                if(boardCount != 3) return "";
                return $"{boards[0].Board.Name}2:{boards[1].Board.Name}1," +
                    $"{boards[1].Board.Name}0:{boards[2].Board.Name}1";
            }
            else if(setup.Map.Name == Map.Leaf)
            {
                if(boardCount != 4) return "";
                return $"{boards[0].Board.Name}0:{boards[1].Board.Name}1," +
                    $"{boards[1].Board.Name}0:{boards[2].Board.Name}2," +
                    $"{boards[2].Board.Name}1:{boards[3].Board.Name}2," +
                    $"{boards[3].Board.Name}1:{boards[0].Board.Name}1";
            }
            else if (setup.Map.Name == Map.Archipelago)
            {
                switch (boardCount)
                {
                    case 2:
                        return $"{boards[0].Board.Name}3:{boards[1].Board.Name}3";
                    case 3:
                        return $"{boards[0].Board.Name}3:{boards[1].Board.Name}3," +
                            $"{boards[1].Board.Name}0:{boards[2].Board.Name}2";
                    case 4:
                        return $"{boards[0].Board.Name}3:{boards[1].Board.Name}3," +
                            $"{boards[0].Board.Name}0:{boards[2].Board.Name}2," +
                            $"{boards[2].Board.Name}3:{boards[3].Board.Name}3," +
                            $"{boards[1].Board.Name}2:{boards[3].Board.Name}0";
                    default:
                        return "";
                }
            }

            return "";
        }

        private string BuildExpansionString(GameSetup setup)
        {
            // Just do this for now, it will be ignored if the user does not own it.
            return "BranchAndClaw,JaggedEarth,Horizons,PromoPack1,PromoPack2,NatureIncarnate";
        }

    }

}