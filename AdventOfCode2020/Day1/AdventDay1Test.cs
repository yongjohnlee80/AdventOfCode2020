using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2020
{
    public class AdventDay1Tests
    {
        public class Day1Tests
        {
            List<int> data;

            public Day1Tests()
            {
                var filename = Path.Combine(System.Environment.CurrentDirectory, "Day1Data.txt");
                Console.WriteLine(filename);
                using (var reader = new StreamReader(filename))
                {
                    this.data = new List<int>();
                    data.AddRange(reader.ReadToEnd().Split("\n").Select(item => System.Convert.ToInt32(item)));

                }
            }

            [SetUp]
            public void Setup()
            {

            }

            [Test]
            public void TestExample()
            {
                var sample = new List<int>() { 1721, 979, 366, 299, 675, 1456 };
                var test = new AdventDay1(sample);
                Assert.That(test.FindSolution(), Is.EqualTo(514579));
            }

            [Test]
            public void TestExample2()
            {
                var sample = new List<int>() { 1721, 979, 366, 299, 675, 1456 };
                var test = new AdventDay1(sample);
                Assert.That(test.FindSolution2(), Is.EqualTo(241861950));
            }

            [Test]
            public void TestSolution()
            {
                //var filename = Path.Combine(System.Environment.CurrentDirectory, "Day1Data.txt");
                //Console.WriteLine(filename);
                //using (var reader = new StreamReader(filename))
                //{
                //    List<int> data = new List<int>();
                //    data.AddRange(reader.ReadToEnd().Split("\n").Select(item => System.Convert.ToInt32(item)));
                    AdventDay1 test = new AdventDay1(data);
                    Console.WriteLine(test.FindSolution());
                //}
                
            }

            [Test]
            public void TestSolution2()
            {
                //var data = new List<int>();
                //string[] text = File.ReadAllLines("Day1Data.txt");
                //foreach(string line in text)
                //{
                //    data.Add(int.Parse(line));
                //}
                var test = new AdventDay1(this.data);
                Console.WriteLine(test.FindSolution2());
            }
        }
    }
}
//145245270