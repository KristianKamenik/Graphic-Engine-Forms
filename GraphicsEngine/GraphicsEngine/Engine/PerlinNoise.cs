using GraphicsEngine.Units;
using System.Security.Policy;


namespace GraphicsEngine.Engine
{
    public class PerlinNoise
    {
        public float[,] Noise { get; private set; }
        public string Seed { get; private set; }
        public PerlinNoise(string seed)
        {
            this.Seed = seed;
            this.Noise = new float[1025, 1025];
        }

        public float GetXZ(Vector2 position, int octaves, int resolution, float bias)
        {
            float fNoise = 0.0f;
            float fScale = 1.0f;
            float fScaleAcc = 0.0f;

            for (int i = 0; i < octaves; i++)
            {
                int pitch = resolution >> i;
                if (pitch <= 0) continue;

                Vector2 sample1 = new Vector2(
                    (position.x / pitch) * pitch,
                    (position.y / pitch) * pitch);

                Vector2 sample2 = new Vector2(
                    (sample1.x + pitch),
                    (sample1.y + pitch));

                float fBlendX = (float)(position.x - sample1.x) / (float)pitch;
                float fBlendZ = (float)(position.y - sample1.y) / (float)pitch;

                float fSampleT = (1.0f - fBlendX) * this.GetNoise(sample1) + fBlendX * this.GetNoise(new Vector2(sample2.x, sample1.y));
                float fSampleB = (1.0f - fBlendX) * this.GetNoise(new Vector2(sample1.x, sample2.y)) + fBlendX * this.GetNoise(sample2);

                fScaleAcc += fScale;
                fNoise += (fBlendZ * (fSampleB - fSampleT) + fSampleT) * fScale;
                fScale = fScale / bias;
            }

            return fNoise / fScaleAcc;
        }

        private float GetNoise(Vector2 position)
        {
            if (this.Noise[(int)position.x, (int)position.y] == 0.0)
            {
                this.Noise[(int)position.x, (int)position.y] = this.GetXZNoise(position);
            }

            return this.Noise[(int)position.x, (int)position.y];
        }
        private float GetXZNoise(Vector2 position)
        {
            string key = position.x.ToString() + position.y.ToString() + this.Seed;
            return (float)Jenkins(key) / (float)uint.MaxValue;
        }
        private uint Jenkins(string key)
        {
            int i = 0;
            uint hash = 0;
            while (i != key.Length)
            {
                hash += key[i++];
                hash += hash << 10;
                hash ^= hash >> 6;
            }

            hash += hash << 3;
            hash ^= hash >> 11;
            hash += hash << 15;
            return hash;
        }
    }
}

