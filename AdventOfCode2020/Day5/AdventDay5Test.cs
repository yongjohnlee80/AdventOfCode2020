using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2020
{
    public class Day5Tests
    {
        string[]? data = null;

        public Day5Tests()
        {
            var filename = Path.Combine(System.Environment.CurrentDirectory, "Day5Data.txt");
            this.data = File.ReadAllLines(filename);
        }

        [Test]
        public void TestExample()
        {
            string[] sample = new string[] { "BFFFBBFRRR", "FFFBBBFRRR", "BBFFBBFRLL" };
            var test = new AdventDay5(sample);
            Assert.That(test.FindSolution(), Is.EqualTo(820));
        }

        [Test]
        public void TestSolution()
        {
            var test = new AdventDay5(this.data);
            Console.WriteLine(test.FindSolution());
        }

        [Test]
        public void TestSolution2()
        {
            var test = new AdventDay5(this.data);
            Console.WriteLine(test.FindSolution2());
        }
    }
}
