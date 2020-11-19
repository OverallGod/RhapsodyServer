using RhapsodyServer.DB;
using RhapsodyServer.Event.Attributes;
using RhapsodyServer.Event.Base;
using RhapsodyServer.Event.Structs;
using RhapsodyServer.Proton;
using RhapsodyServer.Structs;
using System.Linq;
using System.Timers;

namespace RhapsodyServer.Event.Handler
{
    public class DialogEventHandler : BasePacketModule
    {
        [Dialog("account")]
        public bool AccountDialog()
        {
            var drop = DialogData as DialogAccount;

            switch (Pair.Key)
            {
                case "dialog_name":
                    {
                        DialogData = new DialogAccount() { Pos = new Vector2i() };
                        return true;
                    }
                case "username":
                    {
                        if (Pair.Value.Length <= 15)
                        {
                            drop.Username = Pair.Value;
                            return true;
                        }
                        break;
                    }
                case "password":
                    {
                        if (Pair.Value.Length <= 15)
                        {
                            drop.Password = Pair.Value;
                            return true;
                        }
                        break;
                    }
                case "edr":
                    {
                        if (Database.IsPlayerExist(drop.Username).Result)
                        {
                            Player.SendConsoleMessage("`4That account is already exist.");
                            return false;
                        }
                        else if (drop.Username.Any(x => !char.IsLetterOrDigit(x)))
                        {
                            Player.SendConsoleMessage("`4Name can't contains any symbol.");
                            return false;
                        }

                        Player.Name = drop.Username;
                        Player.Password = drop.Password;
                        Player.SendGrowID();
                        Player.SendSound("piano_nice");
                        Player.Save();
                        Player.SendConsoleMessage("`5Please wait! Redirecting..");

                        Timer timer = new Timer(2000);
                        timer.Start();
                        timer.Elapsed += (sender, e) =>
                        {
                            Player.SendHelloPacket();
                            timer.Dispose();
                        };

                        break;
                    }
            }

            return false;
        }

        [Dialog("drop")]
        public bool DropDialog()
        {
            var drop = DialogData as DialogDrop;

            switch (Pair.Key)
            {
                case "dialog_name":
                    {
                        DialogData = new DialogDrop() { Pos = new Vector2i() };
                        return true;
                    }
                case "count":
                    {
                        if (int.TryParse(Pair.Value, out var count))
                        {
                            drop.Count = count;
                        }
                        return true;
                    }
                case "itemID":
                    {
                        if (int.TryParse(Pair.Value, out var id))
                        {
                            drop.ItemId = id;
                        }
                        return true;
                    }
                case "edr":
                    {
                        Visual.ApplyDrop(Player, World, 
                            new Vector2(Player.Pos.X + (Player.RotatingLeft ? -32 : 32), Player.Pos.Y),
                            drop.ItemId, drop.Count, true);
                        break;
                    }
            }
            return false;
        }
    }
}
