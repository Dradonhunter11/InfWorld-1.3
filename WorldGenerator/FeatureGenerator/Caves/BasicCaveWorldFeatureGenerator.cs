using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfWorld.Utils.Math;
using InfWorld.World.Region;
using Terraria;

namespace InfWorld.WorldGenerator.FeatureGenerator
{
    class BasicCaveWorldFeatureGenerator : WorldFeatureGenerator
    {
        private static FastNoise caveNoise;

        public BasicCaveWorldFeatureGenerator(int seed = 1337)
        {
            caveNoise = new FastNoise(seed);
            caveNoise.SetFractalGain(0.07f);
            caveNoise.SetFractalOctaves(5);
            caveNoise.SetFractalLacunarity(0.003f);
            caveNoise.SetNoiseType(FastNoise.NoiseType.Simplex);
        }

        public override bool Apply(Tile[,] tileArray, int x, int y)
        {
            for (int i = 0; i < Chunk.ChunkWidth; i++)
            {
                for (int j = 0; j < Chunk.ChunkHeight; j++)
                {
                    
                }
            }
            return true;
        }
    }
}
