using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode2020
{
    /*************************************************************************
     * class for unit testing.
     * delete it before integration
     **************************************************************************/
    internal class AdventDay5
    {
        // fields
        string[] data;
        // constructor
        public AdventDay5(string[] data)
        {
            this.data = data;
        }
        // methods.
        public int FindSolution()
        {
            var passIDs = new BoardingPass(data);
            return passIDs.GetMaxID();
        }

        public int FindSolution2()
        {
            var tickets = new BoardingPass(data);
            return tickets.FindEmptySeat();
        }
    }

    /**************************************************************************
     * Implement classes on separate files once test is completed.
     * Author: Yong Sung Lee.
     * Date: 2021-11-24.
     **************************************************************************/

    /**************************************************************************
     * Type BoardingPass
     * Converts a corpus of boarding pass (binary space partitioning) into
     * seatIDs.
     **************************************************************************/
    public class BoardingPass
    {
        public List<int> seatID = new List<int>();
        public int Count { get; set; }

        public BoardingPass(string[] passes)
        {
            foreach (string pass in passes)
            {
                // F & L => first half or zero in binary
                // B & R => second half or one in binary
                string temp = pass.Replace("F", "0").Replace("B", "1").Replace("L", "0").Replace("R", "1");
                // Higher 7 Binary Digits are compensated with multiplication of 8 thus,
                // just combine two parts of the binary space partitionaing as is.
                seatID.Add(Convert.ToInt32(temp, 2));
            }
            Count = seatID.Count; // Number of tickets.
        }

        public int GetMaxID()
        {
            seatID.Sort();
            return seatID[seatID.Count - 1]; // The last ticket number after the sort.
        }

        public int FindEmptySeat()
        {
            int max = GetMaxID();
            int mid = max / 2; // Mid position of the seating ID number.

            // search from the middle for an empty seat.
            for (int i = 0; i < mid; i++)
            {
                if (isEmpty(mid + i)) return mid + i;
                else if(isEmpty(mid - i)) return mid - i;
            }
            return 0; // not found.

            // Check whether the seating ID is missing from the group.
            bool isEmpty(int n)
            {
                if (!seatID.Contains(n) && seatID.Contains(n + 1) && seatID.Contains(n - 1)) return true;
                else return false;
            }
        }
    }
}