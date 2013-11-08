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
    /// Represents a Widget that draws its own various elements depending on current user interaction state.
    /// </summary>
    public abstract class Control : Widget
    {
        public event EventHandler Pushed;

        public event EventHandler PushReleased;


        /// <summary>
        /// The pigments used when the control is drawn
        /// </summary>
        public ControlPigments Pigments { get; private set; }

        /// <summary>
        /// Gets the tooltip string displayed when hovering over this control.  Return null or empty
        /// to disable tooltip. The default base implementation returns the string passed by the control template.
        /// Override to add state-specific tooltips.
        /// </summary>
        public virtual string ToolTipText
        {
            get
            {
                return toolTipText;
            }
        }

        private string toolTipText;

        /// <summary>
        /// Get whether a frame border is drawn around the control
        /// </summary>
        public bool HasFrame { get; private set; }

        /// <summary>
        /// How the control's frame is drawn, if HasFrame is true
        /// </summary>
        public FrameDefinition FrameDefinition { get; set; }

        /// <summary>
        /// The frame Title of the control, drawn if HasFrame is true
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Where the frame title is drawn (if HasFrame is true)
        /// </summary>
        public FrameTitleLocation TitleLocation { get; set; }

        /// <summary>
        /// The rectangle of the control's drawing area in local space. If the Control has a frame, this will return
        /// a Rectangle starting at 1,1 and sized to fit inside the frame. If there is no frame, then this will
        /// return a Rectangle at 0,0 and the same size as the entire Control.
        /// </summary>
        protected Rectangle ViewRect { get; private set; }

        public bool IsBeingPushed { get; private set; }

        /// <summary>
        /// Construct a new Control object
        /// </summary>
        protected Control(Point position, ControlTemplate template)
            :base(position, template.GetFinalSize())
        {
            Pigments = template.Pigments;

            if (Pigments == null)
                Pigments = new ControlPigments();

            
            FrameDefinition = template.FrameDefinition;
            Title = template.Title;
            TitleLocation = template.TitleLocation;
            toolTipText = template.ToolTipText;
            KeyboardMode = template.KeyboardMode;
            SetLayer(template.Layer);

            HasFrame = template.HasFrame;

            if (HasFrame)
            {
                ViewRect = new Rectangle(new Point(1, 1), new Size(Size.Width - 2, Size.Height - 2));
            }
            else
            {
                ViewRect = new Rectangle(new Point(0, 0), Size);
            }
        }

        /// <summary>
        /// Calls DrawContent(), and draws the frame and title if applicable.
        /// For custom drawing, override DrawContent, DrawFrame, and DrawFrameTitle instead of this method.
        /// To customize colors, override GetCurrentViewPigment and GetCurrentBorderPigment
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
        /// Override to return custom view (content) colors
        /// </summary>
        /// <returns></returns>
        protected virtual Pigment GetCurrentViewPigment()
        {
            if (IsBeingPushed)
                return Pigments.ViewSelected;
             
            if (IsMouseOver)
                return Pigments.ViewMouseOver;

            if (HasFocus)
                return Pigments.ViewFocused;
            
            return Pigments.ViewNormal;
        }

        /// <summary>
        /// Get the border pigment according to state using the Style property.
        /// Override to return custom border colors
        /// </summary>
        /// <returns></returns>
        protected virtual Pigment GetCurrentBorderPigment()
        {
            if (IsBeingPushed)
                return Pigments.BorderSelected;

            if (IsMouseOver)
                return Pigments.BorderMouseOver;

            if (HasFocus)
                return Pigments.BorderFocused;

            return Pigments.BorderNormal;
        }

        protected void DoPush()
        {
            if (IsBeingPushed)
                return;

            IsBeingPushed = true;

            OnPushed();
        }

        protected void DoPushRelease()
        {
            if (!IsBeingPushed)
                return;

            IsBeingPushed = false;

            OnPushReleased();
        }

        protected internal override void OnKeyUp(KeyRawEventData keyInfo)
        {
            base.OnKeyUp(keyInfo);

            if (keyInfo.Key == KeyCode.Enter || keyInfo.Key == KeyCode.KeypadEnter)
            {
                DoPushRelease();
            }
        }

        protected internal override void OnKeyDown(KeyRawEventData keyInfo)
        {
            base.OnKeyDown(keyInfo);

            if (keyInfo.Key == KeyCode.Enter || keyInfo.Key == KeyCode.KeypadEnter)
            {
                DoPush();
            }
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

        protected virtual void OnPushed()
        {
            if (Pushed != null)
                Pushed(this, null);
        }

        protected virtual void OnPushReleased()
        {
            if (PushReleased != null)
                PushReleased(this, null);
        }

        protected internal override void OnMouseButtonDown(MouseMessageData mouseInfo)
        {
            base.OnMouseButtonDown(mouseInfo);

            if (mouseInfo.Button == MouseButton.Left)
            {
                DoPush();
            }
        }


        protected internal override void OnMouseLeave()
        {
            base.OnMouseLeave();

            if (IsBeingPushed)
                DoPushRelease();
        }

        protected internal override void OnMouseButtonUp(MouseMessageData mouseInfo)
        {
            base.OnMouseButtonUp(mouseInfo);

            if (mouseInfo.Button == MouseButton.Left)
            {
                DoPushRelease();
            }
        }
    }
}
