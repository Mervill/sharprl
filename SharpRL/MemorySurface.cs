//Copyright (c) 2012 Shane Baker
//
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in
//all copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using SharpRL.Framework;

namespace SharpRL
{
    /// <summary>
    /// Represents an offscreen drawing surface and exposes methods to handle printing characters
    /// </summary>
    public class MemorySurface : Surface
    {
        private Cell[] cells;

        /// <summary>
        /// Create an offscreen surface of the given size
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        public MemorySurface(int columns, int rows)
            : base(columns, rows)
        {
            cells = new Cell[columns * rows];

            for (int i = 0; i < cells.Length; i++)
            {
                cells[i].ch = DefaultChar;
                cells[i].fgColor = DefaultForeground;
                cells[i].bgColor = DefaultBackground;
            }

        }

        /// <summary>
        /// Clears the entire surface, using the default colors and character.
        /// </summary>
        public override void Clear()
        {
            for (int i = 0; i < cells.Length; i++)
            {
                cells[i].bgColor = DefaultBackground;
                cells[i].fgColor = DefaultForeground;
                cells[i].ch = DefaultChar;
            }
        }

        override internal Cell GetCellUnchecked(int cx, int cy)
        {
            return cells[cx + cy * Width];
        }

        internal override void SetCellUnchecked(int cx, int cy, char? ch, Color? fg, Color? bg)
        {
            int index = cx + cy * Width;
            if (ch.HasValue)
                cells[index].ch = ch.Value;
            if (fg.HasValue)
                cells[index].fgColor = fg.Value;
            if (bg.HasValue)
                cells[index].bgColor = bg.Value;
        }

        /// <summary>
        /// Create a SurfaceView to this surface using the specified viewport
        /// </summary>
        /// <param name="viewPort"></param>
        /// <returns></returns>
        public SurfaceView CreateView(Rectangle viewPort)
        {
            Rectangle vp = Rectangle.Intersect(viewPort, Rect);

            if (vp.IsEmpty)
                throw new ArgumentException("The given viewport does not intersect with this Surface");

            return new SurfaceView(this, vp);
        }
    }
}
