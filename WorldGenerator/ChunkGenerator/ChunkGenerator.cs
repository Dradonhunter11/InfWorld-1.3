using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfWorld.World.Region;
using InfWorld.WorldGenerator.FeatureGenerator;
using Terraria;

namespace InfWorld.WorldGenerator.ChunkGenerator
{
    public abstract class ChunkGenerator
    {
        public List<WorldFeatureGenerator> FeatureGenerators = new List<WorldFeatureGenerator>();

        public int Seed;

        protected Tile[,] GetNewTileArray(int tileType = -1)
        {
            Tile[,] newTileArray = new Tile[Chunk.ChunkWidth, Chunk.ChunkHeight];
            for (int i = 0; i < Chunk.ChunkWidth; i++)
            {
                for (int j = 0; j < Chunk.ChunkHeight; j++)
                {
                    newTileArray[i, j] = new Tile();
                    newTileArray[i, j].type = (ushort) ((tileType == -1) ? 0 : tileType);
                    newTileArray[i, j].active(tileType != -1);
                    newTileArray[i, j].frameX = 0;
                    newTileArray[i, j].frameY = 0;
                }
            }

            return newTileArray;
        }

        public abstract Tile[,] Generate(int x, int y);

        public ChunkGenerator(int seed = 1337)
        {
            this.Seed = seed;
        }
    }
}
