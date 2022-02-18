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
    /// 
    /// </summary>
    internal sealed class LobbyFloor
    {
        private Dictionary<(int x, int y, int z), LobbyTile> tiles;

        public LobbyFloor()
        {
            tiles = new Dictionary<(int x, int y, int z), LobbyTile>();
        }

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
        /// if true, flip the tile.
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

        private void DailyTileFlip()
        {
            HashSet<LobbyTile> changes = new HashSet<LobbyTile>();
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
