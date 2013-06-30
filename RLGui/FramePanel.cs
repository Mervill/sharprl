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
using System.Text;
using System.Drawing;
using SharpRL;

namespace RLGui
{
    /// <summary>
    /// Represents a Panel that has a frame (border) area and an inside client area.
    /// </summary>
    public class FramePanel : Widget
    {
        /// <summary>
        /// Construct a FramePanel given the rectangle region (in console space).  The client area will be sized
        /// to fit inside the border.
        /// </summary>
        /// <param name="rect"></param>
        public FramePanel(Rectangle rect)
            : this(rect.Location, rect.Size)
        { }

        /// <summary>
        /// Construct a FramePanel given the position (in console space) and size.  The client area will be sized
        /// to fit inside the border.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="size"></param>
        public FramePanel(Point pos, Size size)
            :base(pos, size)
        {
            frameSurface = new MemorySurface(Size.Width, Size.Height);
            clientSurface = new MemorySurface(Size.Width - 2, Size.Height - 2);
            ClientRect = Rectangle.Inflate(Rect, -1, -1);

            FramePigment = new Pigment(Color.White, Color.Black);
        }

        MemorySurface frameSurface;
        MemorySurface clientSurface;

        /// <summary>
        /// Translates the provided console space point to client space
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        protected Point ConsoleToClientSpace(Point pos)
        {
            var p = new Point(pos.X - ClientPosition.X, pos.Y - ClientPosition.Y);

            return p;
        }

        /// <summary>
        /// Get the client region of the window in console space
        /// </summary>
        public Rectangle ClientRect { get; private set; }

        /// <summary>
        /// Gets the client area upper left position in console space
        /// </summary>
        public Point ClientPosition { get { return ClientRect.Location; } }

        /// <summary>
        /// Gets the client area size
        /// </summary>
        public Size ClientSize { get { return ClientRect.Size; } }

        /// <summary>
        /// If false, the border will be drawn automatically using the FramePigment.  If true,
        /// then custom drawing code should be provided in OnRedrawFrame.
        /// </summary>
        public bool CustomDrawFrame { get; set; }

        private Pigment framePigment;
        /// <summary>
        /// The colors used to draw the frame if CustomDrawFrame is false
        /// </summary>
        public Pigment FramePigment 
        {
            get { return framePigment; }
            set
            {
                if (value == null)
                    framePigment = Pigment.WhiteBlack;
                else
                    framePigment = value;
            }
        }

        /// <summary>
        /// Re-size the FramePanel.
        /// </summary>
        /// <param name="newSize"></param>
        public override void SetSize(Size newSize)
        {
            base.SetSize(newSize);

            frameSurface = new MemorySurface(Size.Width, Size.Height);
            clientSurface = clientSurface = new MemorySurface(Size.Width - 2, Size.Height - 2);
            ClientRect = Rectangle.Inflate(Rect, -1, -1);
        }

        /// <summary>
        /// Base method blits the FramePanel's memory surface to the console
        /// </summary>
        /// <param name="renderTo"></param>
        protected internal override void OnRender(Surface renderTo)
        {
            Surface.Blit(frameSurface, renderTo, Position.X, Position.Y);
            Surface.Blit(clientSurface, renderTo, Position.X + 1, Position.Y + 1);
        }

        /// <summary>
        /// Base method calls OnRedrawClient and OnRedrawFrame
        /// </summary>
        protected internal override void OnPaint()
        {
            OnRedrawClient(clientSurface);

            OnRedrawFrame(frameSurface);
        }

        /// <summary>
        /// Use the provided Surface to draw in the client area.  Note that 0,0 is the upper left corner
        /// of the client area, not the FramePanel (i.e. client space)
        /// </summary>
        /// <param name="clientSurface"></param>
        protected virtual void OnRedrawClient(Surface clientSurface)
        {
            if (RedrawClient != null)
                RedrawClient(this, new EventArgs<Surface>(clientSurface));
        }

        public event EventHandler<EventArgs<Surface>> RedrawClient;

        /// <summary>
        /// Override and use the provided surface to draw the frame.  Note that 0,0 is the upper left
        /// corner of the FramePanel.
        /// </summary>
        /// <param name="frameSurface"></param>
        protected virtual void OnRedrawFrame(Surface frameSurface)
        {
            if (!CustomDrawFrame)
            {
                frameSurface.DrawFrame(new Rectangle(0, 0, Size.Width, Size.Height),
                    null, false, FramePigment.Foreground, FramePigment.Background);
            }

            if (RedrawFrame != null)
                RedrawFrame(this, new EventArgs<Surface>(frameSurface));
        }

        public event EventHandler<EventArgs<Surface>> RedrawFrame;

        /// <summary>
        /// Returns true if the mouse is within the client area
        /// </summary>
        public bool IsMouseOverClient { get; private set; }

        /// <summary>
        /// Returns true if the mouse is within the frame area.  Only the border area counts
        /// as being inside the frame area.
        /// </summary>
        public bool IsMouseOverFrame { get; private set; }

        /// <summary>
        /// Returns the current mouse position in client space
        /// </summary>
        public Point ClientMousePosition { get; private set; }

        /// <summary>
        /// Returns true if the left button is currently down while over the client area
        /// </summary>
        public bool IsClientBeingPushed { get; private set; }

        private MouseMessageData TranslateMessageToClient(MouseMessageData mouseInfo)
        {
            return new MouseMessageData(mouseInfo.ConsoleLocation, mouseInfo.PixelPosition, mouseInfo.Button,
                ConsoleToClientSpace(mouseInfo.ConsoleLocation));
        }

