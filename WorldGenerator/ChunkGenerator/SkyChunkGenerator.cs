using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfWorld.Utils.Math;
using Terraria;

namespace InfWorld.WorldGenerator.ChunkGenerator
{
    class SkyChunkGenerator : ChunkGenerator
    {
        public SkyChunkGenerator(int seed = 1337) : base(1337)
        {
            
        }

        public override Tile[,] SetupTerrain(int x, int y)
        {
            return GetNewTileArray();
        }
    }
}
