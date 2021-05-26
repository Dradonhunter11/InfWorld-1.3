using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Repository.Hierarchy;
using Terraria;

namespace InfWorld.Patching.Detours
{
    static partial class Detours
    {
        private static void NewLoadWorldTiles(On.Terraria.IO.WorldFile.orig_LoadWorldTiles orig, BinaryReader reader, bool[] importance)
        {
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                //LogManager.GetLogger("Line").Info(19 + $" {Main.maxTilesX}");
                float num = (float)i / (float)Main.maxTilesX;
                Main.statusText = Lang.gen[51].Value + " " + (int)((double)num * 100.0 + 1.0) + "%";
                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    int num2 = -1;
                    byte b;
                    byte b2 = b = 0;
                    //LogManager.GetLogger("Line").Info(26 + $" {InfWorld.Tile[i, j]}");
                    Tile t = new Tile();
                    //LogManager.GetLogger("Line").Info(28 + $" {t}");
                    byte b3 = reader.ReadByte();
                    if ((b3 & 1) == 1)
                    {
                        b2 = reader.ReadByte();
                        if ((b2 & 1) == 1) b = reader.ReadByte();

                    }

                    byte b4;
                    if ((b3 & 2) == 2)
                    {
                        t.active(active: true);
                        if ((b3 & 0x20) == 32)
                        {
                            b4 = reader.ReadByte();
                            num2 = reader.ReadByte();
                            num2 = ((num2 << 8) | b4);
                        }
                        else
                        {
                            num2 = reader.ReadByte();
                        }

                        t.type = (ushort)num2;
                        if (importance[num2])
                        {
                            t.frameX = reader.ReadInt16();
                            t.frameY = reader.ReadInt16();
                            if (t.type == 144) t.frameY = 0;
                        }
                        else
                        {
                            t.frameX = -1;
                            t.frameY = -1;
                        }

                        if ((b & 8) == 8) t.color(reader.ReadByte());
                    }

                    if ((b3 & 4) == 4)
                    {
                        t.wall = reader.ReadByte();
                        if ((b & 0x10) == 16) t.wallColor(reader.ReadByte());
                    }

                    b4 = (byte)((b3 & 0x18) >> 3);
                    if (b4 != 0)
                    {
                        t.liquid = reader.ReadByte();
                        if (b4 > 1)
                        {
                            if (b4 == 2)
                                t.lava(lava: true);
                            else
                                t.honey(honey: true);
                        }
                    }

                    if (b2 > 1)
                    {
                        if ((b2 & 2) == 2) t.wire(wire: true);

                        if ((b2 & 4) == 4) t.wire2(wire2: true);

                        if ((b2 & 8) == 8) t.wire3(wire3: true);

                        b4 = (byte)((b2 & 0x70) >> 4);
                        if (b4 != 0 && Main.tileSolid[t.type])
                        {
                            if (b4 == 1)
                                t.halfBrick(halfBrick: true);
                            else
                                t.slope((byte)(b4 - 1));
                        }
                    }

                    if (b > 0)
                    {
                        if ((b & 2) == 2) t.actuator(actuator: true);

                        if ((b & 4) == 4) t.inActive(inActive: true);

                        if ((b & 0x20) == 32) t.wire4(wire4: true);
                    }

                    int num3;
                    switch ((byte)((b3 & 0xC0) >> 6))
                    {
                        case 0:
                            num3 = 0;
                            break;
                        case 1:
                            num3 = reader.ReadByte();
                            break;
                        default:
                            num3 = reader.ReadInt16();
                            break;
                    }

                    if (num2 != -1)
                    {
                        if ((double)j <= Main.worldSurface)
                        {
                            if ((double)(j + num3) <= Main.worldSurface)
                            {
                                Terraria.WorldGen.tileCounts[num2] += (num3 + 1) * 5;
                            }
                            else
                            {
                                int num4 = (int)(Main.worldSurface - (double)j + 1.0);
                                int num5 = num3 + 1 - num4;
                                Terraria.WorldGen.tileCounts[num2] += num4 * 5 + num5;
                            }
                        }
                        else
                        {
                            Terraria.WorldGen.tileCounts[num2] += num3 + 1;
                        }
                    }

                    while (num3 > 0)
                    {
                        j++;
                        if (t != null)
                        {
                            //LogManager.GetLogger("155").Debug(InfWorld.Tile[i, j] + $"(X : {i} Y : {j})");
                            //LogManager.GetLogger("156").Debug(t);
                            //InfWorld.Tile[i, j].CopyFrom(t);
                        }

                        num3--;
                    }
                }
            }

            Terraria.WorldGen.AddUpAlignmentCounts(clearCounts: true);
        }

    }
}
