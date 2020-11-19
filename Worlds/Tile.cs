using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RhapsodyServer.Worlds
{
    public class Tile
    {
        public int Id { get; set; }
        public int GrowTime { get; set; }
        public int EffectId { get; set; }
        public int ItemKind { get; set; }
        public int DropChance { get; set; }

        public byte BreakHits { get; set; }
        public byte ActionType { get; set; }
        public byte ClothingType { get; set; }
        public byte CollisionType { get; set; }

        public short Rarity { get; set; }

        public string Name { get; set; } = "";
        public string Description { get; set; } = "";

        public static List<Tile> Tiles { get; } = new List<Tile>();

        public static Tile Parse(int id)
        {
            if (id < 0 || id >= Tiles.Count)
                return Zero;

            return Tiles[id];
        }

        public static bool TryParse(int id, out Tile tile)
        {
            tile = Zero;

            if (id < 0 || id >= Tiles.Count) return false;

            tile = Tiles[id];

            return true;
        }

        public static Tile Zero { get; set; }
    }
}
