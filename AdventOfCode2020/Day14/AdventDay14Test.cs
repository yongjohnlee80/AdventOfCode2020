using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2020.Day14
{
    internal class AdventDay14Test
    {
        [Test]
        public void TestSample()
        {
            string[] sample = @"
mask = XXXXXXXXXXXXXXXXXXXXXXXXXXXXX1XXXX0X
mem[8] = 11
mem[7] = 101
mem[8] = 0".Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            var test = new AdventDay14Solution(sample);
            Assert.That(test.GetSolution1(), Is.EqualTo(165));
        }

        [Test]
        public void TestSolution1()
        {
            var test = new AdventDay14Solution("Day14Data.txt");
            Console.WriteLine(test.GetSolution1());
        }
    }
}
