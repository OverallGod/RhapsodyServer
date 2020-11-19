using Evzer.Utilities;
using RhapsodyServer.DB;
using RhapsodyServer.Event.Attributes;
using RhapsodyServer.Event.Base;
using RhapsodyServer.Proton;
using RhapsodyServer.ServerCore;
using RhapsodyServer.Worlds;
using System;

namespace RhapsodyServer.Event.Handler
{
    public class TextEventHandler : BasePacketModule
    {
        [TextPacket("requestedName", RequireLobby = false)]
        public void OnGuestLogin()
        {
            if (Rt.TryGet("requestedName", out var requestedName))
            {
                Player.RequestedName = requestedName;

                DialogBuilder db = new DialogBuilder();

                db.SetDefaultColor()
                    .AddBigLabel("Create account")
                    .AddSmallSpacer()
                    .AddTextInput("username", "Username:", "", 15)
                    .AddTextInput("password", "Password:", "", 15)
                    .AddSmallSpacer()
                    .EndDialog("account", "", "OK");

                Player.SendDialog(db);
            }
        }

        [TextPacket("action|dialog_return\n", DefaultLobby = true)]
        public void OnDialogReturn()
        {
            if (Player.InDialog)
            {
                Player.InDialog = false;
                Data += "edr|1\n";
                new DialogEvent(Player, World, RTVar.Parse(Data));
            }
            else Player.Disconnect();
        }
        
        [TextPacket("action|drop")]
        public void OnDrop()
        {
            var pair = Rt.Get(1);

            if (pair.Values.Count == 2 && pair.Value == "itemID") {
                if (int.TryParse(pair.Values[1], out var id))
                {
                    int x = (int)(Player.Pos.X + (Player.RotatingLeft ? -32 : 32)) / 32, y = (int)(Player.Pos.Y) / 32;

                    if (World.Blocks[x + y * World.Width].Fg.CollisionType == 1)
                    {
                        Player.SendTextOverlay("You can't drop that here, face somewhere with open space.");
                        Player.SendSound("cant_place_tile");
                        return;
                    }

                    if (Player.Inventory.Find(id, out var amount))
                    {
                        DialogBuilder db = new DialogBuilder();
                        db.SetDefaultColor()
                            .AddLabelWithIcon($"`wDrop {Tile.Parse(id).Name}``", id, false)
                            .AddTextBox("How many to drop?")
                            .AddTextInput("count", "", amount.ToString(), 5)
                            .AddEmbedData("itemID", id)
                            .EndDialog("drop", "Cancel", "OK");

                        Player.SendDialog(db);
                    }
                    else Player.Disconnect();
                }
                else Player.Disconnect();
            }
            else Player.Disconnect();
        }

        [TextPacket("action|join_request", RequireLobby = true)]
        public async void OnJoinRequest()
        {
            if (Rt.TryGet("name", out var name))
            {
                var world = await Database.GetWorld(name.ToUpper());
                world.Enter(Player);
            }
        }

        [TextPacket("action|enter_game\n", RequireLobby = false)]
        public void OnEnterGame()
        {
            Player.Inventory.Add(18, 1);
            Player.Inventory.Add(32, 1);
            Player.Inventory.Add(2, 200);
            Player.InLobby = true;
            Player.SendRequestWorldMenu();
        }

        [TextPacket("action|quit_to_exit", RequireLobby = true)]
        public void OnQuitToExit()
        {
            World.RemovePlayer(Player);
        }

        [TextPacket("action|quit", DefaultLobby = true)]
        public void OnQuit()
        {
            Player.Disconnect();
        }

        [TextPacket("action|refresh_item_data\n", RequireLobby = false)]
        public void OnRefreshItemData()
        {
            Player.SendLog("One moment, updating items data..");
            Player.Send(ItemBuilder.ItemData);
        }

        [TextPacket("tankIDName", RequireLobby = false)]
        public async void OnAccountLogin()
        {
            if (Rt.TryGet("tankIDName", out var name) && Rt.TryGet("tankIDPass", out var password))
            {
                Player.Name = name;
                Player.Password = password;

                if (await Database.ValidatePlayer(Player))
                {
                    Console.WriteLine("An Account Logged In\n" +
                    $"    [-] Username = {name}\n" +
                    $"    [-] Password = {password}\n");

                    Player.SendLogin();
                }
            }
        }
    }
}
