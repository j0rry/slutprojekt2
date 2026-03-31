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
