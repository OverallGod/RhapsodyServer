using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RhapsodyServer.Client
{
    public class Clothes
    {
        public int Hair = 0x0;
        public int Shirt = 0x0;
        public int Pants = 0x0;
        public int Feet = 0x0;
        public int Face = 0x0;
        public int Hand = 0x0;
        public int Back = 0x0;
        public int Mask = 0x0;
        public int Neck = 0x0;
        public int Ances = 0x0;

        [BsonRepresentation(BsonType.Int32, AllowOverflow = true)]
        public uint SkinColor = 0x8295C3FF;
    }
}
