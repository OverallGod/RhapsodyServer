using ENet.Managed;
using RhapsodyServer.Client;
using RhapsodyServer.DB;
using RhapsodyServer.Packet;
using RhapsodyServer.Structs;
using RhapsodyServer.Worlds;
using System;
using System.Threading.Tasks;

namespace RhapsodyServer.Event
{
    public class TankEvent
    {

        public TankEvent(Player player, ENetPacket packet)
        {
            Handle(player, TankPacket.Unpack(packet.Data.ToArray()), Database.GetWorld(player.CurrentWorld).Result);
        }

        private void Handle(Player player, TankPacket packet, World world)
        {
            bool send = true;

            packet.NetId = player.NetId;

            switch (packet.Type)
            {
                case 0x0:
                    {
                        send = OnMoving(player, packet);
                        break;
                    }
                case 0x3:
                    {
                        send = packet.Tile switch
                        {
                            18 => OnPunching(packet, world),
                            32 => OnWrenching(player, packet, world),
                            _ => OnPlacing(player, packet, world),
                        };

                        if (packet.Tile != 32)
                            world.Save();

                        break;
                    }
                case 0x7:
                    {
                        OnTileActivated(player, world, packet);
                        break;
                    }
                case 0xa:
                    {
                        break;
                    }

                case 0xb:
                    {
                        Visual.ApplyCollect(player, world, packet.Tile, packet.Pos);
                        break;
                    }

                case 0x12:
                    {
                        break;
                    }
                default:
                    send = false;
                    break;
            }

            if (send)
            {
                foreach (var ply in world.Players)
                {
                    ply.Send(packet);
                }
            }
        }

        private void OnTileActivated(Player player, World world, TankPacket packet)
        {
            var tile = Tile.Parse(packet.Tile);

            if (tile.ActionType == 20 || tile.ActionType == 107)
            {
                switch (tile.ClothingType)
                {
                    case 0:
                        {
                            player.Clothes.Mask = player.Clothes.Mask == tile.Id ? 0 : tile.Id;
                        }
                        break;
                    case 1:
                        {
                            player.Clothes.Shirt = player.Clothes.Shirt == tile.Id ? 0 : tile.Id;
                        }
                        break;
                    case 2:
                        {
                            player.Clothes.Pants = player.Clothes.Pants == tile.Id ? 0 : tile.Id;
                        }
                        break;
                    case 3:
                        {
                            player.Clothes.Feet = player.Clothes.Feet == tile.Id ? 0 : tile.Id;
                        }
                        break;
                    case 4:
                        {
                            player.Clothes.Face = player.Clothes.Face == tile.Id ? 0 : tile.Id;
                        }
                        break;
                    case 5:
                        {
                            player.Clothes.Hand = player.Clothes.Hand == tile.Id ? 0 : tile.Id;
                        }
                        break;
                    case 6:
                        {
                            if (tile.ClothingType == 6)
                            {
                                player.Clothes.Back = player.Clothes.Back == tile.Id ? 0 : tile.Id;
                                if (player.Clothes.Back != 0)
                                {
                                    if (Tile.Parse(player.Clothes.Back).ItemKind == 4)
                                    {
                                        //player.State.InWings = true;
                                    }
                                }
                                //else player.State.InWings = false;
                            }
                            else
                            {
                                player.Clothes.Ances = player.Clothes.Ances == tile.Id ? 0 : tile.Id;
                            }
                        }
                        break;
                    case 7:
                        {
                            player.Clothes.Hair = player.Clothes.Hair == tile.Id ? 0 : tile.Id;
                        }
                        break;
                    case 8:
                        {
                            player.Clothes.Neck = player.Clothes.Neck == tile.Id ? 0 : tile.Id;
                        }
                        break;
                }

                foreach (var a in world.Players)
                {
                    player.SendClothes(a, true);
                }
            }
            else return;
        }

        private bool OnMoving(Player player, TankPacket packet)
        {
            player.Pos = packet.Pos;
            player.RotatingLeft = (packet.State & 0x10) != 0;
            return true;
        }

        private bool OnPunching(TankPacket packet, World world)
        {
            int x = packet.Pos3.X, y = packet.Pos3.Y, index = x + y * world.Width;

            var now = DateTime.Now;
            var block = world.Blocks[index];
            var tile = block.Fg.Id == 0 ? block.Bg : block.Fg;

            if (tile.Id == 0) return false;

            packet.Type = packet.Tile = 0x8;

            if ((now - block.Info.BrokenTime).TotalSeconds >= tile.DropChance)
            {
                block.Info.BrokenCount = 2;
            }
            else if (block.Info.BrokenCount >= tile.BreakHits)
            {
                packet.Type = 0x3;
                packet.Tile = 18;

                block.Info.BrokenCount = 1;

                if (block.Fg.Id == 0)
                {
                    block.Bg = Tile.Zero;
                }
                else
                {
                    block.Fg = Tile.Zero;
                }
            }
            else block.Info.BrokenCount++;

            block.Info.BrokenTime = now;

            Parallel.ForEach(world.Players, ply =>
            {
                ply.Send(packet);
            });

            return false;
        }

        private bool OnWrenching(Player player, TankPacket packet, World world)
        {
            int x = packet.Pos3.X, y = packet.Pos3.Y, index = x + y * world.Width;

            var block = world.Blocks[index];

            return false;
        }

        private bool OnPlacing(Player player, TankPacket packet, World world)
        {
            if (!player.Inventory.Find(packet.Tile, out _)) return false;

            int x = packet.Pos3.X, y = packet.Pos3.Y, index = x + y * world.Width;

            var tile = Tile.Parse(packet.Tile);
            var block = world.Blocks[index];

            if (tile.ActionType == 0x8)
                return false;

            player.Inventory.Remove(tile.Id, 1);

            if (tile.ActionType == 18)
            {
                block.Bg = tile;
            }
            else
            {
                if (block.Fg.Id != 0) return false;

                block.Fg = tile;
            }

            return true;
        }
    }
}
