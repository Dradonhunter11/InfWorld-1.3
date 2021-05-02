using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using InfWorld.Chunks;
using log4net;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Cil;
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

        public static World Tile = new World();

        public static InfWorld Instance;

        public override void Load()
        {
            DirectoryInfo di = new DirectoryInfo("MonoModDump");
            if (di.Exists)
            {
                foreach (var fileInfo in di.GetFiles())
                {
                    fileInfo.Delete();
                }
            }
            //ILPatching.Load();
            Instance = this;
            IlPatching.Load();
            MassPatcher.StartPatching();
            InitMonoModDumps();
            IL.Terraria.Projectile.VanillaAI += il =>
            {
                var cursor = new ILCursor(il);
                for (int nmbloop = 0; nmbloop < 3; nmbloop++)
                {

                    if (cursor.TryGotoNext(i => i.MatchLdloc(out _),
                        i => i.MatchLdcI4(out _),
                        i => i.MatchBge(out _),
                        i => i.MatchLdcI4(out _),
                        i => i.MatchStloc(out _),
                        i => i.MatchLdloc(out _),
                        i => i.MatchLdsfld(out _)))
                    {
                        int initPoint = cursor.Index;
                        ILog log = LogManager.GetLogger("VanillaAI debug");
                        log.Debug("Entrypoint found");
                        log.Debug($"Current instruction : [{cursor.Previous.Offset}] {cursor.Previous.OpCode.Name} {cursor.Previous.Operand}");
                        if (cursor.TryGotoNext(i => i.MatchLdsfld(typeof(Main), nameof(Main.maxTilesY)),
                            i => i.MatchStloc(out _)))
                        {
                            log.Debug("Pattern found");
                            cursor.Index += 2;
                            int afterPoint = cursor.Index;
                            var instruction = cursor.Next;
                            cursor.Index = initPoint;
                            cursor.Emit(OpCodes.Br, instruction);
                            cursor.Index = afterPoint + 1;
                        }
                    }
                }
            };
            IL.Terraria.Projectile.Kill += il =>
            {
                var cursor = new ILCursor(il);
                for (int nmbloop = 0; nmbloop < 3; nmbloop++)
                {

                    if (cursor.TryGotoNext(i => i.MatchNop(), 
                        i => i.MatchNop(), 
                        i => i.MatchLdloc(out _),
                        i => i.MatchNop(),
                        i => i.MatchNop(),
                        i => i.MatchLdcI4(out _),
                        i => i.MatchBge(out _),
                        i => i.MatchLdcI4(out _),
                        i => i.MatchStloc(out _),
                        i => i.MatchNop(),
                        i => i.MatchNop(),
                        i => i.MatchLdloc(out _),
                        i => i.MatchNop(),
                        i => i.MatchNop(),
                        i => i.MatchLdsfld(out _),
                        i => i.MatchBle(out _)))
                    {
                        int initPoint = cursor.Index;
                        ILog log = LogManager.GetLogger("VanillaAI debug");
                        log.Debug("Entrypoint found");
                        log.Debug($"Current instruction : [{cursor.Previous.Offset}] {cursor.Previous.OpCode.Name} {cursor.Previous.Operand}");
                        if (cursor.TryGotoNext(i => i.MatchLdsfld(typeof(Main), nameof(Main.maxTilesY)),
                            i => i.MatchStloc(out _)))
                        {
                            log.Debug("Pattern found");
                            cursor.Index += 2;
                            int afterPoint = cursor.Index;
                            var instruction = cursor.Next;
                            cursor.Index = initPoint;
                            cursor.Emit(OpCodes.Br, instruction);
                            cursor.Index = afterPoint + 1;
                        }
                    }
                }
            };
            IL.Terraria.WorldGen.KillTile += il =>
            {
                var cursor = new ILCursor(il);

                if (cursor.TryGotoNext(i => i.MatchRet()))
                {
                    var jumpPoint = cursor.Next.Next;
                    cursor.Index = 0;
                    cursor.Emit(OpCodes.Br, jumpPoint);
                }
            };
            IL.Terraria.WorldGen.KillWall += il =>
            {
                var cursor = new ILCursor(il);

                if (cursor.TryGotoNext(i => i.MatchRet()))
                {
                    ILog log = LogManager.GetLogger("VanillaAI debug");
                    log.Debug("Entrypoint found");
                    log.Debug($"Current instruction : [{cursor.Previous.Offset}] {cursor.Previous.OpCode.Name} {cursor.Previous.Operand}");
                    var jumpPoint = cursor.Next.Next;
                    cursor.Index = 0;
                    cursor.Emit(OpCodes.Br, jumpPoint);
                }
            };
            DisableMonoModDumps();
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

        internal static class IlPatching
        {
            public static void Load()
            {
                if (!Environment.Is64BitProcess)
                {
                    /*throw new Exception(
                        "Infinite world cannot be loaded on 32bit tML, pls use the 64bit version of tML to load this mod.");*/
                }
                On.Terraria.WorldGen.clearWorld += delegate (On.Terraria.WorldGen.orig_clearWorld orig) { return; };
                On.Terraria.Main.ClampScreenPositionToWorld += orig =>
                    { return; };
                On.Terraria.IO.WorldFile.LoadWorldTiles += (orig, reader, importance) =>
                {
                    for (int i = 0; i < Main.maxTilesX; i++)
                    {
                        float num = (float)i / (float)Main.maxTilesX;
                        Main.statusText = Lang.gen[51].Value + " " + (int)((double)num * 100.0 + 1.0) + "%";
                        for (int j = 0; j < Main.maxTilesY; j++)
                        {
                            int num2 = -1;
                            byte b;
                            byte b2 = b = 0;
                            Tile t = Tile[i, j];
                            byte b3 = reader.ReadByte();
                            if ((b3 & 1) == 1)
                            {
                                b2 = reader.ReadByte();
                                if ((b2 & 1) == 1)
                                    b = reader.ReadByte();
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
                                    if (t.type == 144)
                                        t.frameY = 0;
                                }
                                else
                                {
                                    t.frameX = -1;
                                    t.frameY = -1;
                                }

                                if ((b & 8) == 8)
                                    t.color(reader.ReadByte());
                            }

                            if ((b3 & 4) == 4)
                            {
                                t.wall = reader.ReadByte();
                                if ((b & 0x10) == 16)
                                    t.wallColor(reader.ReadByte());
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
                                if ((b2 & 2) == 2)
                                    t.wire(wire: true);

                                if ((b2 & 4) == 4)
                                    t.wire2(wire2: true);

                                if ((b2 & 8) == 8)
                                    t.wire3(wire3: true);

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
                                if ((b & 2) == 2)
                                    t.actuator(actuator: true);

                                if ((b & 4) == 4)
                                    t.inActive(inActive: true);

                                if ((b & 0x20) == 32)
                                    t.wire4(wire4: true);
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
                                if (t != null)
                                {
                                    InfWorld.Tile[i, j].CopyFrom(t);
                                }
                                num3--;
                            }
                        }
                    }

                    WorldGen.AddUpAlignmentCounts(clearCounts: true);
                };
                On.Terraria.WorldGen.do_playWorldCallBack += (orig, context) =>
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
                    WorldFile.loadWorld(Main.ActiveWorldFileData.IsCloudSave);
                    if (WorldGen.loadFailed || !WorldGen.loadSuccess)
                    {
                        WorldFile.loadWorld(Main.ActiveWorldFileData.IsCloudSave);
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
                            WorldFile.loadWorld(Main.ActiveWorldFileData.IsCloudSave);
                            if (WorldGen.loadFailed || !WorldGen.loadSuccess)
                            {
                                WorldFile.loadWorld(Main.ActiveWorldFileData.IsCloudSave);
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
                        if (Main.sectionManager == null)
                            Main.sectionManager = new WorldSections(Main.maxTilesX / 200, Main.maxTilesY / 150);
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

                    Main.OnTick += FinishPlayWorld;
                };
                On.Terraria.Player.BordersMovement += (orig, self) => { return; };
                On.Terraria.WorldGen.InWorld += (orig, i, i1, fluff) => { return true; };
                IL.Terraria.Main.DrawTiles += il =>
                {
                    var cursor = new ILCursor(il);

                    Instruction jumpPoint = null;

                    if (cursor.TryGotoNext(
                        i => i.MatchLdsfld(out _),
                        i => i.MatchCallvirt(out _),
                        i => i.MatchLdcI4(out _),
                        i => i.MatchBle(out _)))
                    {
                        jumpPoint = cursor.Next;
                    }

                    cursor.Index = 0;
                    if (jumpPoint == null)
                    {
                        InfWorld.Instance.Logger.Error("Jump point not found");
                    }

                    if (cursor.TryGotoNext(i => i.MatchLdloc(out _),
                        i => i.MatchLdcI4(out _),
                        i => i.MatchBge(out _)))
                    {
                        cursor.Emit(OpCodes.Br, jumpPoint);
                    }
                };
                IL.Terraria.WorldGen.PlaceTile += il =>
                {
                    var cursor = new ILCursor(il);
                    Instruction jumpPoint = null;
                    if (cursor.TryGotoNext(i => i.MatchLdsfld(out _),
                        i => i.MatchLdarg(0),
                        i => i.MatchLdarg(1)))
                    {
                        jumpPoint = cursor.Next;
                        cursor.Index = 0;
                        cursor.Emit(OpCodes.Br, jumpPoint);
                    }
                };
                IL.Terraria.Player.Update += il =>
                {
                    var cursor = new ILCursor(il);
                    int indexCursor = 0;
                    if (cursor.TryGotoNext(
                        i => i.MatchLdsfld(out _),
                        i => i.MatchLdsfld(out _),
                        i => i.MatchLdcI4(out _),
                        i => i.MatchSub(),
                        i => i.MatchBlt(out _)))
                    {
                        indexCursor = cursor.Index;
                    }

                    if (indexCursor != 0)
                    {
                        if (cursor.TryGotoNext(
                            i => i.MatchLdsfld(out _),
                            i => i.MatchLdsfld(out _),
                            i => i.MatchLdcI4(out _),
                            i => i.MatchSub(),
                            i => i.MatchLdsfld(out _),
                            i => i.MatchCall(out _)))
                        {
                            var jumpPoint = cursor.Next;
                            cursor.Index = indexCursor;
                            cursor.Emit(OpCodes.Br, jumpPoint);
                        }
                    }
                };
                On.Terraria.Collision.SolidTiles += (orig, x, endX, y, endY) =>
                {

                    for (int i = x; i < endX + 1; i++)
                    {
                        for (int j = y; j < endY + 1; j++)
                        {
                            if (InfWorld.Tile[i, j] == null)
                                return false;

                            if (InfWorld.Tile[i, j].active() && !InfWorld.Tile[i, j].inActive() &&
                                Main.tileSolid[InfWorld.Tile[i, j].type] && !Main.tileSolidTop[InfWorld.Tile[i, j].type])
                                return true;
                        }
                    }

                    return false;
                };
            }

            internal static bool SearchBoundOccurrenceWithoutNop(ILCursor cursor)
            {
                
                return true;
            }
        }

        internal static void FinishPlayWorld()
        {
            Main.OnTick -= FinishPlayWorld;
            Main.player[Main.myPlayer].Spawn();
            Main.player[Main.myPlayer].Update(Main.myPlayer);
            Main.ActivePlayerFileData.StartPlayTimer();
            _lastSeed = Main.ActiveWorldFileData.Seed;
            Player.Hooks.EnterWorld(Main.myPlayer);
            WorldFile.SetOngoingToTemps();
            Main.PlaySound(11);
            Main.resetClouds = true;
            noMapUpdate = false;
        }

        public void Trash()
        {
            var tile = InfWorld.Tile[0, 0];
            InfWorld.Tile[0, 0] = new Tile();
        }



        internal static class MassPatcher
        {
            internal static Type[] GetAllTypeInCurrentAssembly(Assembly asm)
            {
                return asm.GetTypes();
            }

            internal static MethodInfo[] GetAllMethodInAType(Type type)
            {
                BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance |
                                     BindingFlags.Static;
                return type.GetMethods(flags);
            }

            public static void StartPatching()
            {
                ILog log = LogManager.GetLogger("Mass Patcher");
                var asm = Assembly.GetAssembly(typeof(Main));

                Type[] array = GetAllTypeInCurrentAssembly(asm);
                for (int i = 0; i < array.Length; i++)
                {
                    Type typeInfo = array[i];
                    MethodInfo[] array1 = GetAllMethodInAType(typeInfo);
                    for (int i1 = 0; i1 < array1.Length; i1++)
                    {
                        MethodInfo methodInfo = array1[i1];
                        try
                        {
                            log.Debug(methodInfo.Name);
                            if (methodInfo.Name == "DrawTiles")
                            {
                                //InfWorld.InitMonoModDumps();
                                log.Debug("Dump initiated");
                            }
                            if (methodInfo.Name == "do_playWorldCallBack")
                                continue;
                            SetLoadingStatusText("Currently patching " + typeInfo.FullName, i * 100 / array.Length);
                            HookEndpointManager.Modify(methodInfo, new ILContext.Manipulator(IlEditing));
                            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("MONOMOD_DMD_TYPE")))
                            {
                                //InfWorld.DisableMonoModDumps();
                                log.Debug("Dump ended");
                            }
                        }
                        catch (Exception e)
                        {
                            log.Error($"Failed to patch : {typeInfo.FullName}", e);
                        }
                    }
                }
                /*
                var flags = BindingFlags.NonPublic | BindingFlags.Static;
                var resolver = (IAssemblyResolver)typeof(Main).Assembly.GetType("Terraria.ModLoader.Core.AssemblyManager").GetField("cecilAssemblyResolver", flags).GetValue(null);

                AssemblyDefinition definition = AssemblyDefinition.ReadAssembly(typeof(Main).Assembly.ManifestModule.FullyQualifiedName, new ReaderParameters(ReadingMode.Immediate)
                {
                    AssemblyResolver = resolver
                });
                TypeInfo info = null;
                definition.MainModule.Types.Clear();
                foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
                {
                    Console.Write(type.Assembly.ManifestModule.FullyQualifiedName);
                    var def = new TypeDefinition(type.Namespace, type.AssemblyQualifiedName, TypeAttributes.Class);
                    Console.Write(def.BaseType.Name);
                    definitions.Add(def);
                }
                definition.MainModule.Types.AddRange(definitions);
                var asmnameref = new AssemblyNameReference("InfWorld", new Version(1, 0, 0, 0));
                definition.MainModule.AssemblyReferences.Add(asmnameref);
                foreach (var mainModuleAssemblyReference in definition.MainModule.AssemblyReferences)
                {
                    Console.WriteLine(mainModuleAssemblyReference.FullName);
                }
                definition.MainModule.Write(Environment.CurrentDirectory + "/tModLoaderPatched.exe");*/
            }
            /*
            public static void PatchBinary()
            {
                var flags = BindingFlags.NonPublic | BindingFlags.Static;
                var resolver = (IAssemblyResolver)typeof(Main).Assembly.GetType("Terraria.ModLoader.Core.AssemblyManager").GetField("cecilAssemblyResolver", flags).GetValue(null);

                AssemblyDefinition terrariaAssemblyDefinition = AssemblyDefinition.ReadAssembly(Environment.CurrentDirectory + "/tModLoaderPatched.exe", new ReaderParameters()
                {
                    AssemblyResolver = resolver
                });
                ModuleDefinition definition = terrariaAssemblyDefinition.MainModule;

                PropertyInfo indexerInfo = typeof(Chunks.World).GetProperty("Item",
                    BindingFlags.Public | BindingFlags.Instance, null, typeof(Tile),
                    new Type[] { typeof(Int32), typeof(Int32) }, null);

                MethodReference getItemReference = definition.Import(indexerInfo.GetGetMethod());
                MethodReference setItemReference = definition.Import(indexerInfo.GetSetMethod());

                
                
                int i = 0;
                foreach (var mods in terrariaAssemblyDefinition.Modules)
                {
                    foreach (var typeDefinition in mods.GetTypes())
                    {
                        i++;
                        SetLoadingStatusText("Patching " + typeDefinition.FullName, i / mods.GetTypes().Count() * 100);
                        foreach (var methods in typeDefinition.Methods)
                        {
                            if (methods.HasBody)
                            {
                                foreach (var instruction in methods.Body.Instructions)
                                {
                                    Object operandType = instruction.Operand;
                                    if (operandType != null && operandType is FieldReference fieldRef)
                                    {
                                        try
                                        {
                                            FieldReference tileReference =
                                                mods.ImportReference(typeof(InfWorld).GetField("tile",
                                                    BindingFlags.Public | BindingFlags.Static));
                                            if (fieldRef.FullName.Contains("Terraria.Tile[0...,0...] Terraria.Main::tile") && instruction.OpCode == OpCodes.Ldsfld)
                                            {
                                                
                                                instruction.Operand = tileReference;
                                                instruction.OpCode = OpCodes.Ldsfld;

                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine(e);
                                        }
                                    }

                                    if (instruction != null && instruction.Operand is Mono.Cecil.MethodReference reference)
                                    {
                                        try
                                        {
                                            if (reference.FullName == ("Terraria.Tile Terraria.Tile[0...,0...]::Get(System.Int32,System.Int32)") && instruction.OpCode == OpCodes.Call)
                                            {
                                                Console.WriteLine("Tile GET reference detected");
                                                instruction.OpCode = OpCodes.Callvirt;
                                                instruction.Operand = getItemReference;
                                            }
                                            if (reference.FullName == ("System.Void Terraria.Tile[0...,0...]::Set(System.Int32,System.Int32,Terraria.Tile)") && instruction.OpCode == OpCodes.Call)
                                            {
                                                Console.WriteLine("Tile SET reference detected");
                                                instruction.OpCode = OpCodes.Callvirt;
                                                instruction.Operand = setItemReference;
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine(e);
                                        }
                                    }
                                }
                            }
                        }

                    }
                }
                terrariaAssemblyDefinition.Write(Environment.CurrentDirectory + "/tModLoaderPatched2.exe");
                //definition.Write(Environment.CurrentDirectory + "/tModLoaderPatched.exe");
            }
            */
            private static List<string> _blacklistLoadFnt = new List<string>()
            {
                "FinishPlayWorld",
                "OnDisconnect",
                "DoClientSizeChanged"
            };

            private static List<TypeDefinition> _definitions = new List<TypeDefinition>();

            internal static void IlEditing(ILContext il)
            {
                PropertyInfo indexerInfo = typeof(Chunks.World).GetProperty("Item",
                    BindingFlags.Public | BindingFlags.Instance, null, typeof(Tile),
                    new Type[] { typeof(Int32), typeof(Int32) }, null);
                MethodReference getItemReference = il.Import(indexerInfo.GetGetMethod());
                MethodReference setItemReference = il.Import(indexerInfo.GetSetMethod());


                foreach (var instruction in il.Body.Instructions)
                {
                    if (instruction.OpCode == OpCodes.Ldftn)
                    {
                        MethodReference function = (MethodReference)instruction.Operand;
                        BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance |
                                             BindingFlags.Static;
                        if (!function.Name.Contains(_blacklistLoadFnt[0]) && !function.Name.Contains(_blacklistLoadFnt[1]) && !function.Name.Contains(_blacklistLoadFnt[2]))
                        {
                            InfWorld.Instance.Logger.Info("Loadfnt detected : " + function.Name);
                            HookEndpointManager.Modify(function.ResolveReflection(), new ILContext.Manipulator(IlEditing));
                        }
                    }
                    Object operandType = instruction.Operand;
                    if (operandType != null && operandType is FieldReference fieldRef)
                    {
                        try
                        {
                            FieldReference tileReference =
                                il.Module.ImportReference(typeof(InfWorld).GetField("Tile",
                                    BindingFlags.Public | BindingFlags.Static));
                            if (fieldRef.FullName.Contains("Terraria.Tile[0...,0...] Terraria.Main::tile") && instruction.OpCode == OpCodes.Ldsfld)
                            {
                                InfWorld.Instance.Logger.Info("Tile reference detected");
                                instruction.Operand = tileReference;
                                instruction.OpCode = OpCodes.Ldsfld;

                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }

                    if (instruction != null && instruction.Operand is Mono.Cecil.MethodReference reference)
                    {
                        try
                        {
                            if (reference.FullName == ("Terraria.Tile Terraria.Tile[0...,0...]::Get(System.Int32,System.Int32)") && instruction.OpCode == OpCodes.Call)
                            {
                                instruction.OpCode = OpCodes.Callvirt;
                                instruction.Operand = getItemReference;

                            }
                            else if (reference.FullName == ("System.Void Terraria.Tile[0...,0...]::Set(System.Int32,System.Int32,Terraria.Tile)") && instruction.OpCode == OpCodes.Call)
                            {
                                InfWorld.Instance.Logger.Info("Tile.Set(x, y) reference detected");
                                instruction.OpCode = OpCodes.Callvirt;
                                instruction.Operand = setItemReference;
                            }
                            else if (reference.FullName.Contains("Terraria.Tile[0...,0...]"))
                            {
                                ILog test = LogManager.GetLogger("Terraria.Tile detector");
                                test.Debug(reference.FullName);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                }
            }

            public static void SetLoadingStatusText(string statusText, int percent)
            {
                Assembly assembly = Assembly.GetAssembly(typeof(Mod));

                var type = assembly.GetType("Terraria.ModLoader.UI.Interface");
                FieldInfo loadModsField = type.GetField("loadMods", BindingFlags.Static | BindingFlags.NonPublic);
                var loadModsValue = loadModsField.GetValue(null);

                Type uiLoadModsType = assembly.GetType("Terraria.ModLoader.UI.UILoadMods");

                MethodInfo setLoadStageMethod = uiLoadModsType.GetMethod("SetLoadStage", BindingFlags.Instance | BindingFlags.Public);

                setLoadStageMethod.Invoke(loadModsValue, new object[] { statusText, -1 });
            }
        }

    }
}