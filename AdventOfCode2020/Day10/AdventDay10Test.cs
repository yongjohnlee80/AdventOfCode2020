using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2020
{
    public class Day10Tests
    {
        string[]? data = null;

        [Test]
        public void TestExample()
        {
            var filename = Path.Combine(System.Environment.CurrentDirectory, "Day10Sample.txt");
            this.data = File.ReadAllLines(filename);
            var test = new AdventDay10(this.data);
            Assert.That(test.FindSolution(), Is.EqualTo(220));
        }

        [Test]
        public void TestExample2()
        {
            var filename = Path.Combine(System.Environment.CurrentDirectory, "Day10Sample.txt");
            this.data = File.ReadAllLines(filename);
            var test = new AdventDay10(this.data);
            Assert.That(test.FindSolution2(), Is.EqualTo(19208));
        }

        [Test]
        public void TestSolution()
        {
            var filename = Path.Combine(System.Environment.CurrentDirectory, "Day10Data.txt");
            this.data = File.ReadAllLines(filename);
            var test = new AdventDay10(this.data);
            Console.WriteLine(test.FindSolution());
        }

        [Test]
        public void TestSolution2()
        {
            var filename = Path.Combine(System.Environment.CurrentDirectory, "Day10Data.txt");
            this.data = File.ReadAllLines(filename);
            var test = new AdventDay10(this.data);
            Console.WriteLine(test.FindSolution2());
        }
    }
}
