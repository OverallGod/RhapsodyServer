using RhapsodyServer.Proton;

namespace RhapsodyServer.Event.Structs
{
    public class DialogAccount : IDialog
    {
        public Vector2i Pos { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
