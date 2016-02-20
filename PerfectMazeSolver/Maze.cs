using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfectMazeSolver
{
    public class MazeFactory
    {
        public Maze CreateMaze()
        {
            return new Maze();
        }

        /// <summary>
        /// Assumes format (i,j,up,right,down,left), where directions are walls
        /// </summary>
        /// <param name="tiles"></param>
        /// <returns></returns>
        public Maze CreateMaze(int[][] tiles,int height,int width)
        {
            MazeTile[,] Tiles = new MazeTile[height, width];
            foreach(int[] tile in tiles)
            {
                MazeTile mazeTile = new MazeTile() { Y = tile[0], X = tile[1] };
                if(tile[2]==1)
                {
                    mazeTile.MyNeighboors.Add(Neighbors.Up);
                }
                if(tile[3]==1)
                {
                    mazeTile.MyNeighboors.Add(Neighbors.Right);
                }
                if(tile[4]==1)
                {
                    mazeTile.MyNeighboors.Add(Neighbors.Down);
                }
                if(tile[5]==1)
                {
                    mazeTile.MyNeighboors.Add(Neighbors.Left);
                }
                Tiles[tile[0], tile[1]] = mazeTile;
            }
            return new Maze() { Tiles = Tiles };
        }

        public Maze CreateMaze(MazeType type,int height,int width)
        {            
            switch(type)
            {
                case MazeType.Snake:
                    return CreateSnakeMaze(height, width);
                case MazeType.Generated:
                    return CreateGeneratedMaze(height, width);
                default:
                    return CreateGeneratedMaze(height, width);
            }
        }

        private Maze CreateGeneratedMaze(int height, int width)
        {
            return (new PerfectMazeGenerator()).GenerateMaze(height, width);
        }

        Maze CreateSnakeMaze(int height,int width)
        {
            MazeTile[,] tiles = new MazeTile[height, width];
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    tiles[j, i] = new MazeTile() { X = i, Y = j };
                    if (j > 0)
                        tiles[j, i].MyNeighboors.Add(Neighbors.Down);

                    if (j < height - 1)
                        tiles[j, i].MyNeighboors.Add(Neighbors.Up);

                    if (i % 2 == 0)
                    {
                        if (j == height - 1 && i < width - 1)
                            tiles[j, i].MyNeighboors.Add(Neighbors.Right);
                        if (j == 0 && i > 0)
                            tiles[j, i].MyNeighboors.Add(Neighbors.Left);
                    }
                    else
                    {
                        if (j == height - 1)
                            tiles[j, i].MyNeighboors.Add(Neighbors.Left);
                        if (j == 0 && i < width - 1)
                            tiles[j, i].MyNeighboors.Add(Neighbors.Right);
                    }
                }
            }

            return new Maze() { Tiles = tiles };
        }
    }


    public class Maze
    {
        public MazeTile[,] Tiles;

        public List<MazeTile> GetConnectedTiles(MazeTile tile)
        {
            List<MazeTile> connectedNeighboors = new List<MazeTile>();
            foreach(Neighbors neighbor in tile.MyNeighboors)
            {
                switch(neighbor)
                {
                    case Neighbors.Up:
                        connectedNeighboors.Add(Tiles[tile.Y + 1, tile.X]);
                        break;
                    case Neighbors.Right:
                        connectedNeighboors.Add(Tiles[tile.Y, tile.X + 1]);
                        break;
                    case Neighbors.Down:
                        connectedNeighboors.Add(Tiles[tile.Y - 1, tile.X]);
                        break;
                    case Neighbors.Left:
                        connectedNeighboors.Add(Tiles[tile.Y, tile.X - 1]);
                        break;
                }
            }
            return connectedNeighboors;
        }

        internal void ConnectTiles(MazeTile tile1, MazeTile tile2)
        {
            if(tile1.X==tile2.X)
            {
                if(tile1.Y>tile2.Y)
                {
                    tile1.MyNeighboors.Add(Neighbors.Down);
                    tile2.MyNeighboors.Add(Neighbors.Up);
                }
                else if(tile2.Y>tile1.Y)
                {
                    tile1.MyNeighboors.Add(Neighbors.Up);
                    tile2.MyNeighboors.Add(Neighbors.Down);
                }
            }

            if (tile1.Y == tile2.Y)
            {
                if (tile1.X < tile2.X)
                {
                    tile1.MyNeighboors.Add(Neighbors.Right);
                    tile2.MyNeighboors.Add(Neighbors.Left);
                }
                else if (tile2.X < tile1.X)
                {
                    tile1.MyNeighboors.Add(Neighbors.Left);
                    tile2.MyNeighboors.Add(Neighbors.Right);
                }
            }
        }

        internal List<MazeTile> GetNeighboorTiles(MazeTile tile)
        {
            List<MazeTile> neighbors = new List<MazeTile>();
            if (tile.Y + 1 <= Tiles.GetUpperBound(0))
                neighbors.Add(Tiles[tile.Y + 1, tile.X]);
            if (tile.X + 1 <= Tiles.GetUpperBound(1))
                neighbors.Add(Tiles[tile.Y, tile.X + 1]);
            if (tile.Y > 0)
                neighbors.Add(Tiles[tile.Y - 1, tile.X]);
            if(tile.X >0)
                neighbors.Add(Tiles[tile.Y, tile.X - 1]);
            return neighbors;
        }

        public MazeTile GetRandomTile()
        {
            Random r = new Random(DateTime.Now.Millisecond);
            int X = r.Next(Tiles.GetUpperBound(1) + 1),
                Y = r.Next(Tiles.GetUpperBound(0) + 1);
            return Tiles[Y, X];
        }
    }

    public enum MazeType
    {
        Snake,
        Generated
    }
}
