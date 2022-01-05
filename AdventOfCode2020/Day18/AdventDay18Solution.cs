using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2020.Day18
{
    internal interface EComponent {}

    internal class Number : EComponent
    {
        public Number(string value)
        {
            Value = Convert.ToInt64(value);
        }
        public long Value { get; set; }
    }

    internal class Braket : EComponent
    {
        public bool Open { get; set; }

        public Braket(bool isOpen = true)
        {
            Open = isOpen;
        }
    }

    internal abstract class Operator : EComponent
    {
        public abstract long Compute(Number left, Number right);
    }

    internal class Add : Operator
    {
        public override long Compute(Number left, Number right)
        {
            return left.Value + right.Value;
        }
    }

    internal class Multiply : Operator
    {
        public override long Compute(Number left, Number right)
        {
            return left.Value * right.Value;
        }
    }

    internal class Expression
    {
        Queue<EComponent> exprQueue = new Queue<EComponent> ();

        private readonly static string ExpressionRegex = @"([*+/\-)(])|([0-9.]+)";

        protected EComponent Convert(string partOfExpression)
        {
            switch (partOfExpression)
            {
                case "+": return new Add();
                case "*": return new Multiply();
                case "(": return new Braket(isOpen: true);
                case ")": return new Braket(isOpen: false);
                default: return new Number(partOfExpression);
            }
        }


        public void Parse(string expression)
        {
            exprQueue.Clear();

            foreach (var temp in Regex.Matches(expression, ExpressionRegex))
            {
                exprQueue.Enqueue(Convert(temp.ToString()));
            }
        }

        public Number Evaluate()
        {
            var item = exprQueue.Dequeue();
            Number left;
            if (item.GetType() == typeof(Braket))
            {
                left = Evaluate();
            }
            else
            {
                left = (Number)item;

            }
            while (exprQueue.Count > 0)
            {
                item = exprQueue.Dequeue();
                if (item.GetType() == typeof(Braket)) return left;
                Operator op = (Operator)item;
                
                item = exprQueue.Dequeue();
                if(item.GetType() == typeof(Braket))
                {
                    Braket braket = (Braket)item;
                    if(braket.Open)
                    {
                        left.Value = op.Compute(left, Evaluate());
                    }
                    else
                    {
                        return left;
                    }
                }
                else
                {
                    Number right = (Number)item;
                    left.Value = op.Compute(left, right);                
                }
            }
            return left;
        }
    }

    internal class Day18Part1Solution
    {
        public long Compute(string fileName)
        {
            LogStat log = new LogStat();
            var lines = File.ReadAllLines(fileName);
            long result = 0;
            foreach (var line in lines)
            {
                var solution = new Expression();
                solution.Parse(line);
                var temp = solution.Evaluate().Value;
                log.Append($"{line} : {temp}\n");
                result += temp;
            }
            log.LogOnFile("Day18_Part1.txt");
            return result;
        }
    }
}
