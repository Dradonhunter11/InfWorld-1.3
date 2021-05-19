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
using Collision = On.Terraria.Collision;
using Main = On.Terraria.Main;
using Player = On.Terraria.Player;
using WorldFile = On.Terraria.IO.WorldFile;
using WorldGen = On.Terraria.WorldGen;

namespace InfWorld.Patching.Detours
{
    static partial class Detours
    {
        public static void Load()
        {
            On.Terraria.Collision.InTileBounds += OnInTileBounds;
            On.Terraria.Collision.SolidTiles += OnSolidTiles;
            On.Terraria.Collision.SolidCollision += CustomSolidCollision;
            On.Terraria.WorldGen.clearWorld += OnClearWorld;
            On.Terraria.WorldGen.InWorld += RemoveInWorldCheck;
            On.Terraria.WorldGen.do_playWorldCallBack += OnDoPlayWorldCallBack;
            On.Terraria.IO.WorldFile.LoadWorldTiles += NewLoadWorldTiles;
            On.Terraria.Main.ClampScreenPositionToWorld += OnClampScreenPositionToWorld;
            On.Terraria.Player.BordersMovement += OnBordersMovement;
        }

        
    }
}
