using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfectMazeSolver
{
    public class AStarSolver : IMazeSolver
    {
        public List<MazeTile> Solve(Maze maze,MazeTile start,MazeTile end)
        {
            int[,] distance = new int[maze.Tiles.GetUpperBound(0) + 1, maze.Tiles.GetUpperBound(1) + 1];
            distance[start.Y, start.X] = 1;
            List<MazeTile> tilesToCheck = new List<MazeTile>() { start},
                tempList=new List<MazeTile>(),currentNeighbors,Path=new List<MazeTile>();
            bool stay = true;

            while(stay)
            {
                foreach(MazeTile tile in tilesToCheck)
                {
                    //prevent adding previously checked neighboors
                    currentNeighbors=maze.GetConnectedTiles(tile).Where(n=>distance[n.Y,n.X]<1).ToList();
                    foreach(MazeTile neighboor in currentNeighbors)
                    {
                        distance[neighboor.Y, neighboor.X] = distance[tile.Y, tile.X] + 1;
                        tempList.Add(neighboor);
                        if (neighboor.Equals(end))
                            stay = false;
                    }
                }
                tilesToCheck.RemoveAll(n=>true);
                tilesToCheck.AddRange(tempList);
                tempList.RemoveAll(n => true);                
            }

            //Walk back from end
            Path.Add(end);
            MazeTile current = end;
            while(!current.Equals(start))
            {
                current = maze.GetConnectedTiles(current)
                    .Where(n => distance[n.Y, n.X] == distance[current.Y, current.X] - 1)
                    .First();
                Path.Add(current);
            }
            
            return Path.Reverse<MazeTile>().ToList();
        }
    }
}
