using GraphicsEngine.Objects;
using GraphicsEngine.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicsEngine.Components
{
    public class Transform
    {
        public Vector3 position { get; set; }
        public Vector3 scale { get; set; }
        public Quaternion rotation { get; set; }
        public WorldObject worldObject { get; private set; }
        public Transform(WorldObject worldObject) 
        { 
            this.worldObject = worldObject;
        }

        public void Move(Vector3 direction)
        {
            position += direction;
        }
        public void Rotate(Quaternion rotation)
        {
            this.rotation = Quaternion.Euler(
                this.rotation.x + rotation.x,
                this.rotation.y + rotation.y,
                this.rotation.z + rotation.z);
        }
    }
}
