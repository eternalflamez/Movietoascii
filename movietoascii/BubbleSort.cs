using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace movietoascii
{
    public static class BubbleSort
    {
        /// <summary>
        /// An unsorted array of integers
        /// </summary>
        private static List<Symbol> sorted;

        /// <summary>
        /// Indicates whether or not the array was changed since the last iteration
        /// </summary>
        private static bool changed = false;

        /// <summary>
        /// The highest point to iterate to
        /// </summary>
        private static int maxPoint;
        
        public static List<Symbol> Sort(List<Symbol> unsorted)
        {
            sorted = unsorted;
            maxPoint = sorted.Count;

            int index = 0;
            while (index != -1)
            {
                index = SolvePair(index);
            }

            return sorted;
        }

        /// <summary>
        /// Sorts a pair of numbers, the current index, and the number after that
        /// </summary>
        /// <param name="index">The current index to sort</param>
        /// <returns>The new index to solve</returns>
        private static int SolvePair(int index)
        {
            if (index + 1 != maxPoint)
            {
                if (sorted[index].Brightness > sorted[index + 1].Brightness)
                {
                    Symbol first = sorted[index];
                    sorted[index] = sorted[index + 1];
                    sorted[index + 1] = first;
                    
                    changed = true;
                }

                return ++index;
            }
            else
            {
                if (changed)
                {
                    maxPoint--;
                    changed = false;
                    return 0;
                }

                return -1;
            }
        }
    }
}
