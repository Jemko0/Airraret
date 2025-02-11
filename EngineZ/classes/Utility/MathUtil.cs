using EngineZ.classes.world;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace EngineZ
{
    public static class MathUtil
    {
        public static float Map(this float value, float fromSource, float toSource, float fromTarget, float toTarget)
        {
            return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
        }

        public static float FloatToTileSnap(float f)
        {
            return (float)(int)Math.Round(f / World.TILESIZE) * World.TILESIZE;
        }
    }
}
