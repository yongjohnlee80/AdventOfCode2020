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
    internal class AdventDay12
    {
        Ferry ferry;

        /// <summary>
        /// Method: LoadCommandsFromFile
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>The total number of the seats.</returns>
        public void LoadCommandsFromFile(string fileName)
        {
            var fname = Path.Combine(System.Environment.CurrentDirectory, fileName);
            var lines = File.ReadAllLines(fname);
            LoadCommandsFromTextLines(lines);
        }

        /// <summary>
        /// Method: LoadCommandsFromTextLines
        /// </summary>
        /// <param name="textLines"></param>
        /// <returns>The total number of the seats.</returns>
        public void LoadCommandsFromTextLines(string[] textLines)
        {
            this.ferry = new Ferry(textLines);
        }

        /// <summary>
        /// First half of the solution.
        /// </summary>
        /// <returns>Manhattan distance of the ferry</returns>
        public int FindSolution()
        {
            return ferry.RunCommands(new JustMoveIt());
        }

        /// <summary>
        /// Second half of the solution.
        /// </summary>
        /// <returns>Manhattan distance of the ferry </returns>
        public long FindSolution2()
        {
            return ferry.RunCommands(new UseWaypoint());
        }
    }

    /**************************************************************************
     * Implement classes on separate files once test is successfully tested.
     * Author: Yong Sung Lee.
     * Date: 2021-12-12
     **************************************************************************/

    /// <summary>
    /// Type: AngleDegrees
    /// An angle representation as an integer in degrees.
    /// </summary>
    internal class AngleDegrees
    {
        protected int angle; // Angle value in degrees.

        // Getter and Setter.
        public int Angle { 
            get { return angle; }
            set { angle = value; }
        }

        /// <summary>
        /// Method: Rad
        /// Returns radian value of the angle as double.
        /// </summary>
        public double Rad { get { return (double)(Math.PI / 180 * angle);  } }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="initialAngle"></param>
        public AngleDegrees(int initialAngle = 0)
        {
            angle = initialAngle;
        }

        /// <summary>
        /// Method: Rotate
        /// Rotates the current angle with additional angle value.
        /// </summary>
        /// <param name="deltaDegrees"></param>
        /// <returns></returns>
        public int Rotate(int deltaDegrees)
        {
            this.angle += deltaDegrees;
            // Constraints: 0 <= angle < 360
            if (angle >= 360) angle -= 360;
            if (angle < 0) angle += 360;
            return angle;
        }
    }

    /// <summary>
    /// Type: VectorInt2
    /// 2D coordinate with two integers, namely X and Y.
    /// </summary>
    internal class VectorInt2
    {
        // Constructor and Getter & Setters
        public VectorInt2(int initialX = 0, int initialY = 0)
        {
            X = initialX;
            Y = initialY;
        }
        public int X { get; set; }
        public int Y { get; set; }

        /// <summary>
        /// Method: ManhattanDistance
        /// Returns the Manhattan Distance value of the Coordinates
        /// in relation to the orgin (initial position).
        /// </summary>
        /// <returns></returns>
        public int ManhattanDistance()
        {
            return Math.Abs(X) + Math.Abs(Y);
        }

        /// <summary>
        /// Method: Move
        /// Move the vector to new location with an angle and magnitude.
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="howFar"></param>
        /// <returns></returns>
        public VectorInt2 Move(AngleDegrees angle, int howFar)
        {
            int xchange = (int)Math.Cos(angle.Rad) * howFar;
            int ychange = (int)Math.Sin(angle.Rad) * howFar;
            X += xchange;
            Y += ychange;
    
            return this;
        }
    }

    /// <summary>
    /// Abstract: FerryInstructions
    /// Strategy Pattern (Composition)
    /// Subclasses must determine how the instructions are translated 
    /// in accordance to the rubric.
    /// </summary>
    internal abstract class FerryInstructions
    {
        protected string[] instructions = new string[0];
                            // collection of instructions - only the reference value.
        public void LoadInstructions(string[] instructions) { this.instructions = instructions; }
                            // Load instructions.
        public abstract void RunInstructions(AngleDegrees ferryAngle, VectorInt2 ferryPosition);
                            // Must implement in the subclasses.
    }

    /// <summary>
    /// Type: JustMoveIt
    /// Move the ferry according to the simple rules described below.
    /// </summary>
    internal class JustMoveIt : FerryInstructions
    {
        public override void RunInstructions(AngleDegrees ferryAngle, VectorInt2 ferryPosition)
        {
            AngleDegrees temp = new AngleDegrees(0);
            foreach (var command in this.instructions)
                            // iterate through all instructions.
            {
                temp.Angle = command[0] switch
                            // translate compass directions into absolute angle value in degrees.
                {
                    'N' => 90,
                    'S' => 270,
                    'W' => 180,
                    'E' => 0,
                    _ => ferryAngle.Angle
                };
                if (command[0] == 'L' || command[0] == 'R')
                            // rotate the ferry Left or Right with an angle
                {
                    var value = Int32.Parse(command.Replace("R", "-").Replace("L", ""));
                    ferryAngle.Rotate(value);
                }
                else
                {
                    var value = Int32.Parse(command.Remove(0, 1));
                    ferryPosition.Move(temp, value);
                            // Move the ferry with the angle, temp, and the magnitude, value.
                }
            }
        }
    }

    /// <summary>
    /// Type: UseWaypoint
    /// Let's use waypoint for moving the ferry.
    /// </summary>
    internal class UseWaypoint : FerryInstructions
    {
        /// !!!!!!!!! IMPORTANT !!!!!!!!!
        /// The waypoint coordinates are supposed to be ahead of the ferry
        /// But to make the computation simple, this coordinates are relative 
        /// to the origin.
        protected VectorInt2 wayPosition;

        public UseWaypoint(int x = 10, int y = 1)
        {
            wayPosition = new VectorInt2(x, y); // Set the waypoint location.
        }

        public override void RunInstructions(AngleDegrees ferryAngle, VectorInt2 ferryPosition)
        {
            AngleDegrees temp = new AngleDegrees(0);
            foreach (var command in this.instructions)
                            // Iterate through all instructions.
            {
                if(command[0] == 'F')
                            // First Case, Move Forward...
                {
                    var value = Int32.Parse(command.Remove(0, 1));
                    ferryPosition.X += (wayPosition.X * value);
                    ferryPosition.Y += (wayPosition.Y * value);
                }
                else if(command[0] == 'R' || command[0] == 'L')
                            // Second Case. Rotate the Waypoint in relation to the ferry
                            // Remember, the waypoint's coordinate is in relation to the origin.
                {
                    // Get the angle.
                    var value = new AngleDegrees(0);
                    value.Rotate(Int32.Parse(command.Replace("R", "-").Replace("L", "")));

                    int tempX = wayPosition.X; 
                                // the original X coordinate is required for the new Y coordinate

                    /// The following mathematical formula worked for the sample instructions...
                    /// but failed miserably for the data set.
                    /// Can't figure out why..... MUST RETURN and PONDER MORE....
                   
                    //double tan = Math.Atan((double)wayPosition.Y / (double)wayPosition.X);
                    //double cos = Math.Cos(tan + value.Rad);
                    //double sin = Math.Sin(tan + value.Rad);

                    //wayPosition.X = (int)(cos * tempX - sin * wayPosition.Y);
                    //wayPosition.Y = (int)(sin * tempX + cos * wayPosition.Y);

                    /// Hard code the possible scenarios... Only Three options, 90, 180, or 270 degree turn...
                    switch (value.Angle)
                    {
                        case 90:
                            tempX = wayPosition.X;
                            wayPosition.X = wayPosition.Y * -1;
                            wayPosition.Y = tempX;
                            break;
                        case 180:
                            wayPosition.X *= -1;
                            wayPosition.Y *= -1;
                            break;
                        case 270:
                            tempX = wayPosition.X;
                            wayPosition.X = wayPosition.Y;
                            wayPosition.Y = tempX * -1;
                            break;
                        default:
                            break;
                    }
                }
                else // The Third case, just move the waypoint using Compass direction scheme.
                {
                    temp.Angle = command[0] switch
                    {
                        'N' => 90,
                        'S' => 270,
                        'W' => 180,
                        'E' => 0,
                        _ => 0
                    };
                    var value = Int32.Parse(command.Remove(0, 1));
                    wayPosition.Move(temp, value);
                }
            }
        }
    }
    
    /// <summary>
    /// Type: Ferry
    /// The ferry in question that I'm on....
    /// </summary>
    internal class Ferry
    {
        // fields... clearly named as follows...
        VectorInt2 position;
        AngleDegrees angle;
        string[] commands;

        public Ferry(string[] commands)
        {
            // Initialization of the fields with default values.
            position = new VectorInt2();
            angle = new AngleDegrees(0);
            this.commands = commands;
        }

        public int RunCommands(FerryInstructions navigateMethod)
                        // Composition with Behavior Class.
        {
            if (commands == null) return 0;
            navigateMethod.LoadInstructions(commands);
            navigateMethod.RunInstructions(angle, position);
            return position.ManhattanDistance();
        }
    }

}