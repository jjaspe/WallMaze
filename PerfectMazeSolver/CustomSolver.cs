using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfectMazeSolver
{
    public class CustomSolver:IMazeSolver
    {
        public List<MazeTile> Solve(Maze maze,MazeTile start,MazeTile end)
        {
            List<MazeTile> good = new List<MazeTile>(),
                bad = new List<MazeTile>();
            bool[,] state = new bool[maze.Tiles.GetUpperBound(0) + 1,
                maze.Tiles.GetUpperBound(1) + 1];
            foreach(MazeTile tile in maze.Tiles)
            {
                if (tile.Equals(start) || tile.Equals(end))
                {
                    state[tile.Y, tile.X] = true;
                    good.Add(tile);
                }
                else if(tile.MyNeighboors.Count<2)
                {
                    state[tile.Y, tile.X] = false;
                    bad.Add(tile);
                }
                else
                {
                    state[tile.Y, tile.X] = true;
                    good.Add(tile);
                }
            }

            bool change;
            List<MazeTile> currentNeighbors;

            do
            {
                change = false;
                for (int i = 0; i < good.Count; i++)
                {
                    MazeTile tile = good[i];
                    if (tile.Equals(start) || tile.Equals(end))
                        continue;
                    currentNeighbors = maze.GetConnectedTiles(tile);
                    if (currentNeighbors.Where(n => state[n.Y, n.X]).Count() < 2)
                    {
                        state[tile.Y, tile.X] = false;
                        bad.Add(tile);
                        good.Remove(tile);
                        change = true;
                    }   
                }
            } while (change) ;

            List<MazeTile> Path = new List<MazeTile>() { start };
            MazeTile current = start;
            while(!current.Equals(end))
            {
                current = maze.GetConnectedTiles(current)
                    .Where(n => good.Contains(n) && !Path.Contains(n)).First();
                Path.Add(current);
            }
            return Path;
        }
    }
}
