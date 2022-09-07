using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _6502Emulator
{
    public static class MMIO
    {
        public static Bitmap Bitmap;

        public static Dictionary<short, int> AddressToIndex = new Dictionary<short, int>();

        private static int scale = 6;
        static MMIO()
        {
            var starting = 0x200;
            var ending = 0x5FF;

            var totalLength = (ending - starting + 1);
            var side = (int)Math.Sqrt(totalLength);

            Bitmap = new Bitmap(side, side);

            for(int i = starting; i <= ending; i++)
            {
                var twoD = (i - starting).OneToTwoD(side);

                Bitmap.SetPixel(twoD.X, twoD.Y, Color.Black);

                AddressToIndex.Add((short)i, i - starting);
            }
        }

        public static void Clear()
        {
            for(int i = 0; i < Bitmap.Width; i++)
            {
                for(int j = 0; j < Bitmap.Height; j++)
                {
                    Bitmap.SetPixel(i, j, Color.Black);
                }
            }
        }
        public static Bitmap ScaledMap()
        {
            Bitmap scaled = new Bitmap(Bitmap.Width * scale, Bitmap.Height * scale);

            for(int i = 0; i < Bitmap.Width; i++)
            {
                for(int j = 0; j < Bitmap.Height; j++)
                {
                    var pixel = Bitmap.GetPixel(i, j);

                    for(int x = 0; x < scale; x++)
                    {
                        for(int y = 0; y < scale; y++)
                        {
                            scaled.SetPixel(x + i * scale, y + j * scale, pixel);
                        }
                    }
                }
            }

            return scaled;
        }
    }
}
