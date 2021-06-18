using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfWorld.Utils.Math;
using InfWorld.World;
using InfWorld.World.Region;
using Terraria.ID;
using Terraria;
using log4net;
using Microsoft.Xna.Framework;

namespace InfWorld.WorldGenerator.ChunkGenerator
{
    class SurfaceChunkGenerator : ChunkGenerator
    { 
        private static FastNoise _noise;

        public SurfaceChunkGenerator(int seed = 1337) : base(1337)
        {
            _noise = new FastNoise(seed);
        }

        public static int[] GetPerlinDisplacements(int displacementCount, float frequency, int maxLimit, float multiplier, int seed, int startingPosition = 0)
        {
            FastNoise noise = new FastNoise(SurfaceChunkGenerator._noise.GetSeed());
            noise.SetNoiseType(FastNoise.NoiseType.Perlin);
            noise.SetFrequency(frequency);
            noise.SetFractalType(FastNoise.FractalType.RigidMulti);

            int[] displacements = new int[displacementCount];
            int startPosition = startingPosition * Chunk.ChunkWidth;
            for (int x = 0; x < displacementCount; x++)
            {
                float noiseValue = noise.GetNoise(x + startPosition, x + startPosition);

                displacements[x] = (int)Math.Floor(noiseValue * maxLimit * multiplier);
            }
                

            return displacements;
        }

        public override Tile[,] Generate(int x1, int y1)
        {
            //LogManager.GetLogger("NO AGAIN").Debug(x1 + ", " + y1);

            Tile[,] chunkBase = new Tile[Chunk.ChunkWidth, Chunk.ChunkHeight];

            
            float[] frequency = new float[] { 0.0077f, 0.0011f, 0.022f, 0.04f };
            float[] limit = new float[] { 0.07f, 0.5f, 0.02f, 0.001f };
            int[][] displacements = new int[frequency.Length][];

            int[] totalDisplacement = new int[Chunk.ChunkWidth];
            for (int i = 0; i < displacements.Length; i++)
            {
                displacements[i] = GetPerlinDisplacements(200, frequency[i], Chunk.ChunkHeight, limit[i], WorldGen._lastSeed, x1);
            }

            for (int i = 0; i < displacements.Length; i++)
            {
                for (int j = 0; j < Chunk.ChunkWidth; j++)
                {
                    totalDisplacement[j] += displacements[i][j];
                }
            }

            for (int x = 0; x < Chunk.ChunkWidth; x++)
            {
                for (int y = 0; y < Chunk.ChunkHeight; y++)
                {
                    Tile tile = new Tile();
                    tile.active(false);
                    chunkBase[x, y] = tile;
                }
            }

            for (int x = 0; x < Chunk.ChunkWidth; x++)
            {
                totalDisplacement[x] = (int) (totalDisplacement[x] / displacements.Length + 75);
                Fill(chunkBase, x, totalDisplacement[x], 1, 1, 0);
            }

            return chunkBase;
        }

        public static void Fill(Tile[,] tileArray, int x, int startingY, int width = 1, int depth = 1, ushort tile = 0)
        {
            for (int i = startingY; i < startingY + depth || i < Chunk.ChunkHeight; i++)
            {
                if(i < 0 || x < 0 || i >= 200 || x >= Chunk.ChunkWidth) continue;
                tileArray[x, i] = new Tile();
                tileArray[x, i].active(true);
                tileArray[x, i].type = TileID.Dirt;
            }
        }
    }
}
