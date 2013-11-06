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

namespace SharpRL.Toolkit
{
    internal abstract class FOVAlgorithm<TFovNode> where TFovNode : IFovCell
    {
        public FOVAlgorithm(FieldOfView<TFovNode> map)
        {
            this.map = map;
        }

        FieldOfView<TFovNode> map;
        public FieldOfView<TFovNode> Map
        {
            get { return map; }
            set
            {
                map = value;
            }
        }

        RangeLimitShape rangeShape;
        protected RangeLimitShape RangeShape
        {
            get { return this.rangeShape; }
            set { this.rangeShape = value; }
        }

        int viewpointX;
        public int ViewPointX
        {
            get { return viewpointX; }
            set { viewpointX = value; }
        }

        int viewpointY;
        public int ViewPointY
        {
            get { return viewpointY; }
            set { viewpointY = value; }
        }

        public abstract void ComputeFOV();

        protected bool IsInMap(int x, int y)
        {
            if (x < 0 || x >= map.grid.Width)
                return false;
            if (y < 0 || y >= map.grid.Height)
                return false;

            return true;
        }

        protected bool IsInRange(int x1, int y1, int x2, int y2, int maxRange)
        {
            if (maxRange <= 0)
                return true;

            switch (rangeShape)
            {
                case RangeLimitShape.Circle:
                    return CheckCircularRange(x1, y1, x2, y2, maxRange);

                case RangeLimitShape.Square:
                    return CheckSquareRange(x1, y1, x2, y2, maxRange);

                case RangeLimitShape.Octagon:
                    return CheckDiamondRange(x1, y1, x2, y2, maxRange);
            }

            return false;
        }

        protected bool CheckSquareRange(int x1, int y1, int x2, int y2, int maxRange)
        {
            int dx = Math.Abs(x1 - x2);
            int dy = Math.Abs(y1 - y2);

            return (dx <= maxRange && dy <= maxRange);
        }

        protected bool CheckDiamondRange(int x1, int y1, int x2, int y2, int maxRange)
        {
            int dx = Math.Abs(x1 - x2);
            int dy = Math.Abs(y1 - y2);

            return (Math.Max(dx, dy) + Math.Min(dx, dy) / 2 <= maxRange);
        }

        protected bool CheckCircularRange(int x1, int y1, int x2, int y2, int maxRange)
        {
            if (x1 == x2) //if they're on the same axis, we only need to test one value, which is computationaly cheaper than what we do below
            {
                return Math.Abs(y1 - y2) <= maxRange;
            }

            if (y1 == y2)
            {
                return Math.Abs(x1 - x2) <= maxRange;
            }

            return (Math.Pow((x1 - x2), 2) + Math.Pow((y1 - y2), 2)) <= Math.Pow(maxRange, 2);
        }
    }
}
