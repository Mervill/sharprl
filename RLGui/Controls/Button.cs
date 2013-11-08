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

namespace RLGui.Controls
{


    /// <summary>
    /// Represents a Panel that has a label and changes rendering pigment
    /// depending on various states
    /// </summary>
    public class Button : Control
    {
        /// <summary>
        /// Construct a Button with a given position and size.
        /// </summary>
        public Button(Point position, ButtonTemplate template)
            :base(position, template)
        {
            
            Label = template.Label;
            HAlignment = template.HAlignment;
            VAlignment = template.VAlignment;
        }

        /// <summary>
        /// Get or set the string used to render the button text
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Get or set the alignment of the label within the button area
        /// </summary>
        public HorizontalAlignment HAlignment { get; set; }

        public VerticalAlignment VAlignment { get; set; }



        /// <summary>
        /// Draws the button label
        /// </summary>
        protected override void DrawContent()
        {
            var pigment = GetCurrentViewPigment();

            DrawingSurface.DefaultBackground = pigment.Background;
            DrawingSurface.DefaultForeground = pigment.Foreground;

            DrawingSurface.Clear();

            DrawingSurface.PrintStringRect(ViewRect, Label, HAlignment, VAlignment);
        }

    }
}
