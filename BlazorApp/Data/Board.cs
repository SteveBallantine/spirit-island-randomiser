using System;
using System.Collections.Generic;

namespace SiRandomizer.Data
{
    public class Board : SelectableComponentBase<Board>, INamedComponent, IExpansionContent
    {
        public static Board A = new Board() { Name = "A" };
        public static Board B = new Board() { Name = "B" };
        public static Board C = new Board() { Name = "C" };
        public static Board D = new Board() { Name = "D" };
        public static Board E = new Board() { Name = "E", Expansion = Expansion.JaggedEarth };
        public static Board F = new Board() { Name = "F", Expansion = Expansion.JaggedEarth };
        public static Board NEast = new Board() { Name = "NE.", Thematic = true, Hide = true,
            ThematicDefinitivePlayerCounts = new List<int>() { 1, 3, 4, 5, 6 }, ThematicNeighbours = new List<Board>() { NWest, East } };
        public static Board NWest = new Board() { Name = "NW.", Thematic = true, Hide = true, 
            ThematicDefinitivePlayerCounts = new List<int>() { 4, 5, 6 }, ThematicNeighbours = new List<Board>() { NEast, West }};
        public static Board East = new Board() { Name = "E.", Thematic = true, Hide = true, 
            ThematicDefinitivePlayerCounts = new List<int>() { 2, 3, 4, 5, 6 }, ThematicNeighbours = new List<Board>() { NEast, West, SEast } };
        public static Board West = new Board() { Name = "W.", Thematic = true, Hide = true, 
            ThematicDefinitivePlayerCounts = new List<int>() { 2, 3, 4, 5, 6 }, ThematicNeighbours = new List<Board>() { NWest, East, SWest } };
        public static Board SEast = new Board() { Name = "SE.", Thematic = true, Hide = true, Expansion = Expansion.JaggedEarth,
            ThematicDefinitivePlayerCounts = new List<int>() { 5, 6 }, ThematicNeighbours = new List<Board>() { SWest, East } };
        public static Board SWest = new Board() { Name = "SW.", Thematic = true, Hide = true, Expansion = Expansion.JaggedEarth,
            ThematicDefinitivePlayerCounts = new List<int>() { 6 }, ThematicNeighbours = new List<Board>() { SEast, West } };

        public Expansion Expansion {get; set;}

        private Board(){} 

        public bool Thematic { get; set; }

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
    }
}
