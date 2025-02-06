using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngineZ.DataStructures;
namespace EngineZ.Utility
{
    public static class Logger
    {
        public static void Log(string category, params object?[]? values)
        {
            System.Diagnostics.Debug.WriteLine(string.Join(' ', values), category);
        }

        public static void Log(ELogCategory category, params object?[]? values)
        {
            System.Diagnostics.Debug.WriteLine(string.Join(' ', values), category.ToString());
        }
    }
}
