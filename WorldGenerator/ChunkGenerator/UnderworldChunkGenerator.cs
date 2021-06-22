using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfWorld.WorldGenerator.FeatureGenerator;
using InfWorld.WorldGenerator.FeatureGenerator.Caves;
using Terraria;
using Terraria.ID;

namespace InfWorld.WorldGenerator.ChunkGenerator
{
    class UnderworldChunkGenerator : ChunkGenerator
    {
        public UnderworldChunkGenerator(int seed = 1337) : base(1337)
        {
            FeatureGenerators.Add(new BasicPerlinCaveWorldFeatureGenerator(seed));
            FeatureGenerators.Add(new BasicCellularCaveWorldFeatureGenerator(seed));
        }

        public override Tile[,] SetupTerrain(int x, int y)
        {
            return GetNewTileArray(TileID.Ash);
        }
    }
}
