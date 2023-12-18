using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphicsEngine.Engine;
using GraphicsEngine.Enums;
using GraphicsEngine.Units;

namespace GraphicsEngine.Objects
{
    public class Block : WorldObject
    {
        public Block(Vector3 position, BlockType blockType, Color surfColor)
        {
            transform.position = position;
            renderer.InitSectors(new Sector[1]
            {
                new Sector(0, 10, new int[EngineManager.instance.mainCamera.screenWidth],
                surfColor, surfColor, new Wall[4]
                {
                    new Wall(0, 0, 10, 0, blockType),
                    new Wall(10, 0, 10, 10, blockType),
                    new Wall(10, 10, 0, 10, blockType),
                    new Wall(0, 10, 0, 0, blockType)
                }),
            });
        }
    }
}
