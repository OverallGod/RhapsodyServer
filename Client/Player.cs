using ENet.Managed;
using Evzer.Utilities;
using MongoDB.Bson.Serialization.Attributes;
using RhapsodyServer.DB;
using RhapsodyServer.Packet;
using RhapsodyServer.Proton;
using RhapsodyServer.ServerCore;
using System;
using System.Text;
using System.Threading.Tasks;

namespace RhapsodyServer.Client
{
    [BsonIgnoreExtraElements]
    public class Player
    {
        [BsonIgnore]
        public ENetPeer Peer { get; set; }

        public string Name { get; set; } = "";
        public string Password { get; set; } = "";
        public string ColorName { get; set; } = "`0";
        public string RequestedName { get; set; } = "";

        public string DisplayName => ColorName + Name;
        public string CurrentWorld { get; set; } = "EXIT";

        public bool InLobby { get; set; }
        public bool InDialog { get; set; }
        public bool RotatingLeft { get; set; }
        
        public int Gems { get; set; }
        public int NetId { get; set; }
        public int UserId { get; set; }

        public Vector2 Pos { get; set; }
        public Clothes Clothes { get; set; }
        public Inventory Inventory { get; set; }

        public Player(ENetPeer peer)
        {
            Pos = Vector2.Zero;
            Clothes = new Clothes();
            Inventory = new Inventory();

            peer.SetUserData(this);
            Peer = peer;
        }

        public async void Save()
        {
            await Task.Run(async () =>
            {
                await Database.SavePlayer(this);
            });
        }

        public void Send(byte[] data)
        {
            if (!Peer.IsNull && Peer.State == ENetPeerState.Connected)
            {
                Peer.Send(0, data, ENetPacketFlags.Reliable);
            }
        }

        public void Send(TankPacket tank) => Send(tank.Pack());

        public void Disconnect()
        {
            Peer.DisconnectLater(0);
        }

        public void Send(VariantList var, int id = -1, int delay = 0)
        {
            TankPacket tank = new TankPacket()
            {
                Type = 0x1,
                State = 0x8,
                NetId = id,
                Tile = delay,
                Size = var.Size,
                Data = var.SerializeToMemory()
            };

            Send(tank);
        }

        public void SendTextPacket(string text)
        {
            byte[] data = new byte[5 + text.Length];

            Buffer.BlockCopy(BitConverter.GetBytes(0x3), 0, data, 0, 4);
            Buffer.BlockCopy(Encoding.ASCII.GetBytes(text), 0, data, 4, text.Length);

            Send(data);
        }

        public void SendRequestWorldMenu()
        {
            VariantList var = new VariantList()
            {
                "OnRequestWorldSelectMenu",
                "default|retard\nadd_button|Showing: `wWorlds``|_catselect_|0.6|3529161471|\nadd_floater|Hey|69|0.55|3529161471\n"
            };

            Send(var);
        }

        public void SendTextOverlay(string text)
        {
            VariantList var = new VariantList() { "OnTextOverlay", text };

            Send(var);
        }

        public void SendTalkBubble(string text, bool append = false) => SendTalkBubble(NetId, text, append);
        public void SendTalkBubble(int netId, string text, bool append = false)
        {
            VariantList var = new VariantList() { "OnTalkBubble", netId, text, 0 };

            if (append)
                var.Add(1);

            Send(var);
        }

        public void SendPlayPositioned(int netId, string soundName, int delay = 0)
        {
            VariantList var = new VariantList() { "OnPlayPositioned", $"audio/{soundName}.wav" };

            Send(var, netId, delay);
        }

        public void SendDialog(string dialog)
        {
            InDialog = true;

            VariantList var = new VariantList() { "OnDialogRequest", dialog + "\n" };

            Send(var);
        }

        public void SendDialog(DialogBuilder db) => SendDialog(db.Result);

        public void SendLogin()
        {
            VariantList var = new VariantList()
            {
                "OnSuperMainStartAcceptLogonHrdxs47254722215a",
                (int)ItemBuilder.ItemDataHash,
                "127.0.0.1",
                "cache/",
                "cc.cz.madkite.freedom org.aqua.gg idv.aqua.bulldog com.cih.gamecih2 com.cih.gamecih com.cih.game_cih cn.maocai.gamekiller com.gmd.speedtime org.dax.attack com.x0.strai.frep com.x0.strai.free org.cheatengine.cegui org.sbtools.gamehack com.skgames.traffikrider org.sbtoods.gamehaca com.skype.ralder org.cheatengine.cegui.xx.multi1458919170111 com.prohiro.macro me.autotouch.autotouch com.cygery.repetitouch.free com.cygery.repetitouch.pro com.proziro.zacro com.slash.gamebuster",
                "proto=84|choosemusic=audio/mp3/about_theme.mp3|active_holiday=0|server_tick=226933875|clash_active=0|drop_lavacheck_faster=1|isPayingUser=0|"
            };

            Send(var);
        }

        public void SendOnRemove(int netId)
        {
            VariantList var = new VariantList() { "OnRemove", "netID|" + netId + "\n" };

            Send(var);
        }

        public void SendClothes(bool withSound = false)
        {
            SendClothes(this, withSound);
        }

        public void SendClothes(Player target, bool withSound = false)
        {
            Vector3 v = new Vector3(Clothes.Hair, Clothes.Shirt, Clothes.Pants);
            Vector3 v2 = new Vector3(Clothes.Feet, Clothes.Face, Clothes.Hand);
            Vector3 v3 = new Vector3(Clothes.Back, Clothes.Mask, Clothes.Neck);
            Vector3 v4 = new Vector3(Clothes.Ances, withSound ? 1.0f : 0.0f, 0.0f);

            VariantList var = new VariantList() { "OnSetClothing", v, v2, v3, (int)Clothes.SkinColor, v4 };

            target.Send(var, NetId);
        }

        public void SendSpawn(Vector2 pos)
        {
            string spawn = "spawn|avatar\nnetID|" + NetId + "\nuserID|" + UserId + "\ncolrect|0|0|20|30\nposXY|" + pos.X + "|" + pos.Y + "\nname|``" + Name + "``\ncountry|us\ninvis|0\nmstate|0\nsmstate|0\ntype|local\n";

            VariantList var = new VariantList() { "OnSpawn", spawn };

            Send(var, -1, -1);
        }

        public void SendSpawn(Player target, Vector2 pos, int id, int userId, string name)
        {
            string spawn = "spawn|avatar\nnetID|" + id + "\nuserID|" + userId + "\ncolrect|0|0|20|30\nposXY|" + pos.X + "|" + pos.Y + "\nname|``" + name + "``\ncountry|us\ninvis|0\nmstate|0\nsmstate|0\n";

            VariantList var = new VariantList() { "OnSpawn", spawn };

            target.Send(var, -1, -1);
        }

        public void SendConsoleMessage(string msg, int delay = 0)
        {
            VariantList var = new VariantList() { "OnConsoleMessage", msg };

            Send(var, -1, delay);
        }

        public void SendFailedToEnterWorld(int delay = 0)
        {
            VariantList var = new VariantList() { "OnFailedToEnterWorld", 1 };

            Send(var, -1, delay);
        }

        public void SendHelloPacket() => Send(new byte[] { 0x1, 0x0, 0x0, 0x0, 0x0 });
        public void SendLog(string msg) => SendTextPacket($"action|log\nmsg|{msg}\n");
        public void SendSound(string soundName) => SendTextPacket($"action|play_sfx\nfile|audio/{soundName}.wav\ndelayMS|0\n");
    }
}