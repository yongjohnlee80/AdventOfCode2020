using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2020
{
    public class Day7Tests
    {
        string[]? data = null;

        [Test]
        public void TestExample()
        {
            var filename = Path.Combine(System.Environment.CurrentDirectory, "Day7Sample.txt");
            this.data = File.ReadAllLines(filename);
            var test = new AdventDay7(this.data);
            Assert.That(test.FindSolution("shiny gold"), Is.EqualTo(4));
        }

        [Test]
        public void TestExample2()
        {
            var filename = Path.Combine(System.Environment.CurrentDirectory, "Day7Sample2.txt");
            this.data = File.ReadAllLines(filename);
            var test = new AdventDay7(this.data);
            Assert.That(test.FindSolution2("shiny gold")-1, Is.EqualTo(126));
        }

        [Test]
        public void TestSolution()
        {
            var filename = Path.Combine(System.Environment.CurrentDirectory, "Day7Data.txt");
            this.data = File.ReadAllLines(filename);
            var test = new AdventDay7(this.data);
            Console.WriteLine(test.FindSolution("shiny gold"));
        }

        [Test]
        public void TestSolution2()
        {
            var filename = Path.Combine(System.Environment.CurrentDirectory, "Day7Data.txt");
            this.data = File.ReadAllLines(filename);
            var test = new AdventDay7(this.data);
            Console.WriteLine(test.FindSolution2("shiny gold")-1);
        }
    }
}
