using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfWorld.Utils.Math;
using Microsoft.Xna.Framework;

namespace InfWorld.Utils
{
    public static class Extensions
    {
        internal static Vector2Int ToVector2I(this Vector2 self)
        {
            return new Vector2Int((int) self.X, (int) self.Y);
        }

        internal static Position2I ToPosition2I(this Vector2 self)
        {
            return new Position2I(self);
        }
    }
}
