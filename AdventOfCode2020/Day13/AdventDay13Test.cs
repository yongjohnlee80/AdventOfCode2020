using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2020
{
    public class Day13Tests
    {
        [Test]
        public void TestExample()
        {
            var test = new AdventDay13();
            Assert.That(test.FindSolution(), Is.EqualTo(295));
        }

        [Test]
        public void TestExample2()
        {
            var test = new AdventDay13();
            Assert.That(test.FindSolution2(), Is.EqualTo(1068781));
        }

        [Test]
        public void TestSolution()
        {
            var test = new AdventDay13();
            Console.WriteLine(test.FindSolution("Day13Data.txt"));
        }

        [Test]
        public void TestSolution2()
        {
            var test = new AdventDay13();
            Console.WriteLine(test.FindSolution2("Day13Data.txt"));
        }
    }
}
