using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using SharpRL;
using SharpRL.Framework;

namespace SharpRLTest
{
    class Example4 : ExampleBase
    {
        Surface menuSurface;
        Point playerPos;
        FieldOfView<Cell> FOV;

        int range;
        FOVMethod method;
        RangeLimitShape shape;

        public Example4()
        {
            currLevel = 4;
            MakeTree();

            menuSurface = new MemorySurface(58, 6);

            FOV = new FieldOfView<Cell>(map);

            playerPos = new Point(surface.Width / 2, surface.Height / 2);

            range = 20;
            method = FOVMethod.MRPAS;
            shape = RangeLimitShape.Square;
        }

        public override string Description
        {
            get { return "Field Of View"; }
        }

        public override void OnKey(KeyRawEventData key)
        {
            switch (key.Key)
            {
                case KeyCode.W:
                    if (playerPos.Y > 0)
                        playerPos.Y--;
                    break;

                case KeyCode.S:
                    if (playerPos.Y < map.Height - 1)
                        playerPos.Y++;
                    break;

                case KeyCode.D:
                    if (playerPos.X < map.Width - 1)
                        playerPos.X++;
                    break;

                case KeyCode.A:
                    if (playerPos.X > 0)
                        playerPos.X--;
                    break;

                case KeyCode.Space:
                    MakeTree();
                    break;

                case KeyCode.KeypadAdd:
                case KeyCode.Add:
                    range++;
                    break;

                case KeyCode.Subtract:
                case KeyCode.KeypadSubtract:
                    if (range > 0)
                        range--;
                    break;

                case KeyCode.Keypad1:
                case KeyCode.Number1:
                    if (method == FOVMethod.MRPAS)
                        method = FOVMethod.RecursiveShadowcasting;
                    else
                        method = FOVMethod.MRPAS;

                    break;

                case KeyCode.Keypad2:
                case KeyCode.Number2:
                    switch (shape)
                    {
                        case RangeLimitShape.Circle:
                            shape = RangeLimitShape.Square;
                            break;

                        case RangeLimitShape.Octagon:
                            shape = RangeLimitShape.Circle;
                            break;

                        case RangeLimitShape.Square:
                            shape = RangeLimitShape.Octagon;
                            break;
                    }
                    break;

            }
        }

        public override Surface Draw()
        {
            surface.Clear();

            FOV.ComputeFov(playerPos.X, playerPos.Y,
                range,
                true,
                method,
                shape);

            DrawMap();
            DrawMenu();

            Surface.BlitAlpha(menuSurface, menuSurface.Rect, surface,
                (surface.Width - menuSurface.Width) / 2,
                surface.Height - menuSurface.Height - 1,
                0.9f);

            return base.Draw();
        }

        void DrawMenu()
        {
            menuSurface.DefaultBackground = Color.Beige;
            menuSurface.DefaultForeground = Color.Black;

            menuSurface.DrawFrame(menuSurface.Rect, null, true);

            menuSurface.PrintString(1, 1, string.Format("[+/-] Increase/Decrease Range, current = {0}", range));
            menuSurface.PrintString(1, 2, string.Format("[1] Toggle FOV Method, current = {0}", method));
            menuSurface.PrintString(1, 3, string.Format("[2] Toggle FOV Shape, current = {0}", shape));
            menuSurface.PrintString(1, 4, "[SPACE] New Random Map");
        }

        override protected void DrawMap()
        {
            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    char c;
                    Color col;

                    if (map[x, y].IsTransparent)
                    {
                        c = '.';
                    }
                    else
                    {
                        c = '#';
                    }

                    if (map[x, y].IsVisible)
                    {
                        col = Color.Gold;
                    }
                    else
                    {
                        col = Color.DarkBlue;
                    }

                    surface.PrintChar(x, y, c, col);
                }
            }

            surface.PrintChar(playerPos.X, playerPos.Y, '@', Color.White, Color.DarkRed);
        }

    }
}
