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
     * Date: 2021-12-07
     **************************************************************************/

    internal class AngleDegrees
    {
        protected int angle;

        public int Angle { 
            get { return angle; }
            set { angle = value; }
        }

        public double Rad { get { return (double)(Math.PI / 180 * angle);  } }

        public AngleDegrees(int initialAngle = 0)
        {
            angle = initialAngle;
        }

        public int Rotate(int deltaDegrees)
        {
            this.angle += deltaDegrees;
            if (angle >= 360) angle -= 360;
            if (angle < 0) angle += 360;
            return angle;
        }
    }

    internal class VectorInt2
    {
        public VectorInt2(int initialX = 0, int initialY = 0)
        {
            X = initialX;
            Y = initialY;
        }
        public int X { get; set; }
        public int Y { get; set; }
        public int ManhattanDistance()
        {
            return Math.Abs(X) + Math.Abs(Y);
        }
        public VectorInt2 Move(AngleDegrees angle, int howFar)
        {
            int xchange = (int)Math.Cos(angle.Rad) * howFar;
            int ychange = (int)Math.Sin(angle.Rad) * howFar;
            X += xchange;
            Y += ychange;
    
            return this;
        }
    }

    internal abstract class FerryInstructions
    {
        protected string[] instructions = new string[0];
        public void LoadInstructions(string[] instructions) { this.instructions = instructions; }
        public abstract void RunInstructions(AngleDegrees ferryAngle, VectorInt2 ferryPosition);
    }

    internal class JustMoveIt : FerryInstructions
    {
        public override void RunInstructions(AngleDegrees ferryAngle, VectorInt2 ferryPosition)
        {
            AngleDegrees temp = new AngleDegrees(0);
            foreach (var command in this.instructions)
            {
                temp.Angle = command[0] switch
                {
                    'N' => 90,
                    'S' => 270,
                    'W' => 180,
                    'E' => 0,
                    _ => ferryAngle.Angle
                };
                if (command[0] == 'L' || command[0] == 'R')
                {
                    var value = Int32.Parse(command.Replace("R", "-").Replace("L", ""));
                    ferryAngle.Rotate(value);
                }
                else
                {
                    var value = Int32.Parse(command.Remove(0, 1));
                    ferryPosition.Move(temp, value);
                }
            }
        }
    }

    internal class UseWaypoint : FerryInstructions
    {
        protected VectorInt2 wayPosition;

        public UseWaypoint(int x = 10, int y = 1)
        {
            wayPosition = new VectorInt2(x, y);
        }

        public override void RunInstructions(AngleDegrees ferryAngle, VectorInt2 ferryPosition)
        {
            AngleDegrees temp = new AngleDegrees(0);
            foreach (var command in this.instructions)
            {
                if(command[0] == 'F')
                {
                    var value = Int32.Parse(command.Remove(0, 1));
                    ferryPosition.X += (wayPosition.X * value);
                    ferryPosition.Y += (wayPosition.Y * value);
                }
                else if(command[0] == 'R' || command[0] == 'L')
                {
                    var value = new AngleDegrees(0);
                    value.Rotate(Int32.Parse(command.Replace("R", "-").Replace("L", "")));
                    int tempX;

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
                else
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
    
    internal class Ferry
    {
        // fields
        VectorInt2 position;
        AngleDegrees angle;
        string[] commands;

        public Ferry(string[] commands)
        {
            position = new VectorInt2();
            angle = new AngleDegrees(0);
            this.commands = commands;
        }

        public int RunCommands(FerryInstructions navigateMethod)
        {
            if (commands == null) return 0;
            navigateMethod.LoadInstructions(commands);
            navigateMethod.RunInstructions(angle, position);
            return position.ManhattanDistance();
        }
    }

}