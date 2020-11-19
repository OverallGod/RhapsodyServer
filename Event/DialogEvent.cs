using RhapsodyServer.Client;
using RhapsodyServer.Event.Attributes;
using RhapsodyServer.Event.Handler;
using RhapsodyServer.Proton;
using RhapsodyServer.Worlds;
using System.Linq;
using System.Reflection;

namespace RhapsodyServer.Event
{
    public class DialogEvent
    {
        public DialogEvent(Player player, World world, RTVar rt)
        {
            Handle(player, world, rt);
        }

        private void Handle(Player player, World world, RTVar rt)
        {
            if (rt.Size < 2)
                return;

            if (rt.Get(1).Key != "dialog_name")
                return;

            var name = "";
            var instance = new DialogEventHandler()
            {
                Player = player,
                World = world,
            };

            foreach (var pair in rt.Pairs)
            {
                if (pair.Key == "" || pair.Value == "")
                {
                    return;
                }
                else if (pair.Key == "tilex")
                {
                    if (int.TryParse(pair.Value, out var x))
                        instance.DialogData.Pos.X = x;
                }
                else if (pair.Key == "tiley")
                {
                    if (int.TryParse(pair.Value, out var y))
                        instance.DialogData.Pos.Y = y;
                }
                else if (pair.Key == "dialog_name")
                {
                    name = pair.Value;
                }

                if (name == "") continue;

                instance.Pair = pair;

                var method = instance.GetType()
                    .GetMethods()
                    .FirstOrDefault(x =>
                    {
                        var attribute = x.GetCustomAttribute(typeof(DialogAttribute));

                        if (attribute == null)
                            return false;

                        var type = attribute.GetType();

                        var property = type?.GetProperty("DialogName");

                        var value = property.GetValue(attribute);

                        return value.ToString() == name;
                    });

                var result = (bool)method?.Invoke(instance, null);

                if (!result) return;
            }
        }
    }
}
