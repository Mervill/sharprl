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
    // Window has two parts - Frame and Client

    public class Window : Component
    {
        public Window(Rectangle rect)
            : this(rect.Location, rect.Size)
        { }

        public Window(Point pos, Size size)
        {
            this.Rect = new Rectangle(pos, size);

            frameSurface = new MemorySurface(Size.Width, Size.Height);
            clientSurface = frameSurface.CreateView(new Rectangle(1, 1, Size.Width - 2, Size.Height - 2));
            ClientRect = Rectangle.Inflate(Rect, -1, -1);
        }

        MemorySurface frameSurface;
        SurfaceView clientSurface;

        public override Point ConsoleToLocalSpace(Point pos)
        {
            var p = new Point(pos.X - Position.X, pos.Y - Position.Y);

            return p;
        }

        public override bool HitTest(Point pos)
        {
            return Rect.Contains(pos);
        }

        protected Point ConsoleToClientSpace(Point pos)
        {
            var p = new Point(pos.X - ClientPosition.X, pos.Y - ClientPosition.Y);

            return p;
        }

        /// <summary>
        /// Get the position of the Window in console space.
        /// </summary>
        public Point Position { get { return Rect.Location; } }

        // <summary>
        /// Get the size of this component's region.
        /// </summary>
        public Size Size { get { return Rect.Size; } }

        /// <summary>
        /// Get the rectangle region of this component in console space
        /// </summary>
        public Rectangle Rect { get; private set; }

        /// <summary>
        /// Get the client region of the window in console space
        /// </summary>
        public Rectangle ClientRect { get; private set; }

        public Point ClientPosition { get { return ClientRect.Location; } }

        public Size ClientSize { get { return ClientRect.Size; } }

        /// <summary>
        /// Re-size the component.
        /// </summary>
        /// <param name="newSize"></param>
        public virtual void SetSize(Size newSize)
        {
            this.Rect = new Rectangle(Position, Size);
            frameSurface = new MemorySurface(Size.Width, Size.Height);
            clientSurface = frameSurface.CreateView(new Rectangle(1, 1, Size.Width - 2, Size.Height - 2));
            ClientRect = Rectangle.Inflate(Rect, -1, -1);

            if (SizeChanged != null)
                SizeChanged(this, new EventArgs<Size>(newSize));

        }
        public event EventHandler<EventArgs<Size>> SizeChanged;

        /// <summary>
        /// Sets the component's position.  This is given in console space.
        /// </summary>
        /// <param name="newPos"></param>
        public virtual void SetPosition(Point newPos)
        {
            this.Rect = new Rectangle(Position, Size);

            if (PositionChanged != null)
                PositionChanged(this, new EventArgs<Point>(newPos));
        }
        public event EventHandler<EventArgs<Point>> PositionChanged;

        protected internal override void OnRender(Surface renderTo)
        {
            base.OnRender(renderTo);

            Surface.Blit(frameSurface, renderTo, Position.X, Position.Y);
        }

        protected internal override void OnPaint()
        {
            base.OnPaint();

            OnRedrawClient(clientSurface);

            OnRedrawFrame(frameSurface);
        }

        protected virtual void OnRedrawClient(Surface clientSurface)
        {
            if (RedrawClient != null)
                RedrawClient(this, new EventArgs<Surface>(clientSurface));
        }

        public event EventHandler<EventArgs<Surface>> RedrawClient;

        protected virtual void OnRedrawFrame(Surface frameSurface)
        {
            frameSurface.DrawFrame(new Rectangle(0, 0, Size.Width, Size.Height),
                null, false, Color.White, Color.Black);

            if (RedrawFrame != null)
                RedrawFrame(this, new EventArgs<Surface>(frameSurface));
        }

        public event EventHandler<EventArgs<Surface>> RedrawFrame;

        public bool IsClientBeingPushed { get; private set; }

        public bool IsClientHovering { get; private set; }

        public bool IsMouseOverClient { get; private set; }

        public bool ClientMousePosition { get; private set; }

        protected override void OnClicked(MouseMessageData mouseInfo)
        {
            base.OnClicked(mouseInfo);

            if (ClientRect.Contains(mouseInfo.ConsoleLocation))
            {
                OnClicked(new MouseMessageData(mouseInfo.ConsoleLocation, mouseInfo.PixelPosition,
                    mouseInfo.Button,
                    ConsoleToClientSpace(mouseInfo.ConsoleLocation)));
            }
        }



        protected virtual void OnClientClicked(MouseMessageData mouseInfo)
        {

        }
    }
}
