using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2020
{
    public class AdventDay2Tests
    {
        public class Day2Tests
        {
            string[]? data = null;

            public Day2Tests()
            {
                var filename = Path.Combine(System.Environment.CurrentDirectory, "Day2Data.txt");
                this.data = File.ReadAllLines(filename);
            }

            [SetUp]
            public void Setup()
            {

            }

            [Test]
            public void TestExample()
            {
                string[] sample = new string[] { "1-3 a: abcde", "1-3 b: cdefg", "2-9 c: ccccccccc" };
                var test = new AdventDay2(sample);
                Assert.That(test.FindSolution(), Is.EqualTo(2));
            }

            [Test]
            public void TestExample2()
            {
                string[] sample = new string[] { "1-3 a: abcde", "1-3 b: cdefg", "2-9 c: ccccccccc" };
                var test = new AdventDay2(sample);
                Assert.That(test.FindSolution2(), Is.EqualTo(1));
            }

            [Test]
            public void TestSolution()
            {
                var test = new AdventDay2(this.data);
                Console.WriteLine(test.FindSolution());
            }

            [Test]
            public void TestSolution2()
            {
                var test = new AdventDay2(this.data);
                Console.WriteLine(test.FindSolution2());
            }

        }
    }
}
