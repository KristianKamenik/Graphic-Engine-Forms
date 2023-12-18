using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace GraphicsEngine.Engine
{
    public struct Sector
    {
        public int bottomHeight { get; set; }
        public int topHeight { get; set; }
        public Wall[] walls { get; set; }
        public Color bottomColor { get; set; }
        public Color upColor { get; set; }
        public int[] surf { get; set; }
        public int surface { get; set; }
        public Sector(int z1, int z2, int[] surf, Color bottomColor, Color upColor, Wall[] walls)
        {
            this.bottomHeight = z1;
            this.topHeight = z2;
            this.walls = walls;
            this.bottomColor = bottomColor;
            this.upColor = upColor;

            surface = 0;
            this.surf = surf;
        }
    }
}
