using RhapsodyServer.Proton;

namespace RhapsodyServer.Event.Structs
{
    public class DialogDrop : IDialog
    {
        public int Count { get; set; }
        public int ItemId { get; set; }
        public Vector2i Pos { get; set; }
    }
}