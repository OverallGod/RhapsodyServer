using MongoDB.Driver;
using RhapsodyServer.Client;
using RhapsodyServer.Proton;
using RhapsodyServer.Structs;
using RhapsodyServer.Worlds;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RhapsodyServer.DB
{
    public static class Database
    {
        public static Dictionary<string, World> Worlds { get; } = new Dictionary<string, World>();

        public static MongoClient Client { get; } = new MongoClient();
        public static IMongoDatabase Db { get; } = Client.GetDatabase("server");
        public static IMongoCollection<World> WorldCollection { get; } = Db.GetCollection<World>("world");
        public static IMongoCollection<Player> PlayerCollection { get; } = Db.GetCollection<Player>("player");

        public static async Task<bool> ValidatePlayer(Player player)
        {
            var pass = player.Password;
            var peer = player.Peer;

            if (await IsPlayerExist(player.Name))
            {
                player = await LoadPlayer(player.Name);
                player.Peer = peer;

                if (player.Password == pass)
                {
                    return true;
                }
            }

            player.SendTextPacket("action|set_url\nurl|https://discord.gg/xsRnm3R\nlabel|`$Join Discord``");
            player.SendTextPacket("action|log\nmsg|`4Unable to log on:`` That `wGrowID`` doesn't seem valid, or the password is wrong.  If you don't have one, press `wCancel``, un-check `w'I have a GrowID'``, then click `wConnect``.\n");
            player.SendTextPacket("action|logon_fail\n");
            player.Disconnect();
            return false;
        }

        public static async Task SavePlayer(Player player)
        {
            await PlayerCollection.ReplaceOneAsync(x => x.Name == player.Name, player, new ReplaceOptions { IsUpsert = true });
        }

        public static async Task<Player> LoadPlayer(string name)
        {
            var playerColl = await PlayerCollection.FindAsync(x => x.Name.ToLower() == name.ToLower());
            var ply = await playerColl.FirstOrDefaultAsync();

            if (ply != null)
            {
                return ply;
            }
            else
            {
                throw new NullReferenceException();
            }
        }

        public static async Task<World> GetWorld(string worldName)
        {
            if (Worlds.TryGetValue(worldName, out var w))
            {
                return w;
            }
            else if (await IsWorldExist(worldName))
            {
                World world = await LoadWorld(worldName);
                Worlds.Add(worldName, world);
                return world;
            }
            return GenerateNormalWorld(worldName);
        }

        public static async Task<World> LoadWorld(string worldName)
        {
            var worldColl = await WorldCollection.FindAsync(x => x.Name == worldName);
            var world = await worldColl.FirstOrDefaultAsync();

            if (world != null)
            {
                world.Players = new List<Player>();
                return world;
            }
            else
            {
                return new World(worldName, 100, 60);
            }
        }

        public static async Task<bool> IsWorldExist(string worldName)
        {
            return await (await WorldCollection.FindAsync(x => x.Name == worldName)).AnyAsync();
        }

        public static async Task<bool> IsPlayerExist(string name)
        {
            return await (await PlayerCollection.FindAsync(x => x.Name == name)).AnyAsync();
        }

        public static async void SaveWorld(World world)
        {
            await WorldCollection.ReplaceOneAsync(x => x.Name == world.Name, world, new ReplaceOptions { IsUpsert = true });
        }

        private static World GenerateNormalWorld(string worldName)
        {
            World world = new World(worldName, 100, 60);

            Random rand = new Random();

            int doorPos = rand.Next(0, 100), width = 100, height = 60;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int square = x + y * 100;
                    world.Blocks[square] = new Block(new Vector2i(x, y));

                    var block = world.Blocks[square];

                    if (y >= height / 2)
                    {
                        if (y >= height - 5)
                        {
                            block.Fg = Tile.Parse(8);
                            block.Bg = Tile.Parse(14);
                        }
                        else if (y == height / 2 && x == doorPos)
                        {
                            block.Fg = Tile.Parse(8);
                            block.Bg = Tile.Parse(14);
                        }
                        else if (y <= height - 5 && y >= height - 10)
                        {
                            var num = rand.Next(0, 76);
                            block.Fg = Tile.Parse(num > 20 ? 2 : (num > 4 ? 4 : 10));
                            block.Bg = Tile.Parse(14);
                        }
                        else
                        {
                            block.Fg = Tile.Parse(2);
                            block.Bg = Tile.Parse(14);
                            if (y >= (height / 2) + 2)
                            {
                                block.Fg = Tile.Parse(rand.Next(0, 51) > 1 ? 2 : 10);
                            }
                        }
                    }
                    else if (y == height / 2 - 1 && x == doorPos)
                    {
                        block.Fg = Tile.Parse(6);
                        block.Bg = Tile.Parse(14);
                    }
                    else
                    {
                        block.Fg = Tile.Parse(0);
                        block.Bg = Tile.Parse(0);
                    }
                }
            }

            SaveWorld(world);
            Worlds.Add(world.Name, world);

            return world;
        }
    }
}
