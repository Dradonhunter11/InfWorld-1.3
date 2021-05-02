using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace InfWorld.Chunks
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
		public const int ChunkSize = 200;

		internal static Dictionary<int, int> SurfaceLevel;
		internal static int HighestLevel = 300;

		public bool Loaded = false;

		/// <summary>
		/// The list of tiles
		/// </summary>
		private readonly List2D<Tile> m_tiles;

		/// <summary>
		/// Initializes a new instance of this class
		/// </summary>
		public Chunk()
		{
            m_tiles = new List2D<Tile>();
            for (int i = 0; i < 200; i++)
            {
				for (int j = 0; j < 200; j++)
                {
                    m_tiles[i, j] = new Tile();
                }
			}
			InstantiateNewChunk();
		}

		public void Generate(int x, int y)
        {
            float[] freq = { 0.077f, 0.1f, 0.2f };
			float[] limit = { 0.3f, 0.05f, 0.02f };
			int[][] displacements = new int[freq.Length][];
			for (int i = 0; i < freq.Length; i++)
			{
				displacements[i] = GetPerlinDisplacements(200, freq[i], 150, limit[i], WorldGen._lastSeed);
			}

			int[] totalDisplacements = new int[200];

			for (int i = 0; i < displacements.Length; i++)
			{
				for (int j = 0; j < 200; j++)
				{
					totalDisplacements[j] += displacements[i][j];
				}
			}

			SurfaceLevel = new Dictionary<int, int>();
			if (true)
			{
				for (int i = 0; i < 200; i++)
				{
					totalDisplacements[i] = (int)(totalDisplacements[i] / displacements.Length + (Main.maxTilesY - 125));
					SurfaceLevel[i] = totalDisplacements[i];

					if (totalDisplacements[i] < HighestLevel || HighestLevel == 0)
					{
						HighestLevel = totalDisplacements[i];
					}

					int dirtDepth = WorldGen.genRand.Next(10, 15);
					Fill(x * 200 + i, totalDisplacements[i], 1, dirtDepth, (ushort)TileID.Dirt);
					Fill(x * 200 + i, totalDisplacements[i] + dirtDepth, 1, 200, TileID.Stone);
					FillAir(x * 200 + i, 0, 1, totalDisplacements[i]);
				}
			}
			else
			{
				for (int i = 0; i < Main.maxTilesX; i++)
				{
					totalDisplacements[i] = (int)(totalDisplacements[i] / displacements.Length + (Main.maxTilesY - 225));
					SurfaceLevel[i] = totalDisplacements[i];

					if (totalDisplacements[i] < HighestLevel || HighestLevel == 0)
					{
						HighestLevel = totalDisplacements[i];
					}

					int dirtDepth = WorldGen.genRand.Next(10, 15);
					Fill(i, totalDisplacements[i], 1, dirtDepth, TileID.Dirt);
				}
			}


			for (int i = 0; i < 200; i++)
			{
				if (WorldGen.genRand.Next(20) == 0)
					WorldGen.TileRunner(x * 200 + i, SurfaceLevel[i] + WorldGen.genRand.Next(20, 50), WorldGen.genRand.Next(14, 20), WorldGen.genRand.Next(20, 28), -1, true);
			}
		}

		private void BaseTerrain(float[] freq, float[] limit, bool top = false)
		{
			int[][] displacements = new int[freq.Length][];
			for (int i = 0; i < freq.Length; i++)
			{
				displacements[i] = GetPerlinDisplacements(Main.maxTilesX, freq[i], Main.maxTilesY - 150, limit[i], WorldGen._lastSeed);
			}

			int[] totalDisplacements = new int[200];

			for (int i = 0; i < displacements.Length; i++)
			{
				for (int j = 0; j < 200; j++)
				{
					totalDisplacements[j] += displacements[i][j];
				}
			}


		}

		public static int[] GetPerlinDisplacements(int displacementCount, float frequency, int maxLimit, float multiplier, int seed)
		{
			FastNoise noise = World.terrainNoise;
			noise.SetNoiseType(FastNoise.NoiseType.Perlin);
			noise.SetFrequency(frequency);
			noise.SetFractalType(FastNoise.FractalType.RigidMulti);

			int[] displacements = new int[displacementCount];

			for (int x = 0; x < displacementCount; x++)
				displacements[x] = (int)Math.Floor(noise.GetNoise(x, x) * maxLimit * multiplier);

			return displacements;
		}

		internal void Fill(int x, int startingY, int one, int depth, ushort tile)
		{
			for (int i = startingY; i < startingY + depth; i++)
			{
				WorldGen.PlaceTile(x, i, tile, false, true);
			}
		}

		internal void FillAir(int x, int depth, int one, int startingY)
		{
			for (int i = startingY; i < startingY + depth; i++)
			{
				m_tiles[x, i].active(false);
			}
		}

		private void InstantiateNewChunk()
		{
			for (int i = 0; i < ChunkSize; i++)
			{
				for (int j = 0; j < ChunkSize; j++)
				{
					m_tiles[i, j] = new Tile();
				}
			}
		}

		/// <summary>
		/// Get the tile at the specified position
		/// </summary>
		/// <param name="pos">Position of the tile to get</param>
		/// <returns>The tile at the specified position</returns>
		public Tile this[Vector2 pos]
		{
			get => this[new Position2I(pos)];
			set => this[new Position2I(pos)] = value;
		}

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
