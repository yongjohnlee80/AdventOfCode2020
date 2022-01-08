using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2020.Day18
{
    /// <summary>
    /// Interface: EComponent
    /// Expression Component
    /// All Component part must implmemnt Evalute() method that returns Number type
    /// </summary>
    internal interface EComponent
    {
        public Number Evaluate();
    }

    /// <summary>
    /// Type: Number
    /// Int64 wrapper.
    /// Change this to Double wrapper for real number computations.
    /// </summary>
    internal class Number : EComponent
    {
        public Number(string value = "0")
        {
            Value = Convert.ToInt64(value);
        }
        public long Value { get; set; }

        public Number Evaluate()
        {
            return this; // Simply returns self.
        }
    }

    /// <summary>
    /// Type: Operator (Abstract)
    /// Base type for binary operators.
    /// </summary>
    internal abstract class Operator : EComponent
    {
        public EComponent Left { get; set; }
        public EComponent Right { get; set; }
        public abstract Number Evaluate();
    }

    /// <summary>
    /// Type: Add
    /// Operator addition
    /// Evaluates the sum of left and right operands.
    /// </summary>
    internal class Add : Operator
    {
        public override Number Evaluate()
        {
            Number temp = new Number();
            temp.Value = Left.Evaluate().Value + Right.Evaluate().Value;
            return temp;
        }
    }

    /// <summary>
    /// Type: Multiply
    /// Operator multiplicataion
    /// Evaluates the product of left and right operands.
    /// </summary>
    internal class Multiply : Operator
    {
        public override Number Evaluate()
        {
            Number temp = new Number();
            temp.Value = Left.Evaluate().Value * Right.Evaluate().Value;
            return temp;
        }
    }

    /// <summary>
    /// Type: Expression
    /// Complex data structure that can hold multiple expression components.
    /// Any sub-expression, usually in between (parenthesis) are also represented
    /// by this data structure.
    /// </summary>
    internal class Expression : EComponent
    {
        /// <summary>
        /// Components queue.
        /// </summary>
        protected Queue<EComponent> components = new Queue<EComponent>();

        public Queue<EComponent> Components { get { return components; } }

        /// <summary>
        /// Strategy Pattern (composition)
        /// Delegates how an expression is evaluated such as the precedence of operators.
        /// </summary>
        protected ExpressionEvaluateMethod method = null;

        /// <summary>
        /// Generic Regex to tokenize mathematical equation that can separate numeric tokens
        /// (including real values) and some basic operators (*, +, /, -) and parenthesis.
        /// </summary>
        private readonly static string ExpressionRegex = @"([*+/\-)(])|([0-9.]+)";

        /// <summary>
        /// Constructor for the primary expression.
        /// </summary>
        /// <param name="method"></param>
        public Expression(ExpressionEvaluateMethod method = null) 
        {
            if (method == null) this.method = new Part2Evaluate();
            else this.method = method;
        }

        /// <summary>
        /// Constructor for the seconday expressions (sub-expressions)
        /// This constructor receives the components queue from the primary expression.
        /// </summary>
        /// <param name="expressionParts"></param>
        /// <param name="method"></param>
        public Expression(Queue<string> expressionParts, ExpressionEvaluateMethod method = null)
        {
            if (method == null) this.method = new Part2Evaluate();
            else this.method = method;
            Parse(expressionParts);
        }

        /// <summary>
        /// Method: Parse
        /// Tokenize string representation of mathematical expression into a string queue
        /// </summary>
        /// <param name="expression"></param>
        public void Parse(string expression)
        {
            Queue<string> parts = new Queue<string>();
            foreach (var temp in Regex.Matches(expression, ExpressionRegex))
            {
                parts.Enqueue(temp.ToString());
            }
            Parse(parts);
        }

        /// <summary>
        /// Method: Parse
        /// Parses string components into EComponent data structures.
        /// </summary>
        /// <param name="expressionParts"></param>
        public void Parse(Queue<string> expressionParts)
        {
            while(expressionParts.Count > 0)
            {
                var item = expressionParts.Dequeue();
                switch (item)
                {
                    case "+": components.Enqueue(new Add());
                        break;
                    case "*": components.Enqueue(new Multiply());
                        break;
                    case "(": components.Enqueue(new Expression(expressionParts, method));
                        break;
                    case ")": return;
                    default: components.Enqueue(new Number(item));
                        break;
                }
            }
        }

        /// <summary>
        /// Method: Evaluate
        /// Delegation.
        /// </summary>
        /// <returns></returns>
        public Number Evaluate()
        {
            return method.Evaluate(this);
        }
    }

    /// <summary>
    /// Interface: ExpressionEvaluateMethod
    /// </summary>
    internal interface ExpressionEvaluateMethod
    {
        public Number Evaluate(Expression expression);
    }

    /// <summary>
    /// Type: Part1Evaluate
    /// </summary>
    internal class Part1Evaluate : ExpressionEvaluateMethod
    {
        /// <summary>
        /// Method: Evaluate
        /// No operator precedences
        /// Evaluate left to right.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public Number Evaluate(Expression expression)
        {
            var queue = expression.Components;

            EComponent left = null;
            EComponent item = null;

            while (queue.Count > 0)
            {
                item = queue.Dequeue();
                if (item.GetType() == typeof(Number) || item.GetType() == typeof(Expression))
                {
                    left = item;
                }
                else
                {
                    Operator op = (Operator)item;
                    op.Left = left;
                    op.Right = queue.Dequeue();
                    left = op;
                }
            }
            return left.Evaluate();
        }
    }

    /// <summary>
    /// Type: Part2Evaluate
    /// </summary>
    internal class Part2Evaluate : ExpressionEvaluateMethod
    {
        /// <summary>
        /// Method: Evalute
        /// + first then *
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public Number Evaluate(Expression expression)
        {
            var queue = expression.Components;

            EComponent item = null;
            EComponent left = null;
            EComponent root = null;

            while (queue.Count > 0)
            {
                item = queue.Dequeue();

                /// Non operator components
                if (item.GetType() == typeof(Number) || item.GetType() == typeof(Expression))
                {
                    left = item;
                }
                else
                {
                    /// Priority operator
                    /// Place them on the bottom left on the tree.
                    if (item.GetType() == typeof(Add))
                    {
                        Operator op = new Add();
                        op.Left = left;
                        op.Right = queue.Dequeue();
                        left = op;
                        root = op;
                    }
                    /// Low priority operator
                    /// Place them on the top of the tree and move the root.
                    else if (item.GetType() == typeof(Multiply))
                    {
                        Operator op = new Multiply();
                        op.Left = (root == null) ? left : root;
                        op.Right = BuildRightTree();
                        root = op;
                        left = op;
                    }
                }
            }
            return root.Evaluate();

            /// Build the right node tree for low priority operator.
            EComponent BuildRightTree()
            {
                EComponent temp = null;
                EComponent number = null;

                /// Until end of the queue or low operator is peeked.
                while (queue.Count > 0 && queue.Peek().GetType() != typeof(Multiply))
                {
                    /// Build a sub tree for all subsequent high priority operators.
                    temp = queue.Dequeue();
                    if (temp.GetType() != typeof(Add))
                    {
                        number = temp;
                    }
                    else
                    {
                        var op = (Operator)temp;
                        op.Left = number;
                        op.Right = queue.Dequeue();
                        number = op;
                    }
                }
                return number;
            }
        }
    }
    /// <summary>
    /// Type: Day18Part1Solution
    /// Part1 Solution
    /// </summary>
    internal class Day18Part1Solution
    {
        public long Compute(string fileName)
        {
            LogStat log = new LogStat();
            var lines = File.ReadAllLines(fileName);
            long result = 0;
            foreach (var line in lines)
            {
                var solution = new Expression(new Part1Evaluate());
                solution.Parse(line);
                var temp = solution.Evaluate().Value;
                log.Append($"{line} : {temp}\n");
                result += temp;
            }
            log.LogOnFile("Day18_Part1.txt");
            return result;
        }
    }

    /// <summary>
    /// Type: Day18Part2Solution
    /// Part2 Solution
    /// </summary>
    internal class Day18Part2Solution
    {
        public long Compute(string fileName)
        {
            LogStat log = new LogStat();
            var lines = File.ReadAllLines(fileName);
            long result = 0;
            foreach (var line in lines)
            {
                var solution = new Expression(new Part2Evaluate());
                solution.Parse(line);
                var temp = solution.Evaluate().Value;
                log.Append($"{line} : {temp}\n");
                result += temp;
            }
            log.LogOnFile("Day18_Part2.txt");
            return result;
        }
    }
}
