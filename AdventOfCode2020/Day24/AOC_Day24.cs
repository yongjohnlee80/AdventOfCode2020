using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NUnit.Framework;

namespace AdventOfCode2020.Day24
{
    internal class AOC_Day24
    {
        [Test]
        public void TestSample()
        {
            var sample = @"
sesenwnenenewseeswwswswwnenewsewsw
neeenesenwnwwswnenewnwwsewnenwseswesw
seswneswswsenwwnwse
nwnwneseeswswnenewneswwnewseswneseene
swweswneswnenwsewnwneneseenw
eesenwseswswnenwswnwnwsewwnwsene
sewnenenenesenwsewnenwwwse
wenwwweseeeweswwwnwwe
wsweesenenewnwwnwsenewsenwwsesesenwne
neeswseenwwswnwswswnw
nenwswwsewswnenenewsenwsenwnesesenew
enewnwewneswsewnwswenweswnenwsenwsw
sweneswneswneneenwnewenewwneswswnese
swwesenesewenwneswnwwneseswwne
enesenwswwswneneswsenwnewswseenwsese
wnwnesenesenenwwnenwsewesewsesesew
nenewswnwewswnenesenwnesewesw
eneswnwswnwsenenwnwnwwseeswneewsenese
neswnwewnwnwseenwseesewsenwsweewe
wseweeenwnesenwwwswnew".Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

            var test = new LobbyFloor();
            test.FollowInstructions(sample);

            Assert.That(test.Part1Solution(), Is.EqualTo(10));
            Assert.That(test.Part2Solution(), Is.EqualTo(2208));
        }

        [Test]
        public void TestPart1Solution()
        {
            var data = File.ReadAllLines("Day24Data.txt");

            var part1 = new LobbyFloor();
            part1.FollowInstructions(data);

            Assert.That(part1.Part1Solution(), Is.EqualTo(326));
        }

        [Test]
        public void TestPart2Solution()
        {
            var data = File.ReadAllLines("Day24Data.txt");

            var part2 = new LobbyFloor();
            part2.FollowInstructions(data);

            Assert.That(part2.Part2Solution(), Is.EqualTo(3979));
        }

        [Test]
        public void TestAdjcentCoordinates()
        {
            var test = new HexCoordinate();
            foreach(var i in test.AdjacentCoordinates)
            {
                Console.WriteLine(i.Coordinates);
            }
        }
    }

    /// <summary>
    /// My vacation hotel lobby floor that is being renovated with black and white tiles.
    /// </summary>
    internal sealed class LobbyFloor
    {
        // new tiles.
        private Dictionary<(int x, int y, int z), LobbyTile> tiles;

        public LobbyFloor()
        {
            tiles = new Dictionary<(int x, int y, int z), LobbyTile>();
        }

        /// <summary>
        /// Parser, converts a series of moving instruction into a cube
        /// hex grid coordinate.
        /// </summary>
        /// <param name="instruction"></param>
        /// <returns></returns>
        private HexCoordinate FindCoordinate(string instruction)
        {
            HexCoordinate position = new HexCoordinate();

            char prev = ' ';
            foreach (var current in instruction.ToCharArray())
            {
                if (current == 'w')
                {
                    switch (prev)
                    {
                        case 'n':
                            position.MoveNorthWest();
                            break;
                        case 's':
                            position.MoveSouthWest();
                            break;
                        default:
                            position.MoveWest();
                            break;
                    }
                }
                else if (current == 'e')
                {
                    switch (prev)
                    {
                        case 'n':
                            position.MoveNorthEast();
                            break;
                        case 's':
                            position.MoveSouthEast();
                            break;
                        default:
                            position.MoveEast();
                            break;
                    }
                }
                prev = current;
            }
            return position;
        }

        /// <summary>
        /// Not all white tiles are counted for since it is a default tile color.
        /// This method makes sure all white tiles adjcent a black tile is registered
        /// in the hash table. This was a lazy implementation to not miss any white tiles
        /// that needed to be flipped as the initial data structure only contained tiles that are mentioned
        /// in the instruction set provided by the tile installers.
        /// 
        /// Thus, this is not very efficient, and potentially a bottle neck.
        /// There are a few optimization improvements are available to implement if we need to compute much
        /// bigger set of data as proposed as follows:
        /// 
        /// 1. Keep record/history of black tiles in a set and create white tile instances where needed.
        /// 2. Keep the min/max value for three axis (x, y, z) and create tile instances for all coordinates
        ///     that fall within the ranges.
        /// 
        /// Although, both modifications will reduce unnecessary map checkings and thereby improve the solution greately,
        /// my personal choice would be #2 as it will be more straight forward and clear and remove any redundent computations,
        /// such as computing inner grids rather than only on the edge coordinates.
        /// </summary>
        private void ExpandGrid()
        {
            List<LobbyTile> blackTiles = new List<LobbyTile>();
            foreach (var tile in tiles.Values)
            {
                if(!tile.IsWhite())
                {
                    blackTiles.Add(tile);
                }
            }

            foreach(var tile in blackTiles)
            {
                var positions = tile.Coordinate.AdjacentCoordinates;
                foreach (var position in positions)
                {
                    if (!tiles.ContainsKey(position.Coordinates))
                    {
                        tiles.Add(position.Coordinates, new LobbyTile(position.X, position.Y, position.Z));
                    }
                }
            }
        }

