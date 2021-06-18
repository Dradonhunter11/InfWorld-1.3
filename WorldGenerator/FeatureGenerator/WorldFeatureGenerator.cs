using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace InfWorld.WorldGenerator.FeatureGenerator
{
    public abstract class WorldFeatureGenerator
    {
        public static readonly World.World World = InfWorld.Tile;

        public abstract bool CanPlace(int x, int y, out Rectangle bound);

        public abstract bool Generate(int x, int y);

        internal void InternalGenerate(int x, int y)
        {
            Rectangle structureBound;
            if (CanPlace(x, y, out structureBound))
            { 
                
            }
        }
    }
}
