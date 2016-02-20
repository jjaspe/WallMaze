using System.Collections.Generic;

namespace PerfectMazeSolver
{
    public interface IMazeSolver
    {
        List<MazeTile> Solve(Maze maze, MazeTile start, MazeTile end);
    }
}