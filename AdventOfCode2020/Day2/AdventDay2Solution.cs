using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2020
{
    internal class AdventDay2
    {
        // fields
        string[] data;

        // constructor
        public AdventDay2(string[] data)
        {
            this.data = data;
        }

        // methods.
        public int FindSolution()
        {
            int result = 0; // The number of Valid Passwords.

            // loop through password set.
            foreach (string s in data)
            {
                // tokenize a line.
                string[] token = Regex.Split(s, @"\s+");

                // the token[0] contains the policy min max of a specific character in the password.
                string[] numbers = Regex.Split(token[0], @"\D+");
                int min, max;
                int.TryParse(numbers[0], out min);
                int.TryParse(numbers[1], out max);

                // the token[1] contains the specific character, key.
                char[] key = token[1].ToCharArray();

                int f = token[2].Where(x=>x==key[0]).Count(); // Count how many keys are in the string.
                
                // if min <= key occurences in the password <= max, validate the password.
                if (f>=min && f<=max)
                {
                    result++;
                }
            }
            return result;
        }

        public int FindSolution2()
        {
            int result = 0; // The number of valid passwords.

            // Loop through password set.
            foreach (string s in data)
            {
                string[] token = Regex.Split(s, @"\s+"); // tokenize current line.
                string[] numbers = Regex.Split(token[0], @"\D+"); // extract numbers.

                int first, second; // required indices for validation.
                int.TryParse(numbers[0], out first);
                int.TryParse(numbers[1], out second);
                first--; second--; // accomodate zero index.

                char[] key = token[1].ToCharArray(); // extract the required key.

                // Requires exactly one position to contain the key; thus, using XOR logic.
                if( token[2].ElementAt(first) == key[0] ^ token[2].ElementAt(second) == key[0] )
                {
                    result++;
                }
            }
            return result;
        }
    }
}
