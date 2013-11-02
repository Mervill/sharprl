using System;
using System.Collections.Generic;
using System.Linq;
using SharpRL;
using RLGui;
using System.Drawing;

namespace RLGuiTest
{
    class Program
    {
        static void Main(string[] args)
        {
            App app = new App();

            app.Run();
        }
    }

    class App
    {
        GameConsole console;

        FontSheet font;

        UIManager manager;

        public App()
        {
            console = new GameConsole(60, 40, "Gui Test", 800, 600);

            console.IsWindowResizable = false;
            console.VerticalSyncEnabled = true;

            font = new FontSheet(console, "terminal16x16_gs_ro.png", FontFormat.GreyscaleAA, FontLayout.InRow);

            console.SetFont(font, true);

            manager = new UIManager(console);

            var panel = new MyPanel(new Rectangle(5,7,20,20));

            var buttonTemplate = new ButtonTemplate()
            {
                Label = "A Button",
                HasFrame = true,
                HAlignment = HorizontalAlignment.Center,
                VAlignment = VerticalAlignment.Top,
                Title = "Button",
                TitleLocation = FrameTitleLocation.UpperLeft,
                MinimumSize = new Size(1,1),
                FrameDefinition = new FrameDefinition()
                {
                    HorizontalLine = (char)SpecialChar.DoubleHorzLine,
                    VerticalLine = (char)SpecialChar.ExclamationDouble,
                    CornerLowerLeft = (char)SpecialChar.DoubleCrossLines,
                    CornerLowerRight = (char)SpecialChar.DoubleCrossLines,
                    CornerUpperLeft = (char)SpecialChar.DoubleCrossLines,
                    CornerUpperRight = (char)SpecialChar.DoubleCrossLines
                }
            };

            var btn = new Button(new Point(1, 1), buttonTemplate);

            var cb = new CheckBox(new Point(15,2),
                new CheckBoxTemplate() { Label = "CheckME", HasFrame = false, UnCheckedChar = 'O', CheckedChar = 'X',
                });
            
            manager.AddComponents(panel, btn, cb);
        }


        public void Run()
        {
            console.Run(null);

            console.Dispose();
        }
    }


    class MyPanel : Panel
    {
        public MyPanel(Rectangle rect)
            : base(rect)
        {
            Rectangle clientRect = new Rectangle(0, 0, Rect.Width, Rect.Height);
        }

        protected override void OnRedraw(Surface drawingSurface)
        {
            base.OnRedraw(drawingSurface);

            drawingSurface.Fill(drawingSurface.Rect, '*');

            if (IsMouseOver)
            {
                drawingSurface.PrintChar(MousePosition.X, MousePosition.Y, '*', Color.Blue, Color.Red);
            }

            drawingSurface.DrawFrame(drawingSurface.Rect);
        }

    }

}
