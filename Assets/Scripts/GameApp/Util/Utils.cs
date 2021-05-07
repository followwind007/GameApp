using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GameApp.Util
{
    public static class Utils
    {
        public static bool IsSubClassOf(object obj, Type type)
        {
            return obj.GetType().IsSubclassOf(type);
        }
        
        public static int GetTimeStamp()
        {
            return Convert.ToInt32(new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds());
        }
        
        private static readonly Dictionary<string, Stopwatch> Watchs = new Dictionary<string, Stopwatch>();
        
        public static void StartWatch(string id)
        {
            if (!Watchs.ContainsKey(id))
            {
                var w = new Stopwatch();
                Watchs[id] = w;
            }
            Watchs[id].Start();
        }

        public static void StopWatch(string id)
        {
            if (Watchs.TryGetValue(id, out var w))
            {
                w.Stop();
                w.Reset();
            }
        }

        public static int ReadWatch(string id)
        {
            return Watchs.TryGetValue(id, out var w) ? Convert.ToInt32(w.ElapsedMilliseconds) : 0;
        }

        public static int ReadResetWatch(string id)
        {
            var t = 0;
            if (Watchs.TryGetValue(id, out var w))
            {
                t = Convert.ToInt32(w.ElapsedMilliseconds);
                w.Restart();
            }

            return t;
        }
        
    }
}