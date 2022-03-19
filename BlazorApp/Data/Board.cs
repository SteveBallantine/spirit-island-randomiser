using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace SiRandomizer.Data
{
    public class Board : SelectableExpansionComponentBase<Board>
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

        [JsonIgnore]
        public bool Thematic { get; private set; }

        /// <summary>
        /// Only relevant if the Thematic flag is true.
        /// A list of the player counts for which this board should
        /// be included as part of the 'definitive' thematic map.
        /// For the original 4 boards, these are listed in the 
        /// Spirit Island manual. 
        /// </summary>
        /// <value></value>
        [JsonIgnore]
        public List<int> ThematicDefinitivePlayerCounts { get; set; }

        /// <summary>
        /// Only relevant if the Thematic flag is true.
        /// A list of the thematic boards that directly neighbour this one.
        /// </summary>
        /// <value></value>
        [JsonIgnore]
        public List<Board> ThematicNeighbours { get; set; }

        /// <summary>
        /// Certain pairs of arcade boards are considered imbalanced when used together 
        /// at low player counts.
        /// This property is a list of the boards that this board is imbalanced with. 
        /// </summary>
        /// <value></value>
        [JsonIgnore]
        public List<Board> ImbalancedWith { get; set; }
        
        public Board() {}

        public Board(
            string name,  
            OverallConfiguration config,
            Expansion expansion,
            bool thematic) 
            : base(name, config, expansion) 
        { 
            Thematic = thematic;
        }

        public override bool IsVisible()
        {
            // Show thematic boards if:
            // 1. Any thematic map is selected.
            // 2. The 'Random thematic boards' option is not blocked.
            var showThematic = Config.Maps.Any(m => m.Selected && m.Thematic) && 
                Config.RandomThematicBoards != OptionChoice.Block;
            // Show arcade boards if any non-thematic map is selected.
            var showArcade = Config.Maps.Any(m => m.Selected && m.Thematic == false);
            // Only show this board if the conditions above are met and the base 
            // conditions are satisfied (i.e. relevant exansion is selected)
            return (Thematic ? showThematic : showArcade) && base.IsVisible();
        }
    }
}
