using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _6502Emulator.VisualizationClasses
{
    public class MappingsHelper
    {
        public Dictionary<int, int> LineIndexToDissassemblyIndex = new Dictionary<int, int>();
        public Dictionary<int, int> DissassemblyIndexToLineIndex = new Dictionary<int, int>();
        public Dictionary<int, int> LineIndexToLength = new Dictionary<int, int>();
    
        public MappingsHelper(string[] code)
        {
            var copy = code;

            code = AssemblyParser.ReplaceDefines(code);
            code = AssemblyParser.RemoveLabels(code);
            code = AssemblyParser.GetRidOfCommentsAndEmptyLines(code);

            var copyOfCopy = copy;

            copy = copy.Select((x) =>
            {
                if (!x.Contains(';')) return x.Trim();

                return x.Substring(0, x.IndexOf(';')).Trim();
            }).ToArray();

            var table = AssemblyParser.GenerateDefineReplacementTable(copy);
            for (int i = 0; i < copy.Length; i++)
            {
                if (copy.Contains("define")) continue;

                foreach (var defineValue in table)
                {
                    if (copy[i].Contains(defineValue.Key))
                    {
                        copy[i] = copy[i].Replace(defineValue.Key, defineValue.Value);
                    }
                }
            }

            int internalCount = 0;
            for (int i = 0; i < copy.Length; i++)
            {
                if (code.Contains(copy[i].Trim()))
                {
                    LineIndexToDissassemblyIndex.Add(i, internalCount);

                    DissassemblyIndexToLineIndex.Add(internalCount, i);
                    
                    LineIndexToLength.Add(i, copyOfCopy[i].Length);

                    internalCount++;
                }
            }
        }
    }
}
