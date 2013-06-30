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
    /// Represents a Panel that has a label and changes rendering pigment
    /// depending on various states
    /// </summary>
    public class Button : Panel
    {
        /// <summary>
        /// Construct a new Button given a position and width
        /// </summary>
        /// <param name="position"></param>
        /// <param name="width"></param>
        public Button(Point position, int width)
            : base(position, new Size(width, 3))
        {
            Label = "";
            Alignment = HorizontalAlignment.Left;
        }
        
        /// <summary>
        /// Construct a Button given it's position, label, horizontal alignment and padding.  The Button
        /// will have a width corresponding to the label length and padding.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="label"></param>
        /// <param name="align"></param>
        /// <param name="padding">The number of blank spaces added to the both sides of the label for determining
        /// the Button width</param>
        public Button(Point position, string label, HorizontalAlignment align, int padding = 0)
            : base(position, ComputeSize(label, padding))
        {
            Label = label;
            Alignment = align;
        }

        /// <summary>
        /// Get or set the string used to render the button text
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Get or set the alignment of the label within the button area
        /// </summary>
        public HorizontalAlignment Alignment { get; set; }

        static Size ComputeSize(string label, int padding)
        {
            return new Size(label.Length + padding * 2 + 2, 3);
        }

        /// <summary>
        /// Redraws the button
        /// </summary>
        /// <param name="drawingSurface"></param>
        protected override void OnRedraw(Surface drawingSurface)
        {
            base.OnRedraw(drawingSurface);

            drawingSurface.DrawFrame(new Rectangle(Point.Empty, Size), null, false, Color.Blue, Color.Black);
            drawingSurface.PrintStringAligned(1, 1, Label, Rect.Width - 2, Alignment);
        }

    }
}
