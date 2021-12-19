using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2020.Day14
{

    internal class AdventDay14Solution
    {
        DockingSystem dockSystem = new DockingSystem();

        public AdventDay14Solution(string fileName)
        {
            dockSystem.LoadDockingData(fileName);
        }

        public AdventDay14Solution(string[] lines)
        {
            dockSystem.LoadDockingData(lines);
        }

        public long GetSolution1()
        {
            return dockSystem.SumOfDockingData();
        }
    }

    internal class BitMask
    {
        char[] value = new char[36];
        long ones, zeros;

        public BitMask(string value)
        {
            this.value = value.Reverse().ToArray();

            ones = 0;
            zeros = 0;

            for (int i = 0; i < this.value.Length; i++)
            {
                switch (this.value[i])
                {
                    case '0':
                        zeros |= (1L << i);
                        break;
                    case '1':
                        ones |= (1L << i);
                        break;
                    default:
                        break;
                }
            }
            zeros = -1 ^ zeros;
        }

        public long MaskValue(long value)
        {
            return (value & zeros) | ones;
        }

        public (long, long) GetMaskValues()
        {
            return (ones, zeros);
        }
    }

    public class DockingParam
    {
        //private readonly static Regex memRegex = new Regex(@"([a-z]{3}) ((\+|-)(\d)+)");

        public int ID { get; set; }

        public long Value { get; set; }

        public DockingParam(int ID, long value)
        {
            this.ID = ID;
            this.Value = value;
        }
    }

    internal class DockingSystem
    {
        Dictionary<int, DockingParam> dockingData = new Dictionary<int, DockingParam>();

        public DockingSystem() { }

        protected bool IsMask(string text)
        {
            if (text.Contains("mask")) return true;
            else return false;
        }

        protected void UpdateDockingData(int id, long value)
        {
            if(dockingData.ContainsKey(id))
            {
                dockingData[id].Value = value;
            }
            else
            {
                dockingData.Add(id, new DockingParam(id, value));
            }
        }

        public void LoadDockingData(string fileName)
        {
            var fname = Path.Combine(System.Environment.CurrentDirectory, fileName);
            var lines = File.ReadAllLines(fname);
            LoadDockingData(lines);
        }

        public void LoadDockingData(string[] lines)
        {
            LogStat log = new LogStat();

            BitMask mask = new BitMask("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");

            foreach(var line in lines)
            {
                if(IsMask(line))
                {
                    var temp = line.Replace("mask", "").Replace("=", "").Replace(" ", "");
                    mask = new BitMask(temp);
                    log.Append($"\nMask = {temp}\n");
                    log.Append($"Values = {mask.GetMaskValues()}\n");
                }
                else
                {
                    string[] numbers = Regex.Split(line, @"\D+");
                    int id = int.Parse(numbers[1]);
                    int value = int.Parse(numbers[2]);
                    log.Append($"mem[{id}] = {value} -> {mask.MaskValue(value)}\n");
                    UpdateDockingData(id, mask.MaskValue(value));
                }
            }

            log.LogOnFile();
        }

        public long SumOfDockingData()
        {
            long sum = 0;
            foreach(var data in dockingData.Values)
            {
                sum += data.Value;
            }
            return sum;
        }
    }

}
