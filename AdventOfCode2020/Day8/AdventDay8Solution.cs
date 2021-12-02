using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode2020
{
    /// <summary>
    /// Type AdventDay8
    /// This is the unit testing entry.
    /// Various Solution methods are implmented to interact with the unit to be tested.
    /// This class also modifies the training data into proper data structure.
    /// </summary>
    internal class AdventDay8
    {
        // fields
        string[] data;
        // constructor
        public AdventDay8(string[] data)
        {
            this.data = data;
        }
        // methods.
        public int FindSolution()
        {
            var gameBoy = new Processor();
            gameBoy.LoadInstructions(data);
            return gameBoy.Run();
        }

        public int FindSolution2()
        {
            var gameBoy = new Processor();
            gameBoy.LoadInstructions(data);
            return gameBoy.Debug();
        }
    }

    /**************************************************************************
     * Implement classes on separate files once test is successfully tested.
     * Author: Yong Sung Lee.
     * Date: 2021-11-28.
     **************************************************************************/

    /// <summary>
    /// Custom Assembly Keywords.
    /// </summary>
    public enum AsmKey
    {
        // instruction set
        nop = 1,
        acc = 2,
        jmp = 4,
        
        mask = 5 // mask is used to flip the command "nop" and "jmp".
    };

    /// <summary>
    /// Type Instruction
    /// holds a single line of instruction with a flag to represent whether instruction
    /// has already processed.
    /// </summary>
    public class Instruction
    {
        // Fields
        AsmKey key = AsmKey.nop;    // command.
        int arg = 0;                // argument.
        bool processed = false;     // processed flag.

        // Constructor
        public Instruction(AsmKey inst, int arg)
        {
            Load(inst, arg);
        }

        /// <summary>
        /// Load Instruction and Argument into memory.
        /// </summary>
        /// <param name="inst">instruction or command</param>
        /// <param name="arg">argument</param>
        /// <returns></returns>
        public Instruction Load(AsmKey inst, int arg)
        {
            key = inst;
            this.arg = arg;
            return this;
        }

        /// <summary>
        /// Retrive method returns tuple of (command, argument).
        /// </summary>
        /// <returns></returns>
        public (AsmKey, int) Retrieve(bool setProcessedFlag = true)
        {
            if(setProcessedFlag) processed = true;
            return (key, arg);
        }

        /// <summary>
        /// FlagProcessed method checks whether instruction has already been processed.
        /// </summary>
        /// <returns></returns>
        public bool FlagProcessed()
        {
            return processed;
        }

        /// <summary>
        /// Flip method modifies the code, namely switching "jmp" and "nop" commands
        /// </summary>
        public void Flip()
        {
            key = key ^ AsmKey.mask;
        }

        /// <summary>
        /// Reset processed flag to false.
        /// </summary>
        public void ResetFlag()
        {
            processed = false;
        }
    }

    /// <summary>
    /// Extionsion methods for List of Instruction
    /// </summary>
    public static class InstructionExtensions
    {
        /// <summary>
        /// Load Instruction from String Data.
        /// </summary>
        /// <param name="input">Self List</param>
        /// <param name="line">Instruction in String Form</param>
        /// <returns></returns>
        public static bool LoadFromString(this List<Instruction> input, string line)
        {
            try
            {
                line = line.Replace("+", ""); // "+" is unnecessary.
                string[] command = line.Split(' '); // split command and argument.
                Enum.TryParse(command[0], out AsmKey asmKey); // parse command string format to enum type.
                int number = Convert.ToInt32(command[1]); // parse argument to integer.
                input.Add(new Instruction(asmKey, number)); // Load the instruction into memory.
                return true;
            }
            catch
            {
                // invalid instruction format.
                return false;
            }
        }

        /// <summary>
        /// Reset processed flag for all instructions.
        /// </summary>
        /// <param name="input">Self List</param>
        public static void ResetFlags(this List<Instruction> input)
        {
            foreach(Instruction i in input)
            {
                i.ResetFlag();
            }
        }
    }

    /// <summary>
    /// Type Processor
    /// Loads a set of instructions and process them.
    /// Contains registor and methods to execute and debug.
    /// </summary>
    public class Processor
    {
        // Registors.
        int acc = 0;

        // Memory (implemented as heap by .NET).
        List<Instruction> data = new List<Instruction>();

        /// <summary>
        /// LoadInstructions method loads textual instruction format into
        /// the {enum, int} format.
        /// </summary>
        /// <param name="textLines">instructions in string format</param>
        /// <returns></returns>
        public bool LoadInstructions(string[] textLines)
        {
            foreach(string line in textLines)
            {
                // Check whether an invalid instruction is found.
                if(!data.LoadFromString(line))
                {
                    return false;
                }
            }
            return true; // All okay!

            // For Debug Purpose. Disregard the following code.
            //StringBuilder log = new StringBuilder();
            //foreach(Instruction i in data)
            //{
            //    log.Append($"{i.Retrieve(false)}\n");
            //}
            //File.WriteAllText("log.txt", log.ToString());
            //return true;
        }

        /// <summary>
        /// Run Method executes the instruction set.
        /// </summary>
        /// <param name="debug">if true, Return negative value of last run instruction pointer
        /// otherwise, return register "acc" value.</param>
        /// <param name="AllowRepetition">Allow infinite loop or not</param>
        /// <returns></returns>
        public int Run(bool debug = false, bool AllowRepetition = false)
        {
            int insPtr = 0; // Instruction Pointer.
            AsmKey key; // instruction.
            int arg = 0; // argument.
            // execute all instructions.
            while(insPtr < data.Count)
            {
                // If infinite loop found
                if (!AllowRepetition && data[insPtr].FlagProcessed())
                {
                    // Debugging mode returns the instruction pointer in negative value.
                    if (debug) return (insPtr * -1);
                    else return acc; // Nondebuggin mode returns the register "acc" value.
                }
                (key, arg) = data[insPtr].Retrieve(); // retrieve instruction and argument pair.
              
                switch(key) // Process instruction.
                {
                    case AsmKey.nop:
                                    insPtr++;
                                    break;
                    case AsmKey.acc: 
                                    acc += arg;
                                    insPtr++;
                                    break;
                    case AsmKey.jmp: 
                                    insPtr += arg;
                                    break;
                    default:
                                    insPtr++;
                                    break;
                }
            }
            return acc; // if all okay, return the register "acc" value.
        }

        /// <summary>
        /// Debugging method finds problems with the code resulting infinite loop and 
        /// trying to fix the issue by switching "jmp" and "nop" commands.
        /// </summary>
        /// <returns></returns>
        public int Debug()
        {
            int result = Run(true); // execute the instructions in Debuggin Mode.
            if (result > 0) return result; // if no issue is found, return the acc value.

            // if problem is found, engage in code fixes.
            int pos = 0; // start from the very beginning for modification.
            do
            {
                Modify(pos); // switch commands "jmp" and "nop"

                // reset register and flags.
                acc = 0;
                data.ResetFlags();

                result = Run(true); // Run in debugging mode.
                Modify(pos++); // Undo modification.
            } while (result <= 0 && pos < data.Count); // until problem is fixed or reached to the end of program.
            return result; // return result value.
        }

        // Switch instruction at position, pos.
        public void Modify(int pos)
        {
            data[pos].Flip();
        }
    }
}