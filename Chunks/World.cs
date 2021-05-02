using System;
using System.Runtime.Serialization;
using log4net;
using Microsoft.Xna.Framework;
using Terraria;

namespace InfWorld.Chunks
{
	/// <summary>
	/// Class containing everything about the world
	/// </summary>
	[Serializable]
	public class World : ISerializable
	{
		/// <summary>
		/// List of Chunks in the World
		/// </summary>
		private readonly List2D<Chunk> _chunks;

		/// <summary>
		/// Initializes a new instance of this class
		/// </summary>
		public World()
		{
			_chunks = new List2D<Chunk>();
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
			get => GetChunkFromTilePos(x, y)[x % Chunk.ChunkSize, y % Chunk.ChunkSize];
			set => GetChunkFromTilePos(x, y)[x % Chunk.ChunkSize, y % Chunk.ChunkSize] = value;
		}

		/// <summary>
		/// Gets a chunk from a specified tile position
		/// </summary>
		/// <param name="x">X position</param>
		/// <param name="y">Y position</param>
		/// <returns>The chunk at the specified tile position</returns>
		public Chunk GetChunkFromTilePos(int x, int y)
		{
			return GetChunkFromChunkPos(x / Chunk.ChunkSize, y / Chunk.ChunkSize);
		}

		/// <summary>
		/// Gets a chunk from a specified chunk position
		/// </summary>
		/// <param name="x">X position</param>
		/// <param name="y">Y position</param>
		/// <returns>The chunk at the specified chunk position</returns>
        public Chunk GetChunkFromChunkPos(int x, int y)
        {
            Chunk chunk = _chunks[x, y];
            if (chunk == null)
            {
                chunk = new Chunk();
                _chunks[x, y] = chunk;
                if (!Main.gameMenu)
                {
                    chunk.Generate(x, y);
                }
            }

            return chunk;
        }

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("list", _chunks);
		}

        public static FastNoise terrainNoise = new FastNoise(WorldGen._genRandSeed);
	}
}
