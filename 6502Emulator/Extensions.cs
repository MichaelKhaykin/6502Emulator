using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _6502Emulator
{
    public static class Extensions
    {

        public static int ToDecimal(this string hexStr)
        {
            int number = 0;
            for(int i = 0; i < hexStr.Length; i++)
            {
                number += Helper.HexToIntMap[hexStr[i]] * (int)Math.Pow(16, hexStr.Length - 1 - i);
            }
            return number;
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
    }
}
