using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2020
{
    public class Day4Tests
    {
        string[]? data = null;

        [Test]
        public void TestExample()
        {
            var filename = Path.Combine(System.Environment.CurrentDirectory, "Day4Data.txt");
            this.data = File.ReadAllLines(filename);
            var requiredFields = new string[] { "byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid" };
            var test = new AdventDay4(this.data);
            Assert.That(test.FindSolution(requiredFields), Is.EqualTo(2));
        }

        [Test]
        public void TestExample2()
        {
            var filename = Path.Combine(System.Environment.CurrentDirectory, "Day4Data3.txt");
            this.data = File.ReadAllLines(filename);
            var test = new AdventDay4(this.data);
            var requiredFields = new string[] { "byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid" };
            Assert.That(test.FindSolution(requiredFields, true), Is.EqualTo(4));
        }

        [Test]
        public void TestSolution()
        {
            var filename = Path.Combine(System.Environment.CurrentDirectory, "Day4Data2.txt");
            this.data = File.ReadAllLines(filename);
            var requiredFields = new string[] { "byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid" };
            var test = new AdventDay4(this.data);
            Console.WriteLine(test.FindSolution(requiredFields));
        }

        [Test]
        public void TestSolution2()
        {
            var filename = Path.Combine(System.Environment.CurrentDirectory, "Day4Data2.txt");
            this.data = File.ReadAllLines(filename);
            var requiredFields = new string[] { "byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid" };
            var test = new AdventDay4(this.data);
            Console.WriteLine(test.FindSolution(requiredFields, true));
        }
    }
}
