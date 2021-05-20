using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfWorld.Chunks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace InfWorld
{
    class InfModWorld : ModWorld
    {


        public override void PreUpdate()
        {
            if(Main.netMode == NetmodeID.MultiplayerClient)
                return;

            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                InfWorld.Tile.Update(Main.LocalPlayer);
            }
        }
    }
}
