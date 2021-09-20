﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InfWorld.World.Region;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace InfWorld
{
    class InfModWorld : ModSystem
    {
        public bool ThreadRunning = false;

        public override void Load()
        {
            InfWorld.Tile = new World.World();
        }

        public override void PreUpdateWorld()
        {
            if(Main.netMode != NetmodeID.Server)
                InfWorld.Map.PreRender((int)(Main.LocalPlayer.position.X / 16f / Chunk.ChunkWidth), (int)(Main.LocalPlayer.position.Y / 16f / Chunk.ChunkHeight));
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            if (Main.netMode == NetmodeID.SinglePlayer && !ThreadRunning)
            {
                Thread thread = new Thread(() =>
                {
                    while (ThreadRunning)
                    {
                        InfWorld.Tile.Update(Main.LocalPlayer);

                        Thread.Sleep(50);
                        if (Main.graphics.GraphicsDevice.IsDisposed)
                        {
                            ThreadRunning = false;
                        }
                    }
                });
                ThreadRunning = true;
                thread.Start();
            }
        }

        public override void PostDrawTiles()
        {
            InfWorld.Map.Draw();
        }
    }
}
