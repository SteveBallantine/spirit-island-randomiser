using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SiRandomizer.Data
{
    public class GameSetup
    {
        public bool AccountForCognitiveLoad { get; set; }
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

        /// <summary>
        /// The true difficulty, as determined by the Spirit Island rules.
        /// </summary>
        /// <value></value>
        public int Difficulty 
        {
            get 
            {
                return BaseDifficulty + 
                    (AdditionalBoards * AdditionalBoardDifficulty);
            }
        }

        /// <summary>
        /// This is the difficulty that is used when deciding if this setup can be 
        /// selected or not, based on the user's min/max difficulty selection.
        /// </summary>
        /// <value></value>
        public double GetComparitiveDifficulty()
        {
            return GetComparitiveDifficulty(this.BoardSetups.Where(b => b.SpiritAspect != null).Count());
        }
        public double GetComparitiveDifficulty(int spiritCount)
        {
            return AccountForCognitiveLoad ?
                Difficulty + GetDifficultyFromSpiritCount(spiritCount) + GetDifficultyFromSpiritComplexity():
                Difficulty;
        }

        private double GetDifficultyFromSpiritCount(int spiritCount)
        {
            return Math.Pow(spiritCount, 2)/6-(double)spiritCount/6;
        }

        private double GetDifficultyFromSpiritComplexity()
        {
            // GameSetup instances are initially created without board setups.
            // In this case, just return 0 (i.e. the same as all spirits being moderate complexity)
            return this.BoardSetups == null ? 0 :
                // TotalComplexity will be a number from 1 (Low Complexity) to 4 (Very High Complexity) 
                // We want to treat moderate complexity as the base line, so subtract 2.
                // Then multiply by 1/3 to get the difficulty adjustement for each spirit.
                this.BoardSetups.Where(b => b.SpiritAspect != null)
                    .Sum(b => (b.SpiritAspect.TotalComplexity - 2) * 0.33);
        }

        public bool HasMapImage =>
            BoardSetups.Count() > 1 &&
            Map.Thematic == false &&
            Map.Name != Map.Archipelago;

        public string MapImageFileName => $"{(BoardSetups.Count())}-{Map.Name.ToLower().Replace(" ", "-")}.png";
    }
}
