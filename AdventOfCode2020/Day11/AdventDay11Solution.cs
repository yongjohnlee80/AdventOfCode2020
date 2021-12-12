using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode2020
{
    /// <summary>
    /// Type AdventDay11
    /// This is the unit testing entry.
    /// Various Solution methods are implmented to interact with the unit to be tested.
    /// This class also modifies the training data into proper data structure.
    /// </summary>
    internal class AdventDay11
    {
        /// <summary>
        /// SeatCoordinator receives seating request from passengers and perform the requests until
        /// the seating equilibrium is met.
        /// </summary>
        Coordinator seatCoordinator;
        // constructor
        public AdventDay11()
        {
            seatCoordinator = new Coordinator();
        }
        /// <summary>
        /// Method: LoadMapFromFile
        /// Seating Map is loaded from a textual file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>The total number of the seats.</returns>
        public int LoadMapFromFile(string fileName)
        {
            var fname = Path.Combine(System.Environment.CurrentDirectory, fileName);
            var lines = File.ReadAllLines(fname);
            return LoadMapFromTextLines(lines);
        }

        /// <summary>
        /// Mthod: LoadMapFromTextLines
        /// The seating map is created from textual data along with its dimensions.
        /// </summary>
        /// <param name="textLines"></param>
        /// <returns>The total number of the seats.</returns>
        public int LoadMapFromTextLines(string[] textLines)
        {
            // Mapping Dimentions.
            seatCoordinator.MaxRow = textLines.Length;
            seatCoordinator.MaxColumn = textLines[0].Length;

            for (int i = 0; i < textLines.Length; i++)
                            // Iterate Rows.
            {
                for (int j = 0; j < textLines[i].Length; j++)
                                // Iterate Columns.
                {
                    if (textLines[i][j] == 'L')
                    {
                        seatCoordinator.AddSeat(i, j);
                    }
                }
            }
            return seatCoordinator.GetSeatCount();
        }

        /// <summary>
        /// First half of the solution.
        /// </summary>
        /// <returns>The number of seats taken according to the first set of rules.</returns>
        public long FindSolution()
        {
            seatCoordinator.LinkSeats(new LinkAdjacentSeats());
            return seatCoordinator.Run();
        }

        /// <summary>
        /// Second half of the solution.
        /// </summary>
        /// <returns>The number of seats taken according to the second set of rules.</returns>
        public long FindSolution2()
        {
            var linkMethod = new LinkFirstSight();
            linkMethod.LoadSeatingMap(seatCoordinator.GetSeatingMap(), seatCoordinator.MaxRow, seatCoordinator.MaxColumn);
            seatCoordinator.LinkSeats(linkMethod);
            return seatCoordinator.Run(5);
        }
    }

    /**************************************************************************
     * Implement classes on separate files once test is successfully tested.
     * Author: Yong Sung Lee.
     * Date: 2021-12-07
     **************************************************************************/
    
    /// <summary>
    /// Enum Type: Seat Status
    /// Can be represented as boolean, but we never know in the future....
    /// </summary>
    internal enum SeatStatus
    {
        Empty,
        Occupied
    }

    /// <summary>
    /// Enum Type: Task types that Coordinator class can perform on Seats.
    /// </summary>
    public enum TaskType
    {
        Vacate,
        Reserve
    }

    /// <summary>
    /// Type: Task
    /// Coordinator class use this data type to push and pull seating related tasks to
    /// appease the passengers until equilibuiem is met.
    /// </summary>
    internal class Task
    {
        public int SeatID { get; set; }
        public TaskType task;

        public Task(int seatID, TaskType task)
        {
            this.SeatID = seatID;
            this.task = task;
        }

        public TaskType GetTask()
        {
            return task;
        }

    }

    /// <summary>
    /// Observe Pattern...
    /// 
    /// Interface: IObserver
    /// Subscriber or Listener interface.
    /// </summary>
    internal interface IObserver
    {
        public void Update(SeatStatus msg);
                // Seating status of linked seats (of interest for each seat) are notified.
    }

    /// <summary>
    /// Interface: ISubject
    /// Event Generator or Publisher interface.
    /// </summary>
    internal interface ISubject
    {
        public void RegisterObserver(IObserver observer);
                        // Register an observer or subscriber.
        public void UnregisterObserver(IObserver observer);
                        // remove an observer
        public void NotifyObservers();
                        // notify change in seating status to all observers.
    }

    /// <summary>
    /// Type: Seat
    /// Each Seat is both subscriber and publisher.
    /// It observes the interested seating positions for any changes to determine
    /// whether the passenger that sits on it should leave or stay.
    /// </summary>
    internal class Seat : IObserver, ISubject
    {
        protected bool occupied = false; // Seat Status
        protected List<IObserver> observers = new List<IObserver>();
                                    // Subscribed Observers.
        protected int adjFilled = 0;
                    // Number of linked (or obseving) seats that are filled.
        
        /// <summary>
        /// Is it seated or vacated? 
        /// That is the question.
        /// </summary>
        /// <returns>bool: occupied</returns>
        public bool IsOccupied()
        {
            return occupied;
        }

        /// <summary>
        /// The Number of linked (or observing) seats that are filled.
        /// </summary>
        /// <returns>int: adjFilled</returns>
        public int HowManyFilled()
        {
            return adjFilled;
        }
        /// <summary>
        /// Register Observer (a Seat)
        /// </summary>
        /// <param name="observer"></param>
        public void RegisterObserver(IObserver observer)
        {
            observers.Add(observer);
        }
        /// <summary>
        /// Unregister.... although never used in this example....
        /// </summary>
        /// <param name="observer"></param>
        public void UnregisterObserver(IObserver observer)
        {
            observers.Remove(observer);
        }
        /// <summary>
        /// Notify its seating status changes to observers.....
        /// This whole program depend on this method.....
        /// </summary>
        public void NotifyObservers()
        {
            SeatStatus msg = (occupied ? SeatStatus.Occupied : SeatStatus.Empty);
            foreach(var o in observers)
            {
                o.Update(msg);
            }
        }
        /// <summary>
        /// This command is issued from Coordinator (or the big brother) who knows it all... 
        /// must follow....
        /// </summary>
        public void Vacate()
        {
            if(occupied)
            {
                occupied = false;
                NotifyObservers();
            }
        }
        /// <summary>
        /// This command is also issued from the Coordinator( or the Skynet) who runs things around here...
        /// must follow.....
        /// </summary>
        public void Reserve()
        {
            if(!occupied)
            {
                occupied=true;
                NotifyObservers();
            }
        }
        /// <summary>
        /// Okay... Someone just changed its status.. let's keep track of it...
        /// </summary>
        /// <param name="msg"></param>
        public void Update(SeatStatus msg)
        {
            if (msg == SeatStatus.Occupied) adjFilled++;
            else adjFilled--;
        }

    }
    /// <summary>
    /// Strategy Pattern.....
    /// 
    /// Mainly create a list of intersting seats that a seat should subscribe to
    /// based on linking behavior..
    /// </summary>
    internal abstract class LinkBehavior
    {
        /// <summary>
        /// Generate Seating ID based on its row and column position.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static int GenerateSeatID(int row, int column)
        {
            if (row < 0 || column < 0) return -1; // out of bound.
            return row << 16 ^ column;
        }
        /// <summary>
        /// reverse the above method....
        /// </summary>
        /// <param name="SeatID"></param>
        /// <returns></returns>
        public (int, int) GetSeatRowCol(int SeatID)
        {
            return (SeatID >> 16, SeatID & 0x0000FFFF);
        }

        /// <summary>
        /// The Crux of this Class...
        /// Must be implemented in the subclass which will be called upon to do 
        /// its diligence by the Coordinator.. the big brother.
        /// </summary>
        /// <param name="SeatID"></param>
        /// <returns></returns>
        public abstract List<int> GetLinkedSeatIDs(int SeatID);
    }
    /// <summary>
    /// Link all adjacent seats together in a network.
    /// </summary>
    internal class LinkAdjacentSeats : LinkBehavior
    {
        /// <summary>
        /// Let's link all adjacent seats together so that they will in the 
        /// loop of what's happening around them.
        /// </summary>
        /// <param name="SeatID"></param>
        /// <returns></returns>
        public override List<int> GetLinkedSeatIDs(int SeatID)
        {
            int row, column;
            (row, column) = GetSeatRowCol(SeatID);
            List<int> ids = new List<int>();
            // Upper Row
            ids.Add(GenerateSeatID(row - 1, column - 1));
            ids.Add(GenerateSeatID(row - 1, column));
            ids.Add(GenerateSeatID(row - 1, column + 1));
            // Current Row
            ids.Add(GenerateSeatID(row, column - 1));
            ids.Add(GenerateSeatID(row, column + 1));
            // Following Row
            ids.Add(GenerateSeatID(row + 1, column - 1));
            ids.Add(GenerateSeatID(row + 1, column));
            ids.Add(GenerateSeatID(row + 1, column + 1));
            return ids;
        }
    }
    /// <summary>
    /// We actually don't care about the empty floors....
    /// Let's connect the Seats that are seeable from each other....
    /// The first seat that is reachable by the 8 directions....
    /// </summary>
    internal class LinkFirstSight : LinkBehavior
    {
        /// <summary>
        /// We better have the knowledge of the floor plan.....
        /// to ignore the empty floor....
        /// </summary>
        protected Dictionary<int, Seat> map;
        protected int maxRow, maxCol;
        public void LoadSeatingMap(Dictionary<int, Seat> map, int maxRow, int maxCol)
        {
            this.map = map;
            this.maxRow = maxRow;
            this.maxCol = maxCol;
        }
        /// <summary>
        /// We search for the very first one in all direction...
        /// Let's expand our circle....
        /// </summary>
        /// <param name="SeatID"></param>
        /// <returns></returns>
        public override List<int> GetLinkedSeatIDs(int SeatID)
        {
            int row, column;
            (row, column) = GetSeatRowCol(SeatID);
            List<int> ids = new List<int>();
            // Upper Left
            for(int i = 1; row - i >= 0 && column - i >= 0; i++)
            {
                int tempID = GenerateSeatID(row - i, column - i);
                if(map.ContainsKey(tempID))
                {
                    ids.Add(tempID);
                    break;
                }
            }
            // Upper Middle
            for (int i = 1; row - i >= 0; i++)
            {
                int tempID = GenerateSeatID(row - i, column);
                if (map.ContainsKey(tempID))
                {
                    ids.Add(tempID);
                    break;
                }
            }
            // Upper Right
            for (int i = 1; row - i >= 0 && column + i < maxCol; i++)
            {
                int tempID = GenerateSeatID(row - i, column + i);
                if (map.ContainsKey(tempID))
                {
                    ids.Add(tempID);
                    break;
                }
            }
            // To Left
            for (int i = 1; column - i >= 0; i++)
            {
                int tempID = GenerateSeatID(row, column - i);
                if (map.ContainsKey(tempID))
                {
                    ids.Add(tempID);
                    break;
                }
            }
            // To Right
            for (int i = 1; column + i < maxCol; i++)
            {
                int tempID = GenerateSeatID(row, column + i);
                if (map.ContainsKey(tempID))
                {
                    ids.Add(tempID);
                    break;
                }
            }
            // Lower Left
            for (int i = 1; row + i < maxRow && column - i >= 0; i++)
            {
                int tempID = GenerateSeatID(row + i, column - i);
                if (map.ContainsKey(tempID))
                {
                    ids.Add(tempID);
                    break;
                }
            }
            // Lower Middle
            for (int i = 1; row + i < maxRow; i++)
            {
                int tempID = GenerateSeatID(row + i, column);
                if (map.ContainsKey(tempID))
                {
                    ids.Add(tempID);
                    break;
                }
            }
            // Lower Right
            for (int i = 1; row + i < maxRow && column + i < maxCol; i++)
            {
                int tempID = GenerateSeatID(row + i, column + i);
                if (map.ContainsKey(tempID))
                {
                    ids.Add(tempID);
                    break;
                }
            }
            return ids;
        }
    }
    /// <summary>
    /// The Big Brother Class who knows it all and run things around here.
    /// It performs three major things....
    /// Do we need three separate classes? Yeah... So we got three...
    /// 
    /// 1. Create a hashing seating map using Data Type Seat along with some stats...
    /// 2. Link Seats into a group according to LinkBehavior (another class).
    /// 3. Generate To Do list (tasks) and performs by ordering Seat objects to its bidding.... like a Big Brother.
    /// </summary>
    internal class Coordinator
    {
        // fields
        Dictionary<int,Seat> map = new Dictionary<int,Seat>();
        List<Task> tasks = new List<Task>();
        int nOccupied = 0;

        public int MaxRow { get; set; }
        public int MaxColumn { get; set; }
        /// <summary>
        /// Generate Seat ID based on its positions (row and column).
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static int GenerateSeatID(int row, int column)
        {
            if (row < 0 || column < 0) return -1;
            return row << 16 ^ column;
        }

        /// <summary>
        /// Getters Here On.
        /// </summary>
        /// <returns></returns>
        public int GetSeatCount() { return map.Count; }

        public Dictionary<int,Seat> GetSeatingMap() { return map; }

        /// <summary>
        /// Add a Seat into the space.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        public void AddSeat(int row, int column)
        {
            map.Add(GenerateSeatID(row, column), new Seat());
        }

        /// <summary>
        /// Group seats together based on linking behavior
        /// </summary>
        /// <param name="linkMethod"></param>
        public void LinkSeats(LinkBehavior linkMethod)
        {
            foreach(int i in map.Keys)
            {
                var adjSeats = linkMethod.GetLinkedSeatIDs(i);
                foreach(var j in adjSeats)
                {
                    if(map.ContainsKey(j)) map[i].RegisterObserver(map[j]);
                }
            }
        }

        /// <summary>
        /// Three rules to follow from the day 11's requirement.
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public bool GenerateTasks(int k)
        {
            foreach(var i in map.Keys)
            {
                // Rule 1
                if(!map[i].IsOccupied() && map[i].HowManyFilled() == 0)
                {
                    tasks.Add(new Task(i, TaskType.Reserve));
                }
                // Rule 2
                if(map[i].IsOccupied() && map[i].HowManyFilled() >= k)
                {
                    tasks.Add(new Task(i, TaskType.Vacate));
                }
                // Rule 3 : DO NOTHING.
            }
            return tasks.Count > 0 ? true : false;
        }

        /// <summary>
        /// Now Let's execute the tasks in the queue.
        /// </summary>
        public void ExecuteTasks()
        {
            foreach(Task task in tasks)
            {
                if(task.GetTask() == TaskType.Reserve)
                            // Save a Spot!
                {
                    map[task.SeatID].Reserve();
                    nOccupied++;
                } 
                else // Or Empty it.
                {
                    map[task.SeatID].Vacate();
                    nOccupied--;
                }
            }
            tasks.Clear();
        }

        /// <summary>
        /// Skynet (Big Brother) Online.
        /// </summary>
        /// <param name="whenToVacate"></param>
        /// <returns></returns>
        public int Run(int whenToVacate = 4)
        {
            while(true)
            {
                if (!GenerateTasks(whenToVacate)) break;
                ExecuteTasks();
            }
            return nOccupied;
        }
    }

}