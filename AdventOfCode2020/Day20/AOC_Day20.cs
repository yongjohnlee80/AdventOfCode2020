using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AdventOfCode2020.Day20
{
    internal class Day20UnitTest
    {
        [Test]
        public void TestParse()
        {
            ElvenImage test = new ElvenImage();
            test.LoadImage("Day20Sample.txt");
            test.Log();
        }

        [Test]
        public void TestSample()
        {
            ElvenImage test = new ElvenImage();
            test.LoadImage("Day20Sample.txt");
            Assert.That(test.Part1Answer(), Is.EqualTo(20899048083289));
        }

        [Test]
        public void TestPart1Solution()
        {
            ElvenImage part1 = new ElvenImage();
            part1.LoadImage("Day20Data.txt");
            Console.WriteLine(part1.Part1Answer());
        }

        [Test]
        public void TestString()
        {
            string str = "abcba";
            string str2 = "abcba";

            if (str == str2) Console.WriteLine("Works!");

            char[] temp = str.ToCharArray();
            Array.Reverse(temp);
            string reverseString = new string(temp);
            if (reverseString == str2) Console.WriteLine("Works, too!");
        }
    }

    public enum EdgePosition
    {
        Top, 
        Bottom,
        Left,
        Right
    }

    internal class TileEdge
    {
        private int parentID;
        private string data;
        private TileEdge connectingEdge = null;
        public EdgePosition Position { get; set; }

        public TileEdge ConnectingEdge { get { return connectingEdge; } }
        public int ParentID { get { return parentID; } }

        public TileEdge(int parentID, string edgeData, EdgePosition position)
        {
            this.parentID = parentID;
            this.data = edgeData;
            this.Position = position;
        }

        public bool IsCompatible(TileEdge other)
        {
            if (this.data == other.data) return true;
            return false;
        }

        public bool IsCompatible(string edgeData)
        {
            if (this.data == edgeData) return true;
            return false;
        }

        public string GetReverse()
        {
            char[] temp = this.data.ToCharArray();
            Array.Reverse(temp);
            string reverse = new string(temp);
            return reverse;
        }

        public bool IsReverseCompatible(TileEdge other)
        {
            if (GetReverse() == other.data) return true;
            return false;
        }

        public bool IsReverseCompatible(string edgeData)
        {
            if (GetReverse() == edgeData) return true;
            return false;
        }

        public bool TryConnect(TileEdge other)
        {
            if(IsCompatible(other) || IsReverseCompatible(other))
            {
                connectingEdge = other;
                return true;
            }
            return false;
        }

        public void ConnectTo(TileEdge neighbor)
        {
            connectingEdge = neighbor;
        }

        public void Flip()
        {
            char[] temp = this.data.ToCharArray();
            Array.Reverse(temp);
            this.data = new string(temp);
        }

        public string GetValue()
        {
            return this.data;
        }
    }

    internal class Tile
    {
        private int tileID;
        
        public int ID { get { return tileID; } }

        private TileEdge[] edges;

        private string[] data;

        public Tile(int id)
        {
            tileID = id;
            // top(0), bottom(1), left(2), and right(3).
            edges = new TileEdge[4];
        }

        public TileEdge[] Load(string[] cells)
        {
            int size = cells.Length;

            List<string> data = new List<string>();
            foreach (string cell in cells)
            {
                data.Add(cell);
            }
            this.data = data.ToArray();

            edges[0] = new TileEdge(tileID, cells[0], EdgePosition.Top);
            edges[1] = new TileEdge(tileID, cells[size - 1], EdgePosition.Bottom);

            char[] left = new char[size];
            char[] right = new char[size];
            int index = 0;

            foreach(string cell in cells)
            {
                left[index] = cell.ToCharArray()[0];
                right[index] = cell.ToCharArray()[size - 1];
                index++;
            }
            edges[2] = new TileEdge(tileID, new string(left), EdgePosition.Left);
            edges[3] = new TileEdge(tileID, new string(right), EdgePosition.Right);

            return edges;
        }

        public TileEdge EdgeAt(EdgePosition position)
        {
            return edges[(int)position];
        }

        public TileEdge[] GetEdges()
        {
            return this.edges;
        }

        public TileEdge? GetEdge(string edgeData)
        {
            foreach(TileEdge edge in this.edges)
            {
                if(edge.IsCompatible(edgeData)) return edge;
            }
            return null;
        }

        private void RotateClockwise()
        {
            TileEdge temp = edges[0];
            edges[0] = edges[2];
            edges[2] = edges[1];
            edges[1] = edges[3];
            edges[3] = temp;
        }

        private void RotateCounterClockwise()
        {
            TileEdge temp = edges[0];
            edges[0] = edges[3];
            edges[3] = edges[1];
            edges[1] = edges[2];
            edges[2] = temp;
        }

        private void FlipUpDown()
        {
            TileEdge temp = edges[0];
            edges[0] = edges[1];
            edges[1] = temp;
            edges[2].Flip();
            edges[3].Flip();
        }

        private void FlipLeftRight()
        {
            TileEdge temp = edges[2];
            edges[2] = edges[3];
            edges[3] = temp;
            edges[0].Flip();
            edges[1].Flip();
        }

        public void RotateTile(int nTurns = 1)
        {
            if(nTurns > 0)
            {
                while(nTurns > 0)
                {
                    RotateCounterClockwise();
                    nTurns--;
                }
            }
            else
            {
                while(nTurns < 0)
                {
                    RotateClockwise();
                    nTurns++;
                }
            }
        }

        public bool IsCornerTile()
        {
            int noEdge = 0;
            foreach(var i in edges)
            {
                if(i.ConnectingEdge == null)
                {
                    noEdge++;
                }
            }
            if (noEdge == 2) return true;
            return false;
        }

        public string GetInfo()
        {
            StringBuilder info = new StringBuilder();
            info.Append($"Tile ID {tileID}, ");
            foreach(var i in edges)
            {
                if(i.ConnectingEdge != null)
                {
                    info.Append($"({i.ConnectingEdge.ParentID})");
                }
                else
                {
                    info.Append($"(None)");
                }
            }
            info.Append("\n");
            //info.Append($"Tile ID {tileID}, top({edges[0].ConnectingEdge.ParentID}), bottom({edges[1].ConnectingEdge.ParentID}), " +
            //    $"left({edges[2].ConnectingEdge.ParentID}), right({edges[3].ConnectingEdge.ParentID})\n");
            foreach (var i in this.data)
            {
                info.Append($"{i}\n");
            }
            return info.ToString();
        }
    }

    internal class ElvenImage
    {
        private Dictionary<int, Tile> tiles = new Dictionary<int, Tile>();

        private Dictionary<string, List<int>> edgeMap = new Dictionary<string, List<int>>();

        public void LoadImage(string fileName)
        {
            var lines = File.ReadAllLines(fileName);
            LoadImage(lines);
        }

        private void ConnectTileEdges()
        {
            foreach(Tile tile in this.tiles.Values)
            {
                foreach(var edge in tile.GetEdges()) // count = 4 : constant time complexity.
                {
                    var temp = edgeMap[edge.GetValue()];
                    if (temp.Count > 1)
                    {
                        foreach(int i in temp) // count = 2 : constant time complexity.
                        {
                            if(tile.ID != i)
                            {
                                var matchingEdge = tiles[i].GetEdge(edge.GetValue());
                                if (matchingEdge != null)
                                {
                                    edge.ConnectTo(matchingEdge);
                                }
                                else
                                {
                                    matchingEdge = tiles[i].GetEdge(edge.GetReverse());
                                    // TODO: tiles[i].Flip correctly!!!
                                    edge.ConnectTo(matchingEdge);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void LoadImage(string[] lines)
        {
            int count = 0;
            int id = 0;
            List<string> tileData = new List<string> ();

            foreach(var line in lines)
            {
                if(line.Contains("Tile"))
                {
                    string[] numbers = Regex.Split(line, @"\D+");
                    foreach(string number in numbers)
                    {
                        if(!string.Empty.Contains(number))
                        {
                            id = int.Parse(number);
                            tiles[id] = new Tile(id);
                            break;
                        }
                    }
                    count = 0;
                    tileData.Clear();
                }
                else if (!string.IsNullOrEmpty(line))
                {
                    tileData.Add(line);
                    count++;

                    if (count == 10)
                    {
                        var currentEdges = tiles[id].Load(tileData.ToArray());
                        foreach(var edge in currentEdges)
                        {
                            if(!edgeMap.ContainsKey(edge.GetValue()))
                            {
                                edgeMap[edge.GetValue()] = new List<int>();
                            }
                            if(!edgeMap.ContainsKey(edge.GetReverse()))
                            {
                                edgeMap[edge.GetReverse()] = new List<int>();
                            }
                            edgeMap[edge.GetValue()].Add(id);
                            edgeMap[edge.GetReverse()].Add(id);
                        }
                        count = 0;
                    }
                }
            }

            ConnectTileEdges();
        }

        public long Part1Answer()
        {
            long answer = 1;
            foreach(var tile in tiles.Values)
            {
                if(tile.IsCornerTile())
                {
                    answer *= tile.ID;
                }
            }
            return answer;
        }

        public void Log(string fileName = "Day20Log.txt")
        {
            LogStat log = new LogStat();
            log.Append($"Number of tiles = {this.tiles.Count}\n\n");
            foreach(var i in tiles.Values)
            {
                log.Append($"{i.GetInfo()}\n");
            }

            foreach(var i in edgeMap)
            {
                log.Append($"Edge {i.Key} : ");
                foreach(var j in i.Value)
                {
                    log.Append($"(id:{j}) ");
                }
                log.Append("\n");
            }
            log.LogOnFile(fileName);
        }
    }
}