        /// <summary>
        /// Count black tiles in the neighbouring coordinates for a specific coordinate.
        /// </summary>
        /// <param name="tile"></param>
        /// <returns></returns>
        private int CountBlackTiles(HexCoordinate tile)
        {
            int count = 0;
            foreach(var adjcent in tile.AdjacentCoordinates)
            {
                if(tiles.ContainsKey(adjcent.Coordinates))
                {
                    if(!tiles[adjcent.Coordinates].IsWhite())
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        /// <summary>
        /// Check the flip constraints for the tile at the given position,
        /// if constraints are met, return true.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private bool CheckPart2Constraint(HexCoordinate position)
        {
            var temp = tiles[position.Coordinates];
            var nBlacks = CountBlackTiles(position);
            if (temp.IsWhite())
            {
                if (nBlacks == 2)
                {
                    return true;
                }
                return false;
            }
            else
            {
                if (nBlacks == 0 || nBlacks > 2)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void DailyTileFlip()
        {
            List<LobbyTile> changes = new List<LobbyTile>();
            ExpandGrid();
            foreach(var tile in tiles.Values)
            {
                if(CheckPart2Constraint(tile.Coordinate))
                {
                    changes.Add(tile);
                }
            }

            foreach(var tile in changes)
            {
                tile.Flip();
            }
        }

        /// <summary>
        /// Follow a list of commands given by the contractor, to track the coordinates of tiles that needs to be flipped.
        /// </summary>
        /// <param name="instructions"></param>
        public void FollowInstructions(string[] instructions)
        {
            foreach (var instruction in instructions)
            {
                var coordinate = FindCoordinate(instruction);
                if (!tiles.ContainsKey(coordinate.Coordinates))
                {
                    tiles.Add(coordinate.Coordinates, new LobbyTile(coordinate.X, coordinate.Y, coordinate.Z));
                }
                tiles[coordinate.Coordinates].Flip();
            }
        }

        /// <summary>
        /// Part1 - Count black tiles based on the given instruction set.
        /// </summary>
        /// <returns></returns>
        public int Part1Solution()
        {
            int result = 0;
            foreach(var tile in tiles.Values)
            {
                if(!tile.IsWhite())
                {
                    result++;
                }
            }
            return result;
        }

        /// <summary>
        /// Part2 - Everyday the tiles flips based on the number of black tiles nearby like an
        /// art exhibit. Count how many black tiles are there after 100 days.
        /// </summary>
        /// <returns></returns>
        public int Part2Solution()
        {
            for(int i = 0; i < 100; i++)
            {
                DailyTileFlip();
            }
            return Part1Solution();
        }
    }

    internal enum HexMove
    {
        NorthWest,
        NorthEast,
        West,
        East,
        SouthWest,
        SouthEast
    }

    /// <summary>
    /// Implementing Cube Coordinates for Hex Grids
    /// </summary>
    internal class HexCoordinate
    {
        private int x, y, z;
        public int X { get { return x; } }
        public int Y { get { return y; } }
        public int Z { get { return z; } }

        public (int, int, int) Coordinates { get { return (x, y, z); } }

        public HexCoordinate(int x = 0, int y = 0, int z = 0)
        {
            this.x = x; 
            this.y = y; 
            this.z = z; 
        }

        public void MoveNorthWest(int delta = 1)
        {
            this.x -= delta;
            this.z += delta;
        }

        public void MoveNorthEast(int delta = 1)
        {
            this.x -= delta;
            this.y += delta;
        }

        public void MoveWest(int delta = 1)
        {
            this.y -= delta;
            this.z += delta;
        }

        public void MoveEast(int delta = 1)
        {
            this.y += delta;
            this.z -= delta;
        }

        public void MoveSouthWest(int delta = 1)
        {
            this.x += delta;
            this.y -= delta;
        }

        public void MoveSouthEast(int delta = 1)
        {
            this.x += delta;
            this.z -= delta;
        }

        public void Move(HexMove movement, int delta = 1)
        {
            switch(movement)
            {
                case HexMove.NorthWest:
                    MoveNorthWest(delta);
                    break;
                case HexMove.NorthEast:
                    MoveNorthEast(delta);
                    break;
                case HexMove.West:
                    MoveWest(delta);
                    break;
                case HexMove.East:
                    MoveEast(delta);
                    break;
                case HexMove.SouthWest:
                    MoveSouthWest(delta);
                    break;
                case HexMove.SouthEast:
                    MoveSouthEast(delta);
                    break;
            }
        }

        /// <summary>
        /// Perfomance can be improved by hard coding six coordinates rather than 
        /// creating a list and object instances for all neighbouring coordinates then
        /// converting to an array.
        /// </summary>
        public HexCoordinate[] AdjacentCoordinates
        {
            get
            {
                List<HexCoordinate> coordinates = new List<HexCoordinate>();
                foreach(var i in Enum.GetValues(typeof(HexMove)))
                {
                    var temp = new HexCoordinate(X, Y, Z);
                    temp.Move((HexMove)i);
                    coordinates.Add(temp);
                }
                return coordinates.ToArray();
            }
        }
    }

    /// <summary>
    /// White or Black? Double sided tile that can be flipped as required.
    /// No Adhesion required (for the LIVE art exhibition reason).
    /// Nonetheless, I was so HAPPY to find out that I didn't have to map out the black tiles to find patterns 
    /// such as sea monster patterns for this puzzle. Although it would've been fun to create an image based on
    /// hex grid data.
    /// </summary>
    internal class LobbyTile
    {
        private readonly HexCoordinate coordinate;

        private bool isWhite = true;

        public LobbyTile(int x, int y, int z)
        {
            coordinate = new HexCoordinate(x, y, z);
        }

        public void Flip()
        {
            isWhite = (isWhite) ? false : true;
        }

        public bool IsWhite()
        {
            return isWhite;
        }

        public HexCoordinate Coordinate { get { return coordinate; } }
    }
}
