using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfWorld.Patching.Detours
{
    static partial class Detours
    {
        private static void OnBordersMovement(On.Terraria.Player.orig_BordersMovement orig, Terraria.Player self)
        {
            return;
        }
    }
}
