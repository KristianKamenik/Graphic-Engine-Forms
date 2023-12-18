using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicsEngine.Units
{
    public struct Vector2
    {
        public float x { get; set; }
        public float y { get; set; }

        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        public Vector2()
        {
            x = 0;
            y = 0;
        }
        #region Statics And Operators

        public static Vector2 Up => new Vector2(0, 1);
        public static Vector2 Down => new Vector2(0, -1);
        public static Vector2 Left => new Vector2(-1, 0);
        public static Vector2 Right => new Vector2(1, 0);
        public static Vector2 Zero => new Vector2(0, 0);
        public static Vector2 One => new Vector2(1, 1);
        public static float Distance(Vector2 a, Vector2 b) => MathF.Sqrt(MathF.Pow(a.x - b.x, 2) + MathF.Pow(a.y - b.y, 2));

        public static implicit operator Vector3(Vector2 a) => new Vector3(a.x, a.y, 0);
        public static explicit operator Vector2(Vector3 a) => new Vector2(a.x, a.y);

        public static Vector2 operator *(Vector2 a, float b) => new Vector2(a.x * b, a.y * b);
        public static Vector2 operator *(Vector2 a, int b) => new Vector2(a.x * b, a.y * b);
        public static Vector2 operator *(Vector2 a, Vector2 b) => new Vector2(a.x * b.x, a.y * b.y);
        public static Vector2 operator +(Vector2 a, Vector2 b) => new Vector2(a.x + b.x, a.y + b.y);
        public static Vector2 operator +(Vector2 a, int b) => new Vector2(a.x + b, a.y + b);
        public static Vector2 operator -(Vector2 a, Vector2 b) => new Vector2(a.x - b.x, a.y - b.y);
        public static Vector2 operator /(Vector2 a, int b) => new Vector2(a.x / b, a.y / b);

        #endregion

        #region Non Statics

        public float Magnitude => MathF.Sqrt(MathF.Pow(x, 2) + MathF.Pow(y, 2));
        public Vector2 Normalized => new Vector2(x / Magnitude, y / Magnitude);

        #endregion
    }
}
