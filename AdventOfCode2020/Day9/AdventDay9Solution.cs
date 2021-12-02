using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode2020
{
    /// <summary>
    /// Type AdventDay8
    /// This is the unit testing entry.
    /// Various Solution methods are implmented to interact with the unit to be tested.
    /// This class also modifies the training data into proper data structure.
    /// </summary>
    internal class AdventDay9
    {
        // fields
        string[] data;
        // constructor
        public AdventDay9(string[] data)
        {
            this.data = data;
        }
        // methods.
        public List<long> ConvertData()
        {
            var numbers = new List<long>();
            foreach(string i in this.data)
            {
                numbers.Add(Convert.ToInt64(i));
            }
            return numbers;
        }
        public long FindSolution(int n = 5)
        {
            var numbers = ConvertData();
            PreambleNumber pNumber = new PreambleNumber(numbers, n);

            /// For Debug Purpose. Disregard the following code.
            
            //StringBuilder log = new StringBuilder();
            //foreach (long i in numbers)
            //{
            //    log.Append($"{i}\n");
            //}
            //File.WriteAllText("log.txt", log.ToString());

            return pNumber.FindFirstError();
        }

        public long FindSolution2(int n = 5)
        {
            var numbers = ConvertData();
            PreambleNumber pNumber = new PreambleNumber(numbers, n);
            return pNumber.FindWeakness();
        }
    }

    /**************************************************************************
     * Implement classes on separate files once test is successfully tested.
     * Author: Yong Sung Lee.
     * Date: 2021-11-28.
     **************************************************************************/

    /// <summary>
    /// Type Preamble Number works with a list of natural numbers to find
    /// whether an element matches the sum of any two numbers among a set of n-previous members
    /// </summary>
    public class PreambleNumber
    {
        List<long> numbers = new List<long>(); // a set of natrual numbers.
        int n; // n dictates how far behind the algorithm can go to find preamble number.

        // constructor / initialization of member variables.
        public PreambleNumber(List<long> data, int n = 5)
        {
            numbers = data;
            this.n = n;
        }

        /// <summary>
        /// This method finds a preamble pair numbers, (a+b) = number[index], that matches the number 
        /// at position index from the set of numbers in its instance.
        /// </summary>
        /// <param name="index">The index that will be used as the sum.</param>
        /// <returns>returns the indices of preamble pair in tuple</returns>
        public (int, int) FindPreamblePair(int index)
        {
            if (index < n) return (0, 0); 
                // the indexed number must be allowed to contained n number of previous number.

            /// Arthor's time complextity analysis.
            /// Let n be the size of collection of number,
            /// Let k be the distance the algorithm can go back to find the preamble pair.
            /// The following code's TC is O(k^2)
            /// When paired with FindFirstError(), the TC becomes O(n*k^2)
            /// When k -> n, the whole Algorithm TC becomes O(n^2),
            /// since (k^2) is bigger than its counter part (n)
            /// But in most cases, the whole Algorithm TC is at O(n)
            /// Thereby, hashing method is considered counter productive in two folds.
            /// First, we must separate the two scenarios where k is smaller then n
            /// and where k is bigger than n, thereby creating fork for two versions.
            /// Secondly, hashing requires extra spacial complexity as well as extra computing methods.
            /// Having the entire collection of numbers in hashing table will cause negative performance 
            /// effect on all other methods developed in the module.
            for(int i = index-n; i <= index-2; i++)
                // loop to find the first in the preamble pair.
            {
                for(int j = i+1; j <= index-1; j++)
                    // loop to find the second in the preamble pair.
                {
                    if(numbers[i]+numbers[j] == numbers[index]) return (i, j);
                        // when the preamble pair is found return their indices as tuple.
                }
            }
            return (0, 0); // if non found, return zero indices for both.
        }

        /// <summary>
        /// Find the first number in the group of numbers that can't find a preamble pair.
        /// If all has at least one preamble pair, then return -1.
        /// </summary>
        /// <returns>returns the number that has been flagged as not 
        /// containing a preamble pair</returns>
        public long FindFirstError()
        {
            int a, b; // temporary variables for preamble pair found.
            for(int i = n; i < numbers.Count; i++)
                // iterate through n - end of list unless a number is found that 
                // does contain a preamble pair.
            {
                (a, b) = FindPreamblePair(i); // Find a preamble pair for a number at index, i.
                if (a + b == 0) return numbers[i]; // if not found, flagged the number and return it.
            }
            return -1; // If no error is found, then return -1
        }

        /// <summary>
        /// This method finds a weakness in the encoding by finding contiguous set of numbers that
        /// can match a number that has been identified as error that doesn't contain a pair of 
        /// preamble pair from the libarary of numbers.
        /// </summary>
        /// <returns>returns the sum of lowest and highest numbers from the contiguous set</returns>
        public long FindWeakness()
            /// Author's Note on TC
            /// The TC of this method is potentailly expensive
            /// as k -> n, O(n^3*n^2) -> O(n^3)
            /// as k -> 1, O(n^2*n^2) -> O(n^2)
        {
            int a, b; // temporary variables to hold the position of preamble pair (indices).
            for (int i = n; i < numbers.Count; i++)
                // loop from nth index to the end of list unless result found earlier.
            {
                (a, b) = FindPreamblePair(i); // Find preamble pair for ith number in the library
                // If preamble pair not found:
                if (a + b == 0)
                {
                    List<long>? result = FindContiguous(i); // Find the contiguous set for the flagged number.
                    if(result != null)
                        // making sure the set is valid.
                    {
                        result.Sort(); // Sort the Contiguous Set (but actually it's implmented as a list).
                        return result.Last()+result.First();
                            // return the sum of smallest and largest member from the Contiguous Set
                    }
                    // else {
                    //          INSERT RETURN HERE (eg. return -2)
                    // }
                }
            }
            return -1; 
                // this return value has two possible scenario (ambiguous)
                // = { No error number found | The found error number doesn't contain contiguous set }
                // If these results need to be separately identified, return some other negative number
                // (eg. -2) at the "INSERT RETURN HERE" labeled line.
        }

        /// <summary>
        /// Find contiguous set for the identified number at position index.
        /// Contiguous set contains neighbouring numbers that can be summed up to
        /// match the identified number.
        /// </summary>
        /// <param name="index">Index position of identified number.</param>
        /// <returns></returns>
        public List<long>? FindContiguous(int index)
        {
            for(int i = 0; i < index; i++)
                // iterate through the library of numbers in its instance.
            {
                /* Temporary variables to hold
                 * sum of temporary set of numbers constructed
                 * second temporary index, j, to help build the set
                 * temporary set (which is actually implmented as list) */
                long sum = 0;
                int j = i + 1;
                var set = new List<long>();

                while (sum <= numbers[index])
                    // the sum of temporary mustn't exceed the identified number
                {
                    set.Add(numbers[j]); // insert the number to the temp set.
                    sum += numbers[j++]; 
                        // temp sum is increased with the currently position number,
                        // and the temp index, j is also moved a position afterward.
                    if (sum == numbers[index]) return set;
                        // when the contiguous set is compiled return it.
                }
            }
            return null; // when no contiguous set is found, then return null.
        }
    }
}