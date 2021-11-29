using System;
using System.Collections.Generic;
using System.Text;

namespace GraphDecomposer.Utils
{
    public static class ArrayUtils
    {
        private static Random r;
        static ArrayUtils()
        {
            r = new Random();            
        }
        
        public static void ShuffleArray<T>(List<T> z)
        {
            for (int i = 0; i < z.Count; i++)
            {
                int j = r.Next(0, z.Count - 1);
                T tmp = z[i];
                z[i] = z[j];
                z[j] = tmp;
            }
        }

        public static T GetRandomElement<T>(List<T> list)
        {
            int ind = r.Next(0, list.Count - 1);

            return list[ind];
        }

        public static bool Equals(int[] a, int[] b)
        {
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                    return false;
            }
            return true;
        }
    }
}
