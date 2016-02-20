using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using PerfectMazeSolver;

namespace MazeSolverTests
{
    [TestClass]
    public class ASolverTests
    {
        [TestMethod]
        public void ASolverTest1()
        {
            int[][] tiles=new int[4][];

            tiles[0] = new int[] { 0, 0, 1, 0, 0, 0 };
            tiles[1] = new int[] { 1, 0, 0, 1, 1, 0 };
            tiles[2] = new int[] { 1, 1, 0, 0, 1, 1 };
            tiles[3] = new int[] { 0, 1, 1, 0, 0, 0 };

            Maze maze = (new MazeFactory()).CreateMaze(tiles, 2, 2);
            List<MazeTile> path = (new AStarSolver()).Solve(maze, maze.Tiles[0, 0], maze.Tiles[0, 1]);

            Assert.AreEqual(4, path.Count);
        }

        [TestMethod]
        public void ASolverTest2()
        {
            int size = 100;
            Maze maze = (new MazeFactory()).CreateMaze(MazeType.Snake, size, size);
            List<MazeTile> path = (new AStarSolver()).
                Solve(maze, maze.Tiles[0, 0], maze.Tiles[0, size-1]);

            Assert.AreEqual(size*size, path.Count);
        }

        [TestMethod]
        public void CustomSolverTest1()
        {
            int size = 80;
            Maze maze = (new MazeFactory()).CreateMaze(MazeType.Snake, size, size);
            List<MazeTile> path = (new CustomSolver()).
                Solve(maze, maze.Tiles[0, 0], maze.Tiles[0, size - 1]);

            Assert.AreEqual(size * size, path.Count);
        }
    }
}
