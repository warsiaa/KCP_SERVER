using System;
using KCP_SERVER.Network;

namespace KCP_SERVER
{
    internal static class Program
    {
        static void Main()
        {
            var server = new KcpServer(7777);
            server.Start();

            Console.WriteLine("KCP Server running. Press ENTER to stop.");
            Console.ReadLine();

            server.Stop();
        }
    }
}
