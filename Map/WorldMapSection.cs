using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfWorld.World.Region;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Map;

namespace InfWorld.Map
{
    class WorldMapSection
    {
        public RenderTarget2D CachedWorldMapSection;

        public MapTile[,] MapTile;
        public readonly int SectionWidth = Chunk.ChunkWidth;
        public readonly int SectionHeight = Chunk.ChunkHeight;

        public Ref<Chunk> ChunkReference;

        public bool Loaded = true;
        private bool _hasChanged = false;



        public WorldMapSection(Chunk chunk)
        {
            MapTile = new MapTile[SectionWidth, SectionHeight];
            ChunkReference = new Ref<Chunk>(chunk);

            UpdateSection(); // Init the section

            CachedWorldMapSection = new RenderTarget2D(Main.graphics.GraphicsDevice, SectionWidth, SectionHeight);
        }

        // Will be revealed by default for now
        // public void revealSection()

        public void UpdateSection()
        {
            if (ChunkReference.Value == null)
            {
                Loaded = false;
                return;
            }

            Chunk chunk = ChunkReference.Value;
            for (int i = 0; i < SectionWidth; i++)
            {
                for (int j = 0; j < SectionHeight; j++)
                {
                    lock (chunk)
                    {
                        Tile tile = chunk[i, j];
                        MapTile mapTile = Terraria.Map.MapTile.Create(chunk[i, j].type, 0, GetMapTileColor(tile));
                        MapTile[i, j] = mapTile;
                    }
                }
            }

            MakeRenderTarget();
        }

        public void MakeRenderTarget()
        {
            lock (CachedWorldMapSection)
            {
                Color[] textureData = new Color[SectionWidth * SectionHeight];
                for (int i = 0; i < SectionWidth; i++)
                {
                    for (int j = 0; j < SectionHeight; j++)
                    {
                        MapTile tile = MapTile[i, j];
                        Color tileColor = MapHelper.GetMapTileXnaColor(ref tile);
                        textureData[i * SectionWidth + j] = tileColor;
                    }
                }
                CachedWorldMapSection.SetData(textureData);
            }
        }

        private byte GetMapTileColor(Tile tile)
        {
            if (tile == null)
            {
                return 0;
            }

            if (tile.active())
            {
                return 255;
            }

            if (tile.wall > 0)
            {
                return 127;
            }

            return 0;
        }
    }
}
