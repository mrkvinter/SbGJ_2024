using System.Collections.Generic;

namespace Code.Utilities
{
    public static class CollectionExtensions
    {
        public static int IndexOf<T>(this IReadOnlyCollection<T> collection, T item)
        {
            var index = 0;
            foreach (var i in collection)
            {
                if (i.Equals(item))
                {
                    return index;
                }

                index++;
            }

            return -1;
        }
        
        public static void Shuffle<T>(this IList<T> list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                var temp = list[i];
                var randomIndex = UnityEngine.Random.Range(i, list.Count);
                list[i] = list[randomIndex];
                list[randomIndex] = temp;
            }
        }
    }
}