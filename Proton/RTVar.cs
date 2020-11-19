using System;
using System.Collections.Generic;

namespace RhapsodyServer.Proton
{
    public class RTVar
    {
        public class Pair
        {
            public string Key { get; set; }
            public string Value { get; set; }
            public List<string> Values { get; } = new List<string>();

            public static Pair Parse(string text)
            {
                Pair pair = new Pair();
                int index = 0;

                foreach (var str in text.Split('|'))
                {
                    if (index == 0)
                        pair.Key = str;
                    else if (index == 1)
                        pair.Value = str;

                    if (index != 0)
                        pair.Values.Add(str);

                    index++;
                }

                return pair;
            }
        }

        public List<Pair> Pairs { get; } = new List<Pair>();
        public int Size => Pairs.Count;

        public Pair Get(string key)
        {
            foreach (var pair in Pairs)
            {
                if (pair.Key == key)
                    return pair;
            }

            throw new KeyNotFoundException();
        }

        public Pair Get(int index)
        {
            if (index < 0 || index >= Pairs.Count)
            {
                Console.WriteLine($"[ERROR] No pair found with index {index}.");
                return new Pair();
            }
            return Pairs[index];
        }

        public bool IsValid()
        {
            if (Pairs.Count < 1)
                return false;

            if (Pairs[0].Values.Count < 1)
                return false;

            return true;
        }

        public bool TryGet(string key, out string value)
        {
            foreach (var pair in Pairs)
            {
                if (pair.Key == key)
                {
                    value = pair.Value;
                    return true;
                }
            }

            value = "";

            return false;
        }

        public bool Contains(string key)
        {
            foreach (var pair in Pairs)
            {
                if (pair.Key == key)
                    return true;
            }
            return false;
        }

        public static RTVar Parse(string text)
        {
            RTVar rt = new RTVar();

            foreach (var str in text.Split('\n'))
            {
                rt.Pairs.Add(Pair.Parse(str));
            }

            return rt;
        }
    }
}
