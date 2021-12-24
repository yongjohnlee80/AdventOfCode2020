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
    /// 
    /// This data type wasn't required after all...
    /// It's just an string wrapper class....
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
        /// <summary>
        /// Method: inRange
        /// checks whether the value is within the range (inclusively)...
        /// Should've named the method with CamelCase but.... too late...
        /// It's only exercise... But let's make a mental note...
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool inRange(int value)
        {
            return (value >= Start && value <= End);
        }
    }
    /// <summary>
    /// Extension Methods......
    /// I love automations.. Don't judge me...
    /// I think it makes my code more readable..
    /// </summary>
    internal static class RangeExtension
    {
        /// <summary>
        /// Method: inRange
        /// **** VERY IMPORTANT ****
        /// This method PERFORMS OR logic operations for checking multi ranges....
        /// THUS!!! if the value satisfies one of the ranges available....
        /// Matter of fact, it could be just better to rename the method to
        /// InAnyRanges() or something...
        /// I should do that...
        /// </summary>
        /// <param name="input"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool InAnyRanges(this List<Range> input, int value)
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

    /// <summary>
    /// Implementing flexibility for a composition or strategy pattern was a total waste in this case....
    /// The Part2 didn't require any modification to how the data was translated.
    /// Oh well... 
    /// </summary>
    class ConstraintFromString : LoadingConstraint
    {
        private readonly static Regex ConstraintRegex = new Regex(@"([a-z ]+): (\d+-\d+) or (\d+-\d+)");

        public Constraint Load(string line)
        {
            Constraint constraint;
            var match = ConstraintRegex.Match(line); // REGEX BABY!!!!

            // if failed!
            if (!match.Success) throw new Exception($"Not understanding what you're trying to say...{line}");

            // all good to go!
            constraint = new Constraint(match.Groups[1].Value);
            constraint.AddRange(match.Groups[2].Value).AddRange(match.Groups[3].Value);
                                        // Add those ranges...
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

        // Part II requirements.
        protected int FieldPosition = 0;
        public List<int> PossiblePositions = new List<int>();

        public Constraint(string type) 
        {
            cType = new ConstraintType(type); // It's just a string really...
        }

        /// <summary>
        /// Method: isType
        /// Mental Note!!!! USE CamelCase!
        /// This method checks whether a constraint name contains a substring...
        /// </summary>
        /// <param name="substring"></param>
        /// <returns></returns>
        public bool isType(string substring)
        {
            return cType.Name.Contains(substring);
        }

        /// <summary>
        /// Getters and Setters!!!
        /// </summary>
        public string Name
        {
            get { return cType.Name; }
        }

        public int Position
        {
            get { return FieldPosition; }
            set { FieldPosition = value; }
        }

        /// <summary>
        /// Method: AddRange
        /// This was uses integers
        /// The following method uses string in the form of 123-456
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Method: Check
        /// checks whether value satisfies any of the ranges this constraint have.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Check(int value)
        {
            return ranges.InAnyRanges(value);
        }

        /// <summary>
        /// IComparable Implementation.. used for sorting in this case... 
        /// don't really need it but let's make it neat.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Constraint? other)
        {
            return this.FieldPosition.CompareTo(other.FieldPosition);
        }
    }

    /// <summary>
    /// ExTENSION BABY!!!!
    /// </summary>
    internal static class ConstraintExtension
    {
        /// <summary>
        /// Method: GetErrorValuesFromTicket
        /// Used to calculate the ticket scanning error rate....
        /// Not sure what that is.. but Part1 requirement.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="ticket"></param>
        /// <returns></returns>
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
                if(!pass) result.Add(value); // collects the bad numbers from each ticket
            }
            return result.ToArray();
        }

        /// <summary>
        /// Method: Checkticket
        /// Check if the numbers on the ticket are all valid...
        /// If not... it means... it's a fake... probably counterfeit sold 
        /// at a corner somewhere in the north pole...
        /// </summary>
        /// <param name="input"></param>
        /// <param name="ticket"></param>
        /// <returns></returns>
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
    /// Type: Ticket
    /// Each tickets got a series of numbers...
    /// </summary>
    internal class Ticket
    {
        protected int[] numbers;

        public int[] Numbers { get { return numbers; } }

        public Ticket(int[] numbers)
        {
            this.numbers = numbers;
        }
        /// <summary>
        /// Like I said... automation....
        /// </summary>
        /// <param name="numbers"></param>
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
    /// Once again, part I requirement.
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
    /// Main Class that puts everything together....
    /// </summary>
    internal class TicketValidator
    {
        /// <summary>
        /// Fields...
        /// error numbers from counterfeit tickets.
        /// constraints from the database
        /// and finally all the tickets available in this place.
        /// </summary>
        protected ErrorValues errors;
        protected List<Constraint> constraints;
        protected List<Ticket> tickets;

        protected LoadingConstraint loadingConstraint; // definately didn't needed it...

        public TicketValidator() 
        {
            errors = new ErrorValues();
            constraints = new List<Constraint>();
            tickets = new List<Ticket>();
        }

        /// <summary>
        /// NOPE! DIDN'T NEED IT!
        /// </summary>
        public void LoadingForPart1()
        {
            loadingConstraint = new ConstraintFromString();
        }

        /// <summary>
        /// Method: LoadData
        /// Load them from a file.
        /// </summary>
        /// <param name="fileName"></param>
        public void LoadData(string fileName) 
        {
            var fname = Path.Combine(System.Environment.CurrentDirectory, fileName);
            var lines = File.ReadAllLines(fname);
            LoadData(lines);
        }

        /// <summary>
        /// Method: LoadData
        /// Load them from texts / string[]
        /// </summary>
        /// <param name="lines"></param>
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

        /// <summary>
        /// Method: TicketScanningErrorRate
        /// Part I ANSWER here.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Method: RemoveTickets
        /// removing counterfeits.
        /// </summary>
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

        /// <summary>
        /// Method: CompilePossibleFieldPOsitions
        /// Just so happens, each constraint seemingly qualifies for more than one section of the tickets initially even
        /// with so much data available from more than 200 tickets..
        /// What do you know? Let's keep track of them all then...
        /// </summary>
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

        /// <summary>
        /// Method: DecipherTicket
        /// Part II requirement...
        /// since each constraint qualifies for multiple sections of the ticket we have to use deduction process...
        /// Think of as playing Solitare but with possible field positions for each constraint...
        /// fortunately... there is always one constraint with only one option left...
        /// Let's eliminate through all...
        /// </summary>
        /// <exception cref="Exception"></exception>
        protected void DecipherTicket()
        {
            CompilePossibleFieldPositions();

            List<Constraint> newLists = new List<Constraint>();
            while(constraints.Count > 0)
            {
                Constraint temp = FindDecipheredConstraint();
                             // it means a constraint with only one available option for its position.

                if (temp == null) throw new Exception("What the!!!"); 
                            // If you see this excetion, then is means you're dealt with impossible case.

                /// Build a new list with confirmed position for each constraint.
                temp.Position = temp.PossiblePositions[0];
                RemovePositionFromOthers(temp.Position); 
                            // This process will make another constraint with only one available option to continue on...

                newLists.Add(temp); // Add it to new list
                constraints.Remove(temp); // Remove it from the old list.
            }
            newLists.Sort();
            constraints = newLists;
            
            /// Helper Functions...
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

        /// <summary>
        /// Method: SolutionPart2
        /// All done... it's very readable....
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// For Part2 sample solution...
        /// </summary>
        /// <returns></returns>
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
