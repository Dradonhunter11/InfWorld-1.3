using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using InfWorld.Chunks;
using Microsoft.Xna.Framework;

namespace InfWorld.WorldGenerator.ChunkGenerator
{
    class ChunkGenerator
    {
        private FastNoise noise;

        public ChunkGenerator(int seed = 1337)
        {
            noise = new FastNoise(seed);
            noise.SetNoiseType(FastNoise.NoiseType.Simplex);
        }

        public Tile[,] Generate()
        {
            bool[,] map = new bool[Chunk.ChunkWidth, Chunk.ChunkHeight];
            for (int x = 0; x < Chunk.ChunkWidth; x++)
            {
                float noiseX = x / 20f;
                for (int y = 0; y < Chunk.ChunkHeight; y++)
                {
                    float noiseY = y / 20f;
                    float noiseValue = noise.GetNoise(noiseX, noiseY);
                    noiseValue /= y / 5f;
                    
                    if(noiseValue > 0.2f) 
                        map[x, y] = true;
                }
            }

            Tile[,] chunkBase = new Tile[Chunk.ChunkWidth, Chunk.ChunkHeight];
            for (int i = 0; i < Chunk.ChunkWidth; i++)
            {
                for (int j = 0; j < Chunk.ChunkHeight; j++)
                {
                    Tile tile = new Tile();
                    if (map[i, j])
                    {
                        tile.active(true);
                        tile.type = TileID.AdamantiteBeam;
                    }
                    else
                    {
                        tile.active(false);
                    }

                    chunkBase[i, j] = tile;
                }
            }

            return chunkBase;
        }
    }
}
