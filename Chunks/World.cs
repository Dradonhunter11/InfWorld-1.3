using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using InfWorld.WorldGenerator.ChunkGenerator;
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
        private int ViewRange = 30;
        private ChunkGenerator generator;

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
            generator = new ChunkGenerator(/*Main.rand.Next()*/);
        }

        public World(int seed)
        {
            _chunks = new List2D<Chunk>();
            generator = new ChunkGenerator(seed);
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
            get => FindChunk(x, y)[x % Chunk.ChunkWidth, y % Chunk.ChunkHeight];
            set => FindChunk(x, y)[x % Chunk.ChunkWidth, y % Chunk.ChunkHeight] = value;
        }

        /// <summary>
        /// Gets a chunk from a specified tile position
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <returns>The chunk at the specified tile position</returns>
        public Chunk GetChunkFromTilePos(int x, int y)
        {
            return FindChunk(new Vector2(x / (float)Chunk.ChunkWidth, y / (float)Chunk.ChunkHeight));
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
            info.AddValue("list", _chunks);
        }

        public Chunk FindChunk(Player player)
        {
            return FindChunk(player.position / 16f);
        }

        public Chunk FindChunk(int x, int y)
        {
            return FindChunk(new Vector2(x, y));
        }

        public Chunk FindChunk(Vector2 position)
        {
            var intPost = new Position2I((int) Math.Floor(position.X / (float)Chunk.ChunkWidth),
                (int) Math.Floor(position.Y / (float)Chunk.ChunkHeight));
            if (_chunks[intPost] != null)
            {
                return _chunks[intPost];
            }

            if (_chunks[intPost] == null)
            {
                LogManager.GetLogger("no").Debug(_chunks.Count);
                _chunks[intPost] = new Chunk(position, generator.Generate(intPost.X, intPost.Y));
                return _chunks[intPost];
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
            for (float x = (player.position.X / 16f) - ViewRange; x > (player.position.Y / 16f) - ViewRange; x++)
            {
                for (float y = (player.position.X / 16f) - ViewRange; y > (player.position.Y / 16f) - ViewRange; y++)
                {
                    Vector2 pos = new Vector2(x, y);
                    Console.Write(pos);
                    pos.X = (float)(Math.Floor(pos.X / Chunk.ChunkWidth) * Chunk.ChunkWidth);
                    pos.Y = (float)(Math.Floor(pos.Y / Chunk.ChunkWidth) * Chunk.ChunkWidth);

                    Chunk chunk = FindChunk(player.position);
                    if (chunk != null) continue;

                    chunk = new Chunk(pos, generator.Generate((int)Math.Floor(pos.X), (int)Math.Floor(pos.Y)));
                    _chunks[(int)Math.Floor(pos.X), (int)Math.Floor(pos.Y)] = chunk;
                }
            }
        }
    }
}
