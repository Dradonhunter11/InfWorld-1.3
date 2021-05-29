using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using InfWorld.Chunks;
using log4net;
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
            noise.SetFractalType(FastNoise.FractalType.FBM);
            noise.SetFrequency(0.05f);
            noise.SetFractalLacunarity(5f);
        }

        /*public Tile[,] Generate()
        {
            bool[,] map = new bool[Chunk.ChunkWidth, Chunk.ChunkHeight];
            for (int x = 0; x < Chunk.ChunkWidth; x++) 
            {
                float noiseX = x / 20f;
                for (int y = 0; y < Chunk.ChunkHeight; y++)
                {
                    float noiseY = y / 20f;
                    int height = (int) Math.Floor(noise.GetNoise(x, y) * 100 * 0.05f) * -1;
                    //noiseValue /= y / 5f;

                    if(height < 3)
                        map[y, x] = true;
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
                    //LogManager.GetLogger("Chunk Generator").Debug(chunkBase);
                    chunkBase[i, j] = tile;
                }
            }

            return chunkBase;
        }*/

        public Tile[,] Generate(int x1, int y1)
        {
            LogManager.GetLogger("Chunk gen").Debug($"Chunk internal position : {x1}, {y1}");
            float baseHeight = 100;
            float smoothness = 20;
            Tile[,] chunkBase = new Tile[Chunk.ChunkWidth, Chunk.ChunkHeight];
            for (int x = 0; x < Chunk.ChunkWidth; x++)
            {
                for (int y = 0; y < Chunk.ChunkHeight; y++)
                {
                    Tile tile = new Tile();
                    tile.active(false);
                    chunkBase[x, y] = tile;
                }
                int height = (int)Math.Round((Chunk.ChunkHeight - 1 - baseHeight) * noise.GetNoise((x / smoothness) + (x1 * Chunk.ChunkWidth), 0));
                //for (int y = Chunk.ChunkHeight - height - 1; y >= 0; y--)
                for (int y = 0; y < height; y++)
                { 
                    if (y >= Chunk.ChunkHeight)
                    {
                        continue;
                    }

                    Tile tile = new Tile(); 
                    //if (height < 3) 
                    {
                        
                        tile.active(true);
                        tile.type = TileID.AdamantiteBeam;
                    }
                    //else
                    {
                        //tile.active(false);
                    }
                    chunkBase[x, Chunk.ChunkHeight - 1 - y] = tile;
                }
            }

            return chunkBase;
        }
    }
}
