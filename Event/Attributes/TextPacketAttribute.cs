using System;

namespace RhapsodyServer.Event.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class TextPacketAttribute : Attribute
    {
        public string TextName { get; }
        public bool RequireLobby { get; set; } = true;
        public bool DefaultLobby { get; set; } = false;

        public TextPacketAttribute(string textName) => TextName = textName;
    }
}
