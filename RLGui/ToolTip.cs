using SharpRL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using SharpRL.Toolkit;

namespace RLGui
{
    public abstract class ToolTip
    {
        public string CurrentText { get; private set; }
        protected MemorySurface DrawingSurface { get; private set; }
        protected Rectangle Rect { get; private set; }
        public AlphaMode Alpha { get; set; }

        public Pigment FramePigment { get; set; }
        public Pigment ViewPigment { get; set; }
        public bool HasFrame { get; set; }

        internal UIManager Manager { get; set; }

        public ToolTip()
        {
            Alpha = AlphaMode.BasicAlpha(0.7f);
            HasFrame = true;
            FramePigment = new Pigment(Color.DarkBlue, Color.LightGray);
            ViewPigment = new Pigment(Color.DarkBlue, Color.LightGray);     
        }

        internal void StartTooltip(string text, Point nearPosition)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("Text cannot be null or empty");

            CurrentText = text;
            Size size;
            if (HasFrame)
            {
                size = new Size(text.Length + 2, 3);
            }
            else
            {
                size = new Size(text.Length, 1);
            }

            Rectangle basicRect = new Rectangle(nearPosition, size);

            int screenRight = Manager.Console.Root.Rect.Right;
            int screenBottom = Manager.Console.Root.Rect.Bottom;

            int dx = 0;
            int dy = 0;

            if (basicRect.Left < 0)
                dx = -basicRect.Left;
            else if (basicRect.Right > screenRight)
                dx = screenRight - basicRect.Right;

            if (basicRect.Top < 0)
                dy = -basicRect.Top;
            else if (basicRect.Bottom > screenBottom)
                dy = screenBottom - basicRect.Bottom;

            Rect = new Rectangle(new Point(nearPosition.X + dx, nearPosition.Y + dy), size);
            
            DrawingSurface = new MemorySurface(size.Width, size.Height);

            OnStart();
        }

        internal void EndTooltip()
        {
            OnEnd();
        }

        protected virtual void OnStart()
        {

        }

        protected virtual void OnEnd()
        {

        }

        protected internal virtual void OnUpdate(float elapsed)
        {

        }

        protected internal abstract void OnPaint();

        protected internal abstract void OnRender(Surface renderTo);
    }

    public class DefaultToolTip : ToolTip
    {


        public DefaultToolTip()
        {

        }

        protected internal override void OnPaint()
        {
            if (HasFrame)
            {
                DrawingSurface.PrintString(1, 1, CurrentText, ViewPigment.Foreground, ViewPigment.Background);
                DrawingSurface.DrawFrame(DrawingSurface.Rect, null, false, 
                    FramePigment.Foreground,FramePigment.Background, null);
            }
            else
            {
                DrawingSurface.PrintString(0, 0, CurrentText, Color.DarkGray, Color.LightSkyBlue);
            }
        }

        protected internal override void OnRender(Surface renderTo)
        {
            Surface.BlitAlpha(DrawingSurface, DrawingSurface.Rect, renderTo, Rect.X, Rect.Y, Alpha);
        }

    }
}
