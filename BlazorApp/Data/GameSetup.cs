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
            return Map.ValidForBoardCount(playerCount + AdditionalBoards) &&
                (Scenario.ValidMaps == null || Scenario.ValidMaps.Any(m => m == Map));
        }

        /// <summary>
        /// For a single adversary, we will use the difficulty modifier from the AdversaryLevel.
        /// For multiple adversaries, the lower of the difficulties is multiplied by 0.6.
        /// (the JE manual recommends 0.5-0.75, but we use an exact figure here)
        /// </summary>
        /// <returns></returns>
        public int LeadingAdversaryDifficultyModifier =>
            MaxAdversaryDifficulty == LeadingAdversary.DifficultyModifier ? 
                MaxAdversaryDifficulty : (int)Math.Round(LeadingAdversary.DifficultyModifier * 0.6);

        public int SupportingAdversaryDifficultyModifier =>
            HasSupportingAdversary == false ? 0 :
            (MaxAdversaryDifficulty == LeadingAdversary.DifficultyModifier ? 
                 (int)Math.Round(SupportingAdversary.DifficultyModifier * 0.6) : SupportingAdversary.DifficultyModifier);

        private int MaxAdversaryDifficulty =>
             HasSupportingAdversary == false ? LeadingAdversary.DifficultyModifier :
                    Math.Max(LeadingAdversary.DifficultyModifier, 
                        SupportingAdversary.DifficultyModifier);

        private int MinAdversaryDifficulty =>
            HasSupportingAdversary == false ? 0 :
                    Math.Min(LeadingAdversary.DifficultyModifier, 
                        SupportingAdversary.DifficultyModifier);

        private int BaseDifficulty => 
            LeadingAdversaryDifficultyModifier +
                SupportingAdversaryDifficultyModifier +
                Map.DifficultyModifier +
                Scenario.DifficultyModifier;
        
        // One additional board is (very roughly!)
        // +2 difficulty if difficulty is 0
        // +3 difficulty if difficulty is 3
        // +4 difficulty if difficulty is 6
        public int AdditionalBoardDifficulty =>
            BaseDifficulty <= 1 ? 2 : BaseDifficulty <= 4 ? 3 : 4;

        public int Difficulty 
        {
            get 
            {
                return BaseDifficulty + 
                    (AdditionalBoards * AdditionalBoardDifficulty);
            }
        }
        public bool HasMapImage =>
            BoardSetups.Count() > 1 &&
            Map.Thematic == false &&
            Map.Name != Map.Archipelago;

        public string MapImageFileName => $"{(BoardSetups.Count())}-{Map.Name.ToLower().Replace(" ", "-")}.png";
    }
}
