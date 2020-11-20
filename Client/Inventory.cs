using ENet.Managed;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using RhapsodyServer.DB;
using RhapsodyServer.Packet;
using RhapsodyServer.Worlds;
using System.Collections.Generic;
using System.Linq;

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

        internal Player Player { get; }
        internal World World => Database.GetWorld(Player.CurrentWorld).Result;

        internal Inventory(Player player) => Player = player;
        public Inventory() { }

        public bool Add(int id, int amount)
        {
            if (id == 0) return false;

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
            if (id == 0) return false;

            if (Find(id, out var a))
            {
                return a + amount <= 200;
            }
            return false;
        }

        public bool Remove(int id, int amount)
        {
            if (id == 0) return false;

            if (Find(id, out var a))
            {
                if (a - amount > 0)
                {
                    Items[id] -= amount;
                }
                else
                {
                    Items.Remove(id);

                    if (Tile.Parse(id).ActionType == 20)
                    {
                        var field = Player.Clothes.GetType().GetFields().FirstOrDefault(x => (int)x.GetValue(Player.Clothes) == id);

                        if (field != null)
                        {
                            field.SetValue(Player.Clothes, 0);

                            foreach (var ply in World.Players)
                            {
                                Player.SendClothes(ply, true);
                            }
                        }
                    }
                }
            }
            else return false;

            ModifiedId = id;
            ModifiedAmount = amount;
            IsAdding = false;

            return true;
        }

        public void Send()
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

            Player.Peer.Send(0, tank.Pack(), ENetPacketFlags.Reliable);
        }

        public void Modify()
        {
            TankPacket tank = new TankPacket()
            {
                Type = 0x0d,
                Tile = ModifiedId
            };

            byte[] data = tank.Pack();

            data[IsAdding ? 7 : 6] = (byte)ModifiedAmount;

            Player.Peer.Send(0, data, ENetPacketFlags.Reliable);
        }

        public bool Find(int id, out int amount) => Items.TryGetValue(id, out amount);
    }
}
