using BlazorUI.Extension;

namespace BlazorUI.Engine;

public class FiveInLine
{
    private int points = 0;
    public int Points{get{return points;}}

    public string Message{get; set;}
    public int StartBallsOnBoard {get;}
    public int NumberOfNextBall {get;}
    private CellModel[,] Board {get;}
    public CellModel[] NextBals {get;}
    public int Board_w {get;}
    public int Board_h {get;}
    public int NumberBallColors {get;}

    private Position? P_Start{get; set;} = null;
    private Position? P_End{get; set;} = null;

    private bool GameOver = false;

    private Move[] Moves = new Move[]
    {
        new Move{Move_X = 1, Move_Y = 0}, //1
        //new Move{Move_X = 1, Move_Y = -1},//2
        new Move{Move_X = 0, Move_Y = -1},//3
        //new Move{Move_X = -1, Move_Y = -1},//4
        new Move{Move_X = -1, Move_Y = 0},//5
        //new Move{Move_X = -1, Move_Y = 1},//6
        new Move{Move_X = 0, Move_Y = 1},//7       
        //new Move{Move_X = 1, Move_Y = 1},//8   
    };

    public FiveInLine() : this(9,9,6,3,8)
    { 
        
    }

    public FiveInLine(int w, int h, int startBallsonBoard, int numberOfNextBall, int numberBallColors) 
    { 
        Board_w = w;
        Board_h = h;
        StartBallsOnBoard = startBallsonBoard;
        NumberOfNextBall = numberOfNextBall;
        NumberBallColors = numberBallColors;
        NextBals = new CellModel[NumberOfNextBall];
        Board = new CellModel[Board_w,Board_h];
        Message = string.Empty;
        points = 0;
        
        InitNextBalls();
        InitBoard();
    }

    public CellModel GetCell(int i, int k){
        return Board[i,k];
    }

    public Position[] MinPath(Position p1, Position p2)
    {
        if(!IsValidPosistion(p1) || !IsValidPosistion(p2) )
            return Array.Empty<Position>();

        var moveBoard = MinPathInitialMatrix(p1, p2);
        if(moveBoard == null)
            return Array.Empty<Position>();
            
        bool founded = false;
        var currentPosition = p2.Clone();
        var path = new List<Position>(){p2};
        while(!founded)
        {
            for (int m = 0; m < Moves.Length; m++)
            {
                var movePosition = MovePosition(currentPosition, Moves[m]);
                founded = movePosition.Equals(p1);
                if(founded || (IsValidPosistion(movePosition) && moveBoard[movePosition.X, movePosition.Y] == moveBoard[currentPosition.X, currentPosition.Y] - 1))
                {
                    path.Add(movePosition);
                    currentPosition = movePosition.Clone();
                    break;
                }
            }
        }

        return path.ToArray();
    }

    public void SelectCell(Position p)
    {
        if(GameOver)
            return;

        if(P_Start == null)
        {
            if(Board.Get(p).HasBall)
            {
                P_Start = p;
                Board.Get(p).Selected = !Board.Get(p).Selected;
            }
        }
        else if(P_End == null && !Board.Get(p).HasBall)
        {
            P_End = p;
            var path = MinPath(P_Start, P_End);
            if(path != null && path.Any() && P_Start!=null && P_End!=null)
            {
                Board.Get(P_End).Ball = Board.Get(P_Start).Ball;
                Board.Get(P_Start).Ball = string.Empty;
                Board.Get(P_Start).Selected = false;
                Check5InLine(P_End);
                GenerateNextBalls();
                Message = string.Empty;

                if(RenewFreePositions().Count < NumberOfNextBall)
                {
                    GameOver = true;
                    Message = "Game Over";
                }
            }
            else
            {
                Message = "No Path";
            }
            CleanPath();
        }
        else if(P_End == null && Board.Get(p).HasBall)
        {
            CleanPath();
            Message = "Wrong selection, there is a ball.";
        }
        else
        {
            CleanPath();
            Message = "Something wrong";
        }
    }

    private void Check5InLine(Position p)
    {
        System.Console.WriteLine("Start");
        //check up down
        System.Console.WriteLine("check up down");
        points += Check5InLine(p, 0, 1, 0, -1);
        //check right left
        System.Console.WriteLine("check right left");
        points += Check5InLine(p, 1, 0, -1, 0);
        //check right up left down
        System.Console.WriteLine("check right up left down");
        points += Check5InLine(p, 1, -1, -1, 1);
        //check right down left up
        System.Console.WriteLine("check right down left up");
        points += Check5InLine(p, -1, -1, 1, 1);
    }

    private int Check5InLine(Position p, int mXup, int mYup, int mXdown, int mYdown)
    {
        var up_donw = new List<Position>(){p};
        var p_up = p.Clone();
        var p_down = p.Clone();
        var has_up = true;
        var has_down = true;
        var p_cell = Board.Get(p);
        while(has_up || has_down)
        {
            p_up.X += mXup;
            p_up.Y += mYup;
            p_down.X += mXdown;
            p_down.Y += mYdown;
            has_down &= IsValidPosistion(p_down) && Board.Get(p_down).HasBall && Board.Get(p_down).Equals(p_cell);
            has_up &= IsValidPosistion(p_up) && Board.Get(p_up).HasBall && Board.Get(p_up).Equals(p_cell);
            if(has_down)
            {
                up_donw.Add(p_down.Clone());
            }

            if(has_up)
            {
                up_donw.Add(p_up.Clone());
            }
        }
        var result = 0;
        if(up_donw.Count>=5)
        {
            foreach (var item in up_donw)
            {
                Board.Get(item).Ball = string.Empty;
            }
            result =  up_donw.Count;
        }
        System.Console.WriteLine(result);
        return result;
    }

