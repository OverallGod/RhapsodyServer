using System;

namespace RhapsodyServer.Worlds
{
    public class BlockInfo
    {
        public bool HaveFire { get; set; }
        public bool HaveWater { get; set; }
        public bool HaveVisual { get; set; }
        public bool HaveExtraVisual { get; set; }

        public bool FacingLeft { get; set; }
        public bool OpenToPublic { get; set; }

        public int BrokenCount { get; set; } = 1;
        public DateTime BrokenTime { get; set; } = DateTime.Now;
    }
}
