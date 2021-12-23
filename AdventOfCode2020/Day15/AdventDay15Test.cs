using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AdventOfCode2020.Day15
{
    internal class AdventDay15Test
    {
        [Test]
        public void TestSample()
        {
            int[] data = { 0, 3, 6 };
            MemoryGame test = new MemoryGame(data);

            Assert.That(test.GetNthNumberSpoken(2020), Is.EqualTo(436));
        }

        [Test]
        public void TestSamples()
        {
            MemoryGame test1 = new MemoryGame(new[] { 1, 3, 2 });
            Assert.That(test1.GetNthNumberSpoken(2020), Is.EqualTo(1));

            MemoryGame test2 = new MemoryGame(new[] { 2, 1, 3 });
            Assert.That(test2.GetNthNumberSpoken(2020), Is.EqualTo(10));

            MemoryGame test3 = new MemoryGame(new[] { 1, 2, 3 });
            Assert.That(test3.GetNthNumberSpoken(2020), Is.EqualTo(27));

            MemoryGame test4 = new MemoryGame(new[] { 2, 3, 1 });
            Assert.That(test4.GetNthNumberSpoken(2020), Is.EqualTo(78));

            MemoryGame test5 = new MemoryGame(new[] { 3, 2, 1 });
            Assert.That(test5.GetNthNumberSpoken(2020), Is.EqualTo(438));

            MemoryGame test6 = new MemoryGame(new[] { 3, 1, 2 });
            Assert.That(test6.GetNthNumberSpoken(2020), Is.EqualTo(1836));
        }
        
        [Test]
        public void TestPart1Data()
        {
            MemoryGame game = new MemoryGame(new[] { 0, 5, 4, 1, 10, 14, 7 });

            Console.WriteLine(game.GetNthNumberSpoken(2020));
        }

        [Test]
        public void TestSamplePart2()
        {
            MemoryGame test1 = new MemoryGame(new[] { 0, 3, 6 });
            Assert.That(test1.GetNthNumberSpoken(30_000_000), Is.EqualTo(175594));
        }

        [Test]
        public void TestPart2Data()
        {
            MemoryGame game2 = new MemoryGame(new[] { 0, 5, 4, 1, 10, 14, 7 });

            Console.WriteLine(game2.GetNthNumberSpoken(30_000_000));
        }
    }
}
