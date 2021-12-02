using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2020
{
    public class AdventDay3Tests
    {
        public class Day3Tests
        {
            string[]? data = null;

            public Day3Tests()
            {
                var filename = Path.Combine(System.Environment.CurrentDirectory, "Day3Data.txt");
                this.data = File.ReadAllLines(filename);
            }

            [SetUp]
            public void Setup()
            {

            }

            [Test]
            public void TestExample()
            {
                var test = new AdventDay3(this.data);
                Assert.That(test.FindSolution(3), Is.EqualTo(7));
            }

            [Test]
            public void TestExample2()
            {
                var test = new AdventDay3(this.data);
                var result = test.FindSolution(1) * test.FindSolution(3) * test.FindSolution(5) * test.FindSolution(7) * test.FindSolution(1, 2);
                Assert.That(result, Is.EqualTo(336));
            }

            [Test]
            public void TestSolution()
            {
                var filename = Path.Combine(System.Environment.CurrentDirectory, "Day3Data2.txt");
                this.data = File.ReadAllLines(filename);
                var test = new AdventDay3(this.data);
                Console.WriteLine(test.FindSolution(3));
            }

            [Test]
            public void TestSolution2()
            {
                var filename = Path.Combine(System.Environment.CurrentDirectory, "Day3Data2.txt");
                this.data = File.ReadAllLines(filename);
                var test = new AdventDay3(this.data);
                List<int> result = new List<int>();
                long total = 1;
                result.Add(test.FindSolution(1));
                result.Add(test.FindSolution(3));
                result.Add(test.FindSolution(5));
                result.Add(test.FindSolution(7));
                result.Add(test.FindSolution(1, 2));
                foreach (int i in result)
                {
                    Console.WriteLine(i);
                    total *= (long)i;
                }
                Console.WriteLine(total);
            }
        }
    }
}
