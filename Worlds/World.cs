using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using RhapsodyServer.Client;
using RhapsodyServer.DB;
using RhapsodyServer.Packet;
using RhapsodyServer.Proton;
using RhapsodyServer.ServerCore;
using RhapsodyServer.Structs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RhapsodyServer.Worlds
{
    [BsonIgnoreExtraElements]
    public class World
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public string Name { get; set; }
        
        public Block[] Blocks { get; set; }

        [BsonIgnore]
        public List<Player> Players { get; set; } = new List<Player>();


        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<int, DroppedItem> DroppedItems { get; set; } = new Dictionary<int, DroppedItem>();
        public int DropId { get; set; } = 1;

        public World(string name, int w, int h)
        {
            Name = name;
            Width = w;
            Height = h;
            Blocks = new Block[w * h];
        }

        public async void Save()
        {
            await Task.Run(() =>
            {
                Database.Worlds[Name] = this;
                Database.SaveWorld(this);
            });
        }

        public void SendPacket(Player player)
        {
            int size = 104 + (DroppedItems.Count * 16) + Width * Height * 20, length = Name.Length;

            TankPacket tank = new TankPacket()
            {
                Type = 0x4,
                State = 0x8,
                Size = size,
                Data = new byte[size]
            };

            tank.Write(0x0f);
            tank.Write((short)0x0);
            tank.Write((short)length);
            tank.Write(Name);
            tank.Write(Width);
            tank.Write(Height);
            tank.Write(Width * Height);

            List<Block> blocks = new List<Block>();

            foreach (var block in Blocks)
            {
                switch (block.Fg.ActionType)
                {
                    case 0x11:
                        {
                            tank.Write((short)block.Fg.Id);
                            tank.Write((short)block.Bg.Id);
                            tank.Skip(4);
                            break;
                        }
                    default:
                        {
                            tank.Write((short)0x0);
                            tank.Write((short)block.Bg.Id);
                            blocks.Add(block);
                            tank.Skip(4);
                            break;
                        }
                }
            }

            tank.Write(DroppedItems.Count);
            tank.Write(DropId - 1);

            foreach (var drop in DroppedItems)
            {
                tank.Write((short)drop.Value.ItemId);
                tank.Write(drop.Value.Pos);
                tank.Write((short)drop.Value.Amount);
                tank.Write(drop.Value.Id);
            }

            player.Send(tank);
            
            foreach (var block in blocks)
            {
                TankPacket packet = new TankPacket()
                {
                    Type = 0x3,
                    Pos = new Vector2(block.Pos.X, (block.Pos.X + block.Pos.Y * Width) / Height),
                    Pos3 = block.Pos,
                    Tile = block.Fg.Id
                };

                player.Send(packet);

            }

        }

        public void Enter(Player player)
        {
            player.CurrentWorld = Name;

            if (!Name.All(x => char.IsLetterOrDigit(x)))
            {
                player.SendConsoleMessage("Name can't contains symbols.");
                player.SendFailedToEnterWorld();
                return;
            }

            Players.Add(player);

            var pos = Vector2.Zero;
            var door = Blocks.FirstOrDefault(x => x.Fg.Id == 6);

            if (door != null)
                pos = new Vector2(door.Pos.X * 32, door.Pos.Y * 32);

            SendPacket(player);

            player.NetId = Core.NetIds++;

            player.SendSpawn(pos);
            player.SendClothes();
            player.Inventory.Send(player.Peer);
        }

        public void RemovePlayer(Player player)
        {
            player.CurrentWorld = "EXIT";

            foreach (var ply in Players)
            {
                ply.SendTalkBubble(player.NetId, "`5<`w" + player.DisplayName + "`5 left, `w" + (Players.Count - 1) + "`` others here>``");
                ply.SendConsoleMessage("`5<`w" + player.DisplayName + "`5 left, `w" + (Players.Count - 1) + "`` others here>``");
                ply.SendPlayPositioned(player.NetId, "door_shut");
                ply.SendOnRemove(player.NetId);

                if (ply.NetId == player.NetId)
                    ply.SendSound("door_shut");
            }

            player.SendRequestWorldMenu();

            Players.Remove(player);

            Save();
        }
    }
}
