using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Raylib_cs;

namespace ColorMindGame
{
    class Sector
    {
        public Color[] ActiveColors { get; set; } = new Color[Game.PIECE_AMOUNT] { Color.BLACK, Color.BLACK, Color.BLACK, Color.BLACK };

        public bool Confirmed
        {
            get
            {
                if (_confirmed)
                {
                    _confirmed = false;
                    return true;
                }
                else return false;
            }
        }
        private bool _confirmed = false;

        int x;
        int y;
        int sizeX;
        int sizeY;
        int middleY;

        List<Color> pegs = new List<Color>();
        int pegSize = 10;

        int[] colorXCoords = new int[Game.PIECE_AMOUNT];
        int colorSize = 15;

        Button confirmButton;

        public Sector(int aX, int aY, int aSizeX, int aSizeY)
        {
            x = aX;
            y = aY;
            sizeX = aSizeX;
            sizeY = aSizeY;
            middleY = (y + sizeY + y) / 2;

            int colorSizeWidth = 3 * sizeX / 4 - sizeX / 4;
            int colorStep = colorSizeWidth / Game.PIECE_AMOUNT;
            for (int i = 0; i < Game.PIECE_AMOUNT; i++)
            {
                colorXCoords[i] = colorStep / 2 + i * colorStep + sizeX / 4;
            }

            confirmButton = new Button(3 * sizeX / 4 + 30, y + 10, sizeX / 6, sizeY - 20, "Confirm", Color.GREEN);
        }

        public void CheckMouseClick(Vector2 mousePos)
        {
            bool full = true;
            for (int i = 0; i < ActiveColors.Length; i++)
            {
                if (ActiveColors[i].Equals(Color.BLACK))
                {
                    full = false;
                    break;
                }
            }
            if (full && confirmButton.CheckClicked(mousePos)) _confirmed = true;

            for (int i = 0; i < colorXCoords.Length; i++)
            {
                double distance = Vector2.Distance(new Vector2(colorXCoords[i], middleY), mousePos);
                if (distance <= colorSize + 3)
                {
                    ChangeColor(i);
                    break;
                }
            }
        }

        public void Display(bool hidden=true, bool active=false)
        {
            Raylib.DrawRectangleLines(x, y, sizeX, sizeY, Color.BLACK);
            if (!hidden)
            {
                DisplayPegs();
                DisplayColors();
                bool full = true;
                for (int i = 0; i < ActiveColors.Length; i++)
                {
                    if (ActiveColors[i].Equals(Color.BLACK))
                    {
                        full = false;
                        break;
                    }
                }
                if (active && full) DisplayButton();
            }

        }

        private void DisplayPegs()
        {
            for (int i = 0; i < pegs.Count; i++)
            {
                Raylib.DrawCircle((i * 25) + 30, middleY, pegSize, pegs[i]);
                Raylib.DrawCircleLines((i * 25) + 30, middleY, pegSize, Color.BLACK);
            }
        }

        private void DisplayColors()
        {
            for (int i = 0; i < Game.PIECE_AMOUNT; i++)
            {
                int size = colorSize;
                if (ActiveColors[i].Equals(Color.BLACK)) size = 3;
                Raylib.DrawCircle(colorXCoords[i], middleY, size, ActiveColors[i]);
                Raylib.DrawCircleLines(colorXCoords[i], middleY, size, Color.BLACK);
            }
        }

        private void ChangeColor(int index)
        {
            if (ActiveColors[index].Equals(Color.BLACK))
            {
                ActiveColors[index] = Piece.PossibleColors[0];
            }
            else
            {
                int colorIndex = Array.IndexOf(Piece.PossibleColors, ActiveColors[index]) + 1;
                if (colorIndex >= Piece.PossibleColors.Length) colorIndex = 0;
                ActiveColors[index] = Piece.PossibleColors[colorIndex];
            }
        }

        public void ClearColors()
        {
            ActiveColors = new Color[Game.PIECE_AMOUNT] { Color.BLACK, Color.BLACK, Color.BLACK, Color.BLACK };
            pegs.Clear();
        }

        private void DisplayButton()
        {
            confirmButton.Display();
        }

        public void CalculatePegs(Sector secretSector)
        {
            Color[] answerColors = secretSector.ActiveColors;
            bool[] answerChecked = new bool[Game.PIECE_AMOUNT];
            bool[] activeChecked = new bool[Game.PIECE_AMOUNT];
            for (int i = 0; i < Game.PIECE_AMOUNT; i++)
            {
                answerChecked[i] = false;
                activeChecked[i] = false;
            }

            for (int i = 0; i < ActiveColors.Length; i++)
            {
                if (ActiveColors[i].Equals(answerColors[i]))
                {
                    pegs.Add(Color.WHITE);
                    answerChecked[i] = true;
                    activeChecked[i] = true;
                }
            }

            for (int i = 0; i < ActiveColors.Length; i++)
            {
                for (int j = 0; j < answerColors.Length; j++)
                 {
                    if (ActiveColors[i].Equals(answerColors[j]) && !activeChecked[i] && !answerChecked[j])
                    {
                        pegs.Add(Color.BLACK);
                        activeChecked[i] = true;
                        answerChecked[j] = true;
                    }
                }
            }
        }
    }
}
