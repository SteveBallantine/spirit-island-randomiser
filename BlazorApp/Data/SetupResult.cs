using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SiRandomizer.Data
{
    public class SetupResult
    {
        public GameSetup Setup { get; set; }
        public long DifficultyOptionsConsidered { get; set; }
        public long BoardSetupOptionsConsidered { get; set; }
    }
}
