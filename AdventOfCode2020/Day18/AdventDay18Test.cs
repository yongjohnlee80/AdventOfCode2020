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
        [Test]
        public void FirstUnitTest()
        {
            string expr = "2 * 3 + (4 * 5)";
            string expr2 = "5 + (8 * 3 + 9 + 3 * 4 * 3)";

            Expression test = new Expression();
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
        }
    }
}
