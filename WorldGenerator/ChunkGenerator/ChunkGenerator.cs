using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfWorld.WorldGenerator.FeatureGenerator;
using Terraria;

namespace InfWorld.WorldGenerator.ChunkGenerator
{
    public abstract class ChunkGenerator
    {
        public List<WorldFeatureGenerator> FeatureGenerators = new List<WorldFeatureGenerator>();

        public int Seed;

        public abstract Tile[,] Generate(int x, int y);

        public ChunkGenerator(int seed = 1337)
        {
            this.Seed = seed;
        }
    }
}
