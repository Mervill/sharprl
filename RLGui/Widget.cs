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
    /// A component that represents a rectangular region (position and size)
    /// </summary>
    public abstract class Widget : Component
    {
        /// <summary>
        /// Construct a Widget given the rectangular region
        /// </summary>
        /// <param name="rect"></param>
        protected Widget(Rectangle rect)
            : this(rect.Location, rect.Size)
        { }

        /// <summary>
        /// Construct a Widget given the position and size
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="size"></param>
        protected Widget(Point pos, Size size)
        {
            this.Rect = new Rectangle(pos, size);
        }

        /// <summary>
        /// Returns true if the point is inside this Widget's rectangular region
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
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
        /// <param name="pos"></param>
        /// <returns></returns>
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
        /// Re-size the Widget.
        /// </summary>
        /// <param name="newSize"></param>
        public virtual void SetSize(Size newSize)
        {
            this.Rect = new Rectangle(Position, Size);

            if (SizeChanged != null)
                SizeChanged(this, new EventArgs<Size>(newSize));

        }

        /// <summary>
        /// Fired when the size of the Widget has been changed by calling SetSize
        /// </summary>
        public event EventHandler<EventArgs<Size>> SizeChanged;

        /// <summary>
        /// Sets the Widget's position.  This is given in console space.
        /// </summary>
        /// <param name="newPos"></param>
        public virtual void SetPosition(Point newPos)
        {
            this.Rect = new Rectangle(Position, Size);

            if (PositionChanged != null)
                PositionChanged(this, new EventArgs<Point>(newPos));
        }

        /// <summary>
        /// Fired when the position of the Widget has been changed by calling SetPosition
        /// </summary>
        public event EventHandler<EventArgs<Point>> PositionChanged;

    }

}
