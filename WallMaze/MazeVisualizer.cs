using Canvas_Window_Template.Basic_Drawing_Functions;
using Canvas_Window_Template.Factories;
using PerfectMazeSolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallMaze
{
    public class MazeVisualizer
    {
        public static bool[,,] CreateWalls(Maze maze)
        {
            bool[,,] hasWall = new bool[maze.Tiles.GetUpperBound(0) + 1, maze.Tiles.GetUpperBound(1) + 1, 2];
            foreach (MazeTile tile in maze.Tiles)
            {
                if(!tile.MyNeighboors.Contains(Neighbors.Right))
                {
                    hasWall[tile.X, tile.Y,0]=true;
                }

                if (!tile.MyNeighboors.Contains(Neighbors.Down))
                {
                    hasWall[tile.X, tile.Y, 1] = true;
                }
            }
            return hasWall;
        }

    }
}
