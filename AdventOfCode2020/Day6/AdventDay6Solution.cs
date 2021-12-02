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
    internal class AdventDay6
    {
        // fields
        string[] data;
        // constructor
        public AdventDay6(string[] data)
        {
            this.data = data;
        }
        // methods.
        public int FindSolution(bool union = true)
        {
            var groups = new List<PassengerGroup>();
            groups.LoadAnswers(data);
            return groups.SurveyAnswers(union);
        }
    }

    /**************************************************************************
     * Implement classes on separate files once test is completed.
     * Author: Yong Sung Lee.
     * Date: 2021-11-25.
     **************************************************************************/


    /**************************************************************************
     * Type PassengerGroup
     **************************************************************************/
    public class PassengerGroup
    {
        // fields
        List<int> indAnswers = new List<int> ();
        public void Append(string answers)
        {
            int result = 0;
            answers = answers.ToLower();
            foreach(char i in answers)
            {
                int temp = 1 << (i - 'a');
                result = result | temp;
            }
            indAnswers.Add(result);
        }
        public int HowMany(int answer)
        {
            string temp = Convert.ToString(answer, 2);
            return temp.Count(x => x == '1');
        }
        public int GroupUnion(bool binaryReturn = false)
        {
            int result = 0;
            foreach(int i in indAnswers)
            {
                result |= i;
            }
            if(binaryReturn) return result;
            else return HowMany(result);
        }
        public int GroupIntersect(bool binaryReturn = false)
        {
            int result = -1;
            foreach(int i in indAnswers)
            {
                result &= i;
            }
            if (binaryReturn) return result;
            else return HowMany(result);
        }
    }

    /**************************************************************************
     * Extension Methods for List<TravelDocument>
     **************************************************************************/
    public static class PassengerGroupExtensions
    {
        // Load yes answered questions. Each group is separated by a empty new line.
        public static void LoadAnswers(this List<PassengerGroup> input, string[] lines)
        {
            if (input.Count == 0) input.Add(new PassengerGroup()); // add a group if none.
            // Process all lines. Each line represents one person's answers.
            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line)) input.Add(new PassengerGroup()); // new group.
                else
                {
                    input[input.Count - 1].Append(line);
                }
            }
        }
        // Count the number of yes answered questions from all groups.
        public static int SurveyAnswers(this List<PassengerGroup> input, bool unionMode = true)
        {
            int count = 0;
            foreach (PassengerGroup i in input)
            {
                if(unionMode)
                {
                    count += i.GroupUnion();
                } else
                {
                    count += i.GroupIntersect();
                }
            }
            return count;
        }
    }
}
