using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicsEngine.Units
{
    public struct Quaternion
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }

        public Quaternion(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public Quaternion()
        {
            x = 0;
            y = 0;
            z = 0;
        }

        public static float Deg2Rad(float degrees) => degrees * (MathF.PI / 180);
        public static Quaternion Euler(Vector3 rotation) => Euler(rotation.x,rotation.y,rotation.z);
        public static Quaternion Euler(float x, float y, float z) => new Quaternion(x, y, z);
        public static Quaternion operator *(Quaternion a, float b) => new Quaternion(a.x * b, a.y * b, a.z * b);
    }
}
