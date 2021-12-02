using System;
using System.Linq;

namespace AdventOfCode2020
{
    internal class AdventDay3
    {
        // fields
        string[] data;
        // constructor
        public AdventDay3(string[] data)
        {
            this.data = data;
        }
        // methods.
        public int FindSolution(int x, int y = 1)
        {
            int tree = 0;
            int pos = 0;
            
            for (int i = y; i < data.Length; i+=y)
            {
                int limit = data[i].Length;
                pos += x;
                if (pos >= limit) pos -= limit;
                if (data[i].ElementAt(pos) == '#') tree++;
            }
            return tree;
        }
    }
}