        /// <summary>
        /// Called when the mouse leaves the FramePanel
        /// </summary>
        protected internal override void OnMouseLeave()
        {
            base.OnMouseLeave();

            IsMouseOverFrame = false;

            if (IsMouseOverClient)
            {
                OnClientLeave();
            }
        }

        /// <summary>
        /// Called when the mouse moves while within the FramePanel region
        /// </summary>
        /// <param name="mouseInfo"></param>
        protected internal override void OnMouseMove(MouseMessageData mouseInfo)
        {
            base.OnMouseMove(mouseInfo);

            if(ClientRect.Contains(mouseInfo.ConsoleLocation))
            {
                IsMouseOverFrame = false;

                if (!IsMouseOverClient)
                {
                    OnClientEnter(TranslateMessageToClient(mouseInfo));
                }

                OnClientMouseMove(TranslateMessageToClient(mouseInfo));
            }
            else
            {
                IsMouseOverFrame = true;

                if (IsMouseOverClient)
                {
                    OnClientLeave();
                }
            }
        }

        /// <summary>
        /// Called when a mouse button is pushed while over the FramePanel region
        /// </summary>
        /// <param name="mouseInfo"></param>
        protected internal override void OnMouseButtonDown(MouseMessageData mouseInfo)
        {
            base.OnMouseButtonDown(mouseInfo);

            if (ClientRect.Contains(mouseInfo.ConsoleLocation))
            {
                OnClientMouseButtonDown(TranslateMessageToClient(mouseInfo));
            }
        }

        /// <summary>
        /// Called when a mouse button is released while within the FramePanel region
        /// </summary>
        /// <param name="mouseInfo"></param>
        protected internal override void OnMouseButtonUp(MouseMessageData mouseInfo)
        {
            base.OnMouseButtonUp(mouseInfo);

            if (ClientRect.Contains(mouseInfo.ConsoleLocation))
            {
                OnClientMouseButtonUp(TranslateMessageToClient(mouseInfo));
            }
        }



        /// <summary>
        /// Called when a left mouse button click happens while within the client region
        /// </summary>
        /// <param name="mouseInfo"></param>
        protected virtual void OnClientClicked(MouseMessageData mouseInfo)
        {
            if (ClientClicked != null)
                ClientClicked(this, new EventArgs<MouseMessageData>(mouseInfo));
        }

        /// <summary>
        /// Fired when the left mouse button has been pushed and released while over the client area
        /// </summary>
        public event EventHandler<EventArgs<MouseMessageData>> ClientClicked;

        /// <summary>
        /// Called when the mouse moves while within the client region
        /// </summary>
        /// <param name="mouseInfo"></param>
        protected virtual void OnClientMouseMove(MouseMessageData mouseInfo)
        {
            ClientMousePosition = mouseInfo.LocalPos;

            if (ClientMouseMove != null)
                ClientMouseMove(this, new EventArgs<MouseMessageData>(mouseInfo));
        }

        /// <summary>
        /// Fired when the mouse has moved while over the client area
        /// </summary>
        public event EventHandler<EventArgs<MouseMessageData>> ClientMouseMove;

        /// <summary>
        /// Called when a mouse button is pushed while within the client region
        /// </summary>
        /// <param name="mouseInfo"></param>
        protected virtual void OnClientMouseButtonDown(MouseMessageData mouseInfo)
        {
            if (mouseInfo.Button == MouseButton.Left)
            {
                IsClientBeingPushed = true;
            }

            if (ClientMouseButtonDown != null)
                ClientMouseButtonDown(this, new EventArgs<MouseMessageData>(mouseInfo));
        }

        /// <summary>
        /// Fired when a mouse button has been pushed down while over the client area
        /// </summary>
        public event EventHandler<EventArgs<MouseMessageData>> ClientMouseButtonDown;

        /// <summary>
        /// Called when a mouse button is released while within the client region
        /// </summary>
        /// <param name="mouseInfo"></param>
        protected virtual void OnClientMouseButtonUp(MouseMessageData mouseInfo)
        {
            if (mouseInfo.Button == MouseButton.Left)
            {
                if (IsClientBeingPushed)
                {
                    IsClientBeingPushed = false;

                    OnClientClicked(mouseInfo);

                }
            }

            if (ClientMouseButtonUp != null)
                ClientMouseButtonUp(this, new EventArgs<MouseMessageData>(mouseInfo));
        }

        /// <summary>
        /// Fired when a mouse button has been released while over the client area
        /// </summary>
        public event EventHandler<EventArgs<MouseMessageData>> ClientMouseButtonUp;

        /// <summary>
        /// Called when the mouse leaves the client area
        /// </summary>
        protected virtual void OnClientLeave()
        {
            IsMouseOverClient = false;
            ClientMousePosition = Point.Empty;

            if (ClientLeave != null)
                ClientLeave(this, null);
        }

        /// <summary>
        /// Fired when the mouse has left the client area
        /// </summary>
        public event EventHandler ClientLeave;

        /// <summary>
        /// Called when the mouse has entered the client area
        /// </summary>
        /// <param name="mouseInfo"></param>
        protected virtual void OnClientEnter(MouseMessageData mouseInfo)
        {
            IsMouseOverClient = true;

            if (ClientEnter != null)
                ClientEnter(this, new EventArgs<MouseMessageData>(mouseInfo));
        }

        /// <summary>
        /// Fired when the mouse has entered the client area
        /// </summary>
        public event EventHandler<EventArgs<MouseMessageData>> ClientEnter;
    }
}
