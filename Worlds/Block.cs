using RhapsodyServer.Proton;

namespace RhapsodyServer.Worlds
{
    public class Block
    {
        public Tile Fg { get; set; }
        public Tile Bg { get; set; }
        public BlockInfo Info { get; set; }

        public Vector2i Pos { get; set; }

        public Block(Vector2i pos)
        {
            Fg   = new Tile();
            Bg   = new Tile();
            Info = new BlockInfo();
            Pos  = pos;
        }
    }
}
