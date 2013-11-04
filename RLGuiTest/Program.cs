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
                //Title = "Button",
                TitleLocation = FrameTitleLocation.UpperLeft,
                ToolTipText = "Push ME!",
                MinimumSize = new Size(1,1),
                
            };

            var btn = new Button(new Point(1, 1), buttonTemplate);

            List<ItemData> listItems = new List<ItemData>()
            {
                new ItemData("Item 1", "The first item"),
                new ItemData("Item 2", "The second item"),
                new ItemData("Item 3", "The third item"),
                new ItemData("Item 4", "The fourth item")
            };

            var lb = new ListBox(new Point(30, 15), new ListBoxTemplate(listItems)
            {
                ToolTipText = "A very fine listbox example thingy"
            });

            var cb = new CheckBox(new Point(15,2),
                new CheckBoxTemplate() { Label = "CheckME", HasFrame = true, UnCheckedChar = 'O', CheckedChar = 'X',
                });

            var eb = new EntryBox(new Point(5, 30), new EntryBoxTemplate()
            {
                NumberOfCharacters = 15,
                ReplaceOnFirstKey = false,
                ValidChars = CharValidationFlags.All
            });

            eb.ValidateField += (o, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Text))
                    {
                        if (e.Text[0] != 'a')
                            e.IsValid = false;
                    }
                };

            manager.AddComponents(panel, btn, cb, lb, eb);
        }

        public void Run()
        {
            console.Run(null);

            console.Dispose();
        }
    }


    class MyPanel : Widget
    {
        public MyPanel(Rectangle rect)
            : base(rect)
        {
            Rectangle clientRect = new Rectangle(0, 0, Rect.Width, Rect.Height);
        }

        protected override void OnPaint()
        {
            char thing = '*';

            if (IsHovering)
                thing = '+';

            DrawingSurface.Fill(DrawingSurface.Rect, thing);
            
            if (IsMouseOver)
            {
                DrawingSurface.PrintChar(MousePosition.X, MousePosition.Y, '*', Color.Blue, Color.Red);
            }

            DrawingSurface.DrawFrame(DrawingSurface.Rect);
        }

    }


}
