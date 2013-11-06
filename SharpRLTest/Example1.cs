using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpRL;
using System.Drawing;
using SharpRL.Toolkit;

namespace SharpRLTest
{
    class Example1 : ExampleBase
    {
        public override string Description
        {
            get { return "True Color Drawing"; }
        }

        float mod;

        public override Surface Draw()
        {
            mod += .05f;

            for (int y = 0; y < surface.Height; y++)
            {
                for (int x = 0; x < surface.Width; x++)
                {
                    float dt = Plasma(mod, x, y);

                    float hue = (float)dt * 360f;
                    Color col = ColorHelper.CreateFromHSV(hue, 1, 1);

                    char c = (char)rnd.GetRandomCharacter("/\\<>");
                    surface.PrintChar(x, y, c, Color.Black, col);
                }
            }

            return base.Draw();
        }

        // Cheap "old school" plasma effect...
        private float Plasma(float time, float x, float y)
        {

            var ot = (float)Math.Cos(time) * 10;

            var z1 = Math.Sin(Dist(0, 0, x + ot, y) / 25);
            var z2 = Math.Sin(Dist(40, 30, x, y + ot) / 40);
            var z3 = Math.Sin((y + time * 3) / 19);
            var z4 = Math.Cos((x - y / 2 + time * 4) / 10);

            return (float)(z1 + z2 * z3 * z4);
        }

        private float Dist(float x0, float y0, float x1, float y1)
        {
            return (float)(Math.Sqrt((x1 - x0) * (x1 - x0) + (y1 - y0) * (y1 - y0)));
        }
    }
}
