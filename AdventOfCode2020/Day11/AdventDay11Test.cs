using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2020
{
    public class Day11Tests
    {
        [Test]
        public void TestExample()
        {
            var test = new AdventDay11();
            test.LoadMapFromFile("Day11Sample.txt");
            Assert.That(test.FindSolution(), Is.EqualTo(37));
        }

        [Test]
        public void TestExample2()
        {
            var test = new AdventDay11();
            test.LoadMapFromFile("Day11Sample.txt");
            Assert.That(test.FindSolution2(), Is.EqualTo(26));
        }

        [Test]
        public void TestSolution()
        {
            var test = new AdventDay11();
            test.LoadMapFromFile("Day11Data.txt");
            Console.WriteLine(test.FindSolution());
        }

        [Test]
        public void TestSolution2()
        {
            var test = new AdventDay11();
            test.LoadMapFromFile("Day11Data.txt");
            Console.WriteLine(test.FindSolution2());
        }
    }
}
