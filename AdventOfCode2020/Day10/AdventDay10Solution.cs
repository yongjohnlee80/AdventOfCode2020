using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode2020
{
    /// <summary>
    /// Type AdventDay10
    /// This is the unit testing entry.
    /// Various Solution methods are implmented to interact with the unit to be tested.
    /// This class also modifies the training data into proper data structure.
    /// </summary>
    internal class AdventDay10
    {
        // fields
        int[] data;
        // constructor
        public AdventDay10(IEnumerable<string> data)
        {
            this.data = data.Select(line => int.Parse(line)).ToArray();
        }

        public long FindSolution()
        {
            var Day10Solution = new LinkedAdapters(data);

            /// For Debug Purpose. Disregard the following code.

            //StringBuilder log = new StringBuilder();
            //foreach (var i in data)
            //{
            //    log.Append($"{i}\n");
            //}
            //File.WriteAllText("log.txt", log.ToString());

            return Day10Solution.Compute(new UseAllAdapters());
        }

        public long FindSolution2()
        {
            var Day10Solution = new LinkedAdapters(data);
            return Day10Solution.Compute(new DistinctArrangement());
        }
    }

    /**************************************************************************
     * Implement classes on separate files once test is successfully tested.
     * Author: Yong Sung Lee.
     * Date: 2021-12-07
     **************************************************************************/
    internal class JoltAdapter
    {
        protected int rating = 0;

        public JoltAdapter(int rating) { this.rating = rating; }
        public int Rating
        {
            get { return rating; }
        }
    }

    internal class LinkedAdapters
    {
        protected LinkedList<JoltAdapter> adapterList;

        public LinkedAdapters(int[] data)
        {
            adapterList = new LinkedList<JoltAdapter>();
            Array.Sort(data);
            foreach(var i in data)
            {
                adapterList.AddLast(new JoltAdapter(i));
            }
            adapterList.AddLast(new JoltAdapter(data.Last() + 3));

        }

        public long Compute(Requirement task)
        {
            return task.Compute(adapterList);
        }
    }

    internal abstract class Requirement
    {
        public abstract long Compute(Object data);
    }

    internal class UseAllAdapters : Requirement
    {
        public override long Compute(Object data)
        {
            var adapterList = data as LinkedList<JoltAdapter>;
            if (adapterList == null) return -1;

            int prev = 0;
            int[] delta = new int[4];
            foreach(var adapter in adapterList)
            {
                int temp = adapter.Rating - prev;
                if (temp <= 3)
                {
                    delta[temp]++;
                    prev = adapter.Rating;
                }
                else return -1;
            }

            return delta[1] * delta[3];
        }
    }

    internal class DistinctArrangement : Requirement
    {
        protected static Dictionary<int,long> nWays = new Dictionary<int,long>();

        protected long ComputeVariation(int variation)
        {
            switch(variation)
            {
                case 0:
                case 1:
                    return 1;
                case 2:
                    return 2;
                case 3:
                    return 4;
                default:
                    if (nWays.ContainsKey(variation)) return nWays[variation];
                    else
                    {
                        long result = ComputeVariation(variation - 1)
                            + ComputeVariation(variation - 2)
                            + ComputeVariation(variation - 3);
                        nWays[variation] = result;
                        return result;
                    }
            }
        }
        public override long Compute(object data)
        {
            var adapterList = (LinkedList<JoltAdapter>)data;
            if (adapterList == null) return -1;

            int variation = 0;
            long total = 1;

            int prev = 0;
            foreach(var adapter in adapterList)
            {
                int temp = adapter.Rating - prev;
                if (temp <= 3)
                {
                    switch (temp)
                    {
                        case 1: 
                            variation++;
                            break;
                        case 3:
                            total *= ComputeVariation(variation);
                            variation = 0;
                            break;
                        default:
                            break;
                    }
                    prev = adapter.Rating;

                }
                else return -1;
            }
            total *= ComputeVariation(variation);
            return total;
        }
    }
}