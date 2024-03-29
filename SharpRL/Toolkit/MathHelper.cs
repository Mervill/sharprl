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

namespace SharpRL.Toolkit
{
    /// <summary>
    /// Static utility class with various numerical methods
    /// </summary>
    public static class MathHelper
    {
        /// <summary>
        /// Returns the specified value clamped to the minimum and maximum values inclusive.
        /// </summary>
        /// <typeparam name="T">An IComparable value type</typeparam>
        /// <param name="value">The value to clamp</param>
        /// <param name="min">The minimum value returned</param>
        /// <param name="max">The maximum value returned</param>
        /// <returns></returns>
        public static T Clamp<T>(T value, T min, T max) where T : struct, IComparable<T>
        {
            T ret = value;
            if (ret.CompareTo(min) < 0)
                ret = min;
            if (ret.CompareTo(max) > 0)
                ret = max;
            return ret;
        }

        /// <summary>
        /// Returns a linearly interpolated value according to the amount.  If amount is 0, then the
        /// value will be equal to "from".  If amount is 1, then value will be equal to "to"
        /// </summary>
        /// <param name="from">The starting value</param>
        /// <param name="to">The ending value</param>
        /// <param name="amount">Amount to interpolate, should be from 0.0 to 1.0</param>
        /// <returns></returns>
        public static float Lerp(float from, float to, float amount)
        {
            amount = Clamp(amount, 0f, 1f);

            return from + amount * (to - from);
        }

        /// <summary>
        /// Returns the maximum of the specified values.
        /// </summary>
        /// <typeparam name="T">An IComparable type</typeparam>
        /// <param name="vals">List of values to compare</param>
        /// <returns></returns>
        public static T Max<T>(params T[] vals) where T : IComparable
        {
            T ret = vals[0];

            for (int i = 1; i < vals.Length; i++)
            {
                if (vals[i].CompareTo(ret) > 0)
                    ret = vals[i];
            }

            return ret;
        }

        /// <summary>
        /// Returns the minimum of the specified values.
        /// </summary>
        /// <typeparam name="T">An IComparable type</typeparam>
        /// <param name="vals">List of values to compare</param>
        /// <returns></returns>
        public static T Min<T>(params T[] vals) where T : IComparable
        {
            T ret = vals[0];

            for (int i = 1; i < vals.Length; i++)
            {
                if (vals[i].CompareTo(ret) < 0)
                    ret = vals[i];
            }

            return ret;
        }
    }
}
