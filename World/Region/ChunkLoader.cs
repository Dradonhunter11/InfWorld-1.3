using System.IO;
using Terraria;

namespace InfWorld.World.Region
{
    class ChunkLoader
    {
        private const string DefaultFileName = "region_{0}_{1}.treg";

        public Chunk ProvideChunk(int regionX, int regionY)
        {
            return null;
        }

        public void QueueChunkUnload()
        {

        }

        public void SaveChunk(Chunk chunk)
        {

        }

        public Chunk LoadChunk(int regX, int regY)
        {
            string path = Path.Combine(Main.WorldPath, Main.ActiveWorldFileData.Name, "Chunk",
                string.Format(DefaultFileName, regX, regY));
            if (!File.Exists(path))
            {
                Chunk newChunk = ProvideChunk(regX, regY);
                SaveChunk(newChunk);
                return LoadChunk(regX, regY);
            }

            if (File.Exists(path))
            {
                using (MemoryStream stream = new MemoryStream(File.ReadAllBytes(path)))
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    
                }
            }

            
            return null;
        }
    }
}
