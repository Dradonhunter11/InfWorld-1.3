using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InfWorld.Map;
using InfWorld.Patching;
using InfWorld.Patching.Detours;
using InfWorld.Patching.ILPatches;
using log4net;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.RuntimeDetour.HookGen;
using MonoMod.Utils;
using Newtonsoft.Json.Linq;
using Terraria;
using Terraria.GameContent.UI.States;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.World.Generation;
using WorldGen = Terraria.WorldGen;
using static Terraria.WorldGen;
using TypeAttributes = Mono.Cecil.TypeAttributes;

namespace InfWorld
{
    public class InfWorld : Mod
    {

        public static World.World Tile = new World.World();
        public static WorldMap Map = new WorldMap();

        public static InfWorld Instance;

        public override void Load()
        {
            if (!Environment.Is64BitProcess)
            {
                //throw new Exception("Infinite world cannot be loaded on 32bit tML, pls use the 64bit version of tML to load this mod.");
            }
            DirectoryInfo di = new DirectoryInfo("MonoModDump");
            if (di.Exists)
            {
                foreach (var fileInfo in di.GetFiles())
                {
                    fileInfo.Delete();
                }
            }
            Instance = this;
            Patching.Detours.Detours.Load();
            IlPatching.Load();
            MassPatcher.StartPatching(typeof(Main));
            InitMonoModDumps();
            DisableMonoModDumps();
            foreach (var mod in ModLoader.Mods)
            {
                if (mod.Name == "ModLoader" || mod.Name == "InfWorld")
                    continue;
                MassPatcher.StartPatching(mod.GetType().Assembly);
            }

            Main.instance.Exiting += (sender, args) =>
            {
                Environment.Exit(0);
            };
        }

        public override void PostAddRecipes()
        {

        }

        

        public static void InitMonoModDumps()
        {
            Environment.SetEnvironmentVariable("MONOMOD_DMD_TYPE", "Auto");
            Environment.SetEnvironmentVariable("MONOMOD_DMD_DEBUG", "1");
            string dumpDir = Path.GetFullPath("MonoModDump");
            Directory.CreateDirectory(dumpDir);
            Environment.SetEnvironmentVariable("MONOMOD_DMD_DUMP", dumpDir);
        }

        public static void DisableMonoModDumps()
        {
            Environment.SetEnvironmentVariable("MONOMOD_DMD_TYPE", "");
            Environment.SetEnvironmentVariable("MONOMOD_DMD_DEBUG", "0");
            Environment.SetEnvironmentVariable("MONOMOD_DMD_DUMP", "");
        }

        
    }
}