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

        public abstract Tile[,] SetupTerrain(int x, int y);

        internal Tile[,] Generate(int x, int y, bool disableTerrain = false)
        {
            Tile[,] generate;
            if (!disableTerrain)
                generate = SetupTerrain(x, y);
            else
                generate = GetNewTileArray();
            if (generate.GetLength(0) != Chunk.ChunkWidth || generate.GetLength(1) != Chunk.ChunkHeight)
            {
                throw new Exception(
                    $"Tile array return by SetupTerrain does not match {Chunk.ChunkWidth}x{Chunk.ChunkHeight}");
            }

            foreach (var worldFeatureGenerator in FeatureGenerators)
            {
                worldFeatureGenerator.Apply(generate, x, y);
            }

            return generate;
        }

        public ChunkGenerator(int seed = 1337)
        {
            this.Seed = seed;
        }
    }
}
