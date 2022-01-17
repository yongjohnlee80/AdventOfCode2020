using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace AdventOfCode2020.Day19
{
    internal class MonsterMessage
    {
        private Dictionary<string, string> rules = new Dictionary<string, string>();
        private List<string> messages = new List<string>();

        private void Parse(string msg)
        {
            if(msg.Contains(":"))
            {
                string[] parts = msg.Split(':');
                rules[parts[0]] = parts[1];
            }
            else
            {
                messages.Add(msg);
            }
        }

        public void LogOnFile()
        {
            LogStat log = new LogStat();

            foreach(var item in rules)
            {
                log.Append($"Rules # {item.Key} : {item.Value}\n");
            }

            string stringPattern = BuildPattern();
            log.Append(stringPattern).Append("\n");

            Regex msgPattern = new Regex(stringPattern);
            foreach(var item in messages)
            {
                log.Append(item).Append($" : {ValidateMessage(item, msgPattern)}\n");
            }

            log.LogOnFile("Day19Log.txt");
        }

        public void Load(string fileName)
        {
            var lines = File.ReadAllLines(fileName);
            Load(lines);
        }

        public void Load(string[] textLines)
        {
            foreach(var line in textLines)
            {
                Parse(line);
            }
        }

        private StringBuilder ExtractRule(string index = "0")
        {
            StringBuilder result = new StringBuilder();

            if(rules[index].Contains("a") || rules[index].Contains("b"))
            {
                result.Append(rules[index].Replace("\"", "").Trim());
            }
            else
            {
                string[] parts = rules[index].Split(' ',StringSplitOptions.RemoveEmptyEntries);
                foreach(var part in parts)
                {
                    if (part.Contains("|")) result.Append(part);
                    else result.Append("(").Append(ExtractRule(part)).Append(")");
                }
            }
            return result;
        }

        public string BuildPattern()
        {
            StringBuilder pattern = new StringBuilder();
            pattern.Append("^").Append(ExtractRule()).Append("$");
            return pattern.ToString();
        }

        private bool ValidateMessage(string msg, Regex regPattern)
        {
            return regPattern.IsMatch(msg);
        }
    }
}
