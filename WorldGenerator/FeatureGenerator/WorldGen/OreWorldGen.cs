using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfWorld.Utils.Math;
using InfWorld.World.Region;
using Terraria;
using Terraria.ID;

namespace InfWorld.WorldGenerator.FeatureGenerator.WorldGen
{
    class OreWorldGen : WorldFeatureGenerator
    {
        private static FastNoise caveNoise;
        private static FastNoise oreTypeNoise;
        private static int seed;

        public Dictionary<int, float> OreDictionary = new Dictionary<int, float>()
        {
            [TileID.Copper] = 0.8f,
            [TileID.Tin] = 0.7f,
            [TileID.Iron] = 0.55f,
            [TileID.Lead] = 0.45f,
            [TileID.Silver] = 0.35f,
            [TileID.Tungsten] = 0.25f,
            [TileID.Gold] = 0.20f,
            [TileID.Platinum] = 0.15f
        };

        public OreWorldGen(int seed = 1337)
        {
            OreWorldGen.seed = seed;
            caveNoise = new FastNoise(seed);
            caveNoise.SetFrequency(0.03f);
            caveNoise.SetFractalOctaves(3);
            caveNoise.SetFractalGain(0.5f);
            caveNoise.SetFractalLacunarity(1.6f);
            caveNoise.SetFractalWeightedStrength(-0.6f);
            //caveNoise.SetFractalPingPongStrength(4.9f);

            caveNoise.SetFractalType(FastNoise.FractalType.FBm);
            caveNoise.SetNoiseType(FastNoise.NoiseType.Perlin);

            oreTypeNoise = new FastNoise(seed);
            oreTypeNoise.SetFrequency(0.06f);
            oreTypeNoise.SetFractalOctaves(1);
            oreTypeNoise.SetFractalGain(0f);
            oreTypeNoise.SetFractalLacunarity(0f);
            oreTypeNoise.SetFractalWeightedStrength(0f);
            oreTypeNoise.SetCellularJitter(2.3f);
            //caveNoise.SetFractalPingPongStrength(4.9f);

            oreTypeNoise.SetFractalType(FastNoise.FractalType.FBm);
            oreTypeNoise.SetNoiseType(FastNoise.NoiseType.Cellular);
            oreTypeNoise.SetCellularDistanceFunction(FastNoise.CellularDistanceFunction.Hybrid);
            oreTypeNoise.SetCellularReturnType(FastNoise.CellularReturnType.CellValue);
        }

        public override bool Apply(Tile[,] tileArray, int x, int y)
        {
            int startingPositionX = x * Chunk.ChunkWidth;
            int startingPositionY = y * Chunk.ChunkHeight;

            //Terraria.WorldGen.TileRunner(x + 195, y + 195, 15, 5, TileID.AdamantiteBeam, true, 1f, -1f);

            

            for (int i = 0; i < Chunk.ChunkWidth; i++)
            {
                for (int j = 0; j < Chunk.ChunkHeight; j++)
                {
                    float noiseValue = (float)(caveNoise.GetNoise((startingPositionX + i), (startingPositionY + j)) * ((y == 1) ? 0.9d : 1.6d));
                    if (noiseValue >= 0.7f && tileArray[i, j].IsActive)
                    {
                        int oreToGen = GetPossibleToGenBasedOnNoise((startingPositionX + i), (startingPositionY + j));
                        if (oreToGen == -1)
                        {
                            continue;
                        }

                        tileArray[i, j].type = (ushort) oreToGen;
                    }
                }
            }
            return true;
        }

        public int GetPossibleToGenBasedOnNoise(int x, int y)
        {
            
            float noiseValue = 1f - oreTypeNoise.GetNoise(x * seed, y - seed);

            float TryGetOre = OreDictionary.Values.OrderBy(x1 => Math.Abs(noiseValue - x1)).ThenByDescending(x1 => x1).First();

            int[] getPossibleOreGen = OreDictionary.Where(i => i.Value == TryGetOre).Select(i => i.Key).ToArray();

            if (getPossibleOreGen.Length == 0)
            {
                return -1;
            }

            if (getPossibleOreGen.Length == 1)
            {
                return getPossibleOreGen[0];
            }

            float percentPerOre = 1f / getPossibleOreGen.Length;

            int oreToGen = (int) (noiseValue * percentPerOre) % getPossibleOreGen.Length;

            return getPossibleOreGen[oreToGen];

        }
    }
}
