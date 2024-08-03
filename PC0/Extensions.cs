using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC0
{
    internal static class Extensions
    {
        static Random rng = new Random();
        public static VariableList<T> RotateThrough<T>(this VariableList<T> list)
        {
            var L = new VariableList<T>();
            L.Add(list[list.Count - 1]);
            for (int i = 0; i < list.Count - 1; i++)
                L.Add(list[i]);
            return L;
        }

        public static int RandomIndex<T>(this HashSet<T> s)
        {
            return rng.Next(s.Count);
        }

        public static bool IsEmpty<T>(this IEnumerable<T> s)
        {
            return s.Count() == 0;
        }

        public static void Shuffle<T>(this List<T> list)
        {
            for(int i = 0; i < list.Count; i++)
            {
                var index = rng.Next(list.Count);
                var val = list[index];
                list.RemoveAt(index);
                list.Add(val);
            }
        }


    }

    


}
