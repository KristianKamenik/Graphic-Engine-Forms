using GraphicsEngine.Components;
using GraphicsEngine.Units;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;

namespace GraphicsEngine.Objects
{
    public class Player : WorldObject
    {
        private float _speed;
        public Camera camera { get; private set; }
        public Controls controls { get; private set; }
        public Player(float speed)
        {
            camera = new Camera(transform);
            controls = new Controls(transform);
            _speed = speed;

            transform.position = new Vector3(0, -50, -20);
        }

        public void Update()
        {
            controls.Update();
            transform.Move(controls.direction * _speed);
            transform.Rotate(controls.rotation * _speed);
        }

    }
}
