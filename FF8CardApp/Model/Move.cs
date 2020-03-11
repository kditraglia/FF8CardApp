using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FF8CardApp.Model
{
    [DebuggerDisplay("{Card.Name} to ({X}, {Y}) with score: {Score}")]
    public class Move
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Card Card { get; set; }
        public int Score { get; set; }
        
    }
}
