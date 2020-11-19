using System;
using System.Collections.Generic;

namespace RhapsodyServer.Proton
{
    public class Vector2
    {
        public static Vector2 Zero => new Vector2(0, 0);

        public float X { get; set; }
        public float Y { get; set; }
        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }
        public Vector2() : this(0, 0) { }

        public override bool Equals(object obj) => base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();

        public static bool operator ==(Vector2 first, Vector2 second)
        {
            return first.X == second.X && first.Y == second.Y;
        }
        public static bool operator !=(Vector2 first, Vector2 second)
        {
            return first.X != second.X && first.Y != second.Y;
        }
        public static Vector2 operator +(Vector2 first, Vector2 second)
        {
            return new Vector2(first.X + second.X, first.Y + second.Y);
        }
        public static Vector2 operator -(Vector2 first, Vector2 second)
        {
            return new Vector2(first.X - second.X, first.Y - second.Y);
        }
        public static Vector2 operator *(Vector2 first, Vector2 second)
        {
            return new Vector2(first.X * second.X, first.Y * second.Y);
        }
        public static Vector2 operator /(Vector2 first, Vector2 second)
        {
            return new Vector2(first.X / second.X, first.Y / second.Y);
        }
        public float Distance(float x, float y)
        {
            float newX = X - x;
            float newY = Y - y;
            return (float)Math.Sqrt(newX * newX + newY * newY);
        }
    }
    public class Vector2i
    {
        public static Vector2i Zero => new Vector2i(0, 0);

        public int X { get; set; }
        public int Y { get; set; }
        public Vector2i(int x, int y)
        {
            X = x;
            Y = y;
        }
        public Vector2i(float x, float y) : this((int)x, (int)y) { }
        public Vector2i() : this(0, 0) { }
        public override bool Equals(object obj) => base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();

        public static bool operator ==(Vector2i first, Vector2i second)
        {
            return first.X == second.X && first.Y == second.Y;
        }
        public static bool operator !=(Vector2i first, Vector2i second)
        {
            return first.X != second.X && first.Y != second.Y;
        }
        public static Vector2i operator +(Vector2i first, Vector2i second)
        {
            return new Vector2i(first.X + second.X, first.Y + second.Y);
        }
        public static Vector2i operator -(Vector2i first, Vector2i second)
        {
            return new Vector2i(first.X - second.X, first.Y - second.Y);
        }
        public static Vector2i operator *(Vector2i first, Vector2i second)
        {
            return new Vector2i(first.X * second.X, first.Y * second.Y);
        }
        public static Vector2i operator /(Vector2i first, Vector2i second)
        {
            return new Vector2i(first.X / second.X, first.Y / second.Y);
        }
        public float Distance(int x, int y)
        {
            float newX = X - x;
            float newY = Y - y;
            return (float)Math.Sqrt(newX * newX + newY * newY);
        }
    }

    public class Vector3
    {
        public static Vector3 Zero => new Vector3(0, 0, 0);

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public Vector3() : this(0, 0, 0) { }
        public override bool Equals(object obj) => base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();

        public static bool operator ==(Vector3 first, Vector3 second)
        {
            return first.X == second.X && first.Y == second.Y && first.Z == second.Z;
        }
        public static bool operator !=(Vector3 first, Vector3 second)
        {
            return first.X != second.X && first.Y != second.Y && first.Z != second.Z;
        }
        public static Vector3 operator +(Vector3 first, Vector3 second)
        {
            return new Vector3(first.X + second.X, first.Y + second.Y, first.Z + second.Z);
        }
        public static Vector3 operator -(Vector3 first, Vector3 second)
        {
            return new Vector3(first.X - second.X, first.Y - second.Y, first.Z - second.Z);
        }
        public static Vector3 operator *(Vector3 first, Vector3 second)
        {
            return new Vector3(first.X * second.X, first.Y * second.Y, first.Z * second.Z);
        }
        public static Vector3 operator /(Vector3 first, Vector3 second)
        {
            return new Vector3(first.X / second.X, first.Y / second.Y, first.Z / second.Z);
        }
        public float Distance(float x, float y, float z)
        {
            float newX = X - x;
            float newY = Y - y;
            float newZ = Z - z;

            return (float)Math.Sqrt(newX * newX + newY * newY + newZ * newZ);
        }
    }
}
