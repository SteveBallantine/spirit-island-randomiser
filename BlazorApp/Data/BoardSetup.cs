using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SiRandomizer.Data
{
    public class BoardSetup
    {
        public Board Board { get; set; }
        public Spirit Spirit { get; set; }
    }
}
