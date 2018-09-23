using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CavernWars
{
    delegate void ProjectileChangeDel(int id, Vector2 position, Vector2 movement, int despawnType);
    delegate void ProjectileHitDel(string targetName, float damage);

    static class GlobalEvents
    {
        public static ProjectileChangeDel projectileChangeDel;
        public static ProjectileHitDel projectileHitDel;
    }
}
