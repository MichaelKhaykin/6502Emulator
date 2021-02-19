using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _6502Emulator
{
    [DebuggerDisplay("{Description}")]
    public class Flag
    {
        public string Description { get; }
        public bool HasValue { get; set; }
        public Flag(string description)
        {
            Description = description;
        }
    }
}
