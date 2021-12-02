using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2020
{
    internal class AdventDay1
    {
        // fields
        int[] data;

        // constructor
        public AdventDay1(IEnumerable<int> data)
        {
            this.data = data.ToArray();
        }

        // methods.
        public int FindSolution()
        {
            int result = 0;
            if (data.Length % 2 != 0) return result;
            for (int i = 0; i < data.Length - 1; i++)
            {
                for (int j = i + 1; j < data.Length; j++)
                {
                    if (data[i] + data[j] == 2020)
                    {
                        result = data[i] * data[j];
                    }
                }
            }
            return result;
        }

        public long FindSolution2()
        {
            int result = 0;

            int left, right;
            Array.Sort(data);

            for (int i = 0; i < data.Length -2; i++)
            {
                left = i + 1;
                right = data.Length - 1;

                while(left < right)
                {
                    result = data[i] + data[left] + data[right];
                    if(result == 2020)
                    {
                        return (long)(data[i] * data[left] * data[right]);
                    } else if (result < 2020)
                    {
                        left++;
                    } else
                    {
                        right--;
                    }
                }
            }
            return 0;
        }
    }
}
