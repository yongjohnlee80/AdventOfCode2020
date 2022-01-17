using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AdventOfCode2020.Day19
{
    internal class AOC_Day19Test
    {
        [Test]
        public void TestParse()
        {
            string[] data = @"
0: 4 1 5
1: 2 3 | 3 2
2: 4 4 | 5 5
3: 4 5 | 5 4
4: a
5: b

ababbb
bababa
abbbab
aaabbb
aaaabbb".Split("\r\n",StringSplitOptions.RemoveEmptyEntries);

            MonsterMessage test = new MonsterMessage();
            test.Load(data);
            test.LogOnFile();
        }
    }
}
