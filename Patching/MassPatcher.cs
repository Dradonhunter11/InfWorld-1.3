using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.RuntimeDetour.HookGen;
using MonoMod.Utils;
using Terraria;
using Terraria.ModLoader;

namespace InfWorld.Patching
{
    internal static class MassPatcher
    {
        private const BindingFlags RequiredFlags =
            BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance |
            BindingFlags.Static | BindingFlags.DeclaredOnly;

        public static void StartPatching(Type sourceType)
        {
            StartPatching(sourceType.Assembly);
        }

        public static void StartPatching(Assembly asm)
        {
            List<Task> tasks = new List<Task>();

            Type[] array = asm.GetTypes();
            SetLoadingStatusText("Currently patching " + asm.FullName, 0);
            for (int i = 0; i < array.Length; i++)
            {
                
                Type type = array[i];
                if(type.FullName != null && type.FullName.Contains("Terraria.GameContent.UI.")) continue;
                if(type.FullName != null && type.FullName.Contains("Terraria.Initializers.UILinksInitializer")) continue;
                if (type.FullName != null && type.FullName.Contains("Terraria.Initializers.UILinksInitializer")) continue;
                if (type.FullName != null && !type.FullName.Contains("Terraria")) continue;
                if (type.FullName != null && type.FullName.Contains("System.")) continue;
                if (type.FullName != null && type.FullName.Contains("Native") || type.FullName.Contains("native")) continue;

                PatchMethod(type);
                Task task = Task.Run(() => PatchMethod(type));
                tasks.Add(task);
            }
            Task.WaitAll(tasks.ToArray());
        }
        
        private static void PatchMethod(Type typeInfo)
        {
            MethodInfo[] array1 = typeInfo.GetMethods(RequiredFlags);
            for (int i1 = 0; i1 < array1.Length; i1++)
            {
                try
                {
                    MethodInfo methodInfo = array1[i1];
                    var declaringType = methodInfo.GetType().DeclaringType;
                    if(declaringType is not null && !declaringType.Name.Contains("Terraria")) 
                        continue;
                    if(methodInfo.IsAbstract)
                        continue;
                    if (methodInfo.Name == "do_playWorldCallBack" || methodInfo.Name.Contains("GetEnumerator") || methodInfo.Name.Contains("OpenPort"))
                        continue;
                    HookEndpointManager.Modify(methodInfo, new ILContext.Manipulator(IlEditing));
                }
                catch (Exception e)
                {
                    InfWorld.Instance.Logger.Debug(typeInfo.FullName + " " + array1[i1].Name + " Error");
                    InfWorld.Instance.Logger.Error(e.Message, e);
                }
            }
        }

        private static void PatchModLoader(ILContext context)
        {
            ILCursor cursor = new ILCursor(context);
            cursor.EmitDelegate<Action>(() =>
            {
                Console.WriteLine(context.Method.Name);
            });
        }

        
        private static List<string> _blacklistLoadFnt = new List<string>()
            {
                "FinishPlayWorld",
                "OnDisconnect",
                "DoClientSizeChanged",
                "SetMonitorOnce",
                "ResolveRule",
                "OnLobbyEntered"
            };

        private static List<TypeDefinition> _definitions = new List<TypeDefinition>();

        internal static PropertyInfo IndexerInfo = typeof(World.World).GetProperty("Item",
            BindingFlags.Public | BindingFlags.Instance, null, typeof(Tile),
            new Type[] { typeof(Int32), typeof(Int32) }, null);
        internal static MethodInfo GetItem = IndexerInfo.GetGetMethod();
        internal static MethodInfo SetItem = IndexerInfo.GetSetMethod();

        internal static void IlEditing(ILContext il)
        {
            MethodReference getItemReference = null;
            MethodReference setItemReference = null;

            foreach (var instruction in il.Body.Instructions)
            {
                if (instruction.OpCode == OpCodes.Ldftn)
                {
                    MethodReference function = (MethodReference)instruction.Operand;
                    if (!function.Name.Contains(_blacklistLoadFnt[0]) && !function.Name.Contains(_blacklistLoadFnt[1]) && !function.Name.Contains(_blacklistLoadFnt[2]) && !function.Name.Contains(_blacklistLoadFnt[3]) && !function.Name.Contains(_blacklistLoadFnt[4]) && !function.Name.Contains(_blacklistLoadFnt[5]))
                    {
                        HookEndpointManager.Modify(function.ResolveReflection(), new ILContext.Manipulator(IlEditing));
                    }
                }
                else if (instruction.OpCode == OpCodes.Ldsfld)
                {
                    if (instruction.Operand is FieldReference fieldRef && fieldRef.DeclaringType.FullName == "Terraria.Main" && fieldRef.Name == "tile")
                    {
                        FieldReference tileReference =
                            il.Module.ImportReference(typeof(InfWorld).GetField("Tile",
                                BindingFlags.Public | BindingFlags.Static));
                        instruction.Operand = tileReference;
                    }
                }
                else if (instruction.OpCode == OpCodes.Ldfld)
                {
                    if (instruction.Operand is FieldReference fieldRef && fieldRef.FieldType.FullName.Contains("World"))
                    {
                        instruction.OpCode = OpCodes.Ldsfld;
                        FieldReference tileReference =
                            il.Module.ImportReference(typeof(InfWorld).GetField("Tile",
                                BindingFlags.Public | BindingFlags.Static));
                        instruction.Operand = tileReference;
                    }
                }
                else if ((instruction.OpCode == OpCodes.Call || instruction.OpCode == OpCodes.Callvirt) && instruction.Operand is Mono.Cecil.MethodReference reference)
                {
                    if (reference.FullName == ("Terraria.Tile Terraria.Tile[0...,0...]::Get(System.Int32,System.Int32)"))
                    {
                        instruction.OpCode = OpCodes.Callvirt;
                        if (getItemReference == null)
                        {
                            getItemReference = il.Import(GetItem);
                        }
                        instruction.Operand = getItemReference;

                    }
                    else if (reference.FullName == ("System.Void Terraria.Tile[0...,0...]::Set(System.Int32,System.Int32,Terraria.Tile)"))
                    {
                        instruction.OpCode = OpCodes.Callvirt;
                        if (setItemReference == null)
                        {
                            setItemReference = il.Import(SetItem);
                        }
                        instruction.Operand = setItemReference;
                    }
                    else if (reference.FullName.Contains("Terraria.Tile[0..., 0...] Terraria.World::get_Tiles()"))
                    {
                        instruction.OpCode = OpCodes.Callvirt;
                        if (setItemReference == null)
                        {
                            setItemReference = il.Import(GetItem);
                        }
                        instruction.Operand = setItemReference;
                    }
                    else if (reference.FullName.Contains("Terraria.Tile[0..., 0...] Terraria.World::set_Tiles(System.Int32,System.Int32,Terraria.Tile)"))
                    {
                        instruction.OpCode = OpCodes.Callvirt;
                        if (setItemReference == null)
                        {
                            setItemReference = il.Import(SetItem);
                        }
                        instruction.Operand = setItemReference;
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
