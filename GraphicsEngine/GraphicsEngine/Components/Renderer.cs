using GraphicsEngine.Engine;
using GraphicsEngine.Objects;
using GraphicsEngine.Units;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Windows.Forms;

namespace GraphicsEngine.Components
{
    public class Renderer
    {
        private Vector3[] _worldAxis;
        private Vector2[] _linePos;
        private float _cosA, _sinA;
        private Sector[] _sectors;
        private List<Pixel> _pixels;

        public WorldObject worldObject { get; private set; }
        private int screenWidth => camera.screenWidth;
        private int screenHeight => camera.screenHeight;
        private int screenWidth2 => camera.screenWidth2;
        private int screenHeight2 => camera.screenHeight2;
        private int scale => camera.scale;
        private Camera camera => EngineManager.instance.mainCamera;

        public Renderer(WorldObject worldObject)
        {
            _pixels = new List<Pixel>();
            _worldAxis = new Vector3[4];
            _linePos = new Vector2[2];
            _cosA = 1;
            _sinA = 0;
            _sectors = new Sector[0];
            this.worldObject = worldObject;
        }

        public void InitSectors(Sector[] sectors)
        {
            _sectors = sectors;

            for (int s = 0; s < _sectors.Length; s++)
            {
                _sectors[s].bottomHeight += (int)Math.Round(worldObject.transform.position.z);
                for (int w = 0; w < _sectors[s].walls.Length; w++)
                {
                    _sectors[s].walls[w].x1 += (int)Math.Round(worldObject.transform.position.x);
                    _sectors[s].walls[w].x2 += (int)Math.Round(worldObject.transform.position.x);
                    _sectors[s].walls[w].y1 += (int)Math.Round(worldObject.transform.position.y);
                    _sectors[s].walls[w].y2 += (int)Math.Round(worldObject.transform.position.y);
                }
            }
        }
        public void Draw()
        {
            _cosA = MathF.Cos(Quaternion.Deg2Rad(camera.a)); _sinA = MathF.Sin(Quaternion.Deg2Rad(camera.a));
            bool dontRender = false;
            Vector2 lookingDir = worldObject.transform.position - EngineManager.instance.mainCamera.parent.position;
            for (int s = 0; s < _sectors.Length; s++)
            {
                SurfaceSelector(s);
                for (int backCulling = 0; backCulling < 2; backCulling++)
                {
                    for (int w = 0; w < _sectors[s].walls.Length; w++)
                    {
                        if (_sectors[s].walls[w].blockType == Enums.BlockType.GrassOrDirt)
                        {
                            if (IsDirt())
                                _sectors[s].walls[w].blockType = Enums.BlockType.Dirt;
                            else
                                _sectors[s].walls[w].blockType = Enums.BlockType.Grass;
                        }


                        dontRender = DontRenderWall(w, lookingDir);
                        if (_sectors[s].surface == 0 && dontRender)
                            continue;
                        SetVectorDifference(backCulling == 0, _sectors[s].walls[w]);
                        SetWorldPos(_sectors[s]);
                        DrawWall(dontRender || backCulling == 0, _sectors[s].walls[w].blockType,ref _sectors[s]);
                    }
                    _sectors[s].surface *= -1;
                }
            }
        }
        private bool DontRenderWall(int index, Vector2 lookingDir)
        {
            switch(index)
            {
                case 0:
                    if (EngineManager.instance.PositionIsTaken(worldObject.transform.position + (Vector3.Backwards * 10))
                        || lookingDir.y < 0)
                        return true;
                    break;
                case 1:
                    if (EngineManager.instance.PositionIsTaken(worldObject.transform.position + (Vector3.Right * 10))
                        || lookingDir.x > 0)
                        return true;
                    break;
                case 2:
                    if (EngineManager.instance.PositionIsTaken(worldObject.transform.position + (Vector3.Foward * 10))
                        || lookingDir.y > 0)
                        return true;
                    break;
                case 3:
                    if (EngineManager.instance.PositionIsTaken(worldObject.transform.position + (Vector3.Left * 10))
                        || lookingDir.x < 0)
                        return true;
                    break;
            }
            return false;
        }
        private bool IsDirt()
        {
            if (!EngineManager.instance.PositionIsTaken(worldObject.transform.position + (Vector3.Down * 10)))
                return false;
            else if (!EngineManager.instance.PositionIsTaken(worldObject.transform.position + (Vector3.Up * 10)))
                return false;
            return true;
        }
        private void SurfaceSelector(int index)
        {
            if (camera.z < _sectors[index].bottomHeight && !EngineManager.instance.PositionIsTaken(worldObject.transform.position + (Vector3.Down * 10)))
                _sectors[index].surface = 1;
            else if (camera.z > _sectors[index].topHeight + _sectors[index].bottomHeight
                 && !EngineManager.instance.PositionIsTaken(worldObject.transform.position + (Vector3.Up * 10)))
                _sectors[index].surface = 2;
            else
                _sectors[index].surface = 0;
        }
        private void SetWorldPos(Sector sector)
        {
            SetWorldX();
            SetWorldY();
            SetWorldZ(sector);

            if (!CheckForClips())
                return;

            WorldToScreen();
        }
        private void SetWorldX()
        {
            _worldAxis[0].x = (int)Math.Round(_linePos[0].x * _cosA - _linePos[0].y * _sinA);
            _worldAxis[1].x = (int)Math.Round(_linePos[1].x * _cosA - _linePos[1].y * _sinA);
            _worldAxis[2].x = _worldAxis[0].x;
            _worldAxis[3].x = _worldAxis[1].x;
        }
        private void SetWorldY()
        {
            _worldAxis[0].y = (int)Math.Round(_linePos[0].y * _cosA + _linePos[0].x * _sinA);
            _worldAxis[1].y = (int)Math.Round(_linePos[1].y * _cosA + _linePos[1].x * _sinA);
            _worldAxis[2].y = _worldAxis[0].y;
            _worldAxis[3].y = _worldAxis[1].y;
        }
        private void SetWorldZ(Sector sector)
        {
            _worldAxis[0].z = (int)Math.Round(sector.bottomHeight - camera.z + camera.l * _worldAxis[0].y / 32.0f);
            _worldAxis[1].z = (int)Math.Round(sector.bottomHeight - camera.z + camera.l * _worldAxis[1].y / 32.0f);
            _worldAxis[2].z = _worldAxis[0].z + sector.topHeight;
            _worldAxis[3].z = _worldAxis[1].z + sector.topHeight;
        }
        private void WorldToScreen()
        {
            _worldAxis[0].x = (int)_worldAxis[0].x * 200 / (int)_worldAxis[0].y + screenWidth2;
            _worldAxis[1].x = (int)_worldAxis[1].x * 200 / (int)_worldAxis[1].y + screenWidth2;
            _worldAxis[2].x = (int)_worldAxis[2].x * 200 / (int)_worldAxis[2].y + screenWidth2;
            _worldAxis[3].x = (int)_worldAxis[3].x * 200 / (int)_worldAxis[3].y + screenWidth2;

            _worldAxis[0].y = (int)_worldAxis[0].z * 200 / (int)_worldAxis[0].y + screenHeight2;
            _worldAxis[1].y = (int)_worldAxis[1].z * 200 / (int)_worldAxis[1].y + screenHeight2;
            _worldAxis[2].y = (int)_worldAxis[2].z * 200 / (int)_worldAxis[2].y + screenHeight2;
            _worldAxis[3].y = (int)_worldAxis[3].z * 200 / (int)_worldAxis[3].y + screenHeight2;
        }
        private bool CheckForClips()
        {
            if (_worldAxis[0].y < 1 && _worldAxis[1].y < 1)
                return false;

            if (_worldAxis[0].y < 1)
            {
                ClipWalls(ref _worldAxis[0], _worldAxis[1]);
                ClipWalls(ref _worldAxis[2], _worldAxis[3]);
            }
            if (_worldAxis[1].y < 1)
            {
                ClipWalls(ref _worldAxis[1], _worldAxis[0]);
                ClipWalls(ref _worldAxis[3], _worldAxis[2]);
            }
            return true;
        }
        private void SetVectorDifference(bool isBackCulling, Wall wall)
        {
            _linePos[0].x = wall.x1 - camera.x; _linePos[0].y = wall.y1 - camera.y;
            _linePos[1].x = wall.x2 - camera.x; _linePos[1].y = wall.y2 - camera.y;

            if (isBackCulling)
            {
                float swp = _linePos[0].x; _linePos[0].x = _linePos[1].x; _linePos[1].x = swp;
                swp = _linePos[0].y; _linePos[0].y = _linePos[1].y; _linePos[1].y = swp;
            }
        }
        private void ClipX()
        {
            if (_worldAxis[0].x < 1)
                _worldAxis[0].x = 1;
            if (_worldAxis[1].x < 1)
                _worldAxis[1].x = 1;
            if (_worldAxis[0].x > screenWidth - 1)
                _worldAxis[0].x = screenWidth - 1;
            if (_worldAxis[1].x > screenWidth - 1)
                _worldAxis[1].x = screenWidth - 1;
        }
        private void ClipY(ref int y1, ref int y2)
        {
            if (y1 < 1)
                y1 = 1;
            if (y2 < 1)
                y2 = 1;
            if (y1 > screenHeight - 1)
                y1 = screenHeight - 1;
            if (y2 > screenHeight - 1)
                y2 = screenHeight - 1;
        }
        private bool DrawSurfaces(int x,int y1, int y2, ref Sector sector)
        {
            if (sector.surface == 1)
            {
                sector.surf[x] = y1;
                return true;
            }
            if (sector.surface == 2)
            {
                sector.surf[x] = y2;
                return true;
            }
            if (sector.surface == -1)
            {
                for (int y = sector.surf[x]; y < y1; y++)
                {
                    SavePixel(x, y, sector.bottomColor);
                }
            }
            if (sector.surface == -2)
            {
                for (int y = y2; y < sector.surf[x]; y++)
                {
                    SavePixel(x, y, sector.upColor);
                }
            }
            return false;
        }
        private void DrawWall(bool dontRender, Enums.BlockType blockType,ref Sector sector)
        {
            int dyb = (int)Math.Round(_worldAxis[1].y - _worldAxis[0].y);
            int dyt = (int)Math.Round(_worldAxis[3].y - _worldAxis[2].y);
            int dx = (int)Math.Round(_worldAxis[1].x - _worldAxis[0].x);
            if (dx == 0)
                dx = 1;
            int xs = (int)_worldAxis[0].x;

            int y1 = 0;
            int y2 = 0;

            int horizontalTexture = 0;
            float horizontalTextureSteps = 16f / (_worldAxis[1].x - _worldAxis[0].x);
            float hSteps = 0;
            if (_worldAxis[0].x < 0)
                horizontalTexture -= (int)MathF.Floor(horizontalTextureSteps * _worldAxis[0].x);

            ClipX();

            for (int x = (int)_worldAxis[0].x; x < _worldAxis[1].x; x++)
            {
                y1 = (int)Math.Round(dyb * (x - xs + 0.5f) / dx + (int)_worldAxis[0].y);
                y2 = (int)Math.Round(dyt * (x - xs + 0.5f) / dx + (int)_worldAxis[2].y);

                int verticalTexture = 0;
                float verticalTextureSteps = 16f / (y2 - y1);
                float vSteps = 0;

                if (y1 < 0)
                    verticalTexture -= (int)MathF.Floor(verticalTextureSteps * y1);

                ClipY(ref y1, ref y2);

                if (DrawSurfaces(x, y1, y2, ref sector))
                    continue;

                if (dontRender)
                    continue;

                for (int y = y1; y < y2; y++)
                {
                    SavePixel(x, y, GetPixelColor(verticalTexture, horizontalTexture, blockType));

                    vSteps += verticalTextureSteps;
                    verticalTexture = (int)MathF.Floor(vSteps);
                }
                hSteps += horizontalTextureSteps;
                horizontalTexture = (int)MathF.Floor(hSteps);
            }

        }
        private Color GetPixelColor(int verticalTexture, int horizontalTexture, Enums.BlockType blockType)
        {
            int pixel = verticalTexture * 16 + horizontalTexture;
            pixel = Math.Max(pixel, 0);
            pixel = Math.Min(pixel, Textures.GetTexturesWalls(blockType).Length - 1);

            return Textures.GetTexturesWalls(blockType)[pixel];
        }
        private void ClipWalls(ref Vector3 worldAxis1,Vector3 worldAxis2)
        {
            float da = worldAxis1.y;
            float db = worldAxis2.y;
            float d = da - db;
            if (d == 0)
                d = 1;
            float s = da / (da - db);

            worldAxis1.x = (int)Math.Round(worldAxis1.x + s * (worldAxis2.x - worldAxis1.x));
            worldAxis1.y = (int)Math.Round(worldAxis1.y + s * (worldAxis2.y - worldAxis1.y));
            if (worldAxis1.y == 0)
                worldAxis1.y = 1;
            worldAxis1.z = (int)Math.Round(worldAxis1.z + s * (worldAxis2.z - worldAxis1.z));
        }
        private void SavePixel(int x, int y, Color color)
        {
            //g.FillRectangle(color, scale * x, scale * y, scale, scale);

            _pixels.Add(new Pixel(x, y, color));
            //EngineManager.instance.EditPixelsBitmap(x, y, color);
        }
        public void DrawPixels()
        {
            Parallel.For(0, _pixels.Count, 
                i => EngineManager.instance.EditPixelsBitmap(_pixels[i].x, _pixels[i].y, _pixels[i].color));
                
            _pixels.Clear();
        }
        
    }
}
