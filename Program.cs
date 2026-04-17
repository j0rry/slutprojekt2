using System.Numerics;
using System.Net.Http;
using Raylib_cs;

Game game = new();

class Game
{
    static public int WindowXScale { get; private set; } = 800;
    static public int WindowYScale { get; private set; } = 800;
    public State CurrentState { get; private set; } = new MainMenuState();
    public State PreviousState { get; private set; }

    public Game()
    {
        Start();
    }

    private void Start()
    {
        Raylib.InitWindow(WindowXScale, WindowYScale, "Game");
        Raylib.SetTargetFPS(60);

        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.White);
            CurrentState.Update(this);
            Raylib.EndDrawing();
        }
    }

    public void ChangeState(State state)
    {
        CurrentState.Exit(this);
        PreviousState = CurrentState;
        CurrentState = state;
        CurrentState.Enter(this);
    }
}

abstract class State
{
    public virtual void Enter(Game game)
    {
        Console.WriteLine("Entering State");
    }
    public virtual void Exit(Game game)
    {
        Console.WriteLine("Exiting State");
    }
    public abstract void Update(Game game);
}

class MainMenuState : State
{
    public override void Update(Game game)
    {
        Raylib.ClearBackground(Color.RayWhite);
        Raylib.DrawText("GAME", Game.WindowXScale / 2 - 60, 100, 60, Color.Black);
        Raylib.DrawText("Press SPACE to start", Game.WindowXScale / 2 - 140, Game.WindowYScale / 2, 24, Color.DarkGray);
        if (Raylib.IsKeyPressed(KeyboardKey.Space))
        {
            game.ChangeState(new GameState());
        }
    }
}

class GameState : State
{
    private List<GameObject> objects = new();

    public override void Enter(Game game)
    {
        Console.WriteLine("Entering Game");
        objects.Clear();
        objects.Add(new Player(400, 400));
        objects.Add(new Boss(150, 150));
    }

    public override void Update(Game game)
    {
        foreach (GameObject obj in objects)
        {
            obj.Update();
            obj.Draw();
        }
        Raylib.DrawText("WASD = move | SPACE = menu", 10, 10, 18, Color.DarkGray);
        if (Raylib.IsKeyPressed(KeyboardKey.Space))
        {
            game.ChangeState(new MainMenuState());
        }
    }

    public override void Exit(Game game)
    {
        Console.WriteLine("Exiting Game");
    }
}

abstract class GameObject
{
    private Vector2Int _pos;
    public Vector2Int Pos
    {
        get => _pos;
        protected set => _pos = value;
    }

    public abstract void Update();
    public virtual void Draw()
    {
        Raylib.DrawText("Object", Pos.X, Pos.Y, 20, Color.Black);
    }
}

class Player : GameObject
{
    public Player(int x, int y)
    {
        Pos = new Vector2Int(x, y);
    }

    public override void Update()
    {
        Vector2 direction = Input.Move();
        Vector2Int move = new Vector2Int(
            (int)(direction.X * 200 * Raylib.GetFrameTime()),
            (int)(direction.Y * 200 * Raylib.GetFrameTime())
        );
        Pos += move;
    }

    public override void Draw()
    {
        Raylib.DrawRectangle(Pos.X, Pos.Y, 40, 40, Color.Blue);
        Raylib.DrawText("Player", Pos.X, Pos.Y - 20, 14, Color.Blue);
    }
}

class Enemy : GameObject
{
    protected int Speed = 60;

    public Enemy(int x, int y)
    {
        Pos = new Vector2Int(x, y);
    }

    public override void Update()
    {
        Pos += new Vector2Int(0, (int)(Speed * Raylib.GetFrameTime()));
    }

    public override void Draw()
    {
        Raylib.DrawRectangle(Pos.X, Pos.Y, 30, 30, Color.Red);
    }
}

class Boss : Enemy
{
    public Boss(int x, int y) : base(x, y)
    {
        Speed = 25;
    }

    public override void Draw()
    {
        Raylib.DrawRectangle(Pos.X, Pos.Y, 60, 60, Color.Purple);
        Raylib.DrawText("BOSS", Pos.X, Pos.Y - 20, 16, Color.Purple);
    }
}

struct Vector2Int
{
    private int _x;
    private int _y;

    public int X { get => _x; }
    public int Y { get => _y; }

    public Vector2Int(int x, int y)
    {
        _x = x;
        _y = y;
    }

    public static Vector2Int operator +(Vector2Int a, Vector2Int b)
    {
        return new Vector2Int(a.X + b.X, a.Y + b.Y);
    }
}

struct Input
{
    public static Vector2 Move()
    {
        int x = 0;
        int y = 0;
        if (Raylib.IsKeyDown(KeyboardKey.A)) x -= 1;
        if (Raylib.IsKeyDown(KeyboardKey.D)) x += 1;
        if (Raylib.IsKeyDown(KeyboardKey.W)) y -= 1;
        if (Raylib.IsKeyDown(KeyboardKey.S)) y += 1;
        return new Vector2(x, y);
    }
}
