using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode2020
{
    /// <summary>
    /// Type LogStat
    /// This class is used to write out program stats onto a file for debugging purposes.
    /// </summary>
    public class LogStat
    {
        StringBuilder logText  = new StringBuilder();

        public LogStat Append(string text)
        {
            logText.Append(text);
            return this;
        }

        public void LogOnFile(string fileName = "log.txt", bool resetBuffer = true)
        {
            File.WriteAllText(fileName, logText.ToString());
            if(resetBuffer) logText.Clear();
        }
    }
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
            // For first half of the problem
        {
            var Day10Solution = new LinkedAdapters(data);

            return Day10Solution.Compute(new UseAllAdapters());
        }

        public long FindSolution2()
            // For second half of the problem
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

    /// <summary>
    /// Type JoltAdapter
    /// In fact, this class isn't necessary at this moment, it works as a integer wrapper class
    /// at this time, but only time can tell if JoltAdapter must be implemented as more complex
    /// data type later on. Thus, implementing as class from the start for possible future 
    /// modification. 
    /// </summary>
    internal class JoltAdapter
    {
        protected int rating = 0; // Jolt Rating.

        public JoltAdapter(int rating) { this.rating = rating; }
        public int Rating
        {
            get { return rating; }
        }
    }

    /// <summary>
    /// Type LinkedAdpaters
    /// This class implements a number of JoltAdpaters in LinkedList data structure.
    /// There is absolutely no reason LinkedList was chosen over other data types such as Array
    /// or ArrayList. It was simply chosen to give author a chance to interact with the 
    /// LinkedList data structure provided by STL.
    /// </summary>
    internal class LinkedAdapters
    {
        protected LinkedList<JoltAdapter> adapterList;
                            // Linked List of Jolt Adapters.

        /// <summary>
        /// Constructor.
        /// Also initialize the linked list.
        /// </summary>
        /// <param name="data"></param>
        public LinkedAdapters(int[] data)
        {
            adapterList = new LinkedList<JoltAdapter>();

            Array.Sort(data); // sort the adapters accoridng to their ratings.
            foreach(var i in data)
            {
                adapterList.AddLast(new JoltAdapter(i));
                        // Add each adapter.
            }
            adapterList.AddLast(new JoltAdapter(data.Last() + 3));
                    // The final rating for the collection which is +3 of the highest.

        }

        // Solve the requirement for the problem.
        public long Compute(Requirement task)
        {
            return task.Compute(adapterList);
        }
    }

    /// <summary>
    /// Type Abstract Requirement
    /// Implementing Strategy Pattern.
    /// Loose coupling with the data type Linked Adapters for future
    /// usage of the mentioned data type.
    /// </summary>
    internal abstract class Requirement
    {
        public abstract long Compute(Object data);
                        // implement this method for your solution.
    }

    /// <summary>
    /// Type UseAllAdapters
    /// This class implements the solution for the first half of the day 10 question.
    /// </summary>
    internal class UseAllAdapters : Requirement
    {
        public override long Compute(Object data)
        {
            // Casting the argument into workable type (linked list).
            var adapterList = data as LinkedList<JoltAdapter>;
            if (adapterList == null) return -1; // can't be null.

            int prev = 0; // linked list makes it hard to access the previous element with index, so let's keep track of it.
            int[] delta = new int[4]; // counter for Jolt differences between elements.
            foreach(var adapter in adapterList)
            {
                int temp = adapter.Rating - prev; // what's the Jolt difference, reall?
                if (temp <= 3)
                    // let's make sure it's connectable between the elements.
                {
                    delta[temp]++;
                    prev = adapter.Rating; // like I said, we need to keep track of the previous element's Jolt rating.
                }
                else return -1; // I shouuld really implement this as "throw new (exception)", next assignment.
            }

            return delta[1] * delta[3]; // This is what we want. 
        }
    }

    /// <summary>
    /// Type DistinctArragement
    /// This class implements the solutoin for the second half of the day 10 question.
    /// </summary>
    internal class DistinctArrangement : Requirement
    {
        protected static Dictionary<int,long> nWays = new Dictionary<int,long>();

        /// <summary>
        /// Helper function to compute the probabilities of the paths.
        /// </summary>
        /// <param name="variation"></param>
        /// <returns></returns>
        protected long ComputeVariation(int variation)
        {
            switch(variation)
            {
                // Base case 0, 1 occurences of Jolt Difference of 1 in sequence.
                case 0:
                case 1:
                    return 1;
                // Jolt Differnce of 1 happening in two
                case 2:
                    return 2;
                // Jolt Differnce of 1 happening in three
                case 3:
                    return 4;
                // If any more than compute using the the previous three path counts.
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
        /// <summary>
        /// Method Compute
        /// This method finds the solution for the second half.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override long Compute(object data)
        {
            var adapterList = (LinkedList<JoltAdapter>)data;
                                    // Cast the argument data into the the linked list
            if (adapterList == null) return -1;
                                    // better not be null.

            int variation = 0; // indicates the consequent frequency of Jolt differnce of one.
            long total = 1; // Total number of path, we begin by one, the linear non variant one.

            int prev = 0; // since author chose linked list, we need to keep track of the previous Jolt rating
                            // to compare. No convinient way of accessing elements with indexing.

            // Let the iteration begin from the first adapter to the last.
            foreach(var adapter in adapterList)
            {
                int temp = adapter.Rating - prev; // what is the Jolt Differnce?
                if (temp <= 3)
                    // Is it less than or equal to 3? This means it is chained and we want this!
                {
                    switch (temp)
                    {
                        case 1: 
                            variation++; // Jolt Differece of One, possibilities of more than one path!
                            break;
                        case 3:
                            // Variation resets and compute the number of paths according to the tracked
                            // frequencies of Jolt Difference of one in sequence.
                            total *= ComputeVariation(variation);
                            variation = 0;
                            break;
                        default:
                            break;
                    }
                    prev = adapter.Rating; // Like I said, we need to keep track of the previous Jolt Rating.

                }
                else return -1; // I really should implement this as throw new exception. Next Time for sure.
            }
            total *= ComputeVariation(variation); // We have to do this just in case the variation count wasn't reset.
            return total;
        }
    }
}