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
    /// Base class for filling out a control's initialization data
    /// </summary>
    public abstract class ControlTemplate
    {
        /// <summary>
        /// Construct a new ControlTemplate object
        /// </summary>
        public ControlTemplate()
        {
            HasFrame = true;

            FrameDefinition = new FrameDefinition();

            TitleLocation = FrameTitleLocation.UpperLeft;

            KeyboardMode = KeyboardInputMode.Focus;

            ToolTipText = null;

            Pigments = new ControlPigments()
            {
                BorderFocused = new Pigment(Color.Blue, Color.Black),
                BorderMouseOver = new Pigment(Color.LightBlue, Color.Black)

            };
        }

        /// <summary>
        /// True if the control should draw a frame (and possibly frame title)
        /// </summary>
        public bool HasFrame { get; set; }

        /// <summary>
        /// The frame definition, used if HasFrame is true
        /// </summary>
        public FrameDefinition FrameDefinition { get; set; }

        /// <summary>
        /// The frame title, used if HasFrame is true
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Where the frame title is drawn, if HasFrame is true
        /// </summary>
        public FrameTitleLocation TitleLocation { get; set; }

        /// <summary>
        /// The default state pigments used for drawing
        /// </summary>
        public ControlPigments Pigments { get; set; }

        /// <summary>
        /// The minimum size of the control
        /// </summary>
        public Size MinimumSize { get; set; }

        /// <summary>
        /// The text shown with a tooltip popup, or empty/null for no tooltip
        /// </summary>
        public string ToolTipText { get; set; }

        /// <summary>
        /// Determines how keyboard focus is handled by the control.
        /// </summary>
        public KeyboardInputMode KeyboardMode { get; set; }

        public int Layer { get; set; }

        /// <summary>
        /// Override to return the size the control should be based other template values
        /// </summary>
        /// <returns></returns>
        public abstract Size CalcSizeToContent();

        /// <summary>
        /// Returns the final size of the control, using CalcSizeToContent and MinimumSize
        /// </summary>
        /// <returns></returns>
        public Size GetFinalSize()
        {
            Size sz = CalcSizeToContent();
            int w = sz.Width;
            int h = sz.Height;

            if (w < MinimumSize.Width)
                w = MinimumSize.Width;

            if (h < MinimumSize.Height)
                h = MinimumSize.Height;

            return new Size(w, h);

        }
    }
}
