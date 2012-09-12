﻿//Copyright (c) 2012 Shane Baker
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

namespace SharpRL.Framework
{
    public static class RandomHelper
    {
        /// <summary>
        /// Returns 0 or 1
        /// </summary>
        /// <param name="rnd"></param>
        /// <returns></returns>
        public static int GetInt(this Random rnd)
        {
            return rnd.Next(0, 2);
        }

        /// <summary>
        /// Returns a random value from min to max, inclusive
        /// </summary>
        /// <param name="rnd"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int GetInt(this Random rnd, int min, int max)
        {
            return rnd.Next(min, max + 1);
        }

        /// <summary>
        /// Has an equal chance of returning true or false.
        /// </summary>
        /// <returns></returns>
        public static bool GetBoolean(this Random rnd)
        {
            return (rnd.GetInt() == 0);
        }

        /// <summary>
        /// Creates a color with randomized red, green and blue values
        /// </summary>
        /// <param name="rnd"></param>
        /// <returns></returns>
        public static Color GetColor(this Random rnd)
        {
            return Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
        }

        /// <summary>
        /// Randomly chooses an Enum value of the given type.
        /// </summary>
        /// <typeparam name="T">Must be an enum, or an exception will be thrown.</typeparam>
        /// <returns></returns>
        public static T GetEnum<T>(this Random rnd)
        {
            T[] values = (T[])Enum.GetValues(typeof(T));

            return values[rnd.Next(0, values.Length)];
        }

        /// <summary>
        /// Randomly chooses a single character from the provided string.
        /// </summary>
        /// <param name="str">Must not be null or empty, or this method will throw an ArgumentException</param>
        /// <returns></returns>
        public static char GetRandomCharacter(this Random rnd, string str)
        {
            if (string.IsNullOrEmpty(str))
                throw new ArgumentException("The string cannot be null or empty", "str");

            int pos = rnd.GetInt(0, str.Length - 1);
            return str[pos];
        }

        /// <summary>
        /// Fake, approximate standard deviation by iterating and summing random values.
        /// </summary>
        /// <param name="rnd"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="iter">Number of iterations performed</param>
        /// <returns></returns>
        public static int StandardDeviation(this Random rnd, int min, int max, int iter)
        {
            double tot = 0;

            for (int i = 0; i < iter; i++)
            {
                tot += rnd.NextDouble();
            }

            int add = min;
            double fact = max - min + 1;
            fact = fact / iter;

            double ret = fact * tot + add;

            return (int)ret;
        }

        public static int Roll(this Random rnd, Dice d)
        {
            int roll = 0;

            for (int i = 0; i < d.NumberOfDice; i++)
            {
                roll += rnd.GetInt(1, d.NumberOfSides);
            }

            roll += d.Modifier;

            return roll;
        }

    }
}
