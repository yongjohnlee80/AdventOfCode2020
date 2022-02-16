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
        private int maxValue = 0;

        private Dictionary<int, CupNode> nodeTable = new Dictionary<int, CupNode>();


        private void Insert(int value)
        {
            if(value > maxValue) maxValue = value;

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

            nodeTable.Add(value, current);
        }

        /// <summary>
        /// Link two CupNodes and return the latter one.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private CupNode LinkCups(CupNode a, CupNode b)
        {
            a.Next = b;
            return b;
        }

        /// <summary>
        /// returns the head of three cup nodes
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
        /// </summary>
        /// <param name="threeCups"></param>
        /// <param name="destination"></param>
        private void InsertThreeCups(CupNode threeCups, CupNode destination)
        {
            var temp = destination.Next;
            LinkCups(destination, threeCups);
            LinkCups(threeCups.Next.Next, temp);
        }

        private int IdentifyDestinationValue()
        {
            var cups = ToArray();
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
                        dest = cups.Max();
                        return dest;
                    }
                }
            }
        }

        private int IdentifyDestinationValue2(CupNode threeCups)
        {
            int[] threes = new int[3] { 
                    threeCups.Value,
                    threeCups.Next.Value,
                    threeCups.Next.Next.Value };

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
            // First Step
            var threeCups = RemoveThreeCups();

            // Second Step
            var destValue = IdentifyDestinationValue2(threeCups);
            var destination = FindNode(destValue);

            // Third Step
            InsertThreeCups(threeCups, destination);

            // Fourth Step
            current = current.Next;
        }

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
