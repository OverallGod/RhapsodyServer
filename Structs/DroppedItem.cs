using RhapsodyServer.Proton;

namespace RhapsodyServer.Structs
{
    public class DroppedItem
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int Amount { get; set; }
        public Vector2 Pos { get; set; }
    }
}
