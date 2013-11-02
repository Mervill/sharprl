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
        protected Widget(Rectangle rect)
        {
            this.Rect = rect;
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


    }

}
