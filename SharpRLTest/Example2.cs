using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpRL;
using System.Drawing;
using SharpRL.Framework;

namespace SharpRLTest
{
    class Example2 : Example1
    {
        Surface offscreenSurface;
        Point currentPosition;

        //float shift;
        Point delta = new Point(1, 1);

        public Example2()
        {
            offscreenSurface = new MemorySurface(20, 10);
            currentPosition = new Point(5, 15);

            // Create a timer that will active every 1/2 second to update the position
            // of the offscreen surface
            timers.AddTimer(0.5f, true,
                (t) =>
                {
                    UpdateOffscreenPosition();
                });
        }

        private void UpdateOffscreenPosition()
        {
            currentPosition.X += delta.X;
            currentPosition.Y += delta.Y;

            if (currentPosition.X + offscreenSurface.Width / 2 >= surface.Width)
            {
                delta.X = -1;
            }
            if (currentPosition.X <= -offscreenSurface.Width / 2)
            {
                delta.X = 1;
            }
            if (currentPosition.Y + offscreenSurface.Height / 2 >= surface.Height)
            {
                delta.Y = -1;
            }
            if (currentPosition.Y <= -offscreenSurface.Height / 2)
            {
                delta.Y = 1;
            }
        }

        public override string Description
        {
            get { return "Offscreen Surface and Blitting"; }
        }

        public override Surface Draw()
        {
            base.Draw();

            DrawOffscreen();

            Surface.BlitAlpha(offscreenSurface, offscreenSurface.Rect,
                surface, currentPosition.X, currentPosition.Y, 0.75f);

            return surface;
        }

        void DrawOffscreen()
        {
            offscreenSurface.DrawFrame(offscreenSurface.Rect, null, true, Color.Violet, Color.Black);

            // Print the message in the offscreen surface, making sure the formatting rectangle is within the frame we just drew
            offscreenSurface.PrintStringRect(
                new Rectangle(1, 1, offscreenSurface.Width - 2, offscreenSurface.Height - 2),
                "This is an off-screen surface, blitted over another surface using simulated alpha transparency",
                HorizontalAlignment.Left,
                VerticalAlignment.Top,
                WrappingType.Word);
        }
    }
}
