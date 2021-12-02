using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode2020
{
    /*************************************************************************
     * class for unit testing.
     * delete it before integration
     **************************************************************************/
    internal class AdventDay7
    {
        // fields
        string[] data;
        // constructor
        public AdventDay7(string[] data)
        {
            this.data = data;
        }
        // methods.
        public int FindSolution(string name)
        {
            var rules = new BagRules();
            rules.LoadRules(data);
            return (rules.FindAllBagsWith(name)).Count;
        }

        public int FindSolution2(string name)
        {
            var rules = new BagRules();
            rules.LoadRules(data);
            return rules.GetNumberOfBags(name);
        }
    }

    /**************************************************************************
     * Implement classes on separate files once test is completed.
     * Author: Yong Sung Lee.
     * Date: 2021-11-25.
     **************************************************************************/


    /// <summary>
    /// Type Bag
    /// This data type contains the information about the luggage, namely the attributes (eg. color)
    /// Also contains the children (the contents) bag names and quantity.
    /// </summary>
    public class Bag
    {
        // fields

        // the contents, namely the children bags and quantity.
        public Dictionary<string, int> children = new Dictionary<string, int>();

        // constructor
        public Bag(string name)
        {
            Name = name;
        }

        // Add a child bag that can be put in.
        public void AddChild(string name, int count)
        {
            children.Add(name, count);
        }

        // Check whether this bag can contain a specific bag.
        public bool HasChild(string name) { return children.ContainsKey(name); }

        // Name attribute.
        public string Name { get; set; }

    }

    /// <summary>
    /// Type BagRules.
    /// Create list of bags with the rules of what children bags each can contain.
    /// </summary>
    public class BagRules
    {
        // fields
 //       public List<Bag> rule = new List<Bag>(); // list of all bags.

        public Dictionary<string, Bag> bags = new Dictionary<string,Bag>();

        public int Count
        {
            get { return bags.Count; } // return number of bags.
        }

        // Load the bags with their rules from text file.
        public void LoadRules(string[] lines)
        {
            foreach (string line in lines)
            {
                // PARENT NODE
                string[] parts = line.Split("contain"); // split the bag and its contents (children bags).
                string nodeName = parts[0].Replace("bags", "").Replace("bag", "").Trim(); // extract the name of the bag.
                if (!bags.ContainsKey(nodeName)) // safety net, check whether the bag already exists.
                {
                    bags.Add(nodeName, new Bag(nodeName)); // create parent node.

                    // CHILDREN NODES
                    string[] nodes = parts[1].Split(","); // separate children nodes.
                    foreach (string node in nodes)
                    {
                        if (node.Contains("no other bags")) break; // if it has no child, move on.
                        string temp = node.Replace(".", "").Trim(); // dot not needed.
                        string[] childName = Regex.Split(temp, @"\d+"); // extracts attributes for node.
                        string[] quantity = Regex.Split(temp, @"\D+"); // extract quantity for the node.
                        string name = childName[1].Replace("bags", "").Replace("bag", "").Trim(); // words {bags, bag} are not needed.
                        int number = Convert.ToInt32(quantity[0]);

                        bags[nodeName].AddChild(name, number); // create child node.
                    }
                }
            }
        }

        // Find all bags that will eventually contained the named bag.
        public List<Bag> FindAllBagsWith(string name)
        {
            var result = new HashSet<Bag>(); // all bags that can contain the named bag.
            var queue = new HashSet<string>(); // priority queue used for searching up the tree (BFS).
            queue.Add(name);
            while (queue.Count > 0) // searching until no more searching required.
            {
                // dequeue the first element.
                string i = queue.First();
                queue.Remove(i);

                // Find all bags that directly contain the dequeued element.
                List<Bag> found = FindBagsWith(bags[i]);
                if (found.Count > 0)
                {
                    // Add the found nodes to queue and results
                    foreach (var item in found)
                    {
                        result.Add(item);
                        queue.Add(item.Name);
                    }
                }
            }
            return result.ToList();
        }

        // Find Bags that can directly contain the child bag. LinearSearch.
        public List<Bag> FindBagsWith(Bag child)
        {
            var result = new List<Bag>();
            foreach (var i in bags.Values)
            {
                if (i.HasChild(child.Name))
                {
                    result.Add(i);
                }
            }
            return result;
        }

        // Find how many bags are there in total with the named bag.
        // Subtract one from the result for the number of bags inside the named bag.
        public int GetNumberOfBags(string name)
        {
            if (bags[name].children.Count == 0)
            {
                return 1; // if leaf node.
            }
            else
            {
                int count = 1; // count self.
                foreach(var i in bags[name].children)
                {
                    count += (i.Value * GetNumberOfBags(i.Key)); // Traverse through children bags (DFS)
                }
                return count;
            }
        }
    }
}
