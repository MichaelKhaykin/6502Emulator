using _6502Emulator.VisualizationClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _6502Emulator
{
    public record MemoryData(int index) : IVisualizeMeAsString
    {
        public string VisualizeMe()
        {
            return $"Memory Address: 0x{index.ToHex()}";
        }
    }
}
