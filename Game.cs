using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;

namespace ColorMindGame
{
    class Game
    {
        const int WIDTH = 600;
        const int HEIGHT = 900;

        public const int PIECE_AMOUNT = 4;

        Random random = new Random();

        enum Phase
        {
            Intro,
            ColorSelect,
            GameEnd,
            Gameplay,
            Info
        }

        Phase currentPhase;

        Button botButton;
        Button humanButton;

        Button infoButton;
        Button infoBackButton;
        string[] infoText =
        {
            "In ColorMindGame, one player is trying to guess the colors the other one has chosen.",
            "The player has 10 guesses, and some clues as to how close they are after each guess.",
            "Black Peg: One color in the sequence has the right color, but wrong position.",
            "White Peg: One color in the sequence has the right color and position.",
            "Once the colors match the secret sequence, you win! If not within 10 turns: you lose!",
            "Bot Player: The secret sequence is chosen at random.",
            "Human Player: Another human secretly chooses the sequence.",
            "SPACE: If wanted, the secret sequence can be revealed.",
            "R: If something goes wrong, you can reset at the intro screen at any time.",
            "BACKSPACE: If you want to try other colors, you can reset them to all be empty.",
            "Tap the Back button or R to return to the intro-screen."
        };

        Sector[] sectors = new Sector[10];
        Sector colorSelect;
        Sector secretSector;
        Sector activeSector;

        public Game()
        {
            Raylib.InitWindow(WIDTH, HEIGHT, "Color Mind Game");
            Raylib.SetTargetFPS(30);

            botButton = new Button(100, 400, 150, 100, "Bot Player", Color.RAYWHITE);
            humanButton = new Button(350, 400, 150, 100, "Human Player", Color.RAYWHITE);
            infoButton = new Button(200, 600, 150, 100, "Info / Help", Color.RAYWHITE);
            infoBackButton = new Button(200, HEIGHT-150, 150, 100, "Back", Color.RAYWHITE);

            for (int i = 0; i < sectors.Length; i++)
            {
                sectors[i] = new Sector(0, i * HEIGHT / (sectors.Length + 1) + 5, WIDTH, HEIGHT / (sectors.Length + 1) - 10);
            }
            secretSector = new Sector(0, sectors.Length * HEIGHT / (sectors.Length + 1) + 5, WIDTH, HEIGHT / (sectors.Length + 1) - 10);
            colorSelect = new Sector(0, HEIGHT / 2 - 50, WIDTH, HEIGHT / (sectors.Length + 1) - 10);
        }

        public void Play()
        {
            currentPhase = Phase.Intro;

            while (!Raylib.WindowShouldClose())
            {
                Update();
                Display();
            }
            Raylib.WindowShouldClose();
        }

