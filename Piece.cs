using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;

namespace ColorMindGame
{
    class Piece
    {
        public static Color[] PossibleColors { get; } = new Color[6] { Color.GRAY, Color.GREEN, Color.YELLOW, Color.BLUE, Color.BEIGE, Color.RED };
    }
}
