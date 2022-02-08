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
        /// <summary>
        /// Relevant Unit Testings.
        /// </summary>
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
        public void TestSamplePart2()
        {
            ElvenImage part2 = new ElvenImage();
            part2.LoadImage("Day20Sample.txt");
            Assert.That(part2.Part2Answer(), Is.EqualTo(273));
        }

        [Test]
        public void TestPart2Solution()
        {
            ElvenImage part2 = new ElvenImage();
            part2.LoadImage("Day20Data.txt");
            Console.WriteLine(part2.Part2Answer());
        }

        /// <summary>
        /// Irrelevant Unit Testings.
        /// </summary>
        [Test]
        public void TestParse()
        {
            ElvenImage test = new ElvenImage();
            test.LoadImage("Day20Data.txt");
            test.Log();
        }

        [Test]
        public void TestRotate()
        {
            string[] data = {
"ABCDEFGHIJ",
"1234567890",
"abcde#ghij",
"KLMNOPQRST",
"UVWXYZABCD",
"klmnopqrst",
"uvwxysabcd",
"!@#$%^&*()",
"098#654321",
"YONGSUNGLE" };

            Tile test = new Tile(1234);
            test.Load(data);

            //Console.WriteLine(test.GetTileRow(3));

            //test.FlipLeftRight();
            test.FlipUpDown();
            //test.Rotate();
            //test.Rotate();
            //test.Rotate();
            //test.Rotate();

            Console.WriteLine(test.GetInfo());
            Console.WriteLine();

            for(int i = 1; i < data.Length-1; i++)
            {
                Console.WriteLine(test.GetTileRow(i));
            }

            Console.WriteLine(test.GetSharpCounts());
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

    /// <summary>
    /// TileEdge positions Top(0), Bottom(1), Left(1), Right(2)...
    /// </summary>
    public enum EdgePosition
    {
        Top, 
        Bottom,
        Left,
        Right
    }

    /// <summary>
    /// Type TileEdge works as a glue between neighbouring tiles
    /// </summary>
    internal class TileEdge
    {
        private int parentID; // TileID
        private string data; // edge data
        private TileEdge connectingEdge = null; // neighbouring Tile.

        /// <summary>
        /// Getters and Setters.
        /// </summary>
        public EdgePosition Position { get; set; }

        public TileEdge ConnectingEdge { get { return connectingEdge; } }
        public int ParentID { get { return parentID; } }

        public TileEdge(int parentID, string edgeData, EdgePosition position)
        {
            this.parentID = parentID;
            this.data = edgeData;
            this.Position = position;
        }

        /// <summary>
        /// IsCompatible method is used to determine whether the edge data matches
        /// the string data provided.
        /// </summary>
        /// <param name="edgeData"></param>
        /// <returns></returns>
        public bool IsCompatible(string edgeData)
        {
            if (this.data == edgeData) return true;
            return false;
        }

        /// <summary>
        /// GetReverse method returns the edge data in reverse.
        /// </summary>
        /// <returns></returns>
        public string GetReverse()
        {
            char[] temp = this.data.ToCharArray();
            Array.Reverse(temp);
            string reverse = new string(temp);
            return reverse;
        }

        /// <summary>
        /// This method is primarily used to check two edges are the match when one is flipped.
        /// </summary>
        /// <param name="edgeData"></param>
        /// <returns></returns>
        public bool IsReverseCompatible(string edgeData)
        {
            if (GetReverse() == edgeData) return true;
            return false;
        }

        /// <summary>
        /// IsPositionCompatible checks the edge positions whether two tiles can be connected as is.
        /// If not, one must be rearranged to be compatible.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsPositionCompatible(TileEdge other)
        {
            if (Position == EdgePosition.Top && other.Position == EdgePosition.Bottom) return true;
            if (Position == EdgePosition.Left && other.Position == EdgePosition.Right) return true;
            if (Position == EdgePosition.Bottom && other.Position == EdgePosition.Top) return true;
            if (Position == EdgePosition.Right && other.Position == EdgePosition.Left) return true;
            return false;
        }

        /// <summary>
        /// Connects the edge to another.
        /// </summary>
        /// <param name="neighbor"></param>
        public void ConnectTo(TileEdge neighbor)
        {
            connectingEdge = neighbor;
        }

        /// <summary>
        /// Reverses the edge data.
        /// </summary>
        public void Flip()
        {
            char[] temp = this.data.ToCharArray();
            Array.Reverse(temp);
            this.data = new string(temp);
        }

        /// <summary>
        /// GetValue returns the edge data (string). 
        /// Technically a getter..
        /// </summary>
        /// <returns></returns>
        public string GetValue()
        {
            return this.data;
        }
    }

    /// <summary>
    /// Type Tile represents tiles of an elven image.
    /// Supports loading data, rotation, and flip operations.
    /// </summary>
    internal class Tile
    {
        private int tileID;
        
        public int ID { get { return tileID; } }

        private TileEdge[] edges;

        private string[] data;

        public int Size { get { return data.Length; } }

        public Tile(int id)
        {
            tileID = id;
            // top(0), bottom(1), left(2), and right(3).
            edges = new TileEdge[4];
        }

        private TileEdge[] ExtractEdges(int size = 10)
        {
            /// Top and bottom edges.
            edges[0] = new TileEdge(tileID, data[0], EdgePosition.Top);
            edges[1] = new TileEdge(tileID, data[size - 1], EdgePosition.Bottom);

            /// Left and right edges.
            char[] left = new char[size];
            char[] right = new char[size];
            int index = 0;

            foreach (string cell in data)
            {
                left[index] = cell.ToCharArray()[0];
                right[index] = cell.ToCharArray()[size - 1];
                index++;
            }
            edges[2] = new TileEdge(tileID, new string(left), EdgePosition.Left);
            edges[3] = new TileEdge(tileID, new string(right), EdgePosition.Right);

            return edges;
        }
        
        /// <summary>
        /// Load method loads tile data and creates edge data.
        /// </summary>
        /// <param name="cells"></param>
        /// <returns></returns>
        public TileEdge[] Load(string[] cells)
        {
            int size = cells.Length;

            /// Loading image/tile data
            List<string> data = new List<string>();
            foreach (string cell in cells)
            {
                data.Add(cell);
            }
            this.data = data.ToArray();

            return ExtractEdges(size);
        }

        /// <summary>
        /// Edge getters.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
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

        /// <summary>
        /// This method is mainly called when the tile has been rotated or flipped to 
        /// update edge position data.
        /// </summary>
        private void UpdateEdgePositions()
        {
            edges[0].Position = EdgePosition.Top;
            edges[1].Position = EdgePosition.Bottom;
            edges[2].Position = EdgePosition.Left;
            edges[3].Position = EdgePosition.Right;
        }

        /// <summary>
        /// Utility method that swaps rows and columns of text array.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string[] RowToColumn(string[] text)
        {
            int length = text.Length;
            List<string> result = new List<string>();

            for(int i = 0; i < length; i++)
            {
                char[] col = new char[length];
                for(int j = 0; j < length; j++)
                {
                    col[j] = text[j][i];
                }
                result.Add(new string(col));
            }
            return result.ToArray();
        }

        /// <summary>
        /// Rotate the tile counter clockwise.
        /// </summary>
        public void Rotate()
        {
            TileEdge temp = edges[0];
            edges[0] = edges[3];
            edges[3] = edges[1];
            edges[1] = edges[2];
            edges[2] = temp;
            edges[2].Flip();
            edges[3].Flip();
            UpdateEdgePositions();

            this.data = RowToColumn(this.data);
            Array.Reverse(this.data);
        }

        /// <summary>
        /// Flips the tile upside down.
        /// </summary>
        public void FlipUpDown()
        {
            TileEdge temp = edges[0];
            edges[0] = edges[1];
            edges[1] = temp;
            edges[2].Flip();
            edges[3].Flip();
            UpdateEdgePositions();

            Array.Reverse(this.data);
        }

        /// <summary>
        /// Flips the tile left right.
        /// </summary>
        public void FlipLeftRight()
        {
            /// Lazy implementation and it works....
            /// but but can hard code these operations to enhance the performance.
            Rotate();
            FlipUpDown();
            Rotate();
            Rotate();
            Rotate();
        }

        /// <summary>
        /// Checks whether this tile only has two neighbouring tiles.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Utility method for building an image with identified mapping locations of all tiles.
        /// This method basically helps to retrieve tile image data without the border.
        /// </summary>
        /// <param name="rowNumber"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public string GetTileRow(int rowNumber, int startIndex = 1, int endIndex = 0)
        {
            if(endIndex == 0)
            {
                endIndex = data.Length - 2;
            }
            return this.data[rowNumber].Substring(startIndex, endIndex);
        }

        public long GetSharpCounts()
        {
            long count = 0;
            foreach(var i in data)
            {
                count += i.Count(f => (f == '#'));
            }
            return count;
        }

        /// <summary>
        /// This method finds how many sea monsters patterns are present in the constructed image.
        /// The following is the make up of a sea monster pattern.
        ///                   # 
        /// #    ##    ##    ###
        /// #  #  #  #  #  #   
        /// 01234567890123456789
        ///
        /// (0,18)
        /// (1,0), (1,5), (1,6), (1,11), (1, 12), (1, 17), (1,18), (1,19)
        /// (2,1), (2,4), (2,7), (2,10), (2,13), (2, 16)
        /// </summary>
        /// <returns></returns>
        public int FindSeaMonsters()
        {
            int[] firstRow = { 18 };
            int[] secondRow = { 0, 5, 6, 11, 12, 17, 18, 19 };
            int[] thridRow = { 1, 4, 7, 10, 13, 16 };

            int count = 0;

            for (int i = 2; i < this.data.Length; i++)
            {
                for(int j = 0; j < this.data[i].Length-19; j++)
                {
                    if (SeaMonsterFound(i, j)) count++;
                }
            }
            return count;

            bool SeaMonsterFound(int row, int col)
            {
                foreach(var k in firstRow)
                {
                    if (data[row - 2][col + k] != '#') return false;
                }
                foreach(var k in secondRow)
                {
                    if (data[row - 1][col + k] != '#') return false;
                }
                foreach(var k in thridRow)
                {
                    if (data[row][col + k] != '#') return false;
                }
                return true;
            }
        }

        /// <summary>
        /// For visualized debugging purposes.
        /// </summary>
        /// <returns></returns>
        public string GetInfo()
        {
            StringBuilder info = new StringBuilder();
            info.Append($"Tile ID {tileID}, ");
            foreach(var i in edges)
            {
                if(i.ConnectingEdge != null)
                {
                    info.Append($"({i.GetValue()}:{i.ConnectingEdge.ParentID})");
                }
                else
                {
                    info.Append($"({i.GetValue()}:None)");
                }
            }
            info.Append("\n");
            foreach (var i in this.data)
            {
                info.Append($"{i}\n");
            }
            return info.ToString();
        }
    }

    /// <summary>
    /// Type ElvenImage which loads tile informations and solves the puzzle and create an image data as a Tile data
    /// structure.
    /// </summary>
    internal class ElvenImage
    {
        /// <summary>
        /// Tile Hashmap.
        /// </summary>
        private Dictionary<int, Tile> tiles = new Dictionary<int, Tile>();

        /// <summary>
        /// Edge mapping for solving puzzle.
        /// </summary>
        private Dictionary<string, List<int>> edgeMap = new Dictionary<string, List<int>>();

        /// <summary>
        /// Corner tiles. For part1 solution.
        /// </summary>
        private List<Tile> corners = new List<Tile>();

        /// <summary>
        /// Completed image data for part2.
        /// </summary>
        private Tile image = null;

        public void LoadImage(string fileName)
        {
            var lines = File.ReadAllLines(fileName);
            LoadImage(lines);
        }

        /// <summary>
        /// Connect the edges of all tiles to their compatibles according to the edgemap data compiled.
        /// </summary>
        private void ConnectTileEdges()
        {
            foreach(Tile tile in this.tiles.Values)
            {
                foreach(var edge in tile.GetEdges()) // count = 4 : constant time complexity.
                {
                    var temp = edgeMap[edge.GetValue()];
                    if (temp.Count == 2)
                    {
                        foreach (int i in temp) // count = 2 : constant time complexity.
                        {
                            if (tile.ID != i)
                            {
                                var matchingEdge = tiles[i].GetEdge(edge.GetValue());
                                if(matchingEdge == null) matchingEdge = tiles[i].GetEdge(edge.GetReverse());
                                if (matchingEdge != null)
                                {
                                    edge.ConnectTo(matchingEdge);
                                    matchingEdge.ConnectTo(edge);
                                }
                                
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Finding corner pieces.. really self explanatory.
        /// </summary>
        private void FindCorners()
        {
            foreach(var tile in this.tiles.Values)
            {
                if(tile.IsCornerTile())
                {
                    corners.Add(tile);
                }
            }
        }

        /// <summary>
        /// BuildImage method is used to map the tiles to their respective positions then create an image data
        /// (another tiles structure with ID 0)
        /// </summary>
        private void BuildImage()
        {
            /// The grid length, NxN which is a squre.
            int length = (int)Math.Sqrt(tiles.Count);

            /// Initialize the mapping data.
            var map = new int[length, length];
            var leftCorner = corners[0]; // pick the first corner piece.
            /// Rotate the corner piece until it can be put into the left top corner.
            while (!(leftCorner.EdgeAt(EdgePosition.Left).ConnectingEdge == null && leftCorner.EdgeAt(EdgePosition.Top).ConnectingEdge == null))
                leftCorner.Rotate();

            Tile prevTile = null;

            /// Solving the puzzle from top left horizontally
            map[0, 0] = leftCorner.ID;
            prevTile = tiles[map[0, 0]];
            for (int i = 0; i < length; i++)
            {
                TileEdge edge1 = null;
                TileEdge edge2 = null;
                for(int j = 1; j < length; j++)
                {
                    edge1 = prevTile.EdgeAt(EdgePosition.Right);
                    edge2 = edge1.ConnectingEdge;
                    map[i, j] = edge2.ParentID;

                    //do
                    //{
                        while (!edge1.IsPositionCompatible(edge2)) tiles[edge2.ParentID].Rotate();
                        if (edge1.IsReverseCompatible(edge2.GetValue())) tiles[edge2.ParentID].FlipUpDown();
                        prevTile = tiles[map[i, j]];
                    //} while (!TopOkay(i, j));
                }
                if(i != length - 1)
                {
                    edge1 = tiles[map[i, 0]].EdgeAt(EdgePosition.Bottom);
                    edge2 = edge1.ConnectingEdge;
                    map[i + 1, 0] = edge2.ParentID;
                    do
                    {
                        while (!edge1.IsPositionCompatible(edge2)) tiles[edge2.ParentID].Rotate();
                        if (edge1.IsReverseCompatible(edge2.GetValue())) tiles[edge2.ParentID].FlipLeftRight();
                        prevTile = tiles[edge2.ParentID];
                        if (prevTile.EdgeAt(EdgePosition.Right).ConnectingEdge == null) prevTile.FlipLeftRight();
                    } while (prevTile.EdgeAt(EdgePosition.Right).ConnectingEdge == null);
                }
            }

            /// Create the image from the mapping positions of all tiles.
            Tile image = new Tile(0);
            StringBuilder imageData = new StringBuilder();
            int tileSize = tiles.Last().Value.Size;
            for(int i = 0; i < length; i++)
            {
                for(int j = 1; j < tileSize-1; j++)
                {
                    for(int k = 0; k < length; k++)
                    {
                        imageData.Append(tiles[map[i, k]].GetTileRow(j));
                    }
                    imageData.Append("\n");
                }
            }
            string[] temp = imageData.ToString().Split("\n",StringSplitOptions.RemoveEmptyEntries);
            image.Load(temp);
            this.image = image;

            /// Debugging purposes.
            var log3 = new LogStat();
            log3.Append(image.GetInfo());
            log3.LogOnFile("day20image.txt");

            var log = new LogStat();
            var log2 = new LogStat();
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    log.Append($"({map[j, i]}) ");
                    log2.Append($"{tiles[map[i,j]].GetInfo()}\n");
                }
                log.Append("\n");
            }
            log.LogOnFile("day20map.txt");
            log2.LogOnFile("day20map2.txt");

            //bool TopOkay(int i, int j)
            //{
            //    if (i == 0) return true;
            //    if (tiles[map[i, j]].EdgeAt(EdgePosition.Top).IsCompatible(tiles[map[i - 1, j]].EdgeAt(EdgePosition.Bottom).GetValue()))
            //        return true;
            //    tiles[map[i, j]].FlipLeftRight();
            //    return false;
            //}
        }

        /// <summary>
        /// LoadImage method loads the elven image data in tiles
        /// Compile the compatible edge map
        /// then maps the tiles to their respective location.
        /// </summary>
        /// <param name="lines"></param>
        public void LoadImage(string[] lines)
        {
            int count = 0;
            int id = 0;
            List<string> tileData = new List<string> ();

            /// Parsing...also builds Edge map (hashing)
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

            /// Self Explanatory procedures done by each method.
            ConnectTileEdges(); // Connect edges to their neighbours.
            FindCorners(); // Find the corner tiles.
            BuildImage(); // Maps the tiles and build an image.
        }

        /// <summary>
        /// Multiply corner tile IDs
        /// </summary>
        /// <returns></returns>
        public long Part1Answer()
        {
            long answer = 1;
            //answer =  corners.Aggregate(1, (answer, next) => answer * next.ID); // next.ID is int32 but answer must be int64... how?
            foreach (var corner in corners)
            {
                answer *= corner.ID;
            }
            return answer;
        }

        public long Part2Answer()
        {
            List<long> count = new List<long>();
            count.Add(image.FindSeaMonsters());
            image.FlipLeftRight();
            count.Add(image.FindSeaMonsters());
            image.FlipUpDown();
            count.Add(image.FindSeaMonsters());
            for(int i = 0; i < 3; i++)
            {
                image.Rotate();
                count.Add(image.FindSeaMonsters());
            }
            return image.GetSharpCounts() - (count.Max() * 15);
        }

        /// <summary>
        /// Debugging purposes.
        /// </summary>
        /// <param name="fileName"></param>
        public void Log(string fileName = "Day20Log.txt")
        {
            LogStat log = new LogStat();
            log.Append($"Number of tiles = {this.tiles.Count}\n\n");
            foreach (var i in tiles.Values)
            {
                log.Append($"{i.GetInfo()}\n");
            }

            foreach (var i in edgeMap)
            {
                log.Append($"Edge {i.Key} : ");
                foreach (var j in i.Value)
                {
                    log.Append($"(id:{j}) ");
                }
                log.Append("\n");
            }

            log.Append("Corner Tiles: \n");
            foreach (var i in corners)
            {
                log.Append($"{i.GetInfo()}\n");
            }
            log.LogOnFile(fileName);
        }
    }
}
