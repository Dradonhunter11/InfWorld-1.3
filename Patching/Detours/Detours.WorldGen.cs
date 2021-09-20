using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Terraria;
using Terraria.IO;
using Terraria.Localization;
using Terraria.Social;
using Terraria.Utilities;


namespace InfWorld.Patching.Detours
{
    static partial class Detours
    {
        private static bool RemoveInWorldCheck(On.Terraria.WorldGen.orig_InWorld orig, int i, int i1, int fluff) => true;

        private static void OnClearWorld(On.Terraria.WorldGen.orig_clearWorld orig)
        {
            return;
        }

		public static void LoadWorld(bool loadFromCloud)
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
						string message = Language.GetTextValue("Error.LoadFailedNoBackup");
						/*if (WorldIO.customDataFail != null)
						{
							message = WorldIO.customDataFail.modName + " " + message;
							message = message + "\n" + WorldIO.customDataFail.InnerException;
						}*/
						Console.WriteLine(message);
						return;
					}
					FileUtilities.Copy(Main.worldPathName, Main.worldPathName + ".bad", isCloudSave);
					FileUtilities.Copy(Main.worldPathName + ".bak", Main.worldPathName, isCloudSave);
					FileUtilities.Delete(Main.worldPathName + ".bak", isCloudSave);
					//WorldIO.LoadDedServBackup(Main.worldPathName, isCloudSave);
					WorldFile.LoadWorld(Main.ActiveWorldFileData.IsCloudSave);
					if (WorldGen.loadFailed || !WorldGen.loadSuccess)
					{
						WorldFile.LoadWorld(Main.ActiveWorldFileData.IsCloudSave);
						if (WorldGen.loadFailed || !WorldGen.loadSuccess)
						{
							FileUtilities.Copy(Main.worldPathName, Main.worldPathName + ".bak", isCloudSave);
							FileUtilities.Copy(Main.worldPathName + ".bad", Main.worldPathName, isCloudSave);
							FileUtilities.Delete(Main.worldPathName + ".bad", isCloudSave);
							//WorldIO.RevertDedServBackup(Main.worldPathName, isCloudSave);
							string message2 = Language.GetTextValue("Error.LoadFailed");
							/*if (WorldIO.customDataFail != null)
							{
								message2 = WorldIO.customDataFail.modName + " " + message2;
								message2 = message2 + "\n" + WorldIO.customDataFail.InnerException;
							}*/
							Console.WriteLine(message2);
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
		}

		internal static void FinishPlayWorld()
        {
			Main.OnTickForInternalCodeOnly -= FinishPlayWorld;
			Main.player[Main.myPlayer].Spawn(PlayerSpawnContext.SpawningIntoWorld);
            Main.player[Main.myPlayer].Update(Main.myPlayer);
            Main.ActivePlayerFileData.StartPlayTimer();
            WorldGen._lastSeed = Main.ActiveWorldFileData.Seed;
            Player.Hooks.EnterWorld(Main.myPlayer);
            WorldFile.SetOngoingToTemps();
            //Main.PlaySound(11);
            Main.resetClouds = true;
            WorldGen.noMapUpdate = false;
        }
    }
}
