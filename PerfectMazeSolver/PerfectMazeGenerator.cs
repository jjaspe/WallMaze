using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfectMazeSolver
{
    public class PerfectMazeGenerator
    {
        public int ySize = 4;
        public int xSize = 4;
        public Maze maze = new Maze();
        public MazeTile[,] MazeTiles;
        public MazeTile currentCell;
        Random r = new Random();
        private int totalCells;
        private int totalVisitedCells=0;
        private List<MazeTile> visitedCells=new List<MazeTile>();
        private bool[,] visited;
        private int lastVisitedCell;
        public int steps=0;
        MazeTile neighbor;

        void Start()
        {
            //Set Inital Positions For Wall Spawning
            SetInitialPositions();
            //Create Cells after walls are created
            CreateCells();
        }

        private void CreateCells()
        {            
            for (int cellRow = 0; cellRow < ySize; cellRow++)
            {
                for (int cellCol = 0; cellCol < xSize; cellCol++)
                {
                    MazeTile currentCell = new MazeTile()
                    {
                        Y=cellRow,
                        X=cellCol
                    };
                    MazeTiles[cellRow,cellCol] = currentCell; 
                }
            }
            visited = new bool[ySize, xSize];
        }

        public Maze GenerateMaze(int height,int width)
        {
            this.ySize = height;
            this.xSize = width;
            Start();
            maze.Tiles = MazeTiles;

            int X = r.Next(xSize - 1),
            Y = r.Next(ySize - 1);

            currentCell = MazeTiles[Y,X];
            visited[Y,X] = true;
            totalVisitedCells++;

            //Continue going until we have visited every cell
            while (totalVisitedCells < totalCells)
            {
                neighbor = GetNeighboor(maze, visited, currentCell);
                if (visited[neighbor.Y, neighbor.X] == false && visited[currentCell.Y, currentCell.X] == true)
                {
                    RemoveWallBetweenNeighbors(currentCell, neighbor);
                    visited[neighbor.Y, neighbor.X] = true;
                    totalVisitedCells++;
                    visitedCells.Add(currentCell);
                    currentCell = neighbor;
                    lastVisitedCell = visitedCells.Count - 1;
                }
            }
            return maze;
        }

        public Maze DoStep()
        {
            if(totalVisitedCells < totalCells)
            {
                neighbor = GetNeighboor(maze, visited, currentCell);
                if (visited[neighbor.Y, neighbor.X] == false && visited[currentCell.Y, currentCell.X] == true)
                {
                    RemoveWallBetweenNeighbors(currentCell, neighbor);
                    visited[neighbor.Y, neighbor.X] = true;
                    totalVisitedCells++;
                    visitedCells.Add(currentCell);
                    currentCell = neighbor;
                    lastVisitedCell = visitedCells.Count - 1;
                }
            }            
            return maze;
        }

        private MazeTile GetNeighboor(Maze maze, bool[,] visited,MazeTile tile)
        {
            currentCell = tile;
            List<MazeTile> neighboors = maze.GetNeighboorTiles(tile).Where(n=>visited[n.Y,n.X]== false).ToList();
            
            //If we have neighbors set our current neighbor to a random neighbor and set the wall we want to break between those neighbors
            if (neighboors != null && neighboors.Count!=0)
            {
                int randomNeighbor = r.Next(neighboors.Count);
                return neighboors[randomNeighbor];
            }
            else
            {
                //Move back one cell in path
                if (lastVisitedCell > 0)
                {
                    return GetNeighboor(maze, visited, visitedCells[lastVisitedCell--]);
                }
                else
                {
                    return null;
                }
            }
        }

        private void RemoveWallBetweenNeighbors(MazeTile tile1,MazeTile tile2)
        {
            maze.ConnectTiles(tile1, tile2);
        }

        private void SetInitialPositions()
        {
            MazeTiles = new MazeTile[ySize, xSize];

            totalCells = xSize * ySize;
            totalVisitedCells = 0;

            visitedCells = new List<MazeTile>();
            lastVisitedCell = 0;
        }
    }
}
