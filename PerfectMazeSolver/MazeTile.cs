using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfectMazeSolver
{
    public class MazeTile
    {
        public int X { get; set; }
        public int Y { get; set; }
        public List<Neighbors> MyNeighboors = new List<Neighbors>();

        public override bool Equals(object obj)
        {
            if (obj is MazeTile)
            {
                return (obj as MazeTile).X == this.X && (obj as MazeTile).Y == this.Y;
            }
            else
                return base.Equals(obj);
        }
    }
}
