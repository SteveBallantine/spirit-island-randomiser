using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SiRandomizer.Data
{
    public class GameSetup
    {
        public AdversaryLevel LeadingAdversary { get; set; }
        public AdversaryLevel SupportingAdversary { get; set; }
        public IEnumerable<BoardSetup> BoardSetups { get; set; }
        public Map Map { get; set; }
        public Scenario Scenario { get; set; }
        public int AdditionalBoards { get; set; }
        public bool HasSupportingAdversary
        {
            get 
            {
                return SupportingAdversary != null &&
                    SupportingAdversary.Parent.Name != Adversary.NoAdversary;
            }
        }
        public bool IsValid(int playerCount)
        {
            // Check if the setup is valid.
            // Some maps must have a specific number of boards so check this against
            // player count and number of additional boards
            return Map.ValidForBoardCount(playerCount);
        }

        public int Difficulty {
            get {
                int maxAdversary = HasSupportingAdversary == false ? LeadingAdversary.DifficultyModifier :
                    Math.Max(LeadingAdversary.DifficultyModifier, 
                        SupportingAdversary.DifficultyModifier);
                int minAdversary = HasSupportingAdversary == false ? 0 :
                    Math.Min(LeadingAdversary.DifficultyModifier, 
                        SupportingAdversary.DifficultyModifier);

                // Adversary difficulty is calculated as the higher 
                // difficulty + 50-75% of the lower difficulty.
                int baseDifficulty = maxAdversary +
                    (int)Math.Round(minAdversary * 0.6) +
                    Map.DifficultyModifier +
                    Scenario.DifficultyModifier;

                // One additional board is (very roughly!)
                // +2 difficulty if difficulty is 0
                // +3 difficulty if difficulty is 3
                // +4 difficulty if difficulty is 6
                int additionalBoardDifficulty = 
                    baseDifficulty <= 1 ? 2 : baseDifficulty <= 4 ? 3 : 4;

                return baseDifficulty + 
                    (AdditionalBoards * additionalBoardDifficulty);
            }
        }
    }
}
