using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Raylib_cs;

namespace ColorMindGame
{
    class Button
    {
        int x;
        int y;
        int sizeX;
        int sizeY;
        string text;
        Color buttonColor;

        public Button(int aX, int aY, int aSizeX, int aSizeY, string aText, Color aButtonColor)
        {
            x = aX;
            y = aY;
            sizeX = aSizeX;
            sizeY = aSizeY;
            text = aText;
            buttonColor = aButtonColor;
        }

        public void Display()
        {
            Raylib.DrawRectangle(x, y, sizeX, sizeY, buttonColor);
            Raylib.DrawRectangleLines(x, y, sizeX, sizeY, Color.BLACK);
            Raylib.DrawText(text, x + 10, (y + sizeY + y) / 2, 20, Color.BLACK);
        }

        public bool CheckClicked(Vector2 mousePos)
        {
            return mousePos.X > x && mousePos.Y > y && mousePos.X < x + sizeX && mousePos.Y < y + sizeY;
        }
    }
}
