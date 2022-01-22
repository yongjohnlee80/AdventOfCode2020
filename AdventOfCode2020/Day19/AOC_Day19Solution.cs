using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace AdventOfCode2020.Day19
{
    /// <summary>
    /// Type MonsterMessage: this class parses input data into two segments, namely the deciphering rules
    /// and the actual messages received. 
    /// This class also provides basic methods in constructing the string pattern for allowed message format to
    /// verify whether the messages recieved are corruped or not (part 1 requirement). 
    /// </summary>
    internal class MonsterMessage
    {
        /// <summary>
        /// Attributes
        /// 
        /// </summary>
        private Dictionary<string, string> rules = new Dictionary<string, string>();
        private List<string> messages = new List<string>();

        /// <summary>
        /// Method Parse(): this method separates message rule and the message then stores them
        /// in the appropriate data structures.
        /// </summary>
        /// <param name="msg"></param>
        private void Parse(string msg)
        {
            /// Message Rule
            if(msg.Contains(":"))
            {
                string[] parts = msg.Split(':');
                rules[parts[0]] = parts[1];
            }
            /// Actual Message
            else
            {
                messages.Add(msg);
            }
        }

        /// <summary>
        /// Method: ExtractRule(): This method extracts the rule (zero by default) and constructs the string 
        /// pattern for allowed message format.
        /// </summary>
        /// <param name="index"></param>
        /// <returns>Regex string pattern</returns>
        private StringBuilder ExtractRule(string index = "0")
        {
            StringBuilder result = new StringBuilder();

            /// Base case, rule represents either "a" or "b".
            if (rules[index].Contains("a") || rules[index].Contains("b"))
            {
                result.Append(rules[index].Replace("\"", "").Trim());
            }
            /// All other cases.
            else
            {
                string[] parts = rules[index].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                foreach (var part in parts)
                {
                    if (part.Contains("|") | part.Contains("(") | part.Contains(")"))
                    {
                        // if "|" or parenthesis, simply add it to the pattern without extraction.
                        result.Append(part);
                    }
                    else
                    {
                        // all other rules, extract them recursively.
                        result.Append("(").Append(ExtractRule(part)).Append(")");
                    }
                }
            }
            return result;
        }


        /// <summary>
        /// Method: ValidateMessage() validates actual messages according to the regex pattern provided.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="regPattern"></param>
        /// <returns></returns>
        private bool ValidateMessage(string msg, Regex regPattern)
        {
            return regPattern.IsMatch(msg);
        }

        /// <summary>
        /// Method LogOnFile(): For debugging purposes. Logs the status of the related information in
        /// neatly organized manner for easier debugging.
        /// This method was primarily used for the first example of the problem to see whether the algorithm
        /// correctly identified and verified the expected messages.
        /// </summary>
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

        /// <summary>
        /// Method Load(): Loads data input either from string array or a file.
        /// </summary>
        /// <param name="fileName"></param>
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

        /// <summary>
        /// Method: BuildPattern() creates regular expression pattern with the rules provided.
        /// </summary>
        /// <returns></returns>
        public string BuildPattern()
        {
            StringBuilder pattern = new StringBuilder();
            pattern.Append("^").Append(ExtractRule()).Append("$");
            return pattern.ToString();
        }


        /// <summary>
        /// Method: Part1Answer() finds how many answers are valid according to the rule zero read
        /// from the input file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public int Part1Answer(string fileName)
        {
            Load(fileName);

            Regex msgPattern = new Regex(BuildPattern());
            int answer = 0;
            foreach(var item in messages)
            {
                if(ValidateMessage(item, msgPattern)) answer++;
            }
            return answer;
        }

        /// <summary>
        /// Method: Part2Answer() finds how many answers are valid after modifications to the rule 8 and 11 as such:
        /// rule 8: 42 | 42 8
        /// rule 11: 42 31 | 42 11 31
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public int Part2Answer(string fileName)
        {
            Load(fileName);

            /// Create Base cases for rule 8 and 11 for multi depth for recursive patterns.
            /// rule "X" is simply the extracted rule "8" before the modification while
            /// rule "Y" is the extracted rule "11" before the modification.
            rules["X"] = ExtractRule("8").ToString();
            rules["Y"] = ExtractRule("11").ToString();

            // depth 0 : Nothing special is required - simple extraction of rule zero.
            List<Regex> msgPattern = new List<Regex>();
            msgPattern.Add(new Regex(BuildPattern()));

            // depth 1 : Modification to the rules 8 and 11 to the first depth.
            rules["8"] = "42 | 42 X";
            rules["11"] = "42 31 | 42 Y 31";
            msgPattern.Add(new Regex(BuildPattern()));

            /// depth 2 to 5, 
            /// 5 has been selected since this much depth created more than enough
            /// character counts for the messages to accomdate all input messages.
            /// It was actually overkill.            
            string mdf8 = "( 42 | 42 X )";
            string mdf11 = "( 42 31 | 42 Y 31 )";
            for (int i = 2; i <= 5; i++)
            {
                // Replace the rule X and Y with higher depth recursive patterns.
                rules["8"] = rules["8"].Replace("X", mdf8);
                rules["11"] = rules["11"].Replace("Y", mdf11);
                msgPattern.Add(new Regex(BuildPattern()));
            }

            /// Verification Process.
            /// Nested Loop seems deceptive but the number of patterns are only 5 (depth)
            /// making the time complexity (worst case) to O(5n) which is O(n).
            int answer = 0;
            foreach(var item in messages)
            {
                foreach(var pattern in msgPattern)
                {
                    if(ValidateMessage(item, pattern))
                    {
                        answer++;
                        break;
                    }
                }
            }
            return answer;
        }
    }
}
