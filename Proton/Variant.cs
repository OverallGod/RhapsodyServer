using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace RhapsodyServer.Proton
{
    public class Variant
    {
        public object Obj { get; set; }

        public enum VarType
        {
            TYPE_UNUSED,
            TYPE_FLOAT,
            TYPE_STRING,
            TYPE_VECTOR2, 
            TYPE_VECTOR3,
            TYPE_UINT32,
            TYPE_ENTITY,
            TYPE_COMPONENT,
            TYPE_RECT,
            TYPE_INT32
        }

        public VarType Type { get; }

        public Variant(int i)
        {
            Type = VarType.TYPE_INT32;
            Obj = i;
        }

        public Variant(uint i)
        {
            Type = VarType.TYPE_UINT32;
            Obj = i;
        }

        public Variant(float i)
        {
            Type = VarType.TYPE_FLOAT;
            Obj = i;
        }

        public Variant(string i)
        {
            Type = VarType.TYPE_STRING;
            Obj = i;
        }

        public Variant(Vector2 i)
        {
            Type = VarType.TYPE_VECTOR2;
            Obj = i;
        }

        public Variant(Vector3 i)
        {
            Type = VarType.TYPE_VECTOR3;
            Obj = i;
        }

        public int GetInt32() => (int)Obj;
        public uint GetUInt32() => (uint)Obj;
        public float GetFloat() => (float)Obj;
        public string GetString() => (string)Obj;
        public Vector2 GetVector2() => (Vector2)Obj;
        public Vector3 GetVector3() => (Vector3)Obj;

        public static implicit operator int(Variant var) => var.GetInt32();
        public static implicit operator Variant(int var) => new Variant(var);

        public static implicit operator uint(Variant var) => var.GetUInt32();
        public static implicit operator Variant(uint var) => new Variant(var);

        public static implicit operator float(Variant var) => var.GetFloat();
        public static implicit operator Variant(float var) => new Variant(var);

        public static implicit operator string(Variant var) => var.GetString();
        public static implicit operator Variant(string var) => new Variant(var);

        public static implicit operator Vector2(Variant var) => var.GetVector2();
        public static implicit operator Variant(Vector2 var) => new Variant(var);

        public static implicit operator Vector3(Variant var) => var.GetVector3();
        public static implicit operator Variant(Vector3 var) => new Variant(var);
    }

    public class VariantList : IEnumerable<Variant>
    {
        List<Variant> Arguments { get; } = new List<Variant>();

        public int Size { get; private set; }

        public byte[] SerializeToMemory()
        {
            byte[] data = new byte[1000];

            int pos = 0, index = 0;

            data[pos++] = (byte)Arguments.Count;
            
            foreach (var arg in Arguments)
            {
                data[pos++] = (byte)index++;
                data[pos++] = (byte)arg.Type;

                switch (arg.Type)
                {
                    case Variant.VarType.TYPE_FLOAT:
                        {
                            Buffer.BlockCopy(BitConverter.GetBytes(arg.GetFloat()), 0, data, pos, 4);
                            pos += 4;
                            break;
                        }
                    case Variant.VarType.TYPE_VECTOR2:
                        {
                            Buffer.BlockCopy(BitConverter.GetBytes(arg.GetVector2().X), 0, data, pos, 4);
                            Buffer.BlockCopy(BitConverter.GetBytes(arg.GetVector2().Y), 0, data, pos + 4, 4);
                            pos += 8;
                            break;
                        }
                    case Variant.VarType.TYPE_VECTOR3:
                        {
                            Buffer.BlockCopy(BitConverter.GetBytes(arg.GetVector3().X), 0, data, pos, 4);
                            Buffer.BlockCopy(BitConverter.GetBytes(arg.GetVector3().Y), 0, data, pos + 4, 4);
                            Buffer.BlockCopy(BitConverter.GetBytes(arg.GetVector3().Z), 0, data, pos + 8, 4);
                            pos += 12;
                            break;
                        }
                    case Variant.VarType.TYPE_STRING:
                        {
                            var str = arg.GetString();
                            Buffer.BlockCopy(BitConverter.GetBytes(str.Length), 0, data, pos, 4);
                            Buffer.BlockCopy(Encoding.ASCII.GetBytes(str), 0, data, pos + 4, str.Length);
                            pos += 4 + str.Length;
                            break;
                        }
                    case Variant.VarType.TYPE_UINT32:
                        {
                            Buffer.BlockCopy(BitConverter.GetBytes(arg.GetUInt32()), 0, data, pos, 4);
                            pos += 4;
                            break;
                        }
                    case Variant.VarType.TYPE_INT32:
                        {
                            Buffer.BlockCopy(BitConverter.GetBytes(arg.GetInt32()), 0, data, pos, 4);
                            pos += 4;
                            break;
                        }
                }
            }

            Array.Resize(ref data, pos);

            Size = pos;

            return data;
        }

        public Variant this[int index]
        {
            get { return Arguments[index]; }
            set { Arguments.Insert(index, value); }
        }

        public void Add(Variant variant) => Arguments.Add(variant);
        IEnumerator IEnumerable.GetEnumerator() => Arguments.GetEnumerator();
        public IEnumerator<Variant> GetEnumerator() => Arguments.GetEnumerator();
    }
}
