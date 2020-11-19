using ENet.Managed;
using RhapsodyServer.DB;
using RhapsodyServer.Event;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RhapsodyServer.ServerCore
{
    class Program
    {
        static async Task Main()
        {
            Build();
            await Database.GetWorld("EXIT");
            Poll();
        }

        private static void Build()
        {
            ItemBuilder.Build();
            Core.Build();
        }

        private static void Poll()
        {
            while (true) 
            { 
                try
                {
                    var service = Core.Host.Service(TimeSpan.Zero);

                    if (service.Type == ENetEventType.None)
                    {
                        if (Core.Host.CheckEvents().Type == ENetEventType.None) continue;
                    }

                    switch (service.Type)
                    {
                        case ENetEventType.Connect:
                            {
                                var point = service.Peer.GetRemoteEndPoint();

                                Console.WriteLine($"Peer connected\n" +
                                    $"    [-] IP : {point.Address}\n" +
                                    $"    [-] Port : {point.Port}\n" +
                                    $"    [-] Address Family : {point.AddressFamily}\n");

                                Events.OnConnected(service.Peer);
                                break;
                            }

                        case ENetEventType.Disconnect:
                            {
                                var point = service.Peer.GetRemoteEndPoint();

                                Console.WriteLine($"Peer disconnected\n" +
                                    $"    [-] IP : {point.Address}\n" +
                                    $"    [-] Port : {point.Port}\n" +
                                    $"    [-] Address Family : {point.AddressFamily}\n");

                                Events.OnDisconnected(service.Peer);
                                break;
                            }
                        case ENetEventType.Receive:
                            {
                                Events.OnPacketReceived(service.Peer, service.Packet);
                                service.Packet.Destroy();
                                break;
                            }
                    }

                    //GC.Collect();

                    continue;
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"Crash occured at {DateTime.Now}\n" +
                        $"    [-] Exception : {exception.Message}\n");
                    continue;
                }
            }
        }
    }
}
