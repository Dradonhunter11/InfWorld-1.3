using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using InfWorld.Utils;
using InfWorld.Utils.Math;
using InfWorld.World.Region;
using InfWorld.WorldGenerator.ChunkGenerator;
using log4net;
using Microsoft.Xna.Framework;
using Terraria;

namespace InfWorld.World
{
    /// <summary>
    /// Class containing everything about the world
    /// </summary>
    [Serializable]
    public class World : ISerializable
    {
        private int m_viewRange = 25;
        private SurfaceChunkGenerator m_generator;
        //private UndergroundChunkGenerator undergroundGenerator;

        private List<SurfaceChunkGenerator> m_chunkGenerators;

        /// <summary>
        /// List of Chunks in the World
        /// </summary>
        private readonly List2D<Chunk> m_chunks;

        /// <summary>
        /// Initializes a new instance of this class
        /// </summary>
        public World()
        {
            m_chunks = new List2D<Chunk>();
            m_generator = new SurfaceChunkGenerator(/*Main.rand.Next()*/);
        }

        public World(int seed)
        {
            m_chunks = new List2D<Chunk>();
            m_generator = new SurfaceChunkGenerator(seed);
        }

        /// <summary>
        /// Gets a tile at the world position
        /// </summary>
        /// <param name="pos">World position</param>
        /// <returns>The tile at the specified position</returns>
        /*public Tile this[Vector2 pos]
        {
            get => this[new Position2I(pos)];
            set => this[new Position2I(pos)] = value;
        }*/

        /// <summary>
        /// Gets a tile at the world position
        /// </summary>
        /// <param name="pos">World position</param>
        /// <returns>The tile at the specified position</returns>
        public Tile this[Position2I pos]
        {
            get => this[pos.X, pos.Y];
            set => this[pos.X, pos.Y] = value;
        }

        /// <summary>
        /// Gets a tile at the world position
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <returns>The tile at the specified position</returns>
        public Tile this[int x, int y]
        {
            get => FindChunk(x / Chunk.ChunkWidth, y / Chunk.ChunkHeight)[x % Chunk.ChunkWidth, y % Chunk.ChunkHeight];
            set => FindChunk(x / Chunk.ChunkWidth, y / Chunk.ChunkHeight)[x % Chunk.ChunkWidth, y % Chunk.ChunkHeight] = value;
        }

        /// <summary>
        /// Gets a chunk from a specified tile position
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <returns>The chunk at the specified tile position</returns>
        public Chunk GetChunkFromTilePos(int x, int y)
        {
            return FindChunk(new Vector2(x, y));
        }

        /// <summary>
        /// Gets a chunk from a specified chunk position
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <returns>The chunk at the specified chunk position</returns>
        public Chunk GetChunkFromChunkPos(int x, int y)
        {
            return FindChunk(x, y);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            info.AddValue("list", m_chunks);
        }

        public Chunk FindChunk(Player player)
        {
            return FindChunk(player.position / 16f / new Vector2(Chunk.ChunkWidth, Chunk.ChunkHeight));
        }

        public Chunk FindChunk(int x, int y)
        {
            return FindChunk(new Vector2(x, y));
        }

        public Chunk FindChunk(Vector2 position)
        {
            var intPost = new Position2I((int) position.X, (int)position.Y);
            //LogManager.GetLogger("no").Debug(intPost);
            //LogManager.GetLogger("no - the stacktrace").Debug(Environment.StackTrace);
            if (m_chunks[intPost] != null)
            {
                return m_chunks[intPost];
            }

            if (m_chunks[intPost] == null)
            {
                m_chunks[intPost] = new Chunk(intPost.ToVector2(), m_generator.Generate(intPost.X, intPost.Y));
                InfWorld.Map.AddMapSection(m_chunks[intPost]);
                return m_chunks[intPost];
            }
            /*for (int i = 0; i < _chunks.Count; i++)
            {
                Vector2 ppos = position;
                Vector2 cpos = _chunks.GetAtIndex(i).Value.position;
                Math.Floor(cpos / Chunk.ChunkWidth);
                if (ppos.X < cpos.X || ppos.Y < cpos.Y || ppos.X > cpos.X + Chunk.ChunkWidth || ppos.Y > cpos.Y + Chunk.ChunkHeight) continue;
                return _chunks.GetAtIndex(i).Value;
            }*/

            
            return null;
        }


        public void Update(Player player)
        {
            LogManager.GetLogger("NO AGAIN!!!").Debug(m_chunks.Count);
            for (int x = (int) ((player.position.X / 16f) - m_viewRange); x > (player.position.X / 16f) + m_viewRange; x++)
            {
                for (int y = (int) ((player.position.Y / 16f) - m_viewRange); y > (player.position.Y / 16f) + m_viewRange; y++)
                {
                    Vector2 pos = new Vector2(x, y);
                    pos.X = (float)(Math.Floor(pos.X / Chunk.ChunkWidth));
                    pos.Y = (float)(Math.Floor(pos.Y / Chunk.ChunkHeight));
                    LogManager.GetLogger("Chunk").Debug(pos);
                    Chunk chunk = FindChunk(pos);
                    if (chunk != null) continue;

                    chunk = new Chunk(pos, m_generator.Generate((int)Math.Floor(pos.X), (int)Math.Floor(pos.Y)));
                    InfWorld.Map.AddMapSection(chunk);
                    m_chunks[(int)Math.Floor(pos.X), (int)Math.Floor(pos.Y)] = chunk;
                }
            }
        }
    }
}
