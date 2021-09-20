using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.IO;
using Terraria.Localization;
using Terraria.Utilities;
using Collision = Terraria.Collision;
using Main = Terraria.Main;
using Player = Terraria.Player;
using WorldFile = Terraria.IO.WorldFile;
using WorldGen = Terraria.WorldGen;

namespace InfWorld.Patching.Detours
{
    static partial class Detours
    {
        public static void Load()
        {
            On.Terraria.IO.WorldFile.ConvertOldTileEntities += orig => { };
            On.Terraria.IO.WorldFile.ClearTempTiles += orig => { };
            On.Terraria.Liquid.QuickWater += (orig, verbose, y, maxY) => { };
            On.Terraria.WorldGen.WaterCheck += orig => { };
            On.Terraria.Collision.InTileBounds += OnInTileBounds;
            //On.Terraria.Collision.SolidTiles += OnSolidTiles;
            //On.Terraria.Collision.SolidCollision += CustomSolidCollision;
            On.Terraria.WorldGen.clearWorld += OnClearWorld;
            On.Terraria.WorldGen.InWorld += RemoveInWorldCheck;
            On.Terraria.WorldGen.playWorldCallBack += (orig, context) =>
            {
                if (Main.rand == null)
                {
                    Main.rand = new UnifiedRandom((int)DateTime.Now.Ticks);
                }

                for (int i = 0; i < 255; i++)
                {
                    if (i != Main.myPlayer)
                    {
                        Main.player[i].active = false;
                    }
                }

                WorldGen.noMapUpdate = true;
                WorldFile.LoadWorld(Main.ActiveWorldFileData.IsCloudSave);
                if (WorldGen.loadFailed || !WorldGen.loadSuccess)
                {
                    WorldFile.LoadWorld(Main.ActiveWorldFileData.IsCloudSave);
                    if (WorldGen.loadFailed || !WorldGen.loadSuccess)
                    {
                        bool isCloudSave = Main.ActiveWorldFileData.IsCloudSave;
                        if (FileUtilities.Exists(Main.worldPathName + ".bak", isCloudSave))
                        {
                            WorldGen.worldBackup = true;
                        }
                        else
                        {
                            WorldGen.worldBackup = false;
                        }

                        if (!Main.dedServ)
                        {
                            if (WorldGen.worldBackup)
                            {
                                Main.menuMode = 200;
                            }
                            else
                            {
                                Main.menuMode = 201;
                            }

                            return;
                        }

                        if (!WorldGen.worldBackup)
                        {
                            string text = Language.GetTextValue("Error.LoadFailedNoBackup");
                            /*if (Terraria.ModLoader.IO.WorldIO.customDataFail != null)
                                {
                                    text = Terraria.ModLoader.IO.WorldIO.customDataFail.modName + " " + text;
                                    text = text + "\n" + Terraria.ModLoader.IO.WorldIO.customDataFail.InnerException;
                                }*/

                            Console.WriteLine(text);
                            return;
                        }

                        FileUtilities.Copy(Main.worldPathName, Main.worldPathName + ".bad", isCloudSave);
                        FileUtilities.Copy(Main.worldPathName + ".bak", Main.worldPathName, isCloudSave);
                        FileUtilities.Delete(Main.worldPathName + ".bak", isCloudSave);
                        //Terraria.ModLoader.IO.WorldIO.LoadDedServBackup(Main.worldPathName, isCloudSave);
                        WorldFile.LoadWorld(Main.ActiveWorldFileData.IsCloudSave);
                        if (WorldGen.loadFailed || !WorldGen.loadSuccess)
                        {
                            WorldFile.LoadWorld(Main.ActiveWorldFileData.IsCloudSave);
                            if (WorldGen.loadFailed || !WorldGen.loadSuccess)
                            {
                                FileUtilities.Copy(Main.worldPathName, Main.worldPathName + ".bak", isCloudSave);
                                FileUtilities.Copy(Main.worldPathName + ".bad", Main.worldPathName, isCloudSave);
                                FileUtilities.Delete(Main.worldPathName + ".bad", isCloudSave);
                                //Terraria.ModLoader.IO.WorldIO.RevertDedServBackup(Main.worldPathName, isCloudSave);
                                string text2 = Language.GetTextValue("Error.LoadFailed");
                                /*if (Terraria.ModLoader.IO.WorldIO.customDataFail != null)
                                    {
                                        text2 = Terraria.ModLoader.IO.WorldIO.customDataFail.modName + " " + text2;
                                        text2 = text2 + "\n" +
                                                Terraria.ModLoader.IO.WorldIO.customDataFail.InnerException;
                                    }*/

                                Console.WriteLine(text2);
                                return;
                            }
                        }
                    }
                }

                if (Main.mapEnabled)
                {
                    Main.Map.Load();
                }

                if (Main.netMode != 2)
                {
                    if (Main.sectionManager == null) Main.sectionManager = new WorldSections(Main.maxTilesX / 200, Main.maxTilesY / 150);
                    Main.sectionManager.SetAllFramesLoaded();
                }

                while (Main.loadMapLock)
                {
                    float num = (float)Main.loadMapLastX / (float)Main.maxTilesX;
                    Main.statusText = Lang.gen[68].Value + " " + (int)(num * 100f + 1f) + "%";
                    Thread.Sleep(0);
                    if (!Main.mapEnabled)
                    {
                        break;
                    }
                }

                if (Main.gameMenu)
                {
                    Main.gameMenu = false;
                }

                if (Main.netMode == 0 && Main.anglerWhoFinishedToday.Contains(Main.player[Main.myPlayer].name))
                {
                    Main.anglerQuestFinished = true;
                }

                Main.OnTickForInternalCodeOnly += FinishPlayWorld;
            };
            On.Terraria.IO.WorldFile.LoadWorldTiles += NewLoadWorldTiles;
            On.Terraria.Main.ClampScreenPositionToWorld += OnClampScreenPositionToWorld;
            //On.Terraria.Main.DrawMap += OnDrawMap;
            On.Terraria.Player.BordersMovement += OnBordersMovement;
        }
    }
}
