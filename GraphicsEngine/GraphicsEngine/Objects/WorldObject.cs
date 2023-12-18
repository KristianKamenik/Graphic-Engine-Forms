using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphicsEngine.Components;

namespace GraphicsEngine.Objects
{
    public class WorldObject
    {
        public Renderer renderer { get; private set; }
        public Transform transform { get; private set; }

        public WorldObject()
        {
            renderer = new Renderer(this);
            transform = new Transform(this);
        }
    }
}
