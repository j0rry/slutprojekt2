using Raylib_cs;

static class Game
{
    static public int WindowXScale { get; private set; } = 800;
    static public int WindowYScale { get; private set; } = 800;
    public static void Start()
    {
        Raylib.InitWindow(WindowXScale, WindowYScale, "Game");
        Raylib.SetTargetFPS(60);

        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.White);

            Raylib.EndDrawing();
        }
    }
}
