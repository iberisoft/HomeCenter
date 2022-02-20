using System.Collections.Generic;

namespace HomeExplorer.Shared
{
    class ColorPalette
    {
        readonly List<Color> m_Colors = new();

        public ColorPalette()
        {
            var white = new Color { R = 255, G = 255, B = 255 };
            AddRange(256, white, 0, 0, -1);             // to yellow
            AddRange(256, m_Colors[^1], 0, -1, 0);      // to red
            AddRange(256, m_Colors[^1], 0, 0, 1);       // to magenta
            AddRange(256, m_Colors[^1], -1, 0, 0);      // to blue
            AddRange(256, m_Colors[^1], 0, 1, 0);       // to cyan
            AddRange(256, m_Colors[^1], 0, 0, -1);      // to green
        }

        private void AddRange(int count, Color color, int deltaR, int deltaG, int deltaB)
        {
            for (var i = 0; i < count; ++i)
            {
                m_Colors.Add(color);

                color.R += deltaR;
                color.G += deltaG;
                color.B += deltaB;
            }
        }

        public int Count => m_Colors.Count;

        public Color this[int index] => m_Colors[index];

        public struct Color
        {
            public int R { get; set; }

            public int G { get; set; }

            public int B { get; set; }

            public override string ToString() => $"{R:x2}{G:x2}{B:x2}";
        }
    }
}
