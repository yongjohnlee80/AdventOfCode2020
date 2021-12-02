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
    internal class AdventDay4
    {
        // fields
        string[] data;
        // constructor
        public AdventDay4(string[] data)
        {
            this.data = data;
        }
        // methods.
        public int FindSolution(string[] requiredFields, bool checkConstraints = false)
        {
            var documents = new List<TravelDocument>();
            documents.LoadDocuments(data);
            return documents.CountValidDocuments(requiredFields, checkConstraints);
        }
    }

    /**************************************************************************
     * Implement classes on separate files once test is completed.
     * Author: Yong Sung Lee.
     * Date: 2021-11-24.
     **************************************************************************/

    // Type Function Pointer for Custom Constraints for TravelDocument Fields.
    public delegate bool FieldConstraints(string field);

    /**************************************************************************
     * Type Travel Document.
     * Stores Document Attributes and Values.
     * Provides Methods for Checking Constraints.
     * Class accomodates custom attrubutes and constraints rules.
     **************************************************************************/
    public class TravelDocument
    {
        // fields
        public Dictionary<string, string> fields = new Dictionary<string, string>(); // input format "attribute:value"
        FieldConstraints? rule; // Field Constraint Checker (function pointer).

        /*******************************************************************************************
         * Validates each document if acceptable.
         * requredFields contains all required fields/attributes a document should contain,
         * constraints {true: check field values against constraints,
         *              false: default value, no checks on field values.
         * customRule is a function pointer(or delegate) for custom constraints implementation,
         *              leave it null to use in-class provided default constraints. 
         *              Refer to CheckConstraint method for detail.
         *******************************************************************************************/
        public bool Validate(string[] requiredFields, bool constraints = false, FieldConstraints? customRule = null)
        {
            rule = (customRule == null)? CheckConstraint : customRule; // check for custom rules (delegate)
            foreach (var field in requiredFields)
            {
                if (!fields.ContainsKey(field)) // check whether document contains required field,
                {
                    return false;
                } 
                else if (constraints) // check field value with constraints.
                {
                    if (!rule(field)) return false;
                }
            }
            return true; // if no issue found.
        }

        /**************************************************************************
         * Default constraint checker provided by the class.
         * Implement your own for custom constraints.
         **************************************************************************/
        public bool CheckConstraint(string field)
        {
            try
            {
                switch (field)
                {
                    case "byr": // 1920 <= Birth Year <= 2002
                        int byr = int.Parse(fields[field]);
                        if(byr >= 1920 && byr <= 2002) return true;
                        break;
                    case "iyr": // 2010 <= Issue Year <= 2020
                        int iyr = int.Parse(fields[field]);
                        if (iyr >= 2010 && iyr <= 2020) return true;
                        break;
                    case "eyr": // 2020 <= Expiration Year <= 2030
                        int eyr = int.Parse(fields[field]);
                        if (eyr >= 2020 && eyr <= 2030) return true;
                        break;
                    case "hgt": // 150cm (59in) <= height <= 193cm (76in)
                        string[] temp = Regex.Split(fields[field], @"\D+");
                        int hgt = int.Parse(temp[0]);
                        if (fields[field].Contains("cm"))
                        {
                            if (hgt >= 150 && hgt <= 193) return true;
                        } else if (fields[field].Contains("in"))
                        {
                            if (hgt >= 59 && hgt <= 76) return true;
                        }
                        break;
                    case "hcl": // Hair color, must be 6 hexadecimals starting with '#'
                        if (Regex.IsMatch(fields[field], @"^#([a-fA-F0-9]{6})$")) return true;
                        break;
                    case "ecl": // Eye color, must be a member of set { "amb", "blu", "brn", "gry", "grn", "hzl", "oth" }
                        HashSet<string> colors = new HashSet<string>() { "amb", "blu", "brn", "gry", "grn", "hzl", "oth" };
                        if(colors.Contains(fields[field])) return true;
                        break;
                    case "pid": // Must be 9 digits.
                        if (Regex.IsMatch(fields[field], @"^([0-9]{9})$")) return true;
                        break;
                    default:
                        break;
                }
            } 
            catch (Exception ex) // Implement Logging for Input Errors....
            {
                Console.WriteLine($"Incorrect Document Form Fields: {ex.Message}");
            }
            return false;
        }
    }

    /**************************************************************************
     * Extension Methods for List<TravelDocument>
     **************************************************************************/
    public static class TravelDocumentExtensions
    {
        // Load text lines containing document information. Each document is separated by a empty new line.
        public static void LoadDocuments(this List<TravelDocument> input, string[] lines)
        {
            if (input.Count == 0) input.Add(new TravelDocument()); // add a document if none.
            // Process all lines.
            foreach(string line in lines)
            {
                if (string.IsNullOrEmpty(line)) input.Add(new TravelDocument()); // new document.
                else
                {
                    string[] fields = Regex.Split(line, @"\s+"); // tokenize each line.
                    foreach (string field in fields)
                    {
                        string[] parts = field.Split(':'); // split attribute with value.
                        input[input.Count - 1].fields.Add(parts[0], parts[1]); // store attribute and value.
                    }
                }
            }
        }
        // Count the number of valid traverl documents given constraints.
        public static int CountValidDocuments(this List<TravelDocument> input, string[] requiredFields, 
                                            bool constraints = false, FieldConstraints? customRule = null)
        {
            int count = 0;
            foreach (TravelDocument document in input)
            {
                if (document.Validate(requiredFields, constraints, customRule)) // validate each document.
                {
                    count++;
                }
            }
            return count;
        }
    }
}
