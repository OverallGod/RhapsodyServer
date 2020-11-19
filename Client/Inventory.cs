using ENet.Managed;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using RhapsodyServer.Packet;
using System.Collections.Generic;

namespace RhapsodyServer.Client
{
    public class Inventory
    {
        public int Size { get; set; } = 36;

        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<int, int> Items { get; set; } = new Dictionary<int, int>();

        private int ModifiedId = 0x0;
        private int ModifiedAmount = 0x0;
        private bool IsAdding = true;

        public bool Add(int id, int amount)
        {
            if (Find(id, out var a))
            {
                if (a + amount > 200)
                    return false;

                Items[id] += amount;

            }
            else
            {
                Items.Add(id, amount);
            }

            ModifiedId = id;
            ModifiedAmount = amount;
            IsAdding = true;

            return true;
        }

        public bool Validate(int id, int amount)
        {
            if (Find(id, out var a))
            {
                return a + amount <= 200;
            }
            return false;
        }

        public bool Remove(int id, int amount)
        {
            if (Find(id, out var a))
            {
                if (a - amount > 0)
                {
                    Items[id] -= amount;
                }
                else
                {
                    Items.Remove(id);
                }
            }
            else return false;

            ModifiedId = id;
            ModifiedAmount = amount;
            IsAdding = false;

            return true;
        }

        public void Send(ENetPeer peer)
        {
            int total = 8 + Items.Count * 4;

            TankPacket tank = new TankPacket()
            {
                Type = 0x9,
                State = 0x8,
                Size = total,
                Data = new byte[total]
            };

            tank.Write((byte)0x1);
            tank.Write(Size);
            tank.Write((byte)Items.Count);

            foreach (var item in Items)
            {
                tank.Write((short)item.Key);
                tank.Write((short)item.Value);
            }

            peer.Send(0, tank.Pack(), ENetPacketFlags.Reliable);
        }

        public void Modify(ENetPeer peer)
        {
            TankPacket tank = new TankPacket()
            {
                Type = 0x0d,
                Tile = ModifiedId
            };

            byte[] data = tank.Pack();

            data[IsAdding ? 7 : 6] = (byte)ModifiedAmount;

            peer.Send(0, data, ENetPacketFlags.Reliable);
        }

        public bool Find(int id, out int amount) => Items.TryGetValue(id, out amount);
    }
}
