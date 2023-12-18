using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicsEngine.Components
{
    public class Camera
    {
        private Transform _parent;

        private const int SCREENWIDTH = 160;
        private const int SCREENHEIGHT = 120;
        private int _res;
        public int scale { get; private set; }
        public int screenWidth => SCREENWIDTH * _res;
        public int screenHeight => SCREENHEIGHT * _res;
        public int screenWidth2 => screenWidth / 2;
        public int screenHeight2 => screenHeight / 2;
        public int screenWidthScaled => screenWidth * scale;
        public int screenHeightScaled => screenHeight * scale;

        public Transform parent => _parent;
        public int x => (int)Math.Round(_parent.position.x);
        public int y => (int)Math.Round(_parent.position.y);
        public int z => (int)Math.Round(_parent.position.z);
        public int a => (int)Math.Round(_parent.rotation.z);
        public int l => (int)Math.Round(_parent.rotation.x);

        public Camera(Transform transform)
        {
            _res = 2;
            scale = 4;
            _parent = transform;
        }
    }
}
