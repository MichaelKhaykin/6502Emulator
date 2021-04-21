using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _6502Emulator
{
    public class ColorWheelControl : Control
    {
        private int radius = 50;
        public int Radius
        {
            get
            {
                return radius;
            }
            set
            {
                radius = value;

                SetBoundsCore(Location.X, Location.Y, 0, 0, BoundsSpecified.Location);

                Image = new Bitmap(Width, Height);

                GenerateBitmap();

                Invalidate();
            }
        }
        Rectangle pictureBoxRectangle => new Rectangle(Location.X, Location.Y, Width, Height);
        public Bitmap Image { get; private set; }

        private double v;
        public double V
        {
            get
            {
                return v;
            }
            set
            {
                if (v < 0 || v > 1) return;
                v = value;

                GenerateBitmap();
            }
        }

        public Color ColorSelected { get; private set; }
        public ColorWheelControl()
        {
            Image = new Bitmap(100, 100);
            V = 1;
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            base.SetBoundsCore(x, y, radius * 2, radius * 2, specified);
        }
        private unsafe bool GenerateBitmap()
        {
            if (v < 0 || v > 1)
            {
                return false;
            }

            BitmapData bitmapData = Image.LockBits(new Rectangle(0, 0, Image.Width, Image.Height), ImageLockMode.ReadWrite, Image.PixelFormat);

            byte* scan0 = (byte*)bitmapData.Scan0.ToPointer();

            var bitsPerPixel = 32;

            for (int x = -Radius; x < Radius; x++)
            {
                for (int y = -Radius; y < Radius; y++)
                {
                    (double length, double angle) = xy2polar(x, y);

                    if (length > Radius)
                    {
                        continue;
                    }

                    double degrees = rad2deg(angle);

                    int adjustedX = x + Radius;
                    int adjustedY = y + Radius;
                    int rowLength = 2 * Radius;

                    byte* data = scan0 + (adjustedX + (adjustedY * rowLength)) * bitsPerPixel / 8;

                    double hue = degrees;
                    
                    (byte red, byte green, byte blue) = hsv2rgb(hue, 1, V);

                    byte alpha = (byte)(V * 255);

                    data[0] = blue;
                    data[1] = green;
                    data[2] = red;
                    data[3] = alpha;
                }
            }

            Image.UnlockBits(bitmapData);

            return true;
        }
        (byte, byte, byte) hsv2rgb(double hue, double saturation, double value)
        {
            double chroma = value * saturation;
            double hue1 = hue / 60;
            double x = chroma * (1 - Math.Abs((hue1 % 2) - 1));
            (double r1, double g1, double b1) = (0, 0, 0);

            if (hue1 >= 0 && hue1 <= 1)
            {
                (r1, g1, b1) = (chroma, x, 0);
            }
            else if (hue1 >= 1 && hue1 <= 2)
            {
                (r1, g1, b1) = (x, chroma, 0);
            }
            else if (hue1 >= 2 && hue1 <= 3)
            {
                (r1, g1, b1) = (0, chroma, x);
            }
            else if (hue1 >= 3 && hue1 <= 4)
            {
                (r1, g1, b1) = (0, x, chroma);
            }
            else if (hue1 >= 4 && hue1 <= 5)
            {
                (r1, g1, b1) = (x, 0, chroma);
            }
            else if (hue1 >= 5 && hue1 <= 6)
            {
                (r1, g1, b1) = (chroma, 0, x);
            }

            double m = value - chroma;
            (double r, double g, double b) = (r1 + m, g1 + m, b1 + m);

            // Change r,g,b values from [0,1] to [0,255]
            return ((byte)(255 * r), (byte)(255 * g), (byte)(255 * b));
        }
        (double Radius, double angle) xy2polar(int x, int y)
        {
            double r = Math.Sqrt(x * x + y * y);
            double phi = Math.Atan2(y, x);
            return (r, phi);
        }
        double rad2deg(double rad)
        {
            return ((rad + Math.PI) / (2 * Math.PI)) * 360;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.DrawImage(Image, new PointF(0, 0));
        }
        
        private void Logic(Point coordinate)
        {
            if (MouseButtons != MouseButtons.Left)
            {
                return;
            }

            var relativeCoordinate = new Point(coordinate.X - Radius, coordinate.Y - Radius);
            var distAwayFromCenter = Math.Sqrt(relativeCoordinate.X * relativeCoordinate.X + relativeCoordinate.Y * relativeCoordinate.Y);

            if (distAwayFromCenter > Radius) return;

            ColorSelected = Image.GetPixel(relativeCoordinate.X + Radius, relativeCoordinate.Y + Radius);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            Logic(e.Location);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            Logic(e.Location);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            Logic(e.Location);
        }
    }
}
