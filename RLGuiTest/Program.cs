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

            var panel = new MyPanel(new Rectangle(5,5,20,20));
            //var frame = new Frame(panel);
            //var btn = new Button(new Point(7, 7), "Button", HorizontalAlignment.Left, 0);

            
            manager.AddComponents(panel);
        }


        public void Run()
        {
            console.Run(null);

            console.Dispose();
        }
    }


    class MyPanel : FramePanel
    {
        public MyPanel(Rectangle rect)
            : base(rect)
        {
            Rectangle clientRect = new Rectangle(0, 0, Rect.Width, Rect.Height);
        }

        protected override void OnRedrawClient(Surface clientSurface)
        {
            base.OnRedrawClient(clientSurface);

            clientSurface.Fill(clientSurface.Rect, '*');

            if (IsMouseOverClient)
            {
                clientSurface.PrintChar(ClientMousePosition.X, ClientMousePosition.Y, '*', Color.Blue, Color.Red);
            }
        }

        protected override void OnUpdate(float elapsed)
        {
            base.OnUpdate(elapsed);

            if (IsMouseOverFrame)
            {
                FramePigment = new Pigment(Color.Red, Color.DarkBlue);
            }
            else
            {
                FramePigment = Pigment.WhiteBlack;
            }
        }

    }

}
