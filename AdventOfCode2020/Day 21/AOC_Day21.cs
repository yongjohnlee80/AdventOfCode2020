using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NUnit.Framework;

namespace AdventOfCode2020.Day_21
{
    internal class AOC_Day21_Tests
    {
        [Test]
        public void TestSample()
        {
            string[] dataLines = @"
mxmxvkd kfcds sqjhc nhms (contains dairy, fish)
trh fvjkl sbzzf mxmxvkd (contains dairy)
sqjhc fvjkl (contains soy)
sqjhc mxmxvkd sbzzf (contains fish)".Split("\r\n",StringSplitOptions.RemoveEmptyEntries);

            AOC_Day21 test = new AOC_Day21(dataLines);
            // Part 1
            Assert.That(test.Part1Answer(), Is.EqualTo(5));
            // Part 2
            Assert.That(test.Part2Answer(), Is.EqualTo("mxmxvkd,sqjhc,fvjkl"));
        }

        [Test]
        public void TestPart1Solution()
        {
            AOC_Day21 test = new AOC_Day21("day21data.txt");
            Console.WriteLine(test.Part1Answer());
        }

        [Test]
        public void TestPart2Solution()
        {
            AOC_Day21 test = new AOC_Day21("day21data.txt");
            Console.WriteLine(test.Part2Answer());
        }
    }

    /// <summary>
    /// Advent Of Code 2020 Day 21
    /// This data structure parses and manages input data in terms of 
    /// 1. keeping the unique token (term) of ingredients' frequencies in the input.
    /// 2. Keeping the listed allergens and its link to possible ingredients.
    /// 3. Providing interface for part1 and part2 solutions of AOC puzzle.
    /// </summary>
    internal class AOC_Day21
    {
        private Dictionary<string, int> termFreq = new Dictionary<string, int>();
        private Dictionary<string, Allergen> allergens = new Dictionary<string, Allergen>();
        private AllergenTracker tracker = new AllergenTracker();

        /// <summary>
        /// Method LoadIngredients() parses the input data into the designed data structures
        /// 1. ingredients (in foreign languages) are recorded with their term frequencies.
        /// 2. listed allergens (in English) are kept separately in Allergen data structure (subscriber)
        ///     and registered with AllergenTracker (publisher)
        /// 3. each allergen's potential links are updated automatically given more informations with its link
        ///     to new potential list of ingredients (performed separately in Allergen class).
        /// </summary>
        /// <param name="data"></param>
        private void LoadIngredients(string[] data)
        {
            foreach(string line in data)
            {
                string[] parts = line.Split("(contains"); // separates ingredients and allergens.

                // process ingredients.
                string[] ingredients = parts[0].Split(" ", StringSplitOptions.RemoveEmptyEntries);
                foreach(var i in ingredients)
                {
                    if (termFreq.ContainsKey(i)) termFreq[i]++;
                    else termFreq.Add(i, 1);
                }

                // process allergens.
                string[] allergens = parts[1].Split(" ", StringSplitOptions.RemoveEmptyEntries);
                foreach(var a in allergens)
                {
                    var temp = a.Trim(',').Trim(')');
                    if(this.allergens.ContainsKey(temp))
                    {
                        // Update existing allergen.
                        this.allergens[temp].Update(ingredients);
                    }
                    else
                    {
                        // Register new allergen.
                        this.allergens[temp] = new Allergen(temp, ingredients, tracker);
                    }
                }
            }
        }

        /******************************************************************************************
         * Constructors.
         */
        public AOC_Day21(string fileName)
        {
            string[] lines = File.ReadAllLines(fileName);
            LoadIngredients(lines);
        }

        public AOC_Day21(string[] lines)
        {
            LoadIngredients(lines);
        }

        /******************************************************************************************
         * Part 1 and Part 2 Answers
         */
        public int Part1Answer()
        {
            int answer = 0;
            // iterate all terms.
            foreach(var i in termFreq)
            {
                // if current term(i) isn't a known allergen, add its term frequency to answer.
                if(!tracker.IsKnownAllergen(i.Key))
                {
                    answer += i.Value;
                }
            }
            return answer;
        }

        public string Part2Answer()
        {
            StringBuilder answer = new StringBuilder();

            // Convert allergen dictionary to list then sort them by their names.
            var list = this.allergens.Values.ToList();
            list.Sort();

            // Compile allergens' direct link to the foreign ingredient.
            foreach(var i in list)
            {
                answer.Append(i.Ingredient);
                if (i != list.Last()) answer.Append(",");
            }
            return answer.ToString();
        }
    }

    /**********************************************************************************************
     * Publisher/Subscriber Interfaces (Oberserver Pattern)
     */

    /// <summary>
    /// Interface Allergen Publisher provides two services
    /// 1. Register/Deregister subscribers
    /// 2. Provides information to update and remove potential links to ingredients that are ruled out.
    ///     a. This publisher automatically calls subscriber interface Update() and Remove() - Active Publishing.
    ///     b. Provides filtered ingredients for an array of input ingredients - Passive Publishing.
    /// </summary>
    interface IAllergenPublisher
    {
        public int RegisterAllergen(IAllergenSubscriber item);
        public void Deregister(IAllergenSubscriber item);

        public string[] FilterIngredients(string[] input);
    }

