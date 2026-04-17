using System.Numerics;
using Raylib_cs;

Game game = new();

class Game
{
    static public int WindowXScale { get; private set; } = 800;
    static public int WindowYScale { get; private set; } = 800;
    public State currentState { get; private set; } = new MainMenuState();
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
            currentState.Update(this);
            Raylib.EndDrawing();
        }
    }

    public void ChangeState(State state)
    {
        currentState.Exit(this);
        PreviousState = currentState;
        currentState = state;
        currentState.Enter(this);
    }
}

abstract class State
{
    public virtual void Enter(Game game)
    {
        Console.WriteLine($"Entering State");
    }
    public virtual void Exit(Game game)
    {
        Console.WriteLine($"Exiting State");
    }
    public abstract void Update(Game game);
}

class MainMenuState : State
{
    public override void Update(Game game)
    {
        Raylib.ClearBackground(Color.Red);
        Raylib.DrawRectangle((Game.WindowXScale / 2) - 100, (Game.WindowYScale / 2) - 100, 100, 100, Color.Blue);
        if (Raylib.IsKeyPressed(KeyboardKey.Space))
        {
            game.ChangeState(new GameState());
        }
    }
}

class GameState : State
{

    public override void Update(Game game)
    {
        Player p = new(400, 400);
        p.Update();
        p.Draw();
        if (Raylib.IsKeyPressed(KeyboardKey.Space))
        {
            game.ChangeState(new MainMenuState());
        }

    }

    public override void Enter(Game game)
    {
        Console.WriteLine("Entering Game");
    }
    public override void Exit(Game game)
    {
        Console.WriteLine("Exiting Game");
    }

}

abstract class GameObject
{
    public Vector2Int pos;
    public abstract void Update();
    public virtual void Draw()
    {
        Raylib.DrawText("GameObject", pos.X, pos.Y, 50, Color.Black);
    }
}

class Player : GameObject
{

    public Player(int x, int y)
    {
        pos.X = x;
        pos.Y = y;
    }

    public override void Update()
    {
        Vector2 direction = Input.Move();


        Vector2Int move = new Vector2Int(
            (int)(direction.X * 10 * Raylib.GetFrameTime()),
            (int)(direction.Y * 10 * Raylib.GetFrameTime())
        );

        pos += move;
    }
}

struct Vector2Int
{
    public int X;
    public int Y;

    public Vector2Int(int x, int y)
    {
        X = x;
        Y = y;
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
