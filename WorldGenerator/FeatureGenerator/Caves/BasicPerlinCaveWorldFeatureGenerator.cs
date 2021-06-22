using System;
using InfWorld.Utils.Math;
using InfWorld.World.Region;
using Terraria;

namespace InfWorld.WorldGenerator.FeatureGenerator.Caves
{
    class BasicPerlinCaveWorldFeatureGenerator : WorldFeatureGenerator
    {
        private static FastNoise caveNoise;

        public BasicPerlinCaveWorldFeatureGenerator(int seed = 1337)
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
                    float noiseValue = (float) (caveNoise.GetNoise((startingPositionX + i), (startingPositionY + j)) * ((y == 1) ? 0.9d : 1.6d));
                    if (noiseValue >= 0.0095f)
                    {
                        tileArray[i, j].active(false);
                    }
                }
            }
            return true;
        }
    }
}
