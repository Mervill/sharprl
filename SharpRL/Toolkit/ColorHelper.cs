//Copyright (c) 2012 Shane Baker
//
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in
//all copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace SharpRL.Toolkit
{
    /// <summary>
    /// Static utility class for working with System.Drawing.Color objects
    /// </summary>
    public static class ColorHelper
    {
        /// <summary>
        /// Linearly interpolates between two colors by the specified amount, returning the new color.
        /// If amount is 0 (or less), then the color will be equal to "from".  If amount is 1 (or more)
        /// the returned color will be equal to "to".
        /// </summary>
        /// <param name="from">The staring color</param>
        /// <param name="to">The ending color</param>
        /// <param name="amount">The amount to interpolate, from 0.0 to 1.0</param>
        /// <returns></returns>
        public static Color Lerp(Color from, Color to, float amount)
        {
            amount = MathHelper.Clamp(amount, 0f, 1f);

            float sr = from.R;
            float sg = from.G;
            float sb = from.B;

            float er = to.R;
            float eg = to.G;
            float eb = to.B;

            byte r = (byte)MathHelper.Lerp(sr, er, amount);
            byte g = (byte)MathHelper.Lerp(sg, eg, amount);
            byte b = (byte)MathHelper.Lerp(sb, eb, amount);

            return Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Create a Color by multiplying two colors.  NewColor = (color1 * color2) / 255
        /// </summary>
        /// <param name="color1">First color</param>
        /// <param name="color2">Second color</param>
        /// <returns>The resulting Color</returns>
        public static Color CreateMultiply(Color color1, Color color2)
        {
            int r, g, b;

            r = ((int)color1.R * color2.R) / 255;
            g = ((int)color1.G * color2.G) / 255;
            b = ((int)color1.B * color2.B) / 255;

            return Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Create a Color by multiplying a color by a value (scales the r,g,b, components by the given value).
        /// </summary>
        /// <param name="color">The color to be multiplied</param>
        /// <param name="val">The scalar value to multiply by</param>
        /// <returns>The resulting Color</returns>
        public static Color CreateMultiply(Color color, float val)
        {
            int r, g, b;

            r = MathHelper.Clamp((int)(color.R * val), 0, 255);
            g = MathHelper.Clamp((int)(color.G * val), 0, 255);
            b = MathHelper.Clamp((int)(color.B * val), 0, 255);

            return Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Create a Color by adding two colors.  NewColor = Min(color1 + color2, 255)
        /// </summary>
        /// <param name="color1">The first color</param>
        /// <param name="color2">The second color</param>
        /// <returns>The resulting Color</returns>
        public static Color CreateAdd(Color color1, Color color2)
        {
            int r, g, b;

            r = Math.Min(color1.R + color2.R, 255);
            g = Math.Min(color1.G + color2.G, 255);
            b = Math.Min(color1.B + color2.B, 255);

            return Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Create a Color by subtracting one color from another.  NewColor = Max(color1 - color2, 0)
        /// </summary>
        /// <param name="color1">The first color, from which color2 is subtracted</param>
        /// <param name="color2">The second color, subtracted from the first</param>
        /// <returns>The resulting Color</returns>
        public static Color CreateSubtract(Color color1, Color color2)
        {
            int r, g, b;

            r = Math.Max(color1.R - color2.R, 0);
            g = Math.Max(color1.G - color2.G, 0);
            b = Math.Max(color1.B - color2.B, 0);

            return Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Create a color from the first 3 least significant bytes of the provided integer, where the red,green and blue
        /// components are packed in order of most to least significant.
        /// </summary>
        /// <example>
        /// CreateFromPackedInt(0xFF00FF) would create a Color with a red component of 255,
        /// a green component of 0, and a blue component of 255.
        /// </example>
        /// <param name="packedColor">The integer containing the color values</param>
        /// <returns>The resulting Color</returns>
        public static Color CreateFromPackedInt(int packedColor)
        {
            int r, g, b;

            r = packedColor & 0xff0000;
            g = packedColor & 0x00ff00;
            b = packedColor & 0x0000ff;

            r = r >> 16;
            g = g >> 8;

            return Color.FromArgb(r, g, b);
        }


        /// <summary>
        /// Creates a color given floats as the components (where each component is between 0.0f and 1.0f inclusive)
        /// </summary>
        /// <param name="r">The red component, from 0.0 to 1.0</param>
        /// <param name="g">The green component, from 0.0 to 1.0</param>
        /// <param name="b">The blue component, from 0.0 to 1.0</param>
        /// <returns>The resulting Color</returns>
        public static Color CreateFromFloatRgb(float r, float g, float b)
        {
            int red = (int)(r * 255);
            int green = (int)(g * 255);
            int blue = (int)(b * 255);

            return Color.FromArgb(red, green, blue);
        }

        /// <summary>
        /// Create a color given hue, saturation and value (HSV)
        /// </summary>
        /// <param name="hue">The hue, from 0 to 360 inclusive</param>
        /// <param name="sat">The saturation, from 0 to 1 inclusive</param>
        /// <param name="val">The value, from 0 to 1 inclusive</param>
        /// <returns>The resulting Color</returns>
        public static Color CreateFromHSV(float hue, float sat, float val)
        {
            if (sat == 0)
            {
                return CreateFromFloatRgb(val, val, val);
            }

            int i;
            float f, p, q, t, h;

            h = (float)hue / 360f;

            i = (int)Math.Floor(h * 6f);
            f = h * 6f - i;
            p = val * (1 - sat);
            q = val * (1 - sat * f);
            t = val * (1 - sat * (1 - f));

            switch (i % 6)
            {
                case 0:
                    return CreateFromFloatRgb(val, t, p);

                case 1:
                    return CreateFromFloatRgb(q, val, p);

                case 2:
                    return CreateFromFloatRgb(p, val, t);

                case 3:
                    return CreateFromFloatRgb(p, q, val);

                case 4:
                    return CreateFromFloatRgb(t, p, val);

                case 5:
                default:
                    return CreateFromFloatRgb(val, p, q);
            }

        }

        /// <summary>
        /// Create a color by replacing the hue component of the given color
        /// </summary>
        /// <param name="color">The source Color</param>
        /// <param name="hue">The hue, from 0 to 360 inclusive</param>
        /// <returns>The resulting Color</returns>
        public static Color CreateFromHue(Color color, float hue)
        {
            float h, s, v;
            GetHSV(color, out h, out s, out v);

            return CreateFromHSV(hue, s, v);
        }

        /// <summary>
        /// Create a color by replacing the saturation component of the given color
        /// </summary>
        /// <param name="color">The source Color</param>
        /// <param name="sat">The saturation, from 0 to 1 inclusive</param>
        /// <returns>The resulting Color</returns>
        public static Color CreateFromSaturation(Color color, float sat)
        {
            float h, s, v;
            GetHSV(color, out h, out s, out v);

            return CreateFromHSV(h, sat, v);
        }

        /// <summary>
        /// Create a color by replacing the value (in HSV space) of the given color
        /// </summary>
        /// <param name="color">The source Color</param>
        /// <param name="val">From 0 to 1 inclusive</param>
        /// <returns>The resulting Color</returns>
        public static Color CreateFromValue(Color color, float val)
        {
            float h, s, v;
            GetHSV(color, out h, out s, out v);

            return CreateFromHSV(h, s, val);
        }

        /// <summary>
        /// Gets the hue, saturation and value of a color
        /// </summary>
        /// <param name="color">The source Color</param>
        /// <param name="h">The hue, from 0 to 360 inclusive</param>
        /// <param name="s">The saturation, from 0 to 1 inclusive</param>
        /// <param name="v">The value, from 0 to 1 inclusive</param>
        public static void GetHSV(this Color color, out float h, out float s, out float v)
        {
            float r = color.R / 255f;
            float g = color.G / 255f;
            float b = color.B / 255f;

            float max = MathHelper.Max(r, g, b);
            float min = MathHelper.Min(r, g, b);

            h = max;
            s = max;
            v = max;

            float d = max - min;

            if (max == 0)
                s = 0;
            else
                s = d / max;

            if (max == min)
                h = 0;
            else
            {
                if (max == r)
                {
                    h = (g - b) / d + (g < b ? 6 : 0);
                }
                else if (max == g)
                {
                    h = (b - r) / d + 2;
                }
                else
                {
                    h = (r - g) / d + 4;
                }

                h = h * 60f;
            }

        }

        /// <summary>
        /// Gets the hue (in HSV) of a color
        /// </summary>
        /// <param name="color">The source Color</param>
        /// <returns>The hue, from 0 to 360 inclusive
        /// </returns>
        public static float GetHue(this Color color)
        {
            float h, s, v;
            GetHSV(color, out h, out s, out v);

            return h;
        }

        /// <summary>
        /// Gets the saturation (in HSV) of a color
        /// </summary>
        /// <param name="color">The source Color</param>
        /// <returns>The saturation, from 0 to 1 inclusive</returns>
        public static float GetSaturation(this Color color)
        {
            float h, s, v;
            GetHSV(color, out h, out s, out v);

            return s;
        }

        /// <summary>
        /// Gets the value (in HSV) of a color
        /// </summary>
        /// <param name="color">The source Color</param>
        /// <returns>The value, from 0 to 1 inclusive</returns>
        public static float GetValue(this Color color)
        {
            float h, s, v;
            GetHSV(color, out h, out s, out v);

            return v;
        }

    }
}
