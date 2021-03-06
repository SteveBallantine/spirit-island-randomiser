using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SiRandomizer.Data
{
    public class SetupResult
    {
        public GameSetup Setup { get; set; }
        public long DifficultyOptionsConsidered { get; set; }
        public long BoardSetupOptionsConsidered { get; set; }
        public bool ShowRandomThematicWarning { get; set; }

        public bool ShowInstructionsAdditionalBoard =>
            Setup.BoardSetups.Any(b => b.SpiritAspect == null);
        public bool ShowInstructionsAdditionalAdversary =>
            Setup.HasSupportingAdversary;
        public bool ShowInstructionsArchipelago =>
            Setup.Map.Name == Map.Archipelago;

        public bool ShowInstructions => 
            ShowInstructionsAdditionalAdversary ||
            ShowInstructionsAdditionalBoard ||
            ShowInstructionsArchipelago;

    }
}
