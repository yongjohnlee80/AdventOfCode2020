using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2020.Day17
{
    internal class AdventDay17Solution
    {
    }

    /// <summary>
    /// API: PocketDimensionAPI
    /// Provides essential interface methods to work in the super-secret Pocket Universe.
    /// Godspeed on your journey between the alternate universe and reality.
    /// 
    /// The rest of the classes employ the following APIs for basic interactions; thus, 
    /// optimizing these APIs will also improve the overall performance.
    /// </summary>
    internal static class PocketDimensionAPI
    {
        // Top Secret Deciphering Pattern for PockID
        private readonly static Regex PockIDRegex = new Regex(@"(-\d+):(-\d+):(-\d+):(-\d+)");

        /// <summary>
        /// Method: CraftPocketID
        /// Creates a string representation of pocket universe coordinates
        /// Mostly used for hashing but also useful for record purposes, eg. on DISKs.
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        internal static string CraftPockID(PocketCoordinate coordinate)
        {
            return $"{coordinate.X}:{coordinate.Y}:{coordinate.Z}:{coordinate.W}";
        }

        internal static string CraftPockID(int x, int y, int z = 0, int w = 0)
        {
            return $"{x}:{y}:{z}:{w}";
        }

        /// <summary>
        /// Method: DecipherCoordinates
        /// Although neverused in this program, I made the method to reverse the process of
        /// CraftPocktID method so that we can convert the string representation back to the 
        /// actual coordinates as a vector.
        /// </summary>
        /// <param name="pockID"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        internal static PocketCoordinate DecipherCoordinate(string pockID)
        {
            var match = PockIDRegex.Match(pockID);
            if (!match.Success) throw new Exception("*** Pocket Dimension Corruptted *** Evacuate Back To Reality!");
            return new PocketCoordinate(Int32.Parse(match.Groups[1].Value),
                                        Int32.Parse(match.Groups[2].Value),
                                        Int32.Parse(match.Groups[3].Value),
                                        Int32.Parse(match.Groups[4].Value) );
        }

        /// <summary>
        /// Method: GetTwoSixCoordinates
        /// This method returns an array of nearby coordinates of a location in the pocket universe,
        /// which contains 26 coordinates.
        /// This method employs three dimentional approach to generate coordinates.
        /// </summary>
        /// <param name="center"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        internal static PocketCoordinate[] GetTwoSixCoordinates(PocketCoordinate center)
        {
            var box26 = new List<PocketCoordinate>();
            for(int z = center.Z-1; z <= center.Z+1; z++)
                for(int y = center.Y-1; y <= center.Y+1; y++)
                    for(int x = center.X-1; x <= center.X+1; x++)
                    {
                        if (!(z == center.Z && y == center.Y && x == center.X))
                            box26.Add(new PocketCoordinate(x, y, z));
                    }

            if (box26.Count != 26) throw new Exception($"26 ERORRS! {box26.Count}");
            return box26.ToArray();
        }

        /// <summary>
        /// Method: GetEightyCoordinates
        /// This method returns an array of nearby coordinates of a location in the pocket universe,
        /// which contains 80 coordinates.
        /// This method employs hyperspace approach (four dimentional) to generate coordinates.
        /// </summary>
        /// <param name="center"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        internal static PocketCoordinate[] GetEightyCoordinates(PocketCoordinate center)
        {
            var box80 = new List<PocketCoordinate>();
            for(int w = center.W-1; w <= center.W+1; w++)
                for (int z = center.Z - 1; z <= center.Z + 1; z++)
                    for (int y = center.Y - 1; y <= center.Y + 1; y++)
                        for (int x = center.X - 1; x <= center.X + 1; x++)
                        {
                            if (!(w == center.W && z == center.Z && y == center.Y && x == center.X))
                                box80.Add(new PocketCoordinate(x, y, z, w));
                        }

            if (box80.Count != 80) throw new Exception($"80 ERORRS! {box80.Count}");
            return box80.ToArray();
        }
    }

    /// <summary>
    /// Type: PocketCoordinate
    /// Coordinates in Pocket Dimension!
    /// Not a ordinate coordinate that we can easily have access to...
    /// Thus, You MUST USE this class to locate and active/deactivate
    /// Secret Conway CUBES in Pocket Dimension..
    /// Good luck!
    /// 
    /// The PART II threw me off at first, since I didn't expect to add another dimention to the 
    /// Pocket Universe. This was a new approach of making the puzzle more complicated.
    /// After some thought, I realized that my exising modules can easily accomodate another dimention by
    /// modifying only a couple of data structure to expand a feature of 4D without breaking the compatibility
    /// with the old use-cases, which was 3D approach.
    /// 
    /// </summary>
    internal class PocketCoordinate
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public int W { get; set; } // Part 2 Addition.

        public string PockID { get; }

        public PocketCoordinate(int x, int y, int z, int w = 0)
        {
            X = x; 
            Y = y; 
            Z = z;
            W = w;

            PockID = PocketDimensionAPI.CraftPockID(x, y, z, w);
        }
    }

    /// <summary>
    /// Type: ConwayCube
    /// The founding energy source for experimental super-secret imaging satelites of Elven
    /// Technologoy. It's very complicated.
    /// </summary>
    internal class ConwayCube : IComparable<ConwayCube>
    {
        protected PocketCoordinate position;
        public string PockID { get; set;}
        public bool UseHyperSpace { get; set; } // Part II Addition.
        public PocketCoordinate Position { get { return position; } }

        public ConwayCube(int x, int y, int z, int w = 0, bool useHyperSpace = false)
        {
            position = new PocketCoordinate(x, y, z, w);
            PockID = PocketDimensionAPI.CraftPockID(position);
            UseHyperSpace = useHyperSpace;
        }

        public ConwayCube(PocketCoordinate position, bool useHyperSpace = false)
        {
            this.position = position;
            PockID = PocketDimensionAPI.CraftPockID(position);
            UseHyperSpace=useHyperSpace;
        }

        public PocketCoordinate[] RetrieveNearbyCoordinates()
        {
            if (UseHyperSpace) return PocketDimensionAPI.GetEightyCoordinates(Position);
            else return PocketDimensionAPI.GetTwoSixCoordinates(position);
        }

        public int CompareTo(ConwayCube? other)
        {
            if (UseHyperSpace) return this.Position.W.CompareTo(other.Position.W);
            else return this.Position.Z.CompareTo(other.Position.Z);
        }
    }

    /// <summary>
    /// Type: PocketUniverse
    /// The general class for interacting with PocketUniverse and its entities.
    /// 
    /// </summary>
    internal class PocketUniverse
    {
        protected Dictionary<string, ConwayCube> activeCubes = new Dictionary<string, ConwayCube>();

        public bool UseHyperSpace { get; set; }

        protected LogStat log = new LogStat();

        public PocketUniverse(bool useHyperSpace = false)
        {
            UseHyperSpace = useHyperSpace;
        }

        protected int HowManyActiveCubesNearBy(PocketCoordinate center)
        {
            int result = 0;
            var temp = new ConwayCube(center, UseHyperSpace);
            foreach (var i in temp.RetrieveNearbyCoordinates())
            {
                if (activeCubes.ContainsKey(i.PockID))
                {
                    result++;
                }
            }
            return result;
        }

        protected bool IsActiveCube(PocketCoordinate position)
        {
            return activeCubes.ContainsKey(position.PockID);
        }

        public void InitCubes(string fileName)
        {
            var fname = Path.Combine(System.Environment.CurrentDirectory, fileName);
            var lines = File.ReadAllLines(fname);
            InitCubes(lines);
        }

        public void InitCubes(string[] mapInTextLines)
        {
            int col = 0;
            foreach(string line in mapInTextLines)
            {
                var temp = line.ToCharArray();
                for (int j = 0; j < mapInTextLines.Length; j++)
                {
                    if(temp[j] == '#')
                    {
                        activeCubes.Add(PocketDimensionAPI.CraftPockID(j, col), new ConwayCube(j, col, 0, 0, UseHyperSpace));
                    }
                }
                col++;
            }
        }

        public int RunCycle()
        {
            HashSet<PocketCoordinate> grids = new HashSet<PocketCoordinate>();
            Dictionary<string, ConwayCube> newActives = new Dictionary<string, ConwayCube>();

            BuildPotentials();

            foreach(var location in grids)
            {
                /// Save your breath if already in the new list.
                if (!newActives.ContainsKey(location.PockID))
                {
                    int nearActive = HowManyActiveCubesNearBy(location);
                                        // get the number of active cubes nearby

                    /// The rules for an already active cube
                    /// Stay active if there are 2 or 3 active cubes nearby
                    /// Otherwise, becomes inactive.....
                    if (IsActiveCube(location)) {
                        if (nearActive == 2 || nearActive == 3)
                        {
                            newActives.Add(location.PockID, new ConwayCube(location, UseHyperSpace));
                        }
                    }
                    /// The rules for an inactive cube
                    /// If there are 3 active cubes nearby active yourself.
                    else
                    {
                        if( nearActive == 3 )
                        {
                            newActives.Add(location.PockID, new ConwayCube(location, UseHyperSpace));
                        }
                    }
                }
            }

            activeCubes = newActives;
            return activeCubes.Count;

            /// Compile Potential Grids of Active Cubes after this cycle.
            /// Potentially improved in the future in terms of selection
            /// depending on the data generated after some tests.
            /// The following selection simply employs "no stones left unturned"
            /// for the time being.
            void BuildPotentials()
            {
                foreach (var cube in activeCubes)
                {
                    grids.Add(cube.Value.Position);
                    foreach (var extra in cube.Value.RetrieveNearbyCoordinates())
                    {
                        grids.Add(extra);
                    }
                }
            }
        }
        
        /// <summary>
        /// Method: FindMinMaxCoordinates
        /// Finds the minimum and maximum ranges of coordinates of active cubes in the 
        /// infinite pocket universe.
        /// 
        /// This method is only used by the logging methods that follows but I categorized it outside
        /// of its only usecase since this method can be useful.
        /// </summary>
        /// <returns></returns>
        public (PocketCoordinate, PocketCoordinate) FindMinMaxCoordinates()
        {
            PocketCoordinate min = new PocketCoordinate(0, 0, 0);
            PocketCoordinate max = new PocketCoordinate(0, 0, 0);

            foreach(var cube in activeCubes)
            {
                if (cube.Value.Position.X < min.X) min.X = cube.Value.Position.X;
                if (cube.Value.Position.Y < min.Y) min.Y = cube.Value.Position.Y;
                if (cube.Value.Position.Z < min.Z) min.Z = cube.Value.Position.Z;
                if (cube.Value.Position.W < min.W) min.W = cube.Value.Position.W;


                if (cube.Value.Position.X > max.X) max.X = cube.Value.Position.X;
                if (cube.Value.Position.Y > max.Y) max.Y = cube.Value.Position.Y;
                if (cube.Value.Position.Z > max.Z) max.Z = cube.Value.Position.Z;
                if (cube.Value.Position.W > max.W) max.W = cube.Value.Position.W;

            }
            return (min, max);
        }

        /// <summary>
        /// The Following Methods are various debug tools that you can use to log active cubes or 
        /// log four dimentional mappings the same way as the Advent of Code 2020, so that you can compare
        /// your mappings with the samples provided by the ADvent of Code.
        /// </summary>
        /// <param name="title"></param>
        public void LogActiveCubes(string title)
        {
            log.Append($"Active Cubes {title}\n");

            foreach(var cube in activeCubes)
            {
                log.Append($"{cube.Value.PockID}\n");
            }

            log.Append($"End of Active Cubes {title}\n\n");
        }

        public void LogPocketMap(string title)
        {
            log.Append($"Pocket Universe Grid Map {title}\n");

            PocketCoordinate min, max;
            (min, max) = FindMinMaxCoordinates();

            for(var w = min.W; w <= max.W; w++)
            {
                for (var z = min.Z; z <= max.Z; z++)
                {
                    log.Append($"\nZ plane: {z}, W plane {w}\n");
                    for (var y = min.Y; y <= max.Y; y++)
                    {
                        for (var x = min.X; x <= max.X; x++)
                        {
                            if (IsActiveCube(new PocketCoordinate(x, y, z)))
                            {
                                log.Append("#");
                            }
                            else
                            {
                                log.Append(".");
                            }
                        }
                        log.Append("\n");
                    }
                }
            }

            log.Append("\n");
        }

        public void WriteLog(string fileName)
        {
            log.LogOnFile(fileName);
        }
    }
}
