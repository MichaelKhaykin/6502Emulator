using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace _6502Emulator
{
    public static class Extensions
    {
        public static bool ToDecimal(this string hexStr, out int x)
        {
            x = 0;
            for (int i = 0; i < hexStr.Length; i++)
            {
                if (Helper.HexToIntMap.ContainsKey(hexStr[i]) == false)
                {
                    return false;
                }

                x += Helper.HexToIntMap[hexStr[i]] * (int)Math.Pow(16, hexStr.Length - 1 - i);
            }
            return true;
        }
        public static string ToHex(this int number)
        {
            int hexBase = 16;

            StringBuilder hexStr = new StringBuilder();

            do
            {
                var quo = number / hexBase;
                var remainder = number % hexBase;

                hexStr.Insert(0, Helper.IntToHexMap[remainder]);

                number = quo;
            } while (number != 0);

            return hexStr.ToString();
        }

        public static Point OneToTwoD(this int index, int width)
        {
            return new Point(index % width, index / width);
        }
        public static bool WillAdditionOverflow(this sbyte b, int val)
        {
            int @checked = b + val;

            return @checked < -127 || @checked > 128;
        }

        public static bool WillSubtractionUnderflow(this sbyte b, int val)
        {
            return b - sbyte.MinValue < val;
        }

        private static List<Type> systemTypes =
                                  Assembly.GetExecutingAssembly().GetType()
                                  .Module.Assembly.GetExportedTypes().ToList();
        public static bool IsNativeType(this object obj)
        {
            return systemTypes.Contains(obj);
        }
        public static bool AmIAnEvenNumber(this int x)
        {
            return x % 2 == 0;
        }
    }
}
