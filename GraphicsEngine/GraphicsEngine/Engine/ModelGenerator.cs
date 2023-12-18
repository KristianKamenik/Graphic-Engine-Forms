using GraphicsEngine.Objects;
using GraphicsEngine.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace GraphicsEngine.Engine
{
    public static class ModelGenerator
    {
        public static WorldObject[] GetTree(Vector3 offset)
        {
            List<WorldObject> tree = new List<WorldObject>
            {
                new Block(offset, Enums.BlockType.Wood, Color.BurlyWood),
                new Block(new Vector3(0, 0, -10) + offset, Enums.BlockType.Wood, Color.BurlyWood)
            };
            for (int i = 0; i < 3; i++)
            {
                for (int x = 0 + i; x < 5 - i; x++)
                {
                    for (int y = 0 + i; y < 5 - i; y++)
                    {
                        tree.Add(new Block(
                            new Vector3(-20 + (10 * x), -20 + (10 * y), -20 - (10 * i)) + offset,
                            Enums.BlockType.Leaves, Color.FromArgb(71, 122, 30)));
                    }
                }
            }
            return tree.ToArray();
        }
    }
}
