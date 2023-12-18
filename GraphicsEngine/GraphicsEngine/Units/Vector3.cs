using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicsEngine.Units
{
    public struct Vector3
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }

        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public Vector3()
        {
            x = 0;
            y = 0;
            z = 0;
        }

        #region Statics And Operators

        public static Vector3 Up => new Vector3(0, 0, 1);
        public static Vector3 Down => new Vector3(0, 0, -1);
        public static Vector3 Left => new Vector3(-1, 0, 0);
        public static Vector3 Right => new Vector3(1, 0, 0);
        public static Vector3 Foward => new Vector3(0, 1, 0);
        public static Vector3 Backwards => new Vector3(0, -1, 0);
        public static Vector3 Zero => new Vector3(0, 0, 0);
        public static Vector3 One => new Vector3(1, 1, 1);
        public static float Distance(Vector3 a, Vector3 b) => MathF.Sqrt(MathF.Pow(a.x - b.x, 2) + MathF.Pow(a.y - b.y, 2));

        public static implicit operator Vector2(Vector3 a) => new Vector2(a.x, a.y);
        public static explicit operator Vector3(Vector2 a) => new Vector3(a.x, a.y, 0);

        public static Vector3 operator *(Vector3 a, float b) => new Vector3(a.x * b, a.y * b, a.z * b);
        public static Vector3 operator *(Vector3 a, int b) => new Vector3(a.x * b, a.y * b, a.z * b);
        public static Vector3 operator *(Vector3 a, Vector3 b) => new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        public static Vector3 operator +(Vector3 a, Vector3 b) => new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
        public static Vector3 operator +(Vector3 a, int b) => new Vector3(a.x + b, a.y + b, a.z + b);
        public static Vector3 operator -(Vector3 a, Vector3 b) => new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
        public static Vector3 operator /(Vector3 a, int b) => new Vector3(a.x / b, a.y / b, a.z / b);

        #endregion

        #region Non Statics

        public float Magnitude => MathF.Sqrt(MathF.Pow(x, 2) + MathF.Pow(y, 2) + MathF.Pow(z, 2));
        public Vector3 Normalized => new Vector3(x / Magnitude, y / Magnitude, z / Magnitude);

        #endregion

    }
}
