using RhapsodyServer.Proton;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RhapsodyServer.Packet
{
    public class TankPacket
    {
        public int Type { get; set; }
        public int Size { get; set; }
        public int Tile { get; set; }
        public int NetId { get; set; }
        public int State { get; set; }
        public int Delay { get; set; }
        public int Target { get; set; }
        public byte[] Data { get; set; } = new byte[0];
        public Vector2 Pos { get; set; } = Vector2.Zero;
        public Vector2 Pos2 { get; set; } = Vector2.Zero;
        public Vector2i Pos3 { get; set; } = Vector2i.Zero;
        public float Padding { get; set; }

        private int MemoryPos { get; set; } = 0;

        public static TankPacket Unpack(byte[] data)
        {
            MemoryStream ms = new MemoryStream(data);
            BinaryReader br = new BinaryReader(ms);

            br.ReadInt32(); // message type

            TankPacket tank = new TankPacket()
            {
                Type = br.ReadInt32(),
                NetId = br.ReadInt32(),
                Target = br.ReadInt32(),
                State = br.ReadInt32(),
                Delay = br.ReadInt32(),
                Tile = br.ReadInt32(),
                Pos = new Vector2(br.ReadSingle(), br.ReadSingle()),
                Pos2 = new Vector2(br.ReadSingle(), br.ReadSingle()),
                Padding = br.ReadInt32(),
                Pos3 = new Vector2i(br.ReadInt32(), br.ReadInt32()),
                Size = br.ReadInt32(),
            };
            return tank;
        }

        public void Skip(int size) => MemoryPos += size;

        public byte[] Pack()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter br = new BinaryWriter(ms);

            br.Write(0x4);
            br.Write(Type);
            br.Write(NetId);
            br.Write(Target);
            br.Write(State);
            br.Write(Delay);
            br.Write(Tile);
            br.Write(Pos.X);
            br.Write(Pos.Y);
            br.Write(Pos2.X);
            br.Write(Pos2.Y);
            br.Write(Padding);
            br.Write(Pos3.X);
            br.Write(Pos3.Y);
            br.Write(Size);
            br.Write(Data);
            br.Write((byte)0);

            return ms.ToArray();
        }

        public void Write(int value)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(value), 0, Data, MemoryPos, sizeof(int));
            MemoryPos += sizeof(int);
        }

        public void Write(string value)
        {
            Buffer.BlockCopy(Encoding.ASCII.GetBytes(value), 0, Data, MemoryPos, value.Length);
            MemoryPos += value.Length;
        }

        public void Write(short value)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(value), 0, Data, MemoryPos, sizeof(short));
            MemoryPos += sizeof(short);
        }

        public void Write(byte value)
        {
            Data[MemoryPos++] = value;
        }

        public void Write(float value)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(value), 0, Data, MemoryPos, sizeof(float));
            MemoryPos += sizeof(float);
        }

        public void Write(uint value)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(value), 0, Data, MemoryPos, sizeof(uint));
            MemoryPos += sizeof(uint);
        }

        public void Write(Vector2 value)
        {
            Write(value.X);
            Write(value.Y);
        }

        public void Write(Vector2i value)
        {
            Write(value.X);
            Write(value.Y);
        }

        public void Write(Vector3 value)
        {
            Write(value.X);
            Write(value.Y);
            Write(value.Z);
        }
    }
}