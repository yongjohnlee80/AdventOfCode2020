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

        [Test]
        public void TestSample2()
        {
            string[] sample = @"
mask = 000000000000000000000000000000X1001X
mem[42] = 100
mask = 00000000000000000000000000000000X0XX
mem[26] = 1".Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            var test = new AdventDay14Solution(sample, new DataLoadingPart2());
            Assert.That(test.GetSolution1(), Is.EqualTo(208));
        }

        [Test]
        public void TestSolution2()
        {
            var test = new AdventDay14Solution("Day14Data.txt", new DataLoadingPart2());
            Console.WriteLine(test.GetSolution1());
        }
    }
}
