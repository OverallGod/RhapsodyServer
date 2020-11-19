using RhapsodyServer.Client;
using RhapsodyServer.Event.Structs;
using RhapsodyServer.Proton;
using RhapsodyServer.Worlds;

namespace RhapsodyServer.Event.Base
{
    public abstract class BasePacketModule
    {
        public RTVar Rt { get; set; }
        public World World { get; set; }
        public Player Player { get; set; }
        public RTVar.Pair Pair { get; set; }

        public string Data { get; set; }

        public IDialog DialogData { get; set; }
    }
}
