﻿using System;
using System.Collections.Generic;
using System.Linq;
using SharpRL;
using SharpRL.Toolkit;
using RLGui;
using System.Drawing;
using RLGui.Controls;

namespace RLGuiTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //App app = new App();

            //app.Run();

            Program program = new Program();

            program.Start();

        }

        GameConsole console;
        FontSheet font;
        View navigationView;

        void Start()
        {
            console = new GameConsole(60, 40, "Gui Test", 800, 600);

            console.IsWindowResizable = false;
            console.VerticalSyncEnabled = true;

            font = new FontSheet(console, "terminal16x16_gs_ro.png", TransparencyMethod.ByValue, FontMapping.InRow);

            console.SetFont(font, true);

            SetupNavigationView();

            console.Run(null);

            console.Dispose();
        }

        void SetupNavigationView()
        {
            navigationView = new View(new Rectangle(0, 37, 60, 3), console.Root);

            navigationView.AddComponents(
                new Button(new Point(0,0), new ButtonTemplate()
                {
                    Label = "Previous",
                    MinimumSize = new Size(30,3),
                    KeyboardMode = KeyboardInputMode.Never,
                }),
                new Button(new Point(30,0),new ButtonTemplate()
                {
                    Label = "Next",
                    MinimumSize = new Size(30,3),
                    KeyboardMode = KeyboardInputMode.Never
                })
                );

            Dispatcher dispatch = new Dispatcher(console, navigationView);

            dispatch.Start();
        }
    }

    class App
    {
        GameConsole console;

        FontSheet font;

        View mainView;

        public App()
        {
            console = new GameConsole(60, 40, "Gui Test", 800, 600);

            console.IsWindowResizable = false;
            console.VerticalSyncEnabled = true;

            font = new FontSheet(console, "terminal16x16_gs_ro.png", TransparencyMethod.ByValue, FontMapping.InRow);

            console.SetFont(font, true);

            Rectangle viewport = new Rectangle(1, 1, 59, 39);
            mainView = new View(viewport, console.Root);

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

            btn.PushReleased += btn_Clicked;

            //var lb = new ListBox(new Point(30, 15), new ListBoxTemplate(listItems)
            //{
            //    ToolTipText = "A very fine listbox example thingy"
            //});

            var lb = new RadioBox(new Point(30, 15), new RadioBoxTemplate(listItems)
            {
                RadioSetChar = (char)7,
                RadioUnsetChar = (char)9,
                CanHaveFocus = true
            });

            var cb = new CheckButton(new Point(15,2),
                new CheckButtonTemplate() { Label = "CheckME", HasFrame = true, UnCheckedChar = 'O', CheckedChar = 'X',
                    Layer = 100,
                });

            var eb = new TextEntry(new Point(5, 30), new TextEntryTemplate()
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

            var nb = new NumberEntry(new Point(25, 30), new NumberEntryTemplate()
            {
                MinimumValue = -50,
                MaximumValue = 100,
                InitialValue = 0
            });

            mainView.AddComponents(panel, btn, cb, lb, eb, nb);

            Dispatcher dispatch = new Dispatcher(console, mainView);

            dispatch.Start();
        }

        void btn_Clicked(object sender, EventArgs e)
        {
            Rectangle rect = (sender as Control).Rect;

            var menu = new MenuBox(new Point(rect.Center().X, rect.Bottom), new MenuBoxTemplate()
                {
                    Items = new List<ItemData>()
                    {
                        new ItemData("New Document"),
                        new ItemData("Open Document"),
                        new ItemData("Pick this"),
                        new ItemData("Exit")
                    },
                    CanHaveFocus = true
                });

            mainView.AddComponents(menu);


        }

        public void Run()
        {
            console.Drawing += console_Drawing;

            console.Run(null);

            console.Dispose();
        }

        void console_Drawing(object sender, EventArgs<float> e)
        {
            
        }
    }

    

    class MyPanel : Widget
    {
        public MyPanel(Rectangle rect)
            : base(rect)
        {
            Rectangle clientRect = new Rectangle(0, 0, Rect.Width, Rect.Height);
        }

        protected override void OnRedraw()
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
