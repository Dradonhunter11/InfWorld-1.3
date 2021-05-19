using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;

namespace InfWorld.Patching.Detours
{
    static partial class Detours
    {
        private static bool OnInTileBounds(On.Terraria.Collision.orig_InTileBounds orig, int i, int i1, int lx, int ly, int hx, int hy)
        {
            return true;
        }

        private static bool OnSolidTiles(On.Terraria.Collision.orig_SolidTiles orig, int x, int endX, int y, int endY)
        {
            for (int i = x; i < endX + 1; i++)
            {
                for (int j = y; j < endY + 1; j++)
                {
                    if (InfWorld.Tile[i, j] == null) return false;

                    if (InfWorld.Tile[i, j].active() && !InfWorld.Tile[i, j].inActive() && Main.tileSolid[InfWorld.Tile[i, j].type] && !Main.tileSolidTop[InfWorld.Tile[i, j].type]) return true;
                }
            }

            return false;
        }

        private static bool CustomSolidCollision(On.Terraria.Collision.orig_SolidCollision orig, Vector2 Position, int Width, int Height)
        {
            int value = (int)(Position.X / 16f) - 1;
            int value2 = (int)((Position.X + (float)Width) / 16f) + 2;
            int value3 = (int)(Position.Y / 16f) - 1;
            int value4 = (int)((Position.Y + (float)Height) / 16f) + 2;
            /*int num = Utils.Clamp(value, 0, Main.maxTilesX - 1);
                value2 = Utils.Clamp(value2, 0, Main.maxTilesX - 1);
                value3 = Utils.Clamp(value3, 0, Main.maxTilesY - 1);
                value4 = Utils.Clamp(value4, 0, Main.maxTilesY - 1);*/
            Vector2 vector = default(Vector2);
            for (int i = value; i < value2; i++)
            {
                for (int j = value3; j < value4; j++)
                {
                    if (Terraria.Main.tile[i, j] != null && !Terraria.Main.tile[i, j].inActive() && Terraria.Main.tile[i, j].active() && Terraria.Main.tileSolid[Terraria.Main.tile[i, j].type] && !Terraria.Main.tileSolidTop[Terraria.Main.tile[i, j].type])
                    {
                        vector.X = i * 16;
                        vector.Y = j * 16;
                        int num2 = 16;
                        if (Main.tile[i, j].halfBrick())
                        {
                            vector.Y += 8f;
                            num2 -= 8;
                        }

                        if (Position.X + (float)Width > vector.X && Position.X < vector.X + 16f && Position.Y + (float)Height > vector.Y && Position.Y < vector.Y + (float)num2) return true;
                    }
                }
            }

            return false;
        }
    }
}
