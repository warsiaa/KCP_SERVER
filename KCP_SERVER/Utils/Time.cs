using System.Diagnostics;

namespace KCP_SERVER.Utils
{
    public static class Time
    {
        private static readonly Stopwatch sw = Stopwatch.StartNew();
        public static uint Now => (uint)sw.ElapsedMilliseconds;
    }
}
