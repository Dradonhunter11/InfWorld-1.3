using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfWorld.Utils;
using InfWorld.Utils.Math;
using InfWorld.World.Region;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace InfWorld.Map
{
    /// <summary>
    /// Vanilla map suck, so let's try to make 1 that does not suck (probably will)
    /// </summary>
    public sealed class WorldMap
    {
        private RenderTarget2D m_texture;
        private List2D<WorldMapSection> m_mapSection;

        private readonly int renderMapDistance = 3;

        public bool HasChanged = true;

        public WorldMap()
        {
            m_mapSection = new List2D<WorldMapSection>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">RegionX origin</param>
        /// <param name="y">RegionY origin</param>
        public void PreRender(int x, int y)
        {
            if (!HasChanged)
            {
                return;
            }

            int x1 = x - renderMapDistance;
            int y1 = y - renderMapDistance;
            int x2 = x + renderMapDistance;
            int y2 = y + renderMapDistance;

            m_texture = new RenderTarget2D(Main.graphics.GraphicsDevice, Chunk.ChunkWidth * renderMapDistance * 2,
                Chunk.ChunkHeight * renderMapDistance * 2);

            lock (m_mapSection)
            {
                Color[] textureData = new Color[(Chunk.ChunkWidth * renderMapDistance * 2) *
                                                (Chunk.ChunkHeight * renderMapDistance * 2)];
                int localX = 0;
                int localY = 0;
                for (int i = x1; i < x2; i++, localX++)
                {
                    for (int j = y1; j < y2; j++, localY++)
                    {
                        if (m_mapSection[i, j] != default(WorldMapSection) || m_mapSection[i, j] == null)
                        {
                            continue;
                        }

                        WorldMapSection section = m_mapSection[i, j];
                        var sectionRender = section.CachedWorldMapSection;
                        Color[] data = new Color[Chunk.ChunkWidth * Chunk.ChunkHeight];
                        sectionRender.GetData(data);

                        for (int k = 0; k < data.GetLength(0); k++)
                        {
                            for (int l = 0; l < data.GetLength(1); l++)
                            {
                                textureData[(localX * Chunk.ChunkWidth + k) + localY * l] = data[k * Chunk.ChunkWidth + l];
                            }
                        }
                    }
                }
                m_texture.SetData(textureData);
                //HasChanged = false;
            }
        }


        public bool AddMapSection(Chunk chunk)
        {
            Position2I position = chunk.Position.ToPosition2I();
            if (m_mapSection[position] != default(WorldMapSection) || m_mapSection[position] == null)
            {
                return false;
            }

            try
            {
                HasChanged = true;
                m_mapSection[position] = new WorldMapSection(chunk);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public bool RemoveMapSection()
        {
            return true;
        }

        public void Draw()
        {
            if(m_texture == null) return;

            var graphicDevice = Main.graphics.GraphicsDevice;

            const int width = 200;
            const int height = 200;

            int positionX = graphicDevice.Viewport.Width - 5 - Main.manaTexture.Width - width - 10;
            int positionY = graphicDevice.Viewport.Height - 5 - Main.heart2Texture.Height - 10;
            
            Rectangle mapDrawingArea = new Rectangle(positionX, positionY, width, height);
            Main.graphics.GraphicsDevice.ScissorRectangle = mapDrawingArea;

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            Main.spriteBatch.Draw(m_texture, mapDrawingArea, new Rectangle(200, 200, 200, 200), Color.White, 0f, new Vector2(200, 200), SpriteEffects.None, 1f);
            Main.spriteBatch.End();

            Main.graphics.GraphicsDevice.ScissorRectangle = graphicDevice.Viewport.Bounds;
        }
    }
}
