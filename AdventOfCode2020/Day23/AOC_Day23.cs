using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AdventOfCode2020.Day23
{
    internal class AOC_Day23
    {
        [Test]
        public void TestPart1Sample()
        {
            LinkedCups test = new LinkedCups();
            test.InitCups("389125467");

            //var answer = test.Part1Solution(10);
            //Assert.That(answer, Is.EqualTo("92658374"));

            var answer = test.Part1Solution(100);
            Assert.That(answer, Is.EqualTo("67384529"));
        }

        [Test]
        public void TestPart1Solution()
        {
            LinkedCups part1 = new LinkedCups();
            part1.InitCups("496138527");

            var answer = part1.Part1Solution();

            Assert.That(answer, Is.EqualTo("69425837"));
        }

        [Test]
        public void TestPart2Sample()
        {
            LinkedCups test = new LinkedCups();
            test.InitCups("389125467");

            var answer = test.Part2Solution();
            Assert.That(answer, Is.EqualTo(149245887792));
        }

        [Test]
        public void TestPart2Solution()
        {
            LinkedCups part2 = new LinkedCups();
            part2.InitCups("496138527");

            Console.WriteLine(part2.Part2Solution());

        }
    }

    internal class CupNode
    {
        private readonly int value;

        public int Value { get { return value; } }
        
        public CupNode Next { get; set; }

        public CupNode(int value)
        {
            this.value = value;
        }
    }

    /// <summary>
    /// Cyclic Linked List
    /// </summary>
    internal sealed class LinkedCups
    {
        private CupNode head = null;
        private CupNode current = null;

        // Part 2 Optimization Add-On
        private int maxValue = 0;
        private Dictionary<int, CupNode> nodeTable = new Dictionary<int, CupNode>();

        /// <summary>
        /// O(1)
        /// </summary>
        /// <param name="value"></param>
        private void Insert(int value)
        {
            if(value > maxValue) maxValue = value; // Max Value for wrapping.

            if(head == null)
            {
                head = new CupNode(value);
                current = head;
            }
            else
            {
                var temp = new CupNode(value);
                LinkCups(current, temp);
                current = temp;
            }

            /// Hashing for indexing, improving from O(n) to O(1).
            nodeTable.Add(value, current);
        }

        /// <summary>
        /// Link two CupNodes and return the latter one.
        /// Gives flexibility to switch to doubly linked list.
        /// </summary>
        /// <param name="a">Preceding Cup</param>
        /// <param name="b">Following Cup</param>
        /// <returns></returns>
        private CupNode LinkCups(CupNode a, CupNode b)
        {
            a.Next = b;
            return b;
        }

        /// <summary>
        /// returns the head of three cup nodes, which is disconnected from the
        /// cyclic linked list. O(1)
        /// </summary>
        /// <returns></returns>
        private CupNode RemoveThreeCups()
        {
            var temp = current.Next;
            LinkCups(current, temp.Next.Next.Next);
            return temp;
        }

        /// <summary>
        /// Insert back the three cups right after the destination node.
        /// O(1)
        /// </summary>
        /// <param name="threeCups"></param>
        /// <param name="destination"></param>
        private void InsertThreeCups(CupNode threeCups, CupNode destination)
        {
            var temp = destination.Next;
            LinkCups(destination, threeCups); // Insert after destination
            LinkCups(threeCups.Next.Next, temp);
        }

        /// <summary>
        /// Following the language of game description.
        /// (inefficient)
        /// </summary>
        /// <returns></returns>
        private int IdentifyDestinationValue()
        {
            var cups = ToArray(); // Bottle Neck.
            var dest = current.Value - 1;
            while(true)
            {
                if(cups.Contains(dest))
                {
                    return dest;
                }
                else
                {
                    --dest;
                    if(dest < 1) // 1 is the lowest input given by the puzzle.
                    {
                        dest = cups.Max(); // Potential Bottle Neck. Superceded by keeping a max member value.
                        return dest;
                    }
                }
            }
        }

        /// <summary>
        /// Version 2
        /// </summary>
        /// <param name="threeCups"></param>
        /// <returns></returns>
        private int IdentifyDestinationValue2(CupNode threeCups)
        {
            /// Instead of looking at the rest of cups, focus on the three cups 
            /// the crab has taken to move.
            int[] threes = new int[3] { 
                    threeCups.Value,
                    threeCups.Next.Value,
                    threeCups.Next.Next.Value };

            /// Initial destination value.
            int dest = current.Value - 1;

            while(true)
            {
                if (dest < 1) dest = this.maxValue;
                if(threes.Contains(dest))
                {
                    --dest;
                }
                else
                {
                    return dest;
                }
            }
        }

        /// <summary>
        /// Improved version with dynamic caching (hashmap).
        /// O(n) to O(1)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public CupNode FindNode(int value)
        {
            //var temp = current;
            //do
            //{
            //    if (temp.Value == value)
            //    {
            //        return temp;
            //    }
            //    else
            //    {
            //        temp = temp.Next;
            //    }
            //} while (temp != current);

            //return null;

            if(nodeTable.ContainsKey(value))
            {
                return nodeTable[value];
            }
            return null;
        }

        public void MakeCrabMove()
        {
            // First Step - Take 3 Cups
            var threeCups = RemoveThreeCups();

            // Second Step - Find Destination Cup
            var destValue = IdentifyDestinationValue2(threeCups);
            var destination = FindNode(destValue);

            // Third Step - Insert 3 Cups at the destination.
            InsertThreeCups(threeCups, destination);

            // Fourth Step - Move the current position.
            current = current.Next;
        }

        /// <summary>
        /// Utility for character conversion.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private int[] StringToIntArray(string data)
        {
            List<int> cups = new List<int>();

            foreach (char c in data.ToCharArray())
            {
                cups.Add(int.Parse(c.ToString()));
            }

            return cups.ToArray();
        }

        /// <summary>
        /// Parser
        /// </summary>
        /// <param name="data"></param>
        public void InitCups(string data)
        {
            foreach(var i in StringToIntArray(data))
            {
                Insert(i);
            }
            LinkCups(current, head);
        }

        public int[] ToArray()
        {
            List<int> cups = new List<int>();

            CupNode temp = current;
            do
            {
                cups.Add(temp.Value);
                temp = temp.Next;
            } while (temp != current);

            return cups.ToArray();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach(var i in ToArray())
            {
                sb.Append(i.ToString());
            }
            return sb.ToString();
        }

        public string Part1Solution(int nMoves = 100)
        {
            current = head;

            for (int i = 0; i < nMoves; i++)
            {
                MakeCrabMove();
            }
            current = FindNode(1);
            return ToString().Remove(0, 1);
        }

        public long Part2Solution()
        {
            // Insert the rest of the cups in the circle.
            for(int i = 10; i <= 1_000_000; i++)
            {
                Insert(i);
            }
            LinkCups(current, head);
            current = head;


            // Make ten million crab moves
            for (int i = 0; i < 10_000_000; i++)
            {
                MakeCrabMove();
            }

            current = FindNode(1);
            long firstCup = current.Next.Value;
            long secondCup = current.Next.Next.Value;

            long answer = firstCup * secondCup;
            return answer;
        }
    }
}