    private void GenerateNextBalls()
    {
        var rnd = GetRandomListPosition(NumberOfNextBall);
        if(rnd.Length < NumberOfNextBall)
        {
            GameOver = true;
            Message = "Game Over";
            return;
        }
        AddBallsBoard(rnd, NextBals.Select(x=>x.Ball).ToArray());
        InitNextBalls();
    }

    private void CleanPath()
    {
        if(P_Start!=null)
            Board[P_Start.X, P_Start.Y].Selected = false;
        P_Start = null;
        P_End = null;
    }

    private List<Position> InitFreePositions()
    {
        List<Position> FreePositions = new List<Position>();
        for (int h = 0; h < Board_h; h++)
        {
            for (int w = 0; w < Board_w; w++)
            {
                FreePositions.Add(new Position{X = w, Y = h});
            }
        } 
        return FreePositions;  
    }

    private List<Position> RenewFreePositions()
    {
        List<Position> FreePositions = new List<Position>();
        for (int h = 0; h < Board_h; h++)
        {
            for (int w = 0; w < Board_w; w++)
            {
                if(!Board[w,h].HasBall)
                    FreePositions.Add(new Position{X = w, Y = h});
            }
        } 
        return FreePositions;
    }

    private void InitBoard()
    {  
        for (int h = 0; h < Board_h; h++)
        {
            for (int w = 0; w < Board_w; w++)
            {
                Board[w,h] = new CellModel();
            }
        }

        var posRan = GetRandomListPosition(StartBallsOnBoard, true);
        AddBallsBoard(posRan);
    }

    private void InitNextBalls()
    { 
        for (int i = 0; i < NumberOfNextBall; i++)
        {
            NextBals[i] = new CellModel{ Ball = RandomBallColor() };
        }
    }

    private Position[] GetRandomListPosition(int n, bool isNew = false)
    {
        List<Position> FreePositions = isNew ? InitFreePositions() : RenewFreePositions();

        if(FreePositions.Count<n)
            return Enumerable.Empty<Position>().ToArray();

        var posRan = new Position[n];
        for (int i = 0; i < n; i++)
        {
            var pos = Random.Shared.Next(0, FreePositions.Count);
            posRan[i] = FreePositions[pos];
            FreePositions.RemoveAt(pos);
        }
        return posRan;
    }

    private void AddBallsBoard(Position[] ps, string[]? colors = null)
    {
        for (int i = 0; i < ps.Length; i++)
        {
            Board[ps[i].X,ps[i].Y].Ball = colors == null ? RandomBallColor() : colors[i];
        }
    }

    private int[,]? MinPathInitialMatrix(Position p1, Position p2)
    {
        var moveBoard = new int[Board_w, Board_h];
        var nextPositions = new List<Position>(){p1};
        moveBoard[p1.X, p1.Y] = 1;
        int goCounter = 2;
        bool founded = false;
        while (nextPositions.Count() != 0 && !founded)
        {
            var tempNextPositions = new List<Position>();
            for (int p = 0; p < nextPositions.Count; p++)
            {
                for (int m = 0; m < Moves.Length; m++)
                {
                    var movePosition = MovePosition(nextPositions[p], Moves[m]);
                    if(movePosition.Equals(p2))
                    {
                        founded = true;
                        moveBoard[movePosition.X, movePosition.Y] = goCounter;
                        break;
                    }

                    if(IsValidPosistion(movePosition) && !Board[movePosition.X, movePosition.Y].HasBall && moveBoard[movePosition.X, movePosition.Y] == 0)
                    {
                        moveBoard[movePosition.X, movePosition.Y] = goCounter;
                        tempNextPositions.Add(movePosition);
                    }
                }
            }
            nextPositions.Clear();
            if(tempNextPositions.Any())
                nextPositions.AddRange(tempNextPositions);
            goCounter++;
        } 

        return founded ? moveBoard : null;
    }

    

    private bool IsValidPosistion(Position p) => p.X > -1 && p.X < Board_w && p.Y > -1 && p.Y < Board_h;
    private Position MovePosition(Position p, Move m) => new Position{X = p.X + m.Move_X, Y = p.Y + m.Move_Y};

    private string RandomBallColor()=>$"b{Random.Shared.Next(1, NumberBallColors)}";
    
}



public class CellModel{
    public string Ball {get; set;} = string.Empty;
    public bool HasBall {get {return !string.IsNullOrEmpty(Ball);}}
    public bool Selected {get; set;} = false;

    public bool Equals(CellModel c)
    {
        return this.Ball == c.Ball;
    }
}

public class Position{
    public int X {get; set;}
    public int Y {get; set;}

    public Position Clone()
    {
        return new Position{X = X, Y = Y};
    }

    public bool Equals(Position p)
    {
        return this.X == p.X && this.Y == p.Y;
    }
}

public class Move{
    public int Move_X {get; set;}
    public int Move_Y {get; set;}
}