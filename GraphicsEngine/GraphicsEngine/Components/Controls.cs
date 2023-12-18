using GraphicsEngine.Objects;
using GraphicsEngine.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicsEngine.Components
{
    public class Controls
    {
        private Transform _parent;
        private HashSet<Keys> _keys;

        public Vector3 direction { get; private set; }
        public Quaternion rotation { get; private set; }

        public Controls(Transform transform)
        {
            _keys = new HashSet<Keys>();
            _parent = transform;
            direction = Vector3.Zero;
        }
        public void Update()
        {
            Rotate();
            Move();
            if (IsKeyDown(Keys.R))
                _parent.rotation = Quaternion.Euler(0, _parent.rotation.y, _parent.rotation.z);
        }
        public void OnKeyPress(object? sender, KeyEventArgs e)
        {
            if(!_keys.Contains(e.KeyCode))
                _keys.Add(e.KeyCode);
        }
        public void OnKeyUp(object? sender, KeyEventArgs e)
        {
            if (_keys.Contains(e.KeyCode))
                _keys.Remove(e.KeyCode);
        }

        private bool IsKeyDown(Keys key) => _keys.Contains(key);
        private void Move()
        {
            Vector3 temp = Vector3.Zero;
            if(IsKeyDown(Keys.W))
            {
                temp.x += MathF.Sin(Quaternion.Deg2Rad(_parent.rotation.z));
                temp.y += MathF.Cos(Quaternion.Deg2Rad(_parent.rotation.z));
            }
            if (IsKeyDown(Keys.S))
            {
                temp.x -= MathF.Sin(Quaternion.Deg2Rad(_parent.rotation.z));
                temp.y -= MathF.Cos(Quaternion.Deg2Rad(_parent.rotation.z));
            }
            if (IsKeyDown(Keys.D))
            {
                temp.x += MathF.Cos(Quaternion.Deg2Rad(_parent.rotation.z));
                temp.y -= MathF.Sin(Quaternion.Deg2Rad(_parent.rotation.z));
            }
            if (IsKeyDown(Keys.A))
            {
                temp.x -= MathF.Cos(Quaternion.Deg2Rad(_parent.rotation.z));
                temp.y += MathF.Sin(Quaternion.Deg2Rad(_parent.rotation.z));
            }
            temp.z = (IsKeyDown(Keys.Space)) ? -1 : (IsKeyDown(Keys.ControlKey)) ? 1 : 0;
            direction = temp;
        }
        private void Rotate()
        {
            rotation = Quaternion.Euler(
                IsKeyDown(Keys.Up) ? 1 : IsKeyDown(Keys.Down) ? -1 : 0,
                0, IsKeyDown(Keys.Left) ? -1 : IsKeyDown(Keys.Right) ? 1 : 0);
        }
    }
}
