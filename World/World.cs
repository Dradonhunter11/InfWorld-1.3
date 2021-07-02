﻿using System;
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
    // TODO : Abstract it for easier support of multiple world type
    /// <summary>
    /// Class containing everything about the world
    /// </summary>
    [Serializable]
    public class World : ISerializable
    {
        private int m_viewRange = 25;
        //private UndergroundChunkGenerator undergroundGenerator;

        /// <summary>
        /// string = GeneratorName
        /// Value = ChunkGenerator
        /// </summary>
        private Dictionary<string, ChunkGenerator> m_chunkGenerators;

        /// <summary>
        /// List of Chunks in the World
        /// </summary>
        private readonly List2D<Chunk> m_chunks;

        private readonly int _worldseed;
        public int Seed => _worldseed;

        /// <summary>
        /// Initializes a new instance of this class
        /// </summary>
        public World()
        {
            m_chunks = new List2D<Chunk>();
            m_chunkGenerators = new Dictionary<string, ChunkGenerator>()
            {
                ["Sky"] = new SkyChunkGenerator(_worldseed),
                ["Surface"] = new SurfaceChunkGenerator(_worldseed),
                ["Underground"] = new UndergroundChunkGenerator(_worldseed),
                ["Underworld"] = new UnderworldChunkGenerator(_worldseed)
            };
        }

        public World(int seed)
        {
            m_chunks = new List2D<Chunk>();
            m_chunkGenerators = new Dictionary<string, ChunkGenerator>()
            {
                ["sky"] = new SkyChunkGenerator(_worldseed),
                ["surface"] = new SurfaceChunkGenerator(_worldseed)
            };
        }

        /// <summary>
        /// Gets a tile at the world position
        /// </summary>
        /// <param name="pos">World position</param>
        /// <returns>The tile at the specified position</returns>
        public Tile this[Vector2 pos]
        {
            get => this[new Position2I(pos)];
            set => this[new Position2I(pos)] = value;
        }

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
            var intPost = new Position2I((int)position.X, (int)position.Y);
            if (m_chunks[intPost] != null)
            {
                return m_chunks[intPost];
            }

            if (m_chunks[intPost] == null)
            {
                m_chunks[intPost] = new Chunk(intPost.ToVector2(), SelectChunkGenerator(intPost.Y).Generate(intPost.X, intPost.Y));
                InfWorld.Map.AddMapSection(m_chunks[intPost]);
                return m_chunks[intPost];
            }

            return null;
        }

        public void Update(Player player)
        {
            for (int x = (int)((player.position.X / 16f) - m_viewRange); x > (player.position.X / 16f) + m_viewRange; x++)
            {
                for (int y = (int)((player.position.Y / 16f) - m_viewRange); y > (player.position.Y / 16f) + m_viewRange; y++)
                {
                    Vector2 pos = new Vector2(x, y);
                    pos.X = (float)(Math.Floor(pos.X / Chunk.ChunkWidth));
                    pos.Y = (float)(Math.Floor(pos.Y / Chunk.ChunkHeight));
                    LogManager.GetLogger("Chunk").Debug(pos);
                    Chunk chunk = FindChunk(pos);
                    if (chunk != null) continue;

                    chunk = new Chunk(pos, SelectChunkGenerator((int) pos.Y).Generate((int)Math.Floor(pos.X), (int)Math.Floor(pos.Y)));
                    InfWorld.Map.AddMapSection(chunk);
                    m_chunks[(int)Math.Floor(pos.X), (int)Math.Floor(pos.Y)] = chunk;
                }
            }
        }

        public ChunkGenerator SelectChunkGenerator(int yLevel)
        {
            switch (yLevel)
            {
                case 0:
                    return m_chunkGenerators["Sky"];
                case 1:
                    return m_chunkGenerators["Surface"];
                case 5:
                    return m_chunkGenerators["Underworld"];
                default:
                    return m_chunkGenerators["Underground"];
            }
        }
    }
}
