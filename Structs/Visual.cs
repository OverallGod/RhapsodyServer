using RhapsodyServer.Client;
using RhapsodyServer.Packet;
using RhapsodyServer.Proton;
using RhapsodyServer.Worlds;
using System;

namespace RhapsodyServer.Structs
{
    public static class Visual
    {
        public const int BLOCK_EXTRA_VISUAL = 0x2;
        public const int BLOCK_WATER        = 0x4;
        public const int BLOCK_FIRE         = 0x10;
        public const int BLOCK_FACING_LEFT  = 0x20;
        public const int BLOCK_VISUAL       = 0x40;
        public const int BLOCK_PUBLIC       = 0x80;

        public static void ApplyCollect(Player player, World world, int dropId, Vector2 position)
        {
            var newPos = player.Pos - position;

            if (newPos.Y > 35 && newPos.X > 35) // checks
            {
                player.SendTalkBubble($"`0Too far away");
                return;
            }

            if (world.Blocks[(int)position.X / 32 + (int)position.Y / 32 * world.Width].Fg.CollisionType == 1) return;

            if (world.DroppedItems.TryGetValue(dropId, out var drop))
            {
                TankPacket tank = new TankPacket()
                {
                    Type = 0xe,
                    NetId = player.NetId,
                    Tile = dropId
                };

                var tile = Tile.Parse(drop.ItemId);

                world.DroppedItems.Remove(dropId);

                if (drop.ItemId == 112)
                {
                    player.Gems += drop.Amount;
                }
                else
                {
                    if (player.Inventory.Add(drop.ItemId, drop.Amount))
                    {
                        string text = "Collected `w" + drop.Amount + " " + tile.Name + "``.";

                        if (tile.Rarity != 999)
                        {
                            text += " Rarity: `w" + tile.Rarity + "``";
                        }

                        player.SendConsoleMessage(text);
                    }
                    else
                    {
                        if (player.Inventory.Find(drop.ItemId, out var a))
                        {
                            int newAmount = 200 - a;
                            int newDropAmount = drop.Amount - newAmount;

                            ApplyDrop(player, world, drop.Pos, drop.ItemId, newDropAmount, false);

                            string text = "Collected `w" + drop.Amount + " " + tile.Name + "``.";

                            if (tile.Rarity != 999)
                            {
                                text += " Rarity: `w" + tile.Rarity + "``";
                            }

                            player.Inventory.Add(drop.ItemId, newAmount);
                            player.SendConsoleMessage(text);
                        }
                    }
                }

                foreach (var ply in world.Players)
                {
                    ply.Send(tank);
                }

                world.Save();
            }
            else
            {
                player.SendTalkBubble("`0Too far away");
            }
        }

        public static void ApplyDrop(Player player, World world, Vector2 pos, int item, float amount, bool fromPlayer)
        {
            if (fromPlayer)
            {
                if (!player.Inventory.Find(item, out var count))
                {
                    return;
                }

                if (count < amount)
                    return;

                player.Inventory.Remove(item, (int)amount);
                player.Inventory.Modify(player.Peer);
            }

            Random rand = new Random();

            if (rand.Next(0, 2) == 0) pos.X += rand.Next(0, 8);
            else pos.X -= rand.Next(0, 8);

            if (rand.Next(0, 2) == 0) pos.Y -= rand.Next(0, 5);

            TankPacket packet = new TankPacket()
            {
                Type = 0xe,
                NetId = -1,
                Tile = item,
                Pos = pos
            };

            byte[] data = packet.Pack();

            Buffer.BlockCopy(BitConverter.GetBytes(amount), 0, data, 20, 4);

            player.Send(data);

            DroppedItem drop = new DroppedItem()
            {
                ItemId = item,
                Amount = (int)amount,
                Pos = pos
            };

            world.DroppedItems.Add(world.DropId++, drop);

            world.Save();
        }
    }
}
