using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2020
{
    public class Day9Tests
    {
        string[]? data = null;

        [Test]
        public void TestExample()
        {
            var filename = Path.Combine(System.Environment.CurrentDirectory, "Day9Sample.txt");
            this.data = File.ReadAllLines(filename);
            var test = new AdventDay9(this.data);
            Assert.That(test.FindSolution(), Is.EqualTo(127));
        }

        [Test]
        public void TestExample2()
        {
            var filename = Path.Combine(System.Environment.CurrentDirectory, "Day9Sample.txt");
            this.data = File.ReadAllLines(filename);
            var test = new AdventDay9(this.data);
            Assert.That(test.FindSolution2(), Is.EqualTo(62));
        }

        [Test]
        public void TestSolution()
        {
            var filename = Path.Combine(System.Environment.CurrentDirectory, "Day9Data.txt");
            this.data = File.ReadAllLines(filename);
            var test = new AdventDay9(this.data);
            Console.WriteLine(test.FindSolution(25));
        }

        [Test]
        public void TestSolution2()
        {
            var filename = Path.Combine(System.Environment.CurrentDirectory, "Day9Data.txt");
            this.data = File.ReadAllLines(filename);
            var test = new AdventDay9(this.data);
            Console.WriteLine(test.FindSolution2(25));
        }
    }
}
