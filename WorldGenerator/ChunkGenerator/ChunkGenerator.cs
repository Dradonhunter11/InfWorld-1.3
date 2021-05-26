using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using InfWorld.Chunks;
using log4net;
using Microsoft.Xna.Framework;

namespace InfWorld.WorldGenerator.ChunkGenerator
{
    class ChunkGenerator
    {
        private FastNoise noise;

        public ChunkGenerator(int seed = 1337)
        {
            noise = new FastNoise(seed);
            noise.SetNoiseType(FastNoise.NoiseType.Perlin);
            noise.SetFrequency(0.005f);
            //noise.SetFractalType(FastNoise.FractalType.RigidMulti);
        }

        /*public Tile[,] Generate()
        {
            bool[,] map = new bool[Chunk.ChunkWidth, Chunk.ChunkHeight];
            for (int x = 0; x < Chunk.ChunkWidth; x++)
            {
                float noiseX = x / 20f;
                for (int y = 0; y < Chunk.ChunkHeight; y++)
                {
                    float noiseY = y / 20f;
                    int height = (int) Math.Floor(noise.GetNoise(x, y) * 100 * 0.05f) * -1;
                    //noiseValue /= y / 5f;

                    if(height < 3)
                        map[y, x] = true;
                }
            }

            Tile[,] chunkBase = new Tile[Chunk.ChunkWidth, Chunk.ChunkHeight];
            for (int i = 0; i < Chunk.ChunkWidth; i++)
            {
                for (int j = 0; j < Chunk.ChunkHeight; j++)
                {
                    Tile tile = new Tile();
                    if (map[i, j])
                    {
                        tile.active(true);
                        tile.type = TileID.AdamantiteBeam;
                    }
                    else
                    {
                        tile.active(false);
                    }
                    //LogManager.GetLogger("Chunk Generator").Debug(chunkBase);
                    chunkBase[i, j] = tile;
                }
            }

            return chunkBase;
        }*/

        public Tile[,] Generate(int x1, int y1)
        {
            float baseHeight = 75;
            float smoothness = 75;
            Tile[,] chunkBase = new Tile[Chunk.ChunkWidth, Chunk.ChunkHeight];
            for (int x = 0; x < Chunk.ChunkWidth; x++)
            {
                for (int y = 0; y < Chunk.ChunkHeight; y++)
                {
                    Tile tile = new Tile();
                    tile.active(false);
                    chunkBase[x, y] = tile;
                }
                int height = (int)Math.Round((Chunk.ChunkHeight - 1 - baseHeight) * Noise2d.Noise((x / smoothness) + (x1 * Chunk.ChunkWidth), 0));
                //for (int y = Chunk.ChunkHeight - height - 1; y >= 0; y--)
                for (int y = 0; y < height; y++)
                { 
                    if (y >= Chunk.ChunkHeight)
                    {
                        continue;
                    }

                    Tile tile = new Tile(); 
                    //if (height < 3) 
                    {
                        
                        tile.active(true);
                        tile.type = TileID.AdamantiteBeam;
                    }
                    //else
                    {
                        //tile.active(false);
                    }
                    chunkBase[x, Chunk.ChunkHeight - 1 - y] = tile;
                }
            }

            return chunkBase;
        }

        public static class Noise2d
        {
            private static Random _random = new Random();
            private static int[] _permutation;

            private static Vector2[] _gradients;

            static Noise2d()
            {
                CalculatePermutation(out _permutation);
                CalculateGradients(out _gradients);
            }

            private static void CalculatePermutation(out int[] p)
            {
                p = Enumerable.Range(0, 256).ToArray();

                /// shuffle the array
                for (var i = 0; i < p.Length; i++)
                {
                    var source = _random.Next(p.Length);

                    var t = p[i];
                    p[i] = p[source];
                    p[source] = t;
                }
            }

            /// <summary>
            /// generate a new permutation.
            /// </summary>
            public static void Reseed()
            {
                CalculatePermutation(out _permutation);
            }

            private static void CalculateGradients(out Vector2[] grad)
            {
                grad = new Vector2[256];

                for (var i = 0; i < grad.Length; i++)
                {
                    Vector2 gradient;

                    do
                    {
                        gradient = new Vector2((float)(_random.NextDouble() * 2 - 1), (float)(_random.NextDouble() * 2 - 1));
                    }
                    while (gradient.LengthSquared() >= 1);

                    gradient.Normalize();

                    grad[i] = gradient;
                }

            }

            private static float Drop(float t)
            {
                t = Math.Abs(t);
                return 1f - t * t * t * (t * (t * 6 - 15) + 10);
            }

            private static float Q(float u, float v)
            {
                return Drop(u) * Drop(v);
            }

            public static float Noise(float x, float y)
            {
                var cell = new Vector2((float)Math.Floor(x), (float)Math.Floor(y));

                var total = 0f;

                var corners = new[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1) };

                foreach (var n in corners)
                {
                    var ij = cell + n;
                    var uv = new Vector2(x - ij.X, y - ij.Y);

                    var index = _permutation[(int)ij.X % _permutation.Length];
                    index = _permutation[(index + (int)ij.Y) % _permutation.Length];

                    var grad = _gradients[index % _gradients.Length];

                    total += Q(uv.X, uv.Y) * Vector2.Dot(grad, uv);
                }

                return Math.Max(Math.Min(total, 1f), -1f);
            }

        }
    }
}
