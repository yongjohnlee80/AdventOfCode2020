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
        public void TestPart1Sample()
        {
            string[] dataLines = @"
mxmxvkd kfcds sqjhc nhms (contains dairy, fish)
trh fvjkl sbzzf mxmxvkd (contains dairy)
sqjhc fvjkl (contains soy)
sqjhc mxmxvkd sbzzf (contains fish)".Split("\r\n",StringSplitOptions.RemoveEmptyEntries);

            AOC_Day21 test = new AOC_Day21(dataLines);
            Assert.That(test.Part1Answer(), Is.EqualTo(5));
        }
    }
    internal class AOC_Day21
    {
        private Dictionary<string, int> termFreq = new Dictionary<string, int>();
        private Dictionary<string, Allergen> allergens = new Dictionary<string, Allergen>();
        private AllergenTracker tracker = new AllergenTracker();

        private void LoadIngredients(string[] data)
        {
            foreach(string line in data)
            {
                string[] parts = line.Split("(contains");

                string[] ingredients = parts[0].Split(" ", StringSplitOptions.RemoveEmptyEntries);
                foreach(var i in ingredients)
                {
                    if (termFreq.ContainsKey(i)) termFreq[i]++;
                    else termFreq.Add(i, 1);
                }

                string[] allergens = parts[1].Split(" ", StringSplitOptions.RemoveEmptyEntries);
                foreach(var a in allergens)
                {
                    var temp = a.Trim(',').Trim(')');
                    if(this.allergens.ContainsKey(temp))
                    {
                        this.allergens[temp].Update(ingredients);
                    }
                    else
                    {
                        this.allergens[temp] = new Allergen(temp, ingredients, tracker);
                    }
                }
            }

            LogStat log = new LogStat();
            log.Append("List of Ingredients\n");
            foreach(var i in termFreq)
            {
                log.Append($"{i.Key} : {i.Value}\n");
            }
            log.Append("\nList of Allergens\n");
            foreach(var i in allergens)
            {
                log.Append($"{i.Key} : ");
                foreach(var j in i.Value.Possibles)
                {
                    log.Append($"{j} ");
                }
                log.Append("\n");
            }
            log.LogOnFile("day21log.txt");
        }

        public AOC_Day21(string fileName)
        {
            string[] lines = File.ReadAllLines(fileName);
            LoadIngredients(lines);
        }

        public AOC_Day21(string[] lines)
        {
            LoadIngredients(lines);
        }

        public int Part1Answer()
        {
            int answer = 0;
            foreach(var i in termFreq)
            {
                if(!tracker.IsKnownAllergen(i.Key))
                {
                    answer += i.Value;
                }
            }
            return answer;
        }
    }

    internal class AllergenTracker
    {
        private List<string> knownAllergens = new List<string>();
        private List<Allergen> allergens = new List<Allergen>();
        public string[] KnownAllergens { get { return knownAllergens.ToArray(); } }

        public int RegisterAllergen(Allergen newAllergen)
        {
            allergens.Add(newAllergen);
            return allergens.Count;
        }

        public string[] FilterIngredients(string[] input)
        {
            var temp = input.ToList();
            foreach(var i in knownAllergens)
            {
                temp.Remove(i);
            }
            return temp.ToArray();
        }

        public void UpdateKnownlists(string identified)
        {
            knownAllergens.Add(identified);
            foreach (var a in allergens)
            {
                a.Remove(identified);
            }
        }

        public bool IsKnownAllergen(string ingredient)
        {
            if (knownAllergens.Contains(ingredient))
            {
                return true;
            }
            return false;
        }
    }

    internal class Allergen
    {
        public string Name { get; init; }

        private AllergenTracker tracker;

        private string[] possibles;
        public string[] Possibles { get { return possibles; } }

        private bool isIdentified = false;

        public string Ingredient
        {
            get
            {
                if(isIdentified) return possibles[0];
                return "unidentified";
            }
        }

        public Allergen(string name, string[] potentialIngredients, AllergenTracker tracker)
        {
            this.Name = name;
            possibles = potentialIngredients;
            this.tracker = tracker;
            tracker.RegisterAllergen(this);
        }

        public int Update(string[] newPotentials)
        {
            if (isIdentified) return 1;

            var filtered = tracker.FilterIngredients(newPotentials);
            var intersect = possibles.Intersect(filtered).ToArray();
            if(intersect.Length == 1)
            {
                isIdentified = true;
                tracker.UpdateKnownlists(intersect[0]);
            }

            this.possibles = intersect;
            return this.possibles.Length;
        }

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
