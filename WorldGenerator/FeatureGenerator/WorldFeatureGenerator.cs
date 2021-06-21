using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;

namespace InfWorld.WorldGenerator.FeatureGenerator
{
    public abstract class WorldFeatureGenerator
    {
        public static readonly World.World World = InfWorld.Tile;

        public abstract bool Apply(Tile[,] tileArray, int x, int y);

    }
}
