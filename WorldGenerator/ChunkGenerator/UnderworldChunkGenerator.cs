using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace InfWorld.WorldGenerator.ChunkGenerator
{
    class UnderworldChunkGenerator : ChunkGenerator
    {
        public UnderworldChunkGenerator(int seed = 1337) : base(1337)
        {

        }

        public override Tile[,] Generate(int x, int y)
        {
            return GetNewTileArray(TileID.Ash);
        }
    }
}
