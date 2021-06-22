using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfWorld.Utils.Math;
using InfWorld.World.Region;
using Terraria;
using Terraria.ID;

namespace InfWorld.WorldGenerator.FeatureGenerator.Caves
{
    class SurfaceCarverWorldFeatureGenerator : WorldFeatureGenerator
    {
        private static FastNoise caveNoise;

        public SurfaceCarverWorldFeatureGenerator(int seed = 1337)
        {
            caveNoise = new FastNoise(seed);
            caveNoise.SetFrequency(0.02f);
            caveNoise.SetFractalOctaves(2);
            caveNoise.SetFractalGain(1.6f);
            caveNoise.SetFractalLacunarity(1.6f);
            caveNoise.SetFractalWeightedStrength(1.3f);
            //caveNoise.SetFractalPingPongStrength(4.9f);

            caveNoise.SetFractalType(FastNoise.FractalType.FBm);
            caveNoise.SetNoiseType(FastNoise.NoiseType.Perlin);
        }

        public override bool Apply(Tile[,] tileArray, int x, int y)
        {
            int startingPositionX = x * Chunk.ChunkWidth;
            int startingPositionY = y * Chunk.ChunkHeight;
            for (int i = 0; i < Chunk.ChunkWidth; i++)
            {
                for (int j = 0; j < Chunk.ChunkHeight; j++)
                {
                    float noiseValue = (float)(caveNoise.GetNoise((startingPositionX + i), (startingPositionY + j)));
                    if (noiseValue >= 0.1f && tileArray[i, j].active())
                    {
                        tileArray[i, j].active(false);
                        tileArray[i, j].wall = WallID.Dirt;
                    }
                }
            }
            return true;
        }
    }
}
