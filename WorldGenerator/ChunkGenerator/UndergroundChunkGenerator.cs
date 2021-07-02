using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfWorld.WorldGenerator.FeatureGenerator;
using InfWorld.WorldGenerator.FeatureGenerator.Caves;
using InfWorld.WorldGenerator.FeatureGenerator.WorldGen;
using Terraria.ID;
using Terraria;

namespace InfWorld.WorldGenerator.ChunkGenerator
{
    class UndergroundChunkGenerator : ChunkGenerator
    {
        public UndergroundChunkGenerator(int seed = 1337) : base(1337)
        {
            FeatureGenerators.Add(new BasicPerlinCaveWorldFeatureGenerator(seed));
            FeatureGenerators.Add(new BasicPerlinCaveWorldFeatureGenerator(seed+1));
            FeatureGenerators.Add(new BasicCellularCaveWorldFeatureGenerator(seed));
            FeatureGenerators.Add(new OreWorldGen(seed));
        }

        public override Tile[,] SetupTerrain(int x, int y)
        {
            return GetNewTileArray(TileID.Stone);
        }
    }
}
