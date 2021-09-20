using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfWorld.Utils.Math;
using InfWorld.World.Region;
using Terraria;

namespace InfWorld.WorldGenerator.FeatureGenerator.Caves
{
    class BasicCellularCaveWorldFeatureGenerator : WorldFeatureGenerator
    {
        private static FastNoise caveNoise;

        public BasicCellularCaveWorldFeatureGenerator(int seed = 1337)
        {
            caveNoise = new FastNoise(seed);
            caveNoise.SetFrequency(0.023f);
            caveNoise.SetFractalOctaves(1);
            caveNoise.SetFractalGain(2f);
            caveNoise.SetFractalLacunarity(0f);
            caveNoise.SetFractalWeightedStrength(0.5f);
            caveNoise.SetCellularDistanceFunction(FastNoise.CellularDistanceFunction.Hybrid);
            caveNoise.SetCellularReturnType(FastNoise.CellularReturnType.Distance2Add);
            caveNoise.SetCellularJitter(1.4f);
            //caveNoise.SetFractalPingPongStrength(4.9f);

            caveNoise.SetFractalType(FastNoise.FractalType.FBm);
            caveNoise.SetNoiseType(FastNoise.NoiseType.Cellular);
        }

        public override bool Apply(Tile[,] tileArray, int x, int y)
        {
            if (y >= 1) return false;
            int startingPositionX = x * Chunk.ChunkWidth;
            int startingPositionY = y * Chunk.ChunkHeight;
            for (int i = 0; i < Chunk.ChunkWidth; i++)
            {
                for (int j = 0; j < Chunk.ChunkHeight; j++)
                {
                    float noiseValue = (float)(caveNoise.GetNoise((startingPositionX + i), (startingPositionY + j)) * ((y == 1) ? 0.9d : 1.6d));
                    if (noiseValue >= 0.5f)
                    {
                        tileArray[i, j].IsActive = (false);
                    }
                }
            }
            return true;
        }
    }
}
