using System;
using System.Collections.Generic;

namespace SiRandomizer.Data
{
    public class Board : SelectableComponentBase<Board>, INamedComponent, IExpansionContent
    {        
        public const string A = "A";
        public const string B = "B";
        public const string C = "C";
        public const string D = "D";
        public const string E = "E";
        public const string F = "F";

        public const string NEast = "NE.";
        public const string NWest = "NW.";
        public const string East = "E.";
        public const string West = "W.";
        public const string SEast = "SE.";
        public const string SWest = "SW.";

        public Expansion Expansion { get; private set; }

        public bool Thematic { get; private set; }

        /// <summary>
        /// Only relevant if the Thematic flag is true.
        /// A list of the player counts for which this board should
        /// be included as part of the 'definitive' thematic map.
        /// For the original 4 boards, these are listed in the 
        /// Spirit Island manual. 
        /// </summary>
        /// <value></value>
        public List<int> ThematicDefinitivePlayerCounts { get; set; }

        /// <summary>
        /// Only relevant if the Thematic flag is true.
        /// A list of the thematic boards that directly neighbour this one.
        /// </summary>
        /// <value></value>
        public List<Board> ThematicNeighbours { get; set; }
        
        public Board(
            string name, 
            Expansion expansion,
            bool thematic,
            bool hide = false) 
            : base(name, hide) 
        { 
            Expansion = expansion;
            Thematic = thematic;
        }
    }
}
