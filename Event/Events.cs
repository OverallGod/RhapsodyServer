using ENet.Managed;
using RhapsodyServer.Client;
using RhapsodyServer.ServerCore;
using System.Threading.Tasks;

namespace RhapsodyServer.Event
{
    public class Events
    {
        public static void OnConnected(ENetPeer peer)
        {
            int count = 0;

            Core.ForEachPeer(x =>
            {
                if (x.GetRemoteEndPoint().Address.ToString() == peer.GetRemoteEndPoint().Address.ToString()) count++;
            });

            Player player = new Player(peer);

            if (count >= 3)
            {
                player.SendLog("`4[FAILED] `5Please log off `4all`5 of your `$account`5 first.");
                player.Disconnect();
                return;
            }

            player.SendHelloPacket();
        }

        public static void OnDisconnected(ENetPeer peer)
        {
            peer.UnsetUserData();
        }

        public static void OnPacketReceived(ENetPeer peer, ENetPacket packet)
        {
            var data = packet.Data.ToArray();

            if (peer.TryGetUserData(out Player player))
            {
                switch (data[0])
                {
                    case 0x2:
                    case 0x3:
                        new TextEvent(player, packet);
                        break;
                    case 0x4:
                        new TankEvent(player, packet);
                        break;
                }
            }
            else return;
        }
    }
}
