using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace FF8CardApp.Model
{
    [DebuggerDisplay("{Name}")]
    public class Card
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public String ImageURL { get; set; }
        public char N { get; set; }
        public char S { get; set; }
        public char E { get; set; }
        public char W { get; set; }
    }
}
