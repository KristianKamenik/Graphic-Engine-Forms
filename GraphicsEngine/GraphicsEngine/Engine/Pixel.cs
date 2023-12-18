using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicsEngine.Engine
{
    public struct Pixel
    {
        public int x { get; private set; }
        public int y { get; private set; }
        public Color color { get; private set; }
        public Pixel(int x, int y, Color color)
        {
            this.x = x;
            this.y = y;
            this.color = color;
        }
    }
}
