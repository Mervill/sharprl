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
    public abstract class ControlTemplate
    {
        public ControlTemplate()
        {
            HasFrame = true;

            FrameDefinition = new FrameDefinition();

            TitleLocation = FrameTitleLocation.UpperLeft;

            Pigments = new ControlPigments();
        }

        public bool HasFrame { get; set; }

        public FrameDefinition FrameDefinition { get; set; }

        public string Title { get; set; }

        public FrameTitleLocation TitleLocation { get; set; }

        public ControlPigments Pigments { get; set; }

        public Size MinimumSize { get; set; }


        public abstract Size CalcSizeToContent();

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

    /// <summary>
    /// Represents a Widget that draws its own various elements depending on current user interaction state.
    /// </summary>
    public abstract class Control : Widget
    {
        /// <summary>
        /// The MemorySurface for this control
        /// </summary>
        protected MemorySurface DrawingSurface { get; private set; }

        /// <summary>
        /// The pigments used when the control is drawn
        /// </summary>
        public ControlPigments Pigments { get; private set; }

        /// <summary>
        /// The tooltip string displayed when hovering over this control.  Set to null or empty
        /// to disable tooltip
        /// </summary>
        public string ToolTip { get; set; }

        /// <summary>
        /// Get or set this control to be active or not.  Inactive controls typically should ignore
        /// action messages (but they still will receive them from the framework)
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Get or set whether a frame border is drawn around the control
        /// </summary>
        public bool HasFrame { get; set; }

        public FrameDefinition FrameDefinition { get; set; }

        public string Title { get; set; }

        public FrameTitleLocation TitleLocation { get; set; }

        protected Rectangle ContentRect { get; private set; }

        /// <summary>
        /// Construct a new Control object
        /// </summary>
        protected Control(Point position, ControlTemplate template)
            :base(new Rectangle(position, template.GetFinalSize()))
        {
            IsActive = true;
            Pigments = template.Pigments;
            DrawingSurface = new MemorySurface(Size.Width, Size.Height);
            FrameDefinition = template.FrameDefinition;
            Title = template.Title;
            TitleLocation = template.TitleLocation;

            HasFrame = template.HasFrame;

            if (HasFrame)
            {
                ContentRect = new Rectangle(new Point(1, 1), new Size(Size.Width - 2, Size.Height - 2));
            }
            else
            {
                ContentRect = new Rectangle(new Point(0, 0), Size);
            }
        }

        /// <summary>
        /// Called by the framework when the control is to be blitted to the console root surface.
        /// </summary>
        /// <param name="renderTo"></param>
        protected internal override void OnRender(Surface renderTo)
        {
            Surface.Blit(DrawingSurface, renderTo, Position.X, Position.Y);
        }

        /// <summary>
        /// Calls DrawContent()
        /// </summary>
        protected internal override void OnPaint()
        {
            DrawContent();

            if (HasFrame)
            {
                DrawFrame();
                if (!string.IsNullOrEmpty(Title))
                {
                    DrawFrameTitle();
                }
            }
        }

        /// <summary>
        /// Get the view pigment according to state using the Style property.
        /// </summary>
        /// <returns></returns>
        protected virtual Pigment GetCurrentViewPigment()
        {
            if (!IsActive)
                return Pigments.ViewInactive;

            if (IsBeingPushed)
                return Pigments.ViewSelected;
             
            if (IsMouseOver)
                return Pigments.ViewMouseOver;
            
            return Pigments.ViewNormal;
        }

        /// <summary>
        /// Get the border pigment according to state using the Style property
        /// </summary>
        /// <returns></returns>
        protected virtual Pigment GetCurrentBorderPigment()
        {
            if (!IsActive)
                return Pigments.BorderInactive;

            if (IsBeingPushed)
                return Pigments.BorderSelected;

            if (IsMouseOver)
                return Pigments.BorderMouseOver;

            return Pigments.BorderNormal;
        }

        /// <summary>
        /// Draws a frame around control using the pigment returned from GetCurrentBorderPigment().  This is called
        /// automatically on OnPaint message if HasFrame is true
        /// </summary>
        protected virtual void DrawFrame()
        {
            var pigment = GetCurrentBorderPigment();

            DrawingSurface.DrawFrame(DrawingSurface.Rect, null, false, pigment.Foreground, pigment.Background, FrameDefinition);


        }

        /// <summary>
        /// Draws the frame title using the pigment returned from GetCurrentBorderPigment().  This is called
        /// automatically on OnPaint message if HasFrame is true and Title is not null or empty
        /// </summary>
        protected virtual void DrawFrameTitle()
        {
            var pigment = GetCurrentBorderPigment();

            int y = 0;
            HorizontalAlignment hAlign = HorizontalAlignment.Left;

            if (TitleLocation == FrameTitleLocation.UpperCenter || TitleLocation == FrameTitleLocation.LowerCenter)
            {
                hAlign = HorizontalAlignment.Center;
            }

            if (TitleLocation == FrameTitleLocation.UpperRight || TitleLocation == FrameTitleLocation.LowerRight)
            {
                hAlign = HorizontalAlignment.Right;
            }

            if (TitleLocation == FrameTitleLocation.LowerRight || TitleLocation == FrameTitleLocation.LowerCenter ||
                TitleLocation == FrameTitleLocation.LowerLeft)
            {
                y = Size.Height - 1;
            }

            DrawingSurface.PrintStringAligned(1, y, Title, Size.Width - 2, hAlign, pigment.Foreground, pigment.Background);
        }

        /// <summary>
        /// Called during OnPaint message.  Override to provide content drawing code
        /// </summary>
        protected abstract void DrawContent();


    }
}
