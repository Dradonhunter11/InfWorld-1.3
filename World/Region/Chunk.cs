using System;
using System.Runtime.Serialization;
using InfWorld.Utils;
using InfWorld.Utils.Math;
using Microsoft.Xna.Framework;
using Terraria;

namespace InfWorld.World.Region
{ 
    /// <summary>
	/// A chunk of tiles in the world
	/// </summary>
	[Serializable]
	public class Chunk : ISerializable
	{
		/// <summary>
		/// The size of every chunk
		/// </summary>
		public static readonly int ChunkWidth = 200;
        public static readonly int ChunkHeight = 200;
        public Vector2 Position;


		public bool Loaded = false;
        public bool Generated = false;

		/// <summary>
		/// The list of tiles
		/// </summary>
		private readonly List2D<Tile> m_tiles;

		/// <summary>
		/// Initializes a new instance of this class
		/// </summary>
		public Chunk(Vector2 position, Tile[,] chunkData)
        {
            this.Position = position;
            m_tiles = new List2D<Tile>();
            for (int i = 0; i < ChunkWidth; i++)
            {
                for (int j = 0; j < ChunkHeight; j++)
                {
                    m_tiles[i, j] = chunkData[i, j];
                    if (m_tiles[i, j] == null)
                    {
                        m_tiles[i, j] = new Tile();
                        m_tiles[i, j].active(false);

                    }
                }
            }
        }

        /// <summary>
		/// Get the tile at the specified position
		/// </summary>
		/// <param name="pos">Position of the tile to get</param>
		/// <returns>The tile at the specified position</returns>
		/*public Tile this[Vector2 pos]
		{
			get => this[new Position2I(pos)];
			set => this[new Position2I(pos)] = value;
		}*/

		/// <summary>
		/// Get the tile at the specified position
		/// </summary>
		/// <param name="pos">Position of the tile to get</param>
		/// <returns>The tile at the specified position</returns>
		public Tile this[Position2I pos]
		{
			get => this[pos.X, pos.Y];
			set => this[pos.X, pos.Y] = value;
		}

		/// <summary>
		/// Get the tile at the specified position
		/// </summary>
		/// <param name="x">X position of the tile</param>
		/// <param name="y">Y position of the tile</param>
		/// <returns>The tile at the specified position</returns>
		public Tile this[int x, int y]
		{
			get => m_tiles[x, y];
			set => m_tiles[x, y] = value;
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("tile", m_tiles);
		}
	}
}
