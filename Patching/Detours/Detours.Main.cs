using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfWorld.Patching.Detours
{
    static partial class Detours
    {
        private static void OnClampScreenPositionToWorld(On.Terraria.Main.orig_ClampScreenPositionToWorld orig)
        {
            return;
        }
    }
}
