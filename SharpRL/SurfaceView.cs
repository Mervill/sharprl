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
    /// Represents a viewport region of another MemorySurface.  All drawing operations use coordinate local to this viewport,
    /// but are translated to the underlying surface
    /// </summary>
    public class SurfaceView : Surface
    {
        Rectangle viewPort;
        MemorySurface child;

        internal SurfaceView(MemorySurface childSurface, Rectangle viewPort)
            : base(viewPort.Width, viewPort.Height)
        {
            this.viewPort = viewPort;
            child = childSurface;
        }

        /// <summary>
        /// Clears the viewport are of the child surface, using this SurfaceView's default colors and character.
        /// </summary>
        public override void Clear()
        {
            for (int y = viewPort.Top; y < viewPort.Bottom; y++)
            {
                for (int x = viewPort.Left; x < viewPort.Right; x++)
                {
                    child.SetCellUnchecked(x, y, DefaultChar, DefaultForeground, DefaultBackground);
                }
            }
        }

        /// <summary>
        /// Translates a Point given in local coordinates to the parent's coordinate system
        /// </summary>
        /// <param name="viewPos"></param>
        /// <returns></returns>
        public Point TranslateViewToParent(Point viewPos)
        {
            return new Point(viewPos.X + viewPort.X, viewPos.Y + viewPort.Y);
        }

        /// <summary>
        /// Translates a Point given in parent's coordinate system to local coordinates
        /// </summary>
        /// <param name="parentPos"></param>
        /// <returns></returns>
        public Point TranslateParentToView(Point parentPos)
        {
            return new Point(parentPos.X - viewPort.X, parentPos.Y - viewPort.Y);
        }

        internal override Surface.Cell GetCellUnchecked(int cx, int cy)
        {
            return child.GetCellUnchecked(cx + viewPort.X, cy + viewPort.Y);
        }

        internal override void SetCellUnchecked(int cx, int cy, char? ch, Color? fg, Color? bg)
        {
            child.SetCellUnchecked(cx + viewPort.X, cy + viewPort.Y, ch, fg, bg);
        }
    }
}
