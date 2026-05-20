using System.Numerics;
using System.Net.Http;
using Raylib_cs;

Game game = new(); // Skapar och startar hela spelet

// Game Klass för gameloop och spel relaterade variabler
class Game
{
    static public int WindowXScale { get; private set; } = 800; // Spel Fönstrets bredd
    static public int WindowYScale { get; private set; } = 800; // spelfönstrets höjd
    public State CurrentState { get; private set; } = new MainMenuState(); // Vilken state spelet ska börja i (Main Menu)
    public State PreviousState { get; private set; } // Förgående state / scen

    public Game()
    {
        Start(); // Körs när game skapas
    }

    private void Start()
    {
        Raylib.InitWindow(WindowXScale, WindowYScale, "Game"); // Fönster storlek samt namn
        Raylib.SetTargetFPS(60); // Sätter fps

        while (!Raylib.WindowShouldClose()) // Kör spel loop tills spelaren stänger fönstret
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.White); // Rensar spel för att rita ut på nytt
            CurrentState.Update(this);
            Raylib.EndDrawing();
        }
    }

    // Byt state metod som sparar ner förgående spelläge samt byter till den nya angivna i parameter.
    // Men också kör Enter metod en gång nör man byter till ny samt Exit metod 1 gång när man byter Spelläge
    public void ChangeState(State state)
    {
        CurrentState.Exit(this);
        PreviousState = CurrentState;
        CurrentState = state;
        CurrentState.Enter(this);
    }
}

// Bas klass för olika spellägen
abstract class State
{
    public virtual void Enter(Game game) // Körs En gång vid bytning av scen
    {
        Console.WriteLine("Entering State");
    }
    public virtual void Exit(Game game) // Körs en gång vid bytning av scene
    {
        Console.WriteLine("Exiting State");
    }
    public abstract void Update(Game game); // Update loop som körs senare i Game klassens spel loop
}

// Meny spelläge
class MainMenuState : State
{
    public override void Update(Game game) // Spellägets utritning som körs i gameloop
    {
        Raylib.ClearBackground(Color.RayWhite); // Ritar backgrund
        Raylib.DrawText("GAME", Game.WindowXScale / 2 - 60, 100, 60, Color.Black); // Ritar ut
        Raylib.DrawText("Press SPACE to start", Game.WindowXScale / 2 - 140, Game.WindowYScale / 2, 24, Color.DarkGray); // Ritar ut Hjälp text
        if (Raylib.IsKeyPressed(KeyboardKey.Space)) // Kollar om spelaren trycker Mellanslag
        {
            game.ChangeState(new GameState()); // Byter till en ny instance av Spelläget
        }
    }
}

// Spelläge
class GameState : State
{
    private List<GameObject> objects = new(); // En dynamisk lista med spel object

    public override void Enter(Game game) // Körs vid scen byte, Så vi lägger till boss och spelaren en gång när spelet startar eller vid scen / state bytning
    {
        Console.WriteLine("Entering Game"); // Ritar ut i konsolen
        objects.Clear();    // Rensar spel objekt
        objects.Add(new Player(400, 400)); // Lägger in en ny instance av spelare i objekt listan med angivna koordinater
        objects.Add(new Boss(150, 150)); // Lägger in en ny instance av Boss i objekt listan med angivna koordinater
    }

    public override void Update(Game game) // Körs i gameloop i game klassen
    {
        // Ritar ut alla GameObjects i objekt listan samt deras update metod.
        foreach (GameObject obj in objects)
        {
            obj.Update(); // Update metod
            obj.Draw(); // Ritar ut metod
        }
        Raylib.DrawText("WASD = move | SPACE = menu", 10, 10, 18, Color.DarkGray); // Ritar ut Hjälptext för spelaren
        if (Raylib.IsKeyPressed(KeyboardKey.Space)) // Kollar vid knappavtryck är mellanslag
        {
            game.ChangeState(new MainMenuState()); // Byta till en ny instance av meny state / scen
        }
    }

    public override void Exit(Game game) // Körs en gång vid scen bytning
    {
        Console.WriteLine("Exiting Game"); // Ritar ut i konsolen
    }
}

