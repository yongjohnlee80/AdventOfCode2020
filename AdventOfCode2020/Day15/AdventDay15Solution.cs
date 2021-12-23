using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2020.Day15
{
    internal class AdventDay15Solution
    {
    }
    /// <summary>
    /// Type: MemoryGame
    /// Advent of Code Day 15
    /// Memory Game with Elves
    /// In this game, the players take turns saying numbers. They begin by taking turns reading from a list of starting numbers (your puzzle input). 
    /// Then, each turn consists of considering the most recently spoken number:
    /// If that was the first time the number has been spoken, the current player says 0.
    /// Otherwise, the number had been spoken before; the current player announces how many turns apart the number is from when it was previously spoken.
    /// </summary>
    internal class MemoryGame
    {
        /// <summary>
        /// Memory Table
        /// Key: Number Spoken
        /// Value: Turn
        /// </summary>
        protected Dictionary<int, int> numbers = new Dictionary<int,int>();
        protected int nextNumber; // The Next number that will be spoken by the opponent or player.
        public int Turn { get; set; } // Turn

        /// <summary>
        ///  Method: RegisterNum
        ///  This method automatically increments the Turn variable.
        ///  Register a spoken number with the memory table.
        ///  returns the next number that will be spoken.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        protected int RegisterNum(int number)
        {
            if(numbers.ContainsKey(number)) // if the number has been spoken before
            {
                int temp = numbers[number];
                numbers[number] = Turn;
                return Turn++ - temp;
            }
            else // if the number is first time spoken
            {
                numbers.Add(number, Turn++);
                return 0;
            }
        }

        /// <summary>
        /// Constructor
        /// Register the starting numbers with the memory table.
        /// </summary>
        /// <param name="startingNum"></param>
        public MemoryGame(int[] startingNum)
        {
            Turn = 1;
            foreach(var i in startingNum)
            {
                nextNumber = RegisterNum(i);
            }
        }

        /// <summary>
        /// Method: GetNthNumberSpoken
        /// Find the number spoken at a specific turn.
        /// </summary>
        /// <param name="nthTurn"></param>
        /// <returns></returns>
        public int GetNthNumberSpoken(int nthTurn)
        {
            while(Turn < nthTurn)
            {
                nextNumber = RegisterNum(nextNumber);
            }
            return nextNumber;
        }
    }
}
