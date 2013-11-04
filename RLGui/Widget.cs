//Copyright (c) 2013 Shane Baker
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
using System.Drawing;
using SharpRL;

namespace RLGui
{
    /// <summary>
    /// A Widget is a component that represents a rectangular region (has a position and size).
    /// A Widget's position and size are immutable once created. User drawing should be done to the provided
    /// DrawingSurface during OnPaint().
    /// </summary>
    /// <remarks>
    /// Derive from Widget when all the extra functionality of Control is not needed.
    /// </remarks>
    public abstract class Widget : Component
    {
        /// <summary>
        /// The MemorySurface for drawing to this widget. This surface is blitted to the GameConsole root
        /// surface when OnRender is called by the framework.
        /// </summary>
        protected MemorySurface DrawingSurface { get; private set; }


        /// <summary>
        /// Construct a Widget object given the rectangular region (in console space) it will occupy.
        /// </summary>
        /// <param name="rect">The position and size of the Widget</param>
        protected Widget(Rectangle rect)
        {
            this.Rect = rect;
            DrawingSurface = new MemorySurface(Size.Width, Size.Height);
        }

        /// <summary>
        /// Construct a Widget object given the rectangular region (in console space) it will occupy.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="size"></param>
        protected Widget(Point position, Size size)
            : this(new Rectangle(position, size))
        { }

        /// <summary>
        /// Returns true if the point is inside this Widget's rectangular region
        /// </summary>
        /// <param name="pos">The position in console space to be checked</param>
        /// <returns>True if the position in within the Widget's region</returns>
        public override bool HitTest(Point pos)
        {
            return Rect.Contains(pos);
        }

        /// <summary>
        /// Get the position of the Widget in console space.
        /// </summary>
        public Point Position { get { return Rect.Location; } }

        /// <summary>
        /// Translates the point to local space.  Used for translating mouse messages to this Widget
        /// </summary>
        /// <param name="pos">The position to be translated</param>
        /// <returns>The position translated to local space</returns>
        public override Point ConsoleToLocalSpace(Point pos)
        {
            var p = new Point(pos.X - Position.X, pos.Y - Position.Y);

            return p;
        }

        /// <summary>
        /// Get the size of the Widget's region.
        /// </summary>
        public Size Size { get { return Rect.Size; } }

        /// <summary>
        /// Get the rectangle region of the Widget in console space
        /// </summary>
        public Rectangle Rect { get; private set; }

        /// <summary>
        /// Called by the framework when the Widget is to be blitted to the console root surface.
        /// </summary>
        /// <param name="renderTo"></param>
        protected internal override void OnRender(Surface renderTo)
        {
            Surface.Blit(DrawingSurface, renderTo, Position.X, Position.Y);
        }
    }

}
