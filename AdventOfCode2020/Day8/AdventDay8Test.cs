using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2020
{
    public class Day8Tests
    {
        string[]? data = null;

        [Test]
        public void TestExample()
        {
            var filename = Path.Combine(System.Environment.CurrentDirectory, "Day8Sample.txt");
            this.data = File.ReadAllLines(filename);
            var test = new AdventDay8(this.data);
            Assert.That(test.FindSolution(), Is.EqualTo(5));
        }

        [Test]
        public void TestExample2()
        {
            var filename = Path.Combine(System.Environment.CurrentDirectory, "Day8Sample.txt");
            this.data = File.ReadAllLines(filename);
            var test = new AdventDay8(this.data);
            Assert.That(test.FindSolution2(), Is.EqualTo(8));
        }

        [Test]
        public void TestSolution()
        {
            var filename = Path.Combine(System.Environment.CurrentDirectory, "Day8Data.txt");
            this.data = File.ReadAllLines(filename);
            var test = new AdventDay8(this.data);
            Console.WriteLine(test.FindSolution());
        }

        [Test]
        public void TestSolution2()
        {
            var filename = Path.Combine(System.Environment.CurrentDirectory, "Day8Data.txt");
            this.data = File.ReadAllLines(filename);
            var test = new AdventDay8(this.data);
            Console.WriteLine(test.FindSolution2());
        }
    }
}
