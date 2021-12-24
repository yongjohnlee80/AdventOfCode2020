using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AdventOfCode2020.Day16
{
    internal class AdventDay16Test
    {
        [Test]
        public void TestSample()
        {
            TicketValidator tester = new TicketValidator();
            tester.LoadingForPart1();

            string[] data = @"
class: 1-3 or 5-7
row: 6-11 or 33-44
seat: 13-40 or 45-50

your ticket:
7,1,14

nearby tickets:
7,3,47
40,4,50
55,2,20
38,6,12".Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            tester.LoadData(data);

            Assert.That(tester.SolutionPart1(), Is.EqualTo(71));
        }

        [Test]
        public void TestDataPart1()
        {
            TicketValidator tester = new TicketValidator();
            tester.LoadingForPart1();

            tester.LoadData("Day16Data.txt");

            Console.WriteLine(tester.SolutionPart1());
        }

        [Test]
        public void TestSample2()
        {
            TicketValidator tester = new TicketValidator();
            tester.LoadingForPart1();

            string[] data = @"
class: 0-1 or 4-19
row: 0-5 or 8-19
seat: 0-13 or 16-19

your ticket:
11,12,13

nearby tickets:
3,9,18
15,1,5
5,14,9".Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            tester.LoadData(data);

            Console.WriteLine(tester.SolutionSamplePart2());
        }


        [Test]
        public void TestDataPart2()
        {
            TicketValidator tester = new TicketValidator();
            tester.LoadingForPart1();

            tester.LoadData("Day16Data.txt");

            Console.WriteLine(tester.SolutionPart2());
        }
    }
}
