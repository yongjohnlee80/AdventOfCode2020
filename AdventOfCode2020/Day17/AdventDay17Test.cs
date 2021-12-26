using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AdventOfCode2020.Day17
{
    internal class AdventDay17Test
    {
        [Test]
        public void TestSample()
        {
            string[] data = @"
.#.
..#
###".Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            PocketUniverse alpha = new PocketUniverse();
            alpha.InitCubes(data);

            int result = 0;
            for(var i = 1; i <= 6; i++)
            {
                result = alpha.RunCycle();
                alpha.LogPocketMap($"Cycle {i}");
            }
            alpha.WriteLog("Log_Day17.txt");

            Assert.AreEqual(112, result);
        }

        [Test]
        public void TestPart1()
        {
            string[] data = @"
#.#.##.#
#.####.#
...##...
#####.##
#....###
##..##..
#..####.
#...#.#.".Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            PocketUniverse beta = new PocketUniverse();
            beta.InitCubes(data);

            int result = 0;
            for (var i = 1; i <= 6; i++) result = beta.RunCycle();

            Console.WriteLine(result);
        }

        [Test]
        public void TestSample2()
        {
            string[] data = @"
.#.
..#
###".Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            PocketUniverse alpha = new PocketUniverse(useHyperSpace: true);
            alpha.InitCubes(data);

            int result = 0;
            for (var i = 1; i <= 6; i++)
            {
                result = alpha.RunCycle();
                alpha.LogPocketMap($"Cycle {i}");
            }
            alpha.WriteLog("Log_Day17.txt");

            Assert.AreEqual(848, result);
        }

        [Test]
        public void TestPart2()
        {
            string[] data = @"
#.#.##.#
#.####.#
...##...
#####.##
#....###
##..##..
#..####.
#...#.#.".Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            PocketUniverse beta = new PocketUniverse(useHyperSpace: true);
            beta.InitCubes(data);

            int result = 0;
            for (var i = 1; i <= 6; i++) result = beta.RunCycle();

            Console.WriteLine(result);
        }
    }
}
