using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _25_06_20_FlightWeight_Design_pattern
{
    class MyRectangle
    {
        public Color Color { get; private set; }
        public Point Location { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public MyRectangle(Color color, Point location, int width, int height)
        {
            Color = color;
            Location = location;
            Width = width;
            Height = height;
        }
    }
}
