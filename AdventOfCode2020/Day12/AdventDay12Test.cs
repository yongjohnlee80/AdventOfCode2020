using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2020
{
    public class Day12Tests
    {
        [Test]
        public void TestExample()
        {
            var test = new AdventDay12();
            test.LoadCommandsFromTextLines(new string[] { "F10", "N3", "F7", "R90", "F11" });
            Assert.That(test.FindSolution(), Is.EqualTo(25));
        }

        [Test]
        public void TestExample2()
        {
            var test = new AdventDay12();
            test.LoadCommandsFromTextLines(new string[] { "F10", "N3", "F7", "R90", "F11" });
            Assert.That(test.FindSolution2(), Is.EqualTo(286));
        }

        [Test]
        public void TestSolution()
        {
            var test = new AdventDay12();
            test.LoadCommandsFromFile("Day12Data.txt");
            Console.WriteLine(test.FindSolution());
        }

        [Test]
        public void TestSolution2()
        {
            var test = new AdventDay12();
            test.LoadCommandsFromFile("Day12Data.txt");
            Console.WriteLine(test.FindSolution2());
        }
    }
}
