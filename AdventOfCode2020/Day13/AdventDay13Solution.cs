using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode2020
{
    /// <summary>
    /// Type AdventDay12
    /// This is the unit testing entry.
    /// Various Solution methods are implmented to interact with the unit to be tested.
    /// This class also modifies the training data into proper data structure.
    /// </summary>
    internal class AdventDay13
    {

        /// <summary>
        /// Method: LoadCommandsFromFile
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>The total number of the seats.</returns>

        /// <summary>
        /// First half of the solution.
        /// </summary>
        /// <returns></returns>
        public int FindSolution(string dataFile = "Day13Sample.txt")
        {
            BusSchedule busSchedule = new BusSchedule();
            busSchedule.BuildFromFile(dataFile);
            return busSchedule.FindEarliestDeparture();
        }

        /// <summary>
        /// Second half of the solution.
        /// </summary>
        /// <returns></returns>
        public long FindSolution2(string dataFile = "Day13Sample.txt")
        {
            BusSchedule busSchedule = new BusSchedule(new LoadAllBusRoutes());
            busSchedule.BuildFromFile(dataFile);
            return busSchedule.FindPart2Answer();
        }
    }

    /**************************************************************************
     * Implement classes on separate files once test is successfully tested.
     * Author: Yong Sung Lee.
     * Date: 2021-12-15
     **************************************************************************/

    /// <summary>
    /// Type: Bus
    /// Contains BusID provide IComparable CompareTo method to be sorted in a collection.
    /// </summary>
    internal class Bus : IComparable<Bus>
    {
        public int ID { get; set; }

        protected int waitTime = 0;

        public int WaitTime {  get { return waitTime; } }

        public Bus(int ID)
        {
            this.ID = ID;
        }

        /// <summary>
        /// Part 1
        /// Computes how many minutes for this bus to departure @ given time stamp.
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public virtual int ComputeWaitTime(int timeStamp)
        {
            waitTime = ID - (timeStamp % ID);
            return waitTime;
        }

        /// <summary>
        /// Part 2
        /// Checks whether this bus departures at this given timestamp.
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public virtual bool CheckPart2(long timeStamp)
        {
            return (timeStamp % ID) == 0;
        }

        /// <summary>
        /// IComparable implementation for sorting.
        /// Is used for part 1.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Bus other)
        {
            return waitTime.CompareTo(other.waitTime);
        }
    }

    /// <summary>
    /// Type: BrokenBus
    /// Unlike part 1, all the out of service buses must be implemented as well.
    /// </summary>
    internal class BrokenBus : Bus
    {
        public BrokenBus(int ID) : base(1) { }
                    /// For the purpose of part 2, all broken bus IDs are 1
                    /// for the simplicity.

        /// <summary>
        /// Method: ComputeWaitTime
        /// Wait all you want, a broken bus will never arrive.
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public override int ComputeWaitTime(int timeStamp)
        {
            return int.MaxValue;
        }

        /// <summary>
        /// Method: CheckPart2
        /// It's always true, meaning you don't have to worry about a broken bus, which will never
        /// arrive anyways.
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public override bool CheckPart2(long timeStamp)
        {
            return true;
        }
    }

    /// <summary>
    /// Procedural Type: LoadBusRoutes
    /// For Composition Purpose (strategy pattern).
    /// </summary>
    internal class LoadBusRoutes
    {
        /// <summary>
        /// Debugging Purpose.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="list"></param>
        public void writeLog(int t, List<Bus> list)
        {
            LogStat log = new LogStat();
            log.Append($"Time Stamp : {t}\n");
            foreach (Bus bus in list)
            {
                log.Append($"Bus {bus.ID}\n");
            }
            log.LogOnFile();
        }
        /// <summary>
        /// Method: LoadRoutesFromFile
        /// Don't be passing around reference variables...
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="timeStamp"></param>
        /// <param name="busList"></param>
        /// <returns></returns>
        public virtual bool LoadRoutesFromFile(string fileName, out int timeStamp, out List<Bus> busList)
        {
            busList = new List<Bus>();
            try
            {
                var fname = Path.Combine(System.Environment.CurrentDirectory, fileName);
                var lines = File.ReadAllLines(fname);

                timeStamp = Int32.Parse(lines[0]); // Timestamp for Part 1 problem.

                var buses = lines[1].Split(",");
                int temp = 0;
                foreach(var b in buses)
                    // Buses... The good one, forget about the bad ones...
                {
                    if(Int32.TryParse(b, out temp))
                    {
                        busList.Add(new Bus(temp));
                    }
                }
                return true;
            }
            catch (Exception ex)
                // File not found or wrong formatting, etc....
            {
                timeStamp = 0;
                busList = null;
                return false;
            }
        }
    }

    /// <summary>
    /// Procedural Type: LoadAllBusRoutes including the bad ones....
    /// I could've just modify the above one... but decided not to take that course.....
    /// To simulate as if this is a huge project.... not wanting to touch the working codes for the part 1....
    /// </summary>
    internal class LoadAllBusRoutes : LoadBusRoutes
    {
        /// <summary>
        /// Pretty much the same stuff.. except now I add the broken bus routes as well....
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="timeStamp"></param>
        /// <param name="busList"></param>
        /// <returns></returns>
        public override bool LoadRoutesFromFile(string fileName, out int timeStamp, out List<Bus> busList)
        {
            busList = new List<Bus>();
            try
            {
                var fname = Path.Combine(System.Environment.CurrentDirectory, fileName);
                var lines = File.ReadAllLines(fname);

                timeStamp = Int32.Parse(lines[0]); // Didn't need this one either....

                var buses = lines[1].Split(",");
                int temp = 0;
                int nBroken = -1; 
                        /// new addition... keeping track of how many bad ones... just in case..
                        /// Although never needed it.....

                foreach (var b in buses)
                {
                    if (Int32.TryParse(b, out temp))
                    {
                        busList.Add(new Bus(temp));
                    }
                    else
                    {
                        busList.Add(new BrokenBus(nBroken--)); // New addition on this class....
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                timeStamp = 0;
                busList = null;
                return false;
            }
        }
    }

    /// <summary>
    /// Type: BusSchedule
    /// Keeping all the bus routes loaded from a file...
    /// </summary>
    internal class BusSchedule
    {
        List<Bus> busList = new List<Bus>();
        int timeStamp = 0;
        LoadBusRoutes extData; // composition class.

        public BusSchedule(LoadBusRoutes? loadingMethod = null)
        {
            if (loadingMethod == null) extData = new LoadBusRoutes();
            else extData = loadingMethod;

        }
        
        public void SetLoadingBehavior(LoadBusRoutes loadingMethod)
        {
            extData = loadingMethod;
        }

        /// <summary>
        /// Build the bus schedules or departure table from a file according to
        /// LoadBusRoute class definition.
        /// Implement a new one, if so desire...
        /// </summary>
        /// <param name="fname"></param>
        public void BuildFromFile(string fname)
        {
            extData.LoadRoutesFromFile(fname, out timeStamp, out busList);
        }

        /// <summary>
        /// Helper method for part 1
        /// </summary>
        /// <param name="timeStamp"></param>
        protected void ComputeWaitTimes(int timeStamp)
        {
            foreach(var i in busList)
            {
                i.ComputeWaitTime(timeStamp);
            }
            busList.Sort();
        }

        /// <summary>
        /// Part 1 Answer.
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public int FindEarliestDeparture(int timeStamp = 0)
        {
            if (timeStamp == 0) timeStamp = this.timeStamp;
                    // unless specified otherwise, use the one provided from the file.
            ComputeWaitTimes(timeStamp);
            var i = busList.First(); // the very first bus that will departure from the time stamp.
            return i.ID * i.WaitTime; // part 1 answer....
        }

        /// <summary>
        /// Part 2 Answer.
        /// </summary>
        /// <returns></returns>
        public long FindPart2Answer()
        {
            long offset = busList.First().ID; // First Bus Departure Time.
            long timeStamp = 0;

            for(int i = 1; i < busList.Count; i++)
            {
                while (!busList[i].CheckPart2(timeStamp+i)) timeStamp += offset;
                        // Staggering Busses' departure time by one minute consecutively.

                offset *= MinFactor(busList[i].ID);
                        // Increase the offset value for the next bus route.
            }

            return timeStamp; // the answer.

            /// Helper Method to find the minimal factor of the number for offset 
            /// Computation.
            long MinFactor(long num)
            {
                for(int i = 2; i <= null/2; i++)
                {
                    if (num % i == 0) return i; // if not a prime number
                }
                return num; // if prime.
            }
        }
    }

}