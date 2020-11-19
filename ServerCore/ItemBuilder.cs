using RhapsodyServer.Packet;
using RhapsodyServer.Worlds;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RhapsodyServer.ServerCore
{
    public static class ItemBuilder
    {
        public static byte[] ItemData { get; private set; }
        public static byte[] SerializedItemData { get; private set; }

        public static uint ItemDataHash { get; private set; }

        public static void Build()
        {
            SerializeItemDatabase();
            BuildItemDatabase();

            Console.WriteLine("Items serialized.\n" +
                $"    [-] Total Bytes : {ItemData.Length}\n" +
                $"    [-] Total Items : {Tile.Tiles.Count}\n" +
                $"    [-] Serialized Bytes : {SerializedItemData.Length}\n" +
                $"    [-] Hash : {ItemDataHash}\n");
        }

        private static void SerializeItemDatabase()
        {
            SerializedItemData = File.ReadAllBytes("items.dat");

            TankPacket tank = new TankPacket()
            {
                Type = 0x10,
                NetId = -1,
                State = 0x8,
                Size = SerializedItemData.Length,
                Data = SerializedItemData
            };

            ItemData = tank.Pack();
            ItemDataHash = HashData(SerializedItemData);
        }

        private static uint HashData(byte[] serializedItemData)
        {
            uint result = 0x55555555;

            foreach (var b in serializedItemData)
            {
                result = (result >> 27) + (result << 5) + b;
            }

            return result;
        }

        private static void BuildItemDatabase()
        {
            string keys = "PBG892FXX982ABC*";

            MemoryStream ms = new MemoryStream(SerializedItemData);
            BinaryReader br = new BinaryReader(ms);

            ms.Seek(0, SeekOrigin.Begin);

            short version = br.ReadInt16();
            int amount = br.ReadInt32(), len, effectId = 0;

            for (int i = 0; i < amount; i++)
            {
                Tile item = new Tile
                {
                    Id = br.ReadInt32()
                };

                br.ReadByte();
                br.ReadByte();
                item.ActionType = br.ReadByte();
                br.ReadByte();

                len = br.ReadInt16();
                byte[] name_bytes = new byte[len];
                for (int j = 0; j < len; j++)
                {
                    name_bytes[j] = (byte)(br.ReadByte() ^ (keys[(j + item.Id) % keys.Length]));
                }
                item.Name = Encoding.ASCII.GetString(name_bytes);
                len = br.ReadInt16();
                ms.Position += len + 4;

                item.ItemKind = br.ReadByte();
                ms.Position += 8;
                item.CollisionType = br.ReadByte();
                item.BreakHits = (byte)(br.ReadByte() / 6);
                item.DropChance = br.ReadInt32();
                item.ClothingType = br.ReadByte();
                item.Rarity = br.ReadInt16();

                ms.Position++;

                len = br.ReadInt16();

                var extra_file_bytes = new byte[len];

                for (int j = 0; j < len; j++)
                {
                    extra_file_bytes[j] = br.ReadByte();
                }

                string extraFile = Encoding.ASCII.GetString(extra_file_bytes);

                ms.Position += 8;

                len = br.ReadInt16();
                ms.Position += len;

                len = br.ReadInt16();
                ms.Position += len;

                len = br.ReadInt16();
                ms.Position += len;

                len = br.ReadInt16();
                ms.Position += len + 16;

                item.GrowTime = br.ReadInt32();

                ms.Position += 4;

                len = br.ReadInt16();
                ms.Position += len;

                len = br.ReadInt16();
                ms.Position += len;

                len = br.ReadInt16();
                ms.Position += len + 80;

                if (version >= 11)
                {
                    len = br.ReadInt16();
                    ms.Position += len;
                }

                if (item.ClothingType == 5)
                {
                    if (extraFile != "" || item.Id == 366)
                    {
                        item.EffectId = effectId++;
                    }
                }

                Tile.Tiles.Add(item);
            }

            Tile.Zero = Tile.Tiles[0];
        }
    }
}
