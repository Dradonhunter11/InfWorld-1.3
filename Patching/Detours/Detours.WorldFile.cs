using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Repository.Hierarchy;
using Terraria;
using Terraria.ID;

namespace InfWorld.Patching.Detours
{
    static partial class Detours
    {
        private static void NewLoadWorldTiles(On.Terraria.IO.WorldFile.orig_LoadWorldTiles orig, BinaryReader reader, bool[] importance)
        {
			for (int i = 0; i < Main.maxTilesX; i++)
			{
				float num = (float)i / (float)Main.maxTilesX;
				Main.statusText = Lang.gen[51].Value + " " + (int)((double)num * 100.0 + 1.0) + "%";
				for (int j = 0; j < Main.maxTilesY; j++)
				{
					int num2 = -1;
					byte b;
					byte b2 = (b = 0);
					Tile tile = InfWorld.Tile[i, j];
					byte b3 = reader.ReadByte();
					if ((b3 & 1) == 1)
					{
						b2 = reader.ReadByte();
						if ((b2 & 1) == 1)
						{
							b = reader.ReadByte();
						}
					}
					byte b4;
					if ((b3 & 2) == 2)
					{
						tile.IsActive = true;
						if ((b3 & 0x20) == 32)
						{
							b4 = reader.ReadByte();
							num2 = reader.ReadByte();
							num2 = (num2 << 8) | b4;
						}
						else
						{
							num2 = reader.ReadByte();
						}
						tile.type = (ushort)num2;
						if (importance[num2])
						{
							tile.frameX = reader.ReadInt16();
							tile.frameY = reader.ReadInt16();
							if (tile.type == 144)
							{
								tile.frameY = 0;
							}
						}
						else
						{
							tile.frameX = -1;
							tile.frameY = -1;
						}
						if ((b & 8) == 8)
						{
							tile.Color = (reader.ReadByte());
						}
					}
					if ((b3 & 4) == 4)
					{
						tile.wall = reader.ReadByte();
						if (tile.wall >= 316)
						{
							tile.wall = 0;
						}
						if ((b & 0x10) == 16)
						{
							tile.WallColor = (reader.ReadByte());
						}
					}
					b4 = (byte)((b3 & 0x18) >> 3);
					if (b4 != 0)
					{
                        tile.LiquidAmount = reader.ReadByte();
						if (b4 > 1)
						{
							if (b4 == 2)
							{
                                tile.LiquidType = 1;
							}
							else
							{
								tile.LiquidType = 2;
							}
						}
					}
					if (b2 > 1)
					{
						if ((b2 & 2) == 2)
						{
							tile.RedWire = true;
						}
						if ((b2 & 4) == 4)
						{
							tile.BlueWire = true;
						}
						if ((b2 & 8) == 8)
						{
							tile.GreenWire = true;
						}
						b4 = (byte)((b2 & 0x70) >> 4);
						if (b4 != 0 && (Main.tileSolid[tile.type] || TileID.Sets.NonSolidSaveSlopes[tile.type]))
						{
							if (b4 == 1)
							{
								tile.IsHalfBlock = true;
							}
							else
							{
								tile.Slope = ((SlopeType)(b4 - 1));
							}
						}
					}
					if (b > 0)
					{
						if ((b & 2) == 2)
						{
							tile.HasActuator = true;
						}
						if ((b & 4) == 4)
						{
							tile.IsActuated = true;
						}
						if ((b & 0x20) == 32)
						{
							tile.YellowWire = true;
						}
						if ((b & 0x40) == 64)
						{
							b4 = reader.ReadByte();
							tile.wall = (ushort)((b4 << 8) | tile.wall);
							if (tile.wall >= 316)
							{
								tile.wall = 0;
							}
						}
					}
					int num3 = (byte)((b3 & 0xC0) >> 6) switch
					{
						0 => 0,
						1 => reader.ReadByte(),
						_ => reader.ReadInt16(),
					};
					if (num2 != -1)
					{
						if ((double)j <= Main.worldSurface)
						{
							if ((double)(j + num3) <= Main.worldSurface)
							{
								WorldGen.tileCounts[num2] += (num3 + 1) * 5;
							}
							else
							{
								int num4 = (int)(Main.worldSurface - (double)j + 1.0);
								int num5 = num3 + 1 - num4;
								WorldGen.tileCounts[num2] += num4 * 5 + num5;
							}
						}
						else
						{
							WorldGen.tileCounts[num2] += num3 + 1;
						}
					}
					while (num3 > 0)
					{
						j++;
						//Main.tile[i, j].CopyFrom(tile);
						num3--;
					}
				}
			}
			WorldGen.AddUpAlignmentCounts(clearCounts: true);

			for (int i = 0; i < 4200; i++)
            {
                for (int j = 0; j < 1200; j++)
                {
                    if(InfWorld.Tile[i, j] != null) return;
                }
            }

            Terraria.WorldGen.AddUpAlignmentCounts(clearCounts: true);
        }

    }
}
