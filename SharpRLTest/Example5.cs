using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using SharpRL.Toolkit;
using SharpRL;

namespace SharpRLTest
{
    class Example5 : ExampleBase
    {
        Surface menuSurface;
        Point start, end;

        SpatialAStar<Cell, object> pather;
        LinkedList<Cell> currentPath;

        Interpolator pathInterp;

        public Example5()
        {
            menuSurface = new MemorySurface(50, 6);

            start = bsp.GetByLevel(bsp.Depth).First().Rect.Center();
            end = bsp.GetByLevel(bsp.Depth).Last().Rect.Center();

            pather = new SpatialAStar<Cell, object>(map);
            currentPath = pather.Search(start, end, null);

            pathInterp = interpolators.AddInterpolator(1f, null, null, true);
        }

        public override string Description
        {
            get { return "Pathfinding"; }
        }

        public override void OnKey(KeyRawEventData key)
        {
            switch (key.Key)
            {
                case KeyCode.Space:
                    MakeTree();

                    start = bsp.GetByLevel(bsp.Depth).First().Rect.Center();
                    end = bsp.GetByLevel(bsp.Depth).Last().Rect.Center();

                    currentPath = pather.Search(start, end, null);
                    break;

            }
        }

        public override void OnMouseClick(MouseEventData mouse)
        {
            if (mouse.Button == MouseButton.Left)
            {
                start = new Point(mouse.CX, mouse.CY);
            }
            else if (mouse.Button == MouseButton.Right)
            {
                end = new Point(mouse.CX, mouse.CY);
            }
            currentPath = pather.Search(start, end, null);
        }

        public override Surface Draw()
        {
            surface.Clear();

            DrawMap();
            DrawPath();

            DrawMenu();

            Surface.BlitAlpha(menuSurface, menuSurface.Rect, surface,
                (surface.Width - menuSurface.Width) / 2,
                surface.Height - menuSurface.Height - 1,
                0.9f);

            return base.Draw();
        }

        void DrawPath()
        {
            if (currentPath == null)
                return;

            int pathLength = currentPath.Count;
            int markerPos = (int)(pathInterp.Progress * pathLength);
            int current = 0;

            foreach (var n in currentPath)
            {
                Color color;

                if (current != markerPos)
                {
                    float percent = current / (float)pathLength;

                    color = ColorHelper.Lerp(Color.Green, Color.Red, percent);
                }
                else
                {
                    color = Color.White;
                }

                surface.SetBackground(n.X, n.Y, color);

                current++;
            }
        }

        void DrawMenu()
        {
            menuSurface.DefaultBackground = Color.Beige;
            menuSurface.DefaultForeground = Color.Black;

            menuSurface.DrawFrame(menuSurface.Rect, null, true);

            menuSurface.PrintString(1, 1, "Left click to place Start Position");
            menuSurface.PrintString(1, 2, "Left click to place End Position");
            menuSurface.PrintString(1, 3, "[SPACE] New Random Map");
        }


    }
}
