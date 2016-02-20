using Canvas_Window_Template;
using Canvas_Window_Template.Basic_Drawing_Functions;
using Canvas_Window_Template.Drawables;
using Canvas_Window_Template.Factories;
using Canvas_Window_Template.Interfaces;
using Canvas_Window_Template.Utilities;
using PerfectMazeSolver;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace WallMaze
{
    public partial class WallMaze : ReadyOpenGlTemplate
    {
        GameController MyController;
        public int xLength = MapFactory.defaultWidth,
            yLength = MapFactory.defaultLength;
        private selectorObj mySelector;
        TileMap Map;
        tileObj PlayerTile;
        CharacterImage pc;
        List<IBlocker> Walls;
        public bool Running { get; private set; }
        GameType gameType;
        double offset;
        List<tileObj> Path = new List<tileObj>();

        int coneLength = 20 * MapFactory.defaultLength;
        int coneAngle = 60;
        int bombRadius = MapFactory.defaultLength/2;
        int laserLength = MapFactory.defaultLength;
        int nadeStrength = 6;
        int nadeRadius = 20;

        public WallMaze()
        {
            InitializeComponent();
            MyView.InitializeContexts();
            
            mySelector = new selectorObj(this.simpleOpenGlView1);
            this.KeyPreview = true;
        }

        void InitializeController()
        {
            MyController = new GameController(yLength,xLength);
            MyController.adjacentChecker = TileMap.AreAdjacent;
            MyController.obstacleChecker = this.HaveObstaclesInBetween;
        }

        public override void Setup()
        {
            InitializeController();
            Map = MapFactory.CreateMap();
            Walls = new List<IBlocker>();
            MyWorld.removeAllEntities();
            MyWorld.add(Map.GetTileList());
            Running = true;
            
            offset = Map.MyOrigin.X;
            MyController.CreateWalls(gameType);            
            createWallObjects();      

            Point characterPosition = MyController.CreateCharacter(xLength, yLength);
            pc = GuardFactory.CreateGuard(characterPosition, Map);
            MyWorld.add(pc);

            PlayerTile = Map.MyTiles[characterPosition.X, characterPosition.Y];
            
        }

        void ResetWalls()
        {
            foreach (IBlocker b in Walls)
                MyWorld.remove(b);
            Walls = new List<IBlocker>();
        }

        void createWallObjects()
        {
            for (int i = 0; i < xLength; i++)
            {
                for (int j = 0; j < yLength; j++)
                {
                    for (int k = 0; k <= MyController.Walls.GetUpperBound(2); k++)
                    {
                        if (MyController.Walls[i, j, k])
                        {
                            SlashType type = (SlashType)k;
                            rectangleObj rect = MapFactory.createWallTile(offset, i, j, type);
                            Walls.Add(rect);
                            MyWorld.add(rect);
                        }
                    }
                }
            }
        }

        public override void drawingLoop()
        {
            Common myDrawer = new Common();
            MyView.setCameraView(simpleOpenGlView.VIEWS.FrontUp);

            while (!MyView.isDisposed() && !this.IsDisposed && Running)
            {
                MyView.setupScene();
                //DRAW SCENE HERE                

                myDrawer.drawWorld(MyWorld);
                //END DRAW SCENE HERE
                MyView.flushScene();
                this.Refresh();
                Application.DoEvents();
            }
        }

        private void _Click(object sender, MouseEventArgs e)
        {
            //Check all objects, see if any was selected
            int id = mySelector.getSelectedObjectId(new int[] { e.X, e.Y }, MyWorld);
            IDrawable selectedObject = MyWorld.getEntity(id);
            tileObj selectedTile;
            //Check type
            if (id > -1)
            {
                if(selectedObject is tileObj)
                {
                    selectedTile = (tileObj)selectedObject;
                    TryMoveTo(selectedTile);
                }                
            }
        }

        private void TryMoveTo(tileObj tile)
        {
            Point tilePosition = Map.GetTilePosition(tile);
            bool moved = MyController.TryMoveTo(tilePosition);
            if (moved)
            {
                pc.Move(tile.origin);
                PlayerTile = tile;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Running = false;
            this.Start();
        }

        private void _KeyPress(object sender, KeyPressEventArgs e)
        {
            Point dest=new Point(-1,-1);
            switch (e.KeyChar)
            {
                case 'w':
                case 'W':
                    dest = new Point(MyController.CharacterPosition.X, MyController.CharacterPosition.Y + 1); 
                    break;
                case 'a':
                case 'A':
                    dest = new Point(MyController.CharacterPosition.X - 1, MyController.CharacterPosition.Y);
                    break;
                case 's':
                case 'S':
                    dest = new Point(MyController.CharacterPosition.X, MyController.CharacterPosition.Y - 1);
                    break;
                case 'd':
                case 'D':
                    dest = new Point(MyController.CharacterPosition.X + 1, MyController.CharacterPosition.Y);
                    break;
                case 'e':
                case 'E':
                    pc.Turn();
                    break;
                case 'q':
                case 'Q':
                    pc.Turn(true);
                    break;
                case 'f':
                case 'F':
                    TryBreakWall();
                    break;
                case 'x':
                case 'X':
                    TryShootWalls();
                    break;
                case 'b':
                case 'B':
                    TryBlowStuffUp(bombRadius);
                    break;
                case 'c':
                case 'C':
                    TryShootCone();
                    break;
                case 'G':
                case 'g':
                    TryThrowNade();
                    break;
            }

            try
            {
                tileObj destTile = Map.MyTiles[dest.Y, dest.X];
                TryMoveTo(destTile);
            }
            catch
            {

            }
        }

        private void TryThrowNade()
        {
            IPoint sourceTile = this.PlayerTile.Center;
            tileObj landingTile = Map.getTilePositionByDistanceAndOrientation(this.PlayerTile,nadeStrength, pc.MyOrientation);
            if(landingTile!=null)
                BlowStuffAroundPoint(nadeRadius, landingTile.origin.toArray());
        }

        private void TryBlowStuffUp(int radius=5)
        {
            double[] charP = PlayerTile.getPosition();
            BlowStuffAroundPoint(radius, charP);
        }

        private void BlowStuffAroundPoint(int radius,double[] center)
        {
            List<IBlocker> toRemove = new List<IBlocker>();
            foreach (IBlocker wall in Walls)
            {
                double[] wallP = wall.getPosition();
                if (VectorMath.GetVectorDistance(center, wallP) <= radius * Map.TileSize)
                {
                    toRemove.Add(wall);
                }
            }
            foreach (IBlocker wall in toRemove)
            {
                Walls.Remove(wall);
                MyWorld.remove(wall);
            }
        }

        private void TryShootWalls()
        {
            IPoint sourceTile = this.PlayerTile.Center,
                endPoint = GetConeEndpoints(sourceTile, pc.MyOrientation, 10)[0];
            RemoveObstacles(sourceTile, endPoint);
        }

        private void TryBreakWall()
        {
            Point wallOrigin = MyController.GetAdjacentPoint(pc.MyOrientation);
            IBlocker wall = MapFactory.GetObstacle(wallOrigin, MyController.CharacterPosition, Map, Walls);
            if (wall != null)
            {
                Walls.Remove(wall);
                MyWorld.remove((IDrawable)wall);
            }
        }

        private void TryShootCone()
        {
            IPoint sourceTile = this.PlayerTile.Center;
            List<IPoint> endpoints = GetConeEndpoints(sourceTile, pc.MyOrientation);
            foreach(IPoint p in endpoints)
            {
                RemoveObstacles(sourceTile, p);
            }
        }

        private IPoint GetLineOfFireEndpoint(IPoint src,Orientation orientation)
        {
            switch (orientation)
            {
                case Orientation.Up:
                    return new pointObj(src.X, Map.MyWidth*Map.TileSize, src.Z);
                case Orientation.Right:
                    return new pointObj(Map.MyHeight*Map.TileSize, src.Y, src.Z);
                case Orientation.Down:
                    return new pointObj(src.X, -Map.MyWidth * Map.TileSize, src.Z);
                case Orientation.Left:
                    return new pointObj(-Map.MyHeight * Map.TileSize, src.Y, src.Z);
                default:
                    return new pointObj(src.X, src.Y, src.Z);
            }
        }        

        private List<IPoint> GetConeEndpoints(IPoint src,Orientation orientation,int angle=-1)
        {
            List<IPoint> endpoints = new List<IPoint>();
            int startAngle = -((int)orientation*45+270);
            if (angle == -1)
                angle = coneAngle;

           
            for (double i = startAngle - angle / 2; i <= startAngle + angle / 2; i += 0.1)
            {
                double radians = i / 180 * Math.PI;
                endpoints.Add(new pointObj(src.X + coneLength * Math.Cos(radians),
                                            src.Y + coneLength * Math.Sin(radians),
                                            src.Z));
            }
            return endpoints;
        }

        private void RemoveObstacles(IPoint source,IPoint destination)
        {
            List<IBlocker> walls = MapFactory.GetObstacles(source, destination, Walls);
            if (walls != null)
            {
                foreach (IBlocker wall in walls)
                {
                    Walls.Remove(wall);
                    MyWorld.remove((IDrawable)wall);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            gameType = GameType.PerfectMaze;
            Running = false;
            Start();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            MyController.GetStartEndTiles();
            MyController.Solve();
            MazeTile first = MyController.solvedPath[0],
                last = MyController.solvedPath.FindLast(n=>true);
            foreach (MazeTile tile in MyController.solvedPath)
                Map.MyTiles[tile.Y, tile.X].MyColor = Common.colorBlue;
            Map.MyTiles[first.Y, first.X].MyColor = Common.colorOrange;
            Map.MyTiles[last.Y, last.X].MyColor = Common.colorOrange;
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(gameType==GameType.PerfectMaze)
            {
                MyController.DoStep();
                ResetWalls();
                createWallObjects();
                tileObj destTile = Map.MyTiles[MyController.CharacterPosition.X, MyController.CharacterPosition.Y];
                pc.Move(destTile.origin);
                PlayerTile = destTile;
            }
        }

        public bool HaveObstaclesInBetween(Point src,Point dest)
        {
            return MapFactory.NoObstacles(src, dest, Map, Walls);
        }
   
    }  
    
    public delegate bool AdjacentChecker(Point charPosition,Point tilePosition);
    public delegate bool ObstacleChecker(Point source,Point destination);

    public class GameController
    {
        public bool[,,] Walls;
        public Point CharacterPosition;
        public AdjacentChecker adjacentChecker;
        public ObstacleChecker obstacleChecker;
        PerfectMazeGenerator mazeGenerator = new PerfectMazeGenerator();
        public List<MazeTile> solvedPath;
        MazeTile t1, t2;

        public GameController(int yLength,int xLength)
        {
            Walls = new bool[yLength, xLength, 2];
        }

        public bool[,,] CreateWalls(GameType gameType)
        {
            Random rand = new Random();
            switch (gameType)
            {
                case GameType.Normal:
                    for (int i = 0; i < Walls.GetUpperBound(0)+1; i++)
                    {
                        for (int j = 0; j < Walls.GetUpperBound(1) + 1; j++)
                        {
                            if (rand.Next(2) == 0)
                            {

                                Walls[i, j, 0] = true;
                            }
                            else
                            {
                                Walls[i, j, 1] = true;
                            }
                        }
                    }
                    break;
                case GameType.PerfectMaze:
                    Maze maze = (mazeGenerator).GenerateMaze(Walls.GetUpperBound(0) + 1, Walls.GetUpperBound(1) + 1);
                    Walls = (MazeVisualizer.CreateWalls(maze));
                    break;
            }
            
            return Walls;
        }

        public Point CreateCharacter(int width,int length)
        {
            Random rand = new Random();
            int x = rand.Next(width), y = rand.Next(length);
            CharacterPosition = new Point(x, y);
            return CharacterPosition;
        }

        public bool TryMoveTo(Point dest)
        {
            if(adjacentChecker(CharacterPosition,dest))
            {
                if(obstacleChecker(CharacterPosition,dest))
                {
                    CharacterPosition = dest;
                    return true;
                }
            }
            return false;
        }

        public Point GetAdjacentPoint(Orientation orientation)
        {
            switch(orientation)
            {
                case Orientation.Up:
                    return new Point(CharacterPosition.X, CharacterPosition.Y + 1);
                case Orientation.Right:
                    return new Point(CharacterPosition.X + 1, CharacterPosition.Y);
                case Orientation.Down:
                    return new Point(CharacterPosition.X, CharacterPosition.Y - 1);
                case Orientation.Left:
                    return new Point(CharacterPosition.X - 1, CharacterPosition.Y);
                default:
                    return new Point();
            }
        }

        internal void DoStep()
        {
            Walls = MazeVisualizer.CreateWalls(mazeGenerator.DoStep());
            CharacterPosition = new Point(mazeGenerator.currentCell.Y, mazeGenerator.currentCell.X);
        }

        public void GetStartEndTiles()
        {
            Maze maze = mazeGenerator.maze;
            t1 = maze.GetRandomTile();
            t2 = maze.GetRandomTile();
        }
        public void Solve()
        {
            IMazeSolver solver = new AStarSolver();  
            solvedPath = solver.Solve(mazeGenerator.maze, t1,t2);
        }
    }

    public enum GameType
    {
        Normal,
        PerfectMaze
    }
}
            