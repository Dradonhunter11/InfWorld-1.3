using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;

namespace InfWorld.WorldGenerator.ChunkGenerator
{
    class UndergroundChunkGenerator : ChunkGenerator
    {
        public UndergroundChunkGenerator(int seed = 1337) : base(1337)
        {

        }

        public override Tile[,] Generate(int x, int y)
        {
            return GetNewTileArray(TileID.Stone);
        }
    }
}
