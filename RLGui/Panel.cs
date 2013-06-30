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
    /// A panel is a Widget that keeps an appropriately sized MemorySurface for drawing.  This surface is passed
    /// to the OnRedraw method to be used for drawing operations, and is blitted to the console when
    /// base.OnRender is called.
    /// </summary>
    public class Panel : Widget
    {
        private MemorySurface surface;

        /// <summary>
        /// Construct a Panel given the position (in console space) and size
        /// </summary>
        /// <param name="position"></param>
        /// <param name="size"></param>
        public Panel(Point position, Size size)
            : base(position, size)
        {
            surface = new MemorySurface(size.Width, size.Height);
        }

        /// <summary>
        /// Construct a Panel given the rectangle region (in console space)
        /// </summary>
        /// <param name="rect"></param>
        public Panel(Rectangle rect)
            : this(rect.Location, rect.Size)
        {

        }

        /// <summary>
        /// Re-sizes the Panel
        /// </summary>
        /// <param name="newSize"></param>
        public override void SetSize(Size newSize)
        {
            base.SetSize(newSize);

            surface = new MemorySurface(newSize.Width, newSize.Height);
        }

        /// <summary>
        /// Base method calls OnRedraw
        /// </summary>
        protected internal override void OnPaint()
        {
            OnRedraw(surface);
        }

        /// <summary>
        /// Base method blits the Panel's memory surface to the console
        /// </summary>
        /// <param name="renderTo"></param>
        protected internal override void OnRender(Surface renderTo)
        {
            Surface.Blit(surface, renderTo, Position.X, Position.Y);
        }

        /// <summary>
        /// Base method raises the Redraw event.  Override this method to provide
        /// drawing code
        /// </summary>
        /// <param name="drawingSurface"></param>
        protected virtual void OnRedraw(Surface drawingSurface)
        {
            if (Redraw != null)
                Redraw(this, new EventArgs<Surface>(drawingSurface));
        }

        public event EventHandler<EventArgs<Surface>> Redraw;
    }
}
