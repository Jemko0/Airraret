using EngineZ.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineZ.RNGenerator
{
    public static class RNG
    {
        /// <summary>
        /// returns a random float from 0-1
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static float GetPseudoRandomFloat(int seed)
        {
            Random random = new Random(seed);
            return (float)((float)random.Next() / (float)int.MaxValue);
        }

        public static float GetPseudoRandomFloat(float seed)
        {
            return GetPseudoRandomFloat((int)seed);
        }
    }
}
