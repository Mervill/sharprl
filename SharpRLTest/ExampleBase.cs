using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpRL;
using SharpRL.Framework;
using System.Drawing;

namespace SharpRLTest
{
    abstract class ExampleBase
    {
        protected Surface surface;
        protected Random rnd = new Random();

        protected TimerCollection timers;
        protected InterpolatorCollection interpolators;

        // Basic map cell used for field of view and pathfinding examples
        protected class Cell : IFovCell, IPathCell<object>
        {
            public Cell(bool isWall, int x, int y)
            {
                this.IsWall = isWall;
                visible = false;

                this.X = x;
                this.Y = y;
            }

            bool visible;

            public int X
            {
                get;
                private set;
            }

            public int Y
            {
                get;
                private set;
            }

            public bool IsWall
            {
                get;
                set;
            }

            public bool IsTransparent
            {
                get
                {
                    if (IsWall)
                        return false;
                    else
                        return true;
                }
            }

            public bool IsVisible
            {
                get
                {
                    return visible;
                }
                set
                {
                    visible = value;
                }
            }

            public bool IsWalkable(object context)
            {
                if (IsWall)
                    return false;
                else
                    return true;
            }
        }

        // A simple map for field of view and pathfinding examples
        protected Array2d<Cell> map;

        protected BSPTree bsp;
        protected int minHSize = 4;
        protected int minVSize = 4;
        protected float maxVRatio = 1f;
        protected float maxHRatio = 1f;

        protected int currLevel = 6;

        protected ExampleBase()
        {
            timers = new TimerCollection();
            interpolators = new InterpolatorCollection();

            surface = new Surface(ExampleApp.ExampleRect.Width, ExampleApp.ExampleRect.Height);

            map = new Array2d<Cell>(ExampleApp.ExampleRect.Width, ExampleApp.ExampleRect.Height);
            for (int y = 0; y < map.Height; y++)
                for (int x = 0; x < map.Width; x++)
                    map[x, y] = new Cell(false, x, y);

            MakeTree();
        }


        protected void MakeTree()
        {
            bsp = new BSPTree(this.surface.Rect);
            bsp.SplitRecursive(currLevel, minHSize, minVSize, maxHRatio, maxVRatio, rnd);

            MakeMapFromBSP();
        }

        protected void MakeMapFromBSP()
        {
            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    map[x, y].IsWall = true;
                }
            }
            // first create all the rooms in the leaves
            foreach (var n in bsp.GetAllLeaves())
            {
                Rectangle rect = n.Rect;

                // shrink the rect by a small random amount
                int shrinkWMax = rect.Width / 3;
                int shrinkHMax = rect.Height / 3;

                shrinkWMax = MathHelper.Clamp(shrinkWMax, 1, shrinkWMax);
                shrinkHMax = MathHelper.Clamp(shrinkHMax, 1, shrinkHMax);

                int shrinkW = -rnd.GetInt(1, shrinkWMax);
                int shrinkH = -rnd.Next(1, shrinkHMax);

                rect.Inflate(shrinkW, shrinkH);

                // now put the room on the map
                foreach (var p in rect.Points())
                {
                    map[p.X, p.Y].IsWall = false;
                }

                // now draw corridoor between each sibling in tree, here we just simplify by
                // tunneling a line from the center of each room rectangle
                for (int d = bsp.Depth - 1; d >= 0; d--)
                {
                    foreach (var node in bsp.GetByLevel(d))
                    {
                        if (node.IsLeaf)
                            continue;

                        var center1 = node.Left.Rect.Center();
                        var center2 = node.Right.Rect.Center();
                        var line = Bresenham.GetLine(center1, center2);

                        foreach (var pos in line)
                        {
                            map[pos.X, pos.Y].IsWall = false;
                        }
                    }
                }
            }
        }

        protected virtual void DrawMap()
        {
            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    if (map[x, y].IsWall)
                        surface.PrintChar(x, y, '#', Color.SlateGray);
                    else
                        surface.PrintChar(x, y, '.', Color.DarkGreen);
                }
            }
        }

        public virtual Surface Draw()
        {
            return surface;
        }

        public virtual void Update(float elapsed)
        {
            timers.Update(elapsed);
            interpolators.Update(elapsed);
        }

        public virtual void OnKey(KeyEventArgs key) { }

        public virtual void OnMouseClick(MouseEventArgs e) { }

        public virtual void OnFontChanged() { }

        public abstract string Description { get; }

    }
}
