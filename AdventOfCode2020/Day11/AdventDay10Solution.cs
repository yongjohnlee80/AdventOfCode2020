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
        // fields
        int[] data;
        // constructor
        public AdventDay11(IEnumerable<string> data)
        {
            this.data = data.Select(line => int.Parse(line)).ToArray();
        }

        //public long FindSolution()
        //    // For first half of the problem
        //{
        //    var Day10Solution = new LinkedAdapters(data);

        //    return Day10Solution.Compute(new UseAllAdapters());
        //}

        //public long FindSolution2()
        //    // For second half of the problem
        //{
        //    var Day10Solution = new LinkedAdapters(data);
        //    return Day10Solution.Compute(new DistinctArrangement());
        //}
    }

    /**************************************************************************
     * Implement classes on separate files once test is successfully tested.
     * Author: Yong Sung Lee.
     * Date: 2021-12-07
     **************************************************************************/
    internal enum Message
    {
        Occupied,
        Vacated
    }
    internal interface IObserver
    {
        public void Update(Message msg);
    }

    internal interface ISubject
    {
        public void RegisterObserver(IObserver observer);
        public void UnregisterObserver(IObserver observer);
        public void NotifyObservers();
    }

    internal class Seat : IObserver, ISubject
    {
        protected bool occupied = false;
        protected List<IObserver> observers = new List<IObserver>();
        protected int adjFilled = 0;

        public void RegisterObserver(IObserver observer)
        {
            observers.Add(observer);
        }

        public void UnregisterObserver(IObserver observer)
        {
            observers.Remove(observer);
        }

        public void NotifyObservers()
        {
            Message msg = Message.Occupied;
            foreach(var o in observers)
            {
                o.Update(msg);
            }
        }

        public void Update(Message msg)
        {

        }

    }

}