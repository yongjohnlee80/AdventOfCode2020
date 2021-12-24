using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2020.Day16
{
    internal class AdventDay16Solution
    {
    }

    /// <summary>
    /// Type: ConstraintType
    /// Just in case for the second half of the solution.
    /// wrapping string describing the constraint type as a class.
    /// may need to add an interface depending on which pattern to employ later on.
    /// </summary>
    internal class ConstraintType
    {
        public string Name { get; set; }

        public ConstraintType(string name)
        {
            Name = name;
        }
    }

    /// <summary>
    /// Type: Range
    /// Simple type to store ranges and check a value if it is in the range or not.
    /// Extention provided for list underneath.
    /// </summary>
    internal class Range
    {
        public int Start { get; set; }
        public int End { get; set; }

        public Range(int start, int end)
        {
            Start = start;
            End = end;
        }

        public bool inRange(int value)
        {
            return (value >= Start && value <= End);
        }
    }

    internal static class RangeExtension
    {
        public static bool inRange(this List<Range> input, int value)
        {
            foreach(var range in input)
            {
                if(range.inRange(value)) return true;
            }
            return false;
        }
    }
    /// <summary>
    /// Interface: LoadingConstraint
    /// Still waiting to see whether constraint type will matter in the second half
    /// of the puzzle or not....
    /// </summary>
    interface LoadingConstraint
    {
        public Constraint Load(string line);
    }

    class ConstraintFromString : LoadingConstraint
    {
        private readonly static Regex ConstraintRegex = new Regex(@"([a-z ]+): (\d+-\d+) or (\d+-\d+)");

        public Constraint Load(string line)
        {
            Constraint constraint;
            var match = ConstraintRegex.Match(line);
            if (!match.Success) throw new Exception($"Not understanding what you're trying to say...{line}");
            constraint = new Constraint(match.Groups[1].Value);
            constraint.AddRange(match.Groups[2].Value).AddRange(match.Groups[3].Value);
            return constraint;
        }
    }

    /// <summary>
    /// Type: Constraint
    /// One of the main data structure which will be used as interactive interface with
    /// other main data type such as tickets and main program class.
    /// Extension also provided for list of constraint
    /// </summary>
    internal class Constraint : IComparable<Constraint>
    {
        protected ConstraintType cType;
        protected List<Range> ranges = new List<Range>();
        protected int FieldPosition = 0;
        public List<int> PossiblePositions = new List<int>();

        public Constraint(string type) 
        {
            cType = new ConstraintType(type);
        }

        public bool isType(string substring)
        {
            return cType.Name.Contains(substring);
        }

        public string Name
        {
            get { return cType.Name; }
        }

        public Constraint AddRange(int start, int end)
        {
            ranges.Add(new Range(start, end));
            return this;
        }

        public Constraint AddRange(string range)
        {
            string[] values = range.Split('-');
            ranges.Add(new Range(Int32.Parse(values[0]), Int32.Parse(values[1])));
            return this;
        }

        public int Position
        {
            get { return FieldPosition; }
            set { FieldPosition = value; }
        }

        public bool Check(int value)
        {
            return ranges.inRange(value);
        }

        public int CompareTo(Constraint? other)
        {
            return this.FieldPosition.CompareTo(other.FieldPosition);
        }
    }

    internal static class ConstraintExtension
    {
        internal static int[] GetErrorValuesFromTicket(this List<Constraint> input, Ticket ticket)
        {
            var result = new List<int>();
            bool pass = false;
            foreach(var value in ticket.Numbers)
            {
                pass = false;
                foreach(Constraint constraint in input)
                {
                    if (constraint.Check(value)) pass = true;
                }
                if(!pass) result.Add(value);
            }
            return result.ToArray();
        }

        internal static bool CheckTicket(this List<Constraint> input, Ticket ticket)
        {
            bool pass = false;
            foreach(var value in ticket.Numbers) {
                pass = false;
                foreach(Constraint constraint in input)
                {
                    if (constraint.Check(value)) pass = true;
                }
                if(!pass) return false;
            }
            return true;
        }
    }

    /// <summary>
    /// Type
    /// </summary>
    internal class Ticket
    {
        protected int[] numbers;

        public int[] Numbers { get { return numbers; } }

        public Ticket(int[] numbers)
        {
            this.numbers = numbers;
        }

        public Ticket(string numbers) // numbers are seperated by ','
        {
            string[] values = numbers.Split(',');
            List<int> temp = new List<int>();
            foreach(var value in values)
            {
                temp.Add(Int32.Parse(value));
            }
            this.numbers = temp.ToArray();
        }
    }

    /// <summary>
    /// Type: ErrorValues
    /// Collection of Bad Ticket Numbers
    /// Provode Checksum of errors, which supposedly is the ticket scanning error rate
    /// </summary>
    internal class ErrorValues
    {
        protected List<int> errors = new List<int>();

        public void Append(int error)
        {
            errors.Add(error);
        }

        public int CheckSum()
        {
            int sum = 0;
            foreach(var i in errors)
            {
                sum += i;
            }
            return sum;
        }
    }

    /// <summary>
    /// Type: TicketValidator
    /// 
    /// </summary>
    internal class TicketValidator
    {
        protected ErrorValues errors;
        protected List<Constraint> constraints;
        protected List<Ticket> tickets;

        protected LoadingConstraint loadingConstraint;

        public TicketValidator() 
        {
            errors = new ErrorValues();
            constraints = new List<Constraint>();
            tickets = new List<Ticket>();
        }

        public void LoadingForPart1()
        {
            loadingConstraint = new ConstraintFromString();
        }

        public void LoadData(string fileName) 
        {
            var fname = Path.Combine(System.Environment.CurrentDirectory, fileName);
            var lines = File.ReadAllLines(fname);
            LoadData(lines);
        }

        public void LoadData(string[] lines) 
        { 
            if(loadingConstraint == null) LoadingForPart1();
            foreach(var line in lines)
            {
                if(!string.IsNullOrEmpty(line))
                {
                    // Loading Constraints
                    if (line.Contains("or")) constraints.Add(loadingConstraint.Load(line));
                    // Loading Tickets
                    else if (!line.Contains("ticket"))
                    {
                        tickets.Add(new Ticket(line));
                    }
                }
            }
        }

        protected int TicketScanningErrorRate()
        {
            foreach (var ticket in tickets)
            {
                foreach (var i in constraints.GetErrorValuesFromTicket(ticket))
                {
                    errors.Append(i);
                }
            }
            return errors.CheckSum();
        }
        
        public int SolutionPart1()
        {
            return TicketScanningErrorRate();
        }

        protected void RemoveBadTickets()
        {
            List<Ticket> badTickets = new List<Ticket>();
            /// Remove Invalid Tickets
            foreach (var ticket in tickets)
            {
                if (!constraints.CheckTicket(ticket))
                {
                    badTickets.Add(ticket);
                }
            }
            foreach (var ticket in badTickets)
            {
                tickets.Remove(ticket);
            }
        }

        protected void CompilePossibleFieldPositions()
        {
            foreach (var constraint in constraints)
            {
                /// Iterate each Field of all tickets
                for (int i = 0; i < tickets[0].Numbers.Length; i++)
                {
                    bool pass = true;
                    foreach (var ticket in tickets)
                    {
                        if (!constraint.Check(ticket.Numbers[i]))
                        {
                            pass = false;
                            break;
                        }
                    }
                    if (pass)
                    {
                        constraint.PossiblePositions.Add(i);
                    }
                }
            }
        }

        protected void DecipherTicket()
        {
            CompilePossibleFieldPositions();

            List<Constraint> newLists = new List<Constraint>();
            while(constraints.Count > 0)
            {
                Constraint temp = FindDecipheredConstraint();
                if (temp == null) throw new Exception("What the!!!");
                temp.Position = temp.PossiblePositions[0];
                RemovePositionFromOthers(temp.Position);
                newLists.Add(temp);
                constraints.Remove(temp);
            }
            newLists.Sort();
            constraints = newLists;

            Constraint FindDecipheredConstraint()
            {
                foreach(var constraint in constraints)
                {
                    if (constraint.PossiblePositions.Count == 1) return constraint;
                }
                return null;
            }

            void RemovePositionFromOthers(int k)
            {
                foreach (var constraint in constraints)
                {
                    constraint.PossiblePositions.Remove(k);
                }
            }
        }

        public long SolutionPart2()
        {
            RemoveBadTickets();
            DecipherTicket();

            long result = 1;
            for(int i = 0; i < constraints.Count; i++)
            {
                if(constraints[i].isType("departure"))
                {
                    result *= tickets[0].Numbers[i];
                }
            }
            return result;
        }

        public string SolutionSamplePart2()
        {
            StringBuilder sb = new StringBuilder();
            SolutionPart2();
            foreach(var constraint in constraints)
            {
                sb.Append($"{constraint.Name} : {constraint.Position}\n");
            }
            return sb.ToString();
        }
    }
}
