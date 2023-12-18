using GraphicsEngine.Components;
using GraphicsEngine.Objects;
using GraphicsEngine.Units;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicsEngine.Engine
{
    public class EngineManager
    {
        private static EngineManager? _instance = null;
        public static EngineManager instance => _instance ?? (_instance = new EngineManager());

        private List<WorldObject> _worldObjects;
        private HashSet<Vector3> _takenPositions;
        private Player _player;
        private DateTime _lastSortUpdate;
        private PerlinNoise _worldGenerator;
        private Random _random;
        private BitmapData? _bitmapData;

        public Bitmap mainCanvas { get; private set; }
        public Camera mainCamera => _player.camera;
        public int worldObjectsCount => _worldObjects.Count;

        public EngineManager()
        {
            _player = new Player(3f);
            _worldObjects = new List<WorldObject>();
            _takenPositions = new HashSet<Vector3>();
            _lastSortUpdate = DateTime.Now;

            _random = new Random();
            _worldGenerator = new PerlinNoise(_random.Next(10,100000).ToString());
            mainCanvas = new Bitmap(mainCamera.screenWidthScaled, mainCamera.screenHeightScaled);
            _bitmapData = null;
        }

        public void Init()
        {
            //ChunkRegion(0, 0, 0, 1, 1);
            LoadChunk(0, 0, 4, 4); //4,4 (4*4 = 16) minecraft chunkk
           /* LoadChunk(4, 0, 4, 4);
            LoadChunk(4, 4, 4, 4);
            LoadChunk(0, 4, 4, 4);*/
        }
        public void Draw(object? sender, PaintEventArgs e) => e.Graphics.DrawImage(mainCanvas, Point.Empty);
        private void ClearBitMap()
        {
            if (_bitmapData is null)
                return;

            Parallel.For(0, mainCamera.screenHeight, y =>
            {
                for (int x = 0; x < mainCamera.screenWidth; x++)
                {
                    EditPixelsBitmap(x, y, Color.White);
                }
            });
        }
        public unsafe void EditPixelsBitmap(int x, int y, Color color)
        {
            if (_bitmapData is null)
                return;

            x *= mainCamera.scale;
            y *= mainCamera.scale;

            for (int v = 0; v < mainCamera.scale; v++)
            {
                for (int u = 0; u < mainCamera.scale; u++)
                {
                    int byteIndex = ((y+v) * _bitmapData.Stride) + ((x+u) * 4);

                    byte* ptr = (byte*)_bitmapData.Scan0.ToPointer();
                    ptr[byteIndex + 0] = color.B; // Blue
                    ptr[byteIndex + 1] = color.G; // Green
                    ptr[byteIndex + 2] = color.R; // Red
                    ptr[byteIndex + 3] = 255; // Alpha
                }
            }
        }
        public void Update()
        {
            Render();
            _player.Update();
        }
        public KeyEventHandler GetPlayerKeyHandlerDown() => _player.controls.OnKeyPress;
        public KeyEventHandler GetPlayerKeyHandlerUp() => _player.controls.OnKeyUp;
        public bool PositionIsTaken(Vector3 position) => _takenPositions.Contains(position);
        private void Render()
        {
            SortRendererOrder();

            _bitmapData = mainCanvas.LockBits(new Rectangle(0, 0, mainCamera.screenWidthScaled, mainCamera.screenHeightScaled), ImageLockMode.WriteOnly, mainCanvas.PixelFormat);
            ClearBitMap();
            Parallel.For(0, _worldObjects.Count, x => _worldObjects[x].renderer.Draw());
            for (int i = 0; i < _worldObjects.Count; i++)
                _worldObjects[i].renderer.DrawPixels();
            mainCanvas.UnlockBits(_bitmapData);
        }
        private void AddWorldObject(WorldObject[] worldObjects)
        {
            foreach(WorldObject worldObject in worldObjects)
            {
                AddWorldObject(worldObject);
            }
        }
        private void AddWorldObject(WorldObject worldObject)
        {
            _worldObjects.Add(worldObject);
            _takenPositions.Add(worldObject.transform.position);
        }
        private void LoadChunk(int offsetX, int offsetY, int lenght, int regionSize)
        {
            List<Vector2> treePos = new List<Vector2>();
            int maxHeight = 0;
            for (int x = 0 + offsetX; x < lenght + offsetX; x++)
            {
                for (int y = 0 + offsetY; y < lenght + offsetY; y++)
                {
                    maxHeight = GetHeight(x, y);
                    for (int z = -1; z < maxHeight; z++)
                    {
                        ChunkRegion(x, y, z, regionSize, maxHeight);
                    }
                    SpawnTree(x,y,regionSize, ref treePos);
                }
            }
        }
        private void SpawnTree(int x, int y, int regionSize, ref List<Vector2> treePos)
        {
            Vector2 pos = new Vector2(
                            (_random.Next(0, regionSize) * 10) + (10 * x * regionSize),
                            (_random.Next(0, regionSize) * 10) + (10 * y * regionSize));
            for (int i = 0;i < treePos.Count; i++)
            {
                if (Vector2.Distance(pos, treePos[i]) < 60)
                    return;
            }
            AddWorldObject(ModelGenerator.GetTree(
                        new Vector3(
                            pos.x,pos.y,
                            GetHeight(x, y) * -10)));
            treePos.Add(pos);
        }
        private void ChunkRegion(int x, int y, int z,int regionSize, int maxHeight)
        {
            for (int offsetX = 0; offsetX < regionSize; offsetX++)
            {
                for (int offsetY = 0; offsetY < regionSize; offsetY++)
                {
                    AddWorldObject(new Block(
                        new Vector3(
                            (offsetX * 10) + (10 * x * regionSize),
                            (offsetY * 10) + (10 * y * regionSize),
                            -(10 * z))

                        ,(z < 0) ? Enums.BlockType.Stone :
                        (z == maxHeight-1) ? Enums.BlockType.Grass : Enums.BlockType.GrassOrDirt
                        
                        ,(z < 0) ? Color.Gray : Color.FromArgb(112, 178, 55)));
                }
            }
        }
        private int GetHeight(int x, int y)
        {
            float noise = _worldGenerator.GetXZ(new Vector2(x, y), 8, 256, 1f);
            if (noise < 0.25f)
                return 1;
            else if (noise < 0.65f)
                return 2;
            else 
                return 3;
        }
        private void SortRendererOrder()
        {
            if (DateTime.Now < _lastSortUpdate)
                return;
            if (_worldObjects.Count == 0)
                return;

            WorldObject[] worldObjects = SortArray(_worldObjects.ToArray(), 0, _worldObjects.Count - 1);
            _worldObjects.Clear();
            _worldObjects.AddRange(worldObjects);

            _lastSortUpdate = DateTime.Now.AddSeconds(1);
        }
        private WorldObject[] SortArray(WorldObject[] array, int leftIndex, int rightIndex)
        {
            int i = leftIndex;
            int j = rightIndex;
            WorldObject pivot = array[leftIndex];
            while (i <= j)
            {
                while (Vector3.Distance(array[i].transform.position,_player.transform.position) 
                    > Vector3.Distance(pivot.transform.position,_player.transform.position))
                {
                    i++;
                }

                while (Vector3.Distance(array[j].transform.position,_player.transform.position) 
                    < Vector3.Distance(pivot.transform.position,_player.transform.position))
                {
                    j--;
                }
                if (i <= j)
                {
                    WorldObject temp = array[i];
                    array[i] = array[j];
                    array[j] = temp;
                    i++;
                    j--;
                }
            }

            if (leftIndex < j)
                SortArray(array, leftIndex, j);
            if (i < rightIndex)
                SortArray(array, i, rightIndex);
            return array;
        }
    }
}
