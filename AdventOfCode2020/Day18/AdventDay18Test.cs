using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AdventOfCode2020.Day18
{
    internal class AdventDay18Test
    {
        /// <summary>
        /// Part 1 Unit Testings.
        /// </summary>
        [Test]
        public void FirstUnitTest()
        {
            string expr = "2 * 3 + (4 * 5)";
            string expr2 = "5 + (8 * 3 + 9 + 3 * 4 * 3)";

            Expression test = new Expression(new Part1Evaluate());
            test.Parse(expr);
            Assert.AreEqual(26, test.Evaluate().Value);
            test.Parse(expr2);
            Assert.AreEqual(437, test.Evaluate().Value);
        }
        
        [Test]
        public void Part1Solution()
        {
            Day18Part1Solution solution = new Day18Part1Solution();
            Console.WriteLine($"Answer is {solution.Compute("AdventDay18Data.txt")}");
                                // Answer is 2743012121210

        }

        /// <summary>
        /// Part 2 Unit Testings.
        /// </summary>
        [Test]
        public void SecondUnitTest()
        {
            string expr = "1 + 2 * 3 + 4 * 5 + 6";
            string expr2 = "1 + (2 * 3) + (4 * (5 + 6))";
            string expr3 = "2 * 3 + (4 * 5)";
            string expr4 = "5 + (8 * 3 + 9 + 3 * 4 * 3)";
            string expr5 = "5 * 9 * (7 * 3 * 3 + 9 * 3 + (8 + 6 * 4))";
            string expr6 = "((2 + 4 * 9) * (6 + 9 * 8 + 6) + 6) + 2 + 4 * 2";

            Expression test = new Expression(new Part2Evaluate());

            test.Parse(expr);
            Assert.AreEqual(231, test.Evaluate().Value);

            test.Parse(expr2);
            Assert.AreEqual(51, test.Evaluate().Value);

            test.Parse(expr3);
            Assert.AreEqual(46, test.Evaluate().Value);

            test.Parse(expr4);
            Assert.AreEqual(1445, test.Evaluate().Value);

            test.Parse(expr5);
            Assert.AreEqual(669060, test.Evaluate().Value);

            test.Parse(expr6);
            Assert.AreEqual(23340, test.Evaluate().Value);
        }

        [Test]
        public void Part2Solution()
        {
            Day18Part2Solution solution = new Day18Part2Solution();
            Console.WriteLine($"Answer is {solution.Compute("AdventDay18Data.txt")}");
                                // Answer is 65658760783597

        }
    }
}