// En abstract GameObjekt som bas klass För att kunna snabbt skapa nya spel objekt.
// Samt tvingar en att implementera egen kod för varje ny spel objekt
abstract class GameObject
{
    // Variabler som Gameobject änvder med inkaspling
    private Vector2Int _pos;
    public Vector2Int Pos
    {
        get => _pos;
        protected set => _pos = value;
    }

    public abstract void Update(); // abstract metod för att tvinga en att skriva egen logik för varje nytt objekt
    public virtual void Draw() // Ritar ut objekt metod
    {
        Raylib.DrawText("Object", Pos.X, Pos.Y, 20, Color.Black); // Ritar ut
    }
}


// Spelar klassen som är ett spel objekt
class Player : GameObject
{
    public Player(int x, int y) // Konstruktor som tar emot x och y värden som spelaren start värden
    {
        Pos = new Vector2Int(x, y);
    }

    // Spelarens logik för att Hantera rörelsen
    public override void Update()
    {
        Vector2 direction = Input.Move(); // Hantera spelarens input riktning

        // Räknar ut hur mycket spelaren ska fluytta denna frame
        // baserat på riktning och delta time
        Vector2Int move = new Vector2Int(
            (int)(direction.X * 200 * Raylib.GetFrameTime()),
            (int)(direction.Y * 200 * Raylib.GetFrameTime())
        );
        Pos += move; // Updatera rörelsen
    }

    // Ritar ut spelaren.
    public override void Draw()
    {
        Raylib.DrawRectangle(Pos.X, Pos.Y, 40, 40, Color.Blue);
        Raylib.DrawText("Player", Pos.X, Pos.Y - 20, 14, Color.Blue);
    }
}

// Enemy är en basklas för fiender i spelet
class Enemy : GameObject
{
    // Fiendens Rörelse hastighet
    protected int Speed = 60;

    // Konstruktor som sätter startposition
    public Enemy(int x, int y)
    {
        Pos = new Vector2Int(x, y);
    }

    public override void Update()
    {
        // Flyttar fienden nedåt varje frame
        // Hastigheten görs fps beroende med delta time
        Pos += new Vector2Int(0, (int)(Speed * Raylib.GetFrameTime()));
    }

    // Ritar ut
    public override void Draw()
    {
        Raylib.DrawRectangle(Pos.X, Pos.Y, 30, 30, Color.Red);
    }
}

// Boss är en version av Enemy
class Boss : Enemy
{
    // Konstruktor som använder sig av bas klassens konstruktor så den tar in x och y for position.
    public Boss(int x, int y) : base(x, y)
    {
        Speed = 25; // Sätter bossens hastighet
    }

    // Ritar ut boss
    public override void Draw()
    {
        Raylib.DrawRectangle(Pos.X, Pos.Y, 60, 60, Color.Purple);
        Raylib.DrawText("BOSS", Pos.X, Pos.Y - 20, 16, Color.Purple);
    }
}

// En struktur som är en position me heltal
struct Vector2Int
{
    // X och Y koordinater
    private int _x;
    private int _y;
    // Properties för att läsa koordinaterna
    public int X { get => _x; }
    public int Y { get => _y; }

    // Konstruktor som sätter startvärden
    public Vector2Int(int x, int y)
    {
        _x = x;
        _y = y;
    }

    // Operator overload som gör att två Vector2Int kan adderas
    public static Vector2Int operator +(Vector2Int a, Vector2Int b)
    {
        return new Vector2Int(a.X + b.X, a.Y + b.Y);
    }
}

// strukt för att hantera spelarens input
struct Input
{
    public static Vector2 Move()
    {
        // Startvärde för rörelseriktning
        int x = 0;
        int y = 0;

        // A = vänster
        if (Raylib.IsKeyDown(KeyboardKey.A)) x -= 1;
        // D = höger
        if (Raylib.IsKeyDown(KeyboardKey.D)) x += 1;
        // W = upp
        if (Raylib.IsKeyDown(KeyboardKey.W)) y -= 1;
        // S = ner
        if (Raylib.IsKeyDown(KeyboardKey.S)) y += 1;
        // Returnera rörelseriktningen som en vector
        return new Vector2(x, y);
    }
}
