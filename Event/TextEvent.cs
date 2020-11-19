using ENet.Managed;
using RhapsodyServer.Client;
using RhapsodyServer.DB;
using RhapsodyServer.Event.Attributes;
using RhapsodyServer.Event.Handler;
using RhapsodyServer.Proton;
using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RhapsodyServer.Event
{
    public class TextEvent
    {
        public TextEvent(Player player, ENetPacket packet)
        {
            Handle(player, GetText(packet.Data.ToArray()));
        }

        private string GetText(byte[] data) 
        {
            Array.Resize(ref data, data.Length - 1);
            return Encoding.ASCII.GetString(data.Skip(4).ToArray());
        }

        private async void Handle(Player player, string text)
        {
            var rt = RTVar.Parse(text);
            var world = await Database.GetWorld(player.CurrentWorld);

            if (!rt.IsValid())
            {
                return;
            }

            var instance = new TextEventHandler()
            {
                Player = player,
                World = world,
                Data = text,
                Rt = rt
            };

            var method = instance.GetType()
                .GetMethods()
                .FirstOrDefault(x =>
                {
                    var attribute = x.GetCustomAttribute(typeof(TextPacketAttribute));

                    if (attribute == null)
                        return false;

                    var type = attribute?.GetType();
                    var textProperty = type?.GetProperty("TextName");
                    var textValue = textProperty?.GetValue(attribute).ToString();

                    var lobbyProperty = type?.GetProperty("RequireLobby");
                    var lobbyValue = (bool) lobbyProperty?.GetValue(attribute);

                    var defaultProperty = type?.GetProperty("DefaultLobby");
                    var defaultValue = (bool) defaultProperty?.GetValue(attribute);

                    if (!defaultValue)
                        if (lobbyValue != player.InLobby)
                            return false;

                    return textValue != null && text.StartsWith(textValue);
                });

            //instance.GetType().GetProperty("Rt").SetValue(instance, rt);
            //instance.GetType().GetProperty("World").SetValue(instance, world);
            //instance.GetType().GetProperty("Player").SetValue(instance, player);

            method?.Invoke(instance, null);
        }
    }
}
