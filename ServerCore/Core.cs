using ENet.Managed;
using System;
using System.Net;

namespace RhapsodyServer.ServerCore
{
    public static class Core
    {
        public static ENetHost Host { get; private set; }
        public static IPEndPoint Address { get; private set; }
        public static int Port { get; private set; }
        public static int NetIds { get; set; } = 1;

        public static void Build()
        {
            ManagedENet.Startup();
            Port = 17091;
            Address = new IPEndPoint(IPAddress.Any, Port);
            Host = new ENetHost(Address, 1024, 1);
            Host.ChecksumWithCRC32();
            Host.CompressWithRangeCoder();

            Console.WriteLine("Server core has been built.\n" +
                $"    [-] Port : {Port}\n");
        }

        public static void ForEachPeer(Action<ENetPeer> action)
        {
            foreach (var peer in Host.PeerList)
            {
                action.Invoke(peer);
            }
        }
    }
}
