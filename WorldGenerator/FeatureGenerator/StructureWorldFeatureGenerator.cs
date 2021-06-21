using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;

namespace InfWorld.WorldGenerator.FeatureGenerator
{
    abstract class StructureWorldFeatureGenerator : WorldFeatureGenerator
    {
        public abstract bool CanPlace(int x, int y, out Rectangle bound);

        public sealed override bool Apply(Tile[,] tileArray, int x, int y)
        {
            Rectangle structureBound;
            if (CanPlace(x, y, out structureBound))
            {
                return true;
            }

            return false;
        }

        public abstract void InternalGenerate(Tile[,] tileArray, int x, int y);
    }
}