        private void Update()
        {
            bool mousePressed = Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON) || Raylib.IsMouseButtonPressed(MouseButton.MOUSE_RIGHT_BUTTON);
            switch (currentPhase)
            {
                case Phase.Intro:
                    if (mousePressed)
                    {
                        if (botButton.CheckClicked(Raylib.GetMousePosition()))
                        {
                            GenerateSecretSector();
                            activeSector = sectors[0];
                            currentPhase = Phase.Gameplay;
                        }
                        if (humanButton.CheckClicked(Raylib.GetMousePosition()))
                        {
                            currentPhase = Phase.ColorSelect;
                        }
                        if (infoButton.CheckClicked(Raylib.GetMousePosition()))
                        {
                            currentPhase = Phase.Info;
                        }
                    }
                    break;

                case Phase.ColorSelect:
                    if (mousePressed)
                    {
                        colorSelect.CheckMouseClick(Raylib.GetMousePosition());
                    }
                    if (Raylib.IsKeyPressed(KeyboardKey.KEY_BACKSPACE))
                    {
                        colorSelect.ClearColors();
                    }
                    if (colorSelect.Confirmed)
                    {
                        ResolveConfirmedSector();
                        activeSector = sectors[0];
                        currentPhase = Phase.Gameplay;
                    }
                    if (Raylib.IsKeyPressed(KeyboardKey.KEY_R))
                    {
                        colorSelect.ClearColors();
                        secretSector.ClearColors();
                        foreach (Sector sector in sectors)
                        {
                            sector.ClearColors();
                        }
                        activeSector = sectors[0];
                        currentPhase = Phase.Intro;
                    }
                    break;

                case Phase.Gameplay:
                    if (mousePressed)
                    {
                        activeSector.CheckMouseClick(Raylib.GetMousePosition());
                    }
                    if (Raylib.IsKeyPressed(KeyboardKey.KEY_BACKSPACE))
                    {
                        activeSector.ClearColors();
                    }
                    if (activeSector.Confirmed) ResolveConfirmedSector();
                    if (Raylib.IsKeyPressed(KeyboardKey.KEY_R))
                    {
                        colorSelect.ClearColors();
                        secretSector.ClearColors();
                        foreach (Sector sector in sectors)
                        {
                            sector.ClearColors();
                        }
                        activeSector = sectors[0];
                        currentPhase = Phase.Intro;
                    }
                    break;

                case Phase.GameEnd:
                    if (Raylib.IsKeyPressed(KeyboardKey.KEY_R))
                    {
                        colorSelect.ClearColors();
                        secretSector.ClearColors();
                        foreach (Sector sector in sectors)
                        {
                            sector.ClearColors();
                        }
                        activeSector = sectors[0];
                        currentPhase = Phase.Intro;
                    }
                    break;

                case Phase.Info:
                    if (mousePressed && infoBackButton.CheckClicked(Raylib.GetMousePosition()))
                    {
                        currentPhase = Phase.Intro;
                    }
                    if (Raylib.IsKeyPressed(KeyboardKey.KEY_BACKSPACE))
                    {
                        currentPhase = Phase.Intro;
                    }
                    if (Raylib.IsKeyPressed(KeyboardKey.KEY_R))
                    {
                        colorSelect.ClearColors();
                        secretSector.ClearColors();
                        foreach (Sector sector in sectors)
                        {
                            sector.ClearColors();
                        }
                        activeSector = sectors[0];
                        currentPhase = Phase.Intro;
                    }
                    break;
            }
        }

        private void Display()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.RAYWHITE);
            switch (currentPhase)
            {
                case Phase.Intro:
                    botButton.Display();
                    humanButton.Display();
                    infoButton.Display();
                    break;

                case Phase.ColorSelect:
                    colorSelect.Display(false, true);
                    break;

                case Phase.Gameplay:
                    foreach (Sector sector in sectors)
                    {
                        if (sector.Equals(activeSector)) sector.Display(false, true);
                        else sector.Display(Array.IndexOf(sectors, sector) > Array.IndexOf(sectors, activeSector));
                    }
                    secretSector.Display(!Raylib.IsKeyDown(KeyboardKey.KEY_SPACE));
                    break;

                case Phase.GameEnd:
                    foreach (Sector sector in sectors)
                    {
                        sector.Display(Array.IndexOf(sectors, sector) > Array.IndexOf(sectors, activeSector));
                    }
                    secretSector.Display(false);
                    break;

                case Phase.Info:
                    for (int i = 0; i < infoText.Length; i++)
                    {
                        Rectangle textRec = new Rectangle(10, i*70, WIDTH-10, HEIGHT);
                        Raylib.DrawTextRec(Raylib.GetFontDefault(), infoText[i], textRec, 20, 3, true, Color.BLACK);
                    }
                    infoBackButton.Display();
                    break;
            }
            Raylib.EndDrawing();
        }

        private void GenerateSecretSector()
        {
            List<Color> colors = new List<Color>();
            for (int i = 0; i < PIECE_AMOUNT; i++)
            {
                colors.Add(Piece.PossibleColors[random.Next(Piece.PossibleColors.Length)]);
            }
            GenerateSecretSector(colors);
        }
        private void GenerateSecretSector(List<Color> colors)
        {
            secretSector.ActiveColors = colors.ToArray();
        }

        private void ResolveConfirmedSector()
        {
            switch (currentPhase)
            {
                case Phase.Gameplay:
                    activeSector.CalculatePegs(secretSector);
                    if (Enumerable.SequenceEqual(activeSector.ActiveColors, secretSector.ActiveColors))
                    {
                        currentPhase = Phase.GameEnd;
                    }
                    else
                    {
                        if (Array.IndexOf(sectors, activeSector) + 1 >= sectors.Length)
                        {
                            currentPhase = Phase.GameEnd;
                        }
                        else activeSector = sectors[Array.IndexOf(sectors, activeSector) + 1];
                    }
                    break;

                case Phase.ColorSelect:
                    GenerateSecretSector(colorSelect.ActiveColors.ToList());
                    break;
            }
        }
    }
}
