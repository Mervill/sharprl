using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpRL.Toolkit;
using System.Drawing;
using SharpRL;

namespace SharpRLTest
{
    class Example3 : ExampleBase
    {
        Surface menuSurface;

        public Example3()
        {
            menuSurface = new MemorySurface(28, 6);
            MakeTree();
        }

        public override string Description
        {
            get { return "Binary Space Partition"; }
        }

        public override void OnKey(KeyRawEventData key)
        {
            switch (key.Key)
            {
                case KeyCode.Space:
                    MakeTree();
                    break;

                case KeyCode.KeypadAdd:
                case KeyCode.Add:
                    currLevel++;
                    MakeTree();
                    break;

                case KeyCode.Subtract:
                case KeyCode.KeypadSubtract:
                    currLevel--;
                    if (currLevel < 0)
                        currLevel = 0;
                    MakeTree();
                    break;

            }
        }

        public override Surface Draw()
        {
            surface.Clear();

            
            //foreach (var n in bsp.GetAllLeaves())
            //{
            //    surface.DrawFrame(n.Rect, null, false, ColorHelper.CreateFromPackedInt(0xffAA22), Color.Black);
            //}

            DrawMap();

            DrawMenu();
            Surface.BlitAlpha(menuSurface, menuSurface.Rect,
                surface,
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

            menuSurface.PrintString(1, 1, "[+] Increase BSP Depth");
            menuSurface.PrintString(1, 2, "[-] Decrease BSP Depth");
            menuSurface.PrintString(1, 3, "[SPACE] New Random Tree");
            menuSurface.PrintString(1, 4, string.Format("Depth = {0}, Leaves = {1}", currLevel, bsp.GetAllLeaves().Count()));
        }


    }
}
