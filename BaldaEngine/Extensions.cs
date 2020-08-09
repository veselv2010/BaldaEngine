using System;
using System.Collections.Generic;
using System.Linq;

namespace BaldaEngine
{
    public static class Extensions
    {
        public static Dictionary<K, V> Merge<K, V>(IEnumerable<Dictionary<K, V>> dictionaries)
        {
            return dictionaries.SelectMany(x => x)
                            .ToDictionary(x => x.Key, y => y.Value);
        }

        public static char[] GetCyrillicAlphabet()
        {
            char ch;
            int n = 0;
            char[] mass = new char[32];
            for (int i = 1072; i <= 1103; i++)
            {
                ch = Convert.ToChar(i);
                mass[n] = ch;
                n++;
            }

            return mass;
        }
    }
}