    /// <summary>
    /// Interface Allergen Subscriber provides three access points to the publisher as described below.
    /// </summary>
    interface IAllergenSubscriber
    {
        // Main point of access. Updates (deduction process) links to potential connections.
        public int Update(string[] input);

        // Extra helper, can be implemented as update() method, but having Remove() implemented
        // improved readability.
        public bool Remove(string input);

        // Provides the string description of the allergen.
        public string ToString();
    }

    /// <summary>
    /// Publisher class : its main functionality is to provide known allergen links to ingredients to Allergen class
    /// in the following ways.
    /// 
    /// 1. Keeps track of known allergens and their identified links to the ingredient (in foreign language)
    /// 2. Updates all allergens when an ingredient was identified as definitive link to a specific allergen.
    ///     (where each allergen should rid of the identified ingredient)
    /// 3. Filters a list of ingredients through known allergens (removing any ingredients with definitive links
    ///     to certain allergens)
    /// 4. Register/deregister subsribers (allergens).
    /// </summary>
    internal class AllergenTracker : IAllergenPublisher
    {
        // known ingredients with definitive links to allergens.
        private List<string> knownAllergens = new List<string>();
        // list of subsribers.
        private List<IAllergenSubscriber> allergens = new List<IAllergenSubscriber>();

        private Queue<IAllergenSubscriber> clearQueue = new Queue<IAllergenSubscriber>();

        public int RegisterAllergen(IAllergenSubscriber newAllergen)
        {
            EmptyClearQueue();
            allergens.Add(newAllergen);
            return allergens.Count;
        }

        public void Deregister(IAllergenSubscriber allergen)
        {
            clearQueue.Enqueue(allergen);
            UpdateKnownlists(allergen.ToString());
        }

        private void EmptyClearQueue()
        {
            while(clearQueue.Count > 0)
            {
                var item = clearQueue.Dequeue();
                allergens.Remove(item);
            }
        }

        /// <summary>
        /// Removes already identified ingredients with definitive allergen connections.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string[] FilterIngredients(string[] input)
        {
            var temp = input.ToList();
            foreach(var i in knownAllergens)
            {
                temp.Remove(i);
            }
            return temp.ToArray();
        }

        /// <summary>
        /// Update the knwon ingredients with definitive allergen list.
        /// Calls subsribers to remove this ingredient.
        /// </summary>
        /// <param name="identified"></param>
        private void UpdateKnownlists(string identified)
        {
            knownAllergens.Add(identified);
            foreach (var a in allergens)
            {
                a.Remove(identified);
            }
        }

        /// <summary>
        /// Cross refereces whether the input in on the list of identified allergen ingredients.
        /// </summary>
        /// <param name="ingredient"></param>
        /// <returns></returns>
        public bool IsKnownAllergen(string ingredient)
        {
            if (knownAllergens.Contains(ingredient))
            {
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Type Allergen (subscriber)
    /// 
    /// This class encapsulates an allergen in the following ways.
    /// 1. its allergy name/type, such as dairy, fish, and so on.
    /// 2. possible links to a number of ingredients in foreign languages.
    /// 3. performs intersect operation with new possible links with existing one minus identifed links.
    /// 4. report back to publisher when it only contains one possible ingredient link (deregisteration).
    /// </summary>
    internal class Allergen : IComparable<Allergen>, IAllergenSubscriber
    {
        public string Name { get; init; }

        private readonly AllergenTracker tracker;

        private string[] possibles;

        private bool isIdentified = false;

        public bool IsIdentified { get { return isIdentified; } }

        public string Ingredient
        {
            get
            {
                if(isIdentified) return possibles[0];
                return "unidentified";
            }
        }

        public override string ToString()
        {
            return Ingredient;
        }

        /// <summary>
        /// For sorting purposes by Name field.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Allergen other)
        {
            if (other == null) return 1;
            return Name.CompareTo(other.Name);
        }

        /******************************************************************************************
         * Constructor
         */
    public Allergen(string name, string[] potentialIngredients, AllergenTracker tracker)
        {
            this.Name = name;
            possibles = potentialIngredients;
            this.tracker = tracker;
            tracker.RegisterAllergen(this);
        }

        /// <summary>
        /// Method Update() performs intersect operation with new set of links with previous one.
        /// deducing from available informaion to get to the final link. 
        /// </summary>
        /// <param name="newPotentials"></param>
        /// <returns></returns>
        public int Update(string[] newPotentials)
        {
            if (isIdentified) return 1;

            /// 1. Obtain filtered ingredient list from publisher 
            /// (this is not a publisher/subscriber convention)
            var filtered = tracker.FilterIngredients(newPotentials);
            var intersect = possibles.Intersect(filtered).ToArray();
            this.possibles = intersect;

            if (intersect.Length == 1)
            {
                /// 2. Report back to the publisher that it has identified ingredient
                /// and it wishes to be deregistered from the subscriber list.
                isIdentified = true;
                tracker.Deregister(this);
            }

            return this.possibles.Length;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ingredient"></param>
        /// <returns></returns>
        public bool Remove(string ingredient)
        {
            List<string> temp = possibles.ToList();
            if(!isIdentified && temp.Contains(ingredient))
            {
                temp.Remove(ingredient);
                Update(temp.ToArray());
                return true;
            }
            return false;
        }
    }
}
