/*
 * Based on code found here: http://www.evilscience.co.uk/?p=225
 * 
 * 
 * */

/*
 * Modifed by Shane Baker 2011
 * Fixed bugs, optimized by removing unnecessary exception handling
 * 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpRL.Toolkit
{
    internal class ShadowCasting<TFovNode> : FOVAlgorithm<TFovNode> where TFovNode : IFovCell
    {
        public ShadowCasting(FieldOfView<TFovNode> map)
            : base(map)
        {

        }

        int maxRange;
        public int MaxRange
        {
            get { return maxRange; }
            set { maxRange = value; }
        }

        bool lightWalls;
        public bool LightWalls
        {
            get { return lightWalls; }
            set { lightWalls = value; }
        }

        public void ComputeFOV(int viewpointX, int viewpointY, int maxRange, bool lightWalls, RangeLimitShape rangeShape)
        {
            this.ViewPointX = viewpointX;
            this.ViewPointY = viewpointY;
            this.maxRange = maxRange;
            this.lightWalls = lightWalls;
            this.RangeShape = rangeShape;

            ComputeFOV();
        }

        public override void ComputeFOV()
        {
            maxDepth = Math.Max(Map.grid.Width, Map.grid.Height);

            for (int octant = 1; octant < 9; octant++)
            {
                Scan(1, octant, 1.0, 0.0, ViewPointX, ViewPointY);
            }
        }

        int maxDepth;

        private double GetSlope(double x1, double y1, double x2, double y2)
        {
            return (x1 - x2) / (y1 - y2);
        }

        private double GetSlopeInv(double x1, double y1, double x2, double y2)
        {
            return (y1 - y2) / (x1 - x2);
        }

        private bool TestCell(int x, int y, bool transparency, int playerX, int playerY)
        {
            if (!IsInMap(x, y))
                return transparency;

            if (!IsInRange(playerX, playerY, x, y, maxRange))
                return false;
            return Map.grid[x, y].IsTransparent == transparency;
        }

        private void Scan(int depth, int octant, double startSlope, double endSlope, int playerX, int playerY)
        {
            int x = 0;
            int y = 0;

            switch (octant)
            {
                case 1:
                    y = playerY - depth;
                    x = playerX - (int)((startSlope * (depth)));

                    while (GetSlope(x, y, playerX, playerY) >= endSlope)
                    {
                        if (IsInMap(x, y) && IsInRange(playerX, playerY, x, y, maxRange))
                        {

                            if (!Map.grid[x, y].IsTransparent) //cell blocked
                            {
                                Map.grid[x, y].IsVisible = lightWalls;
                                //if prior open AND within range
                                if (TestCell(x - 1, y, true, playerX, playerY))
                                {
                                    //recursion
                                    Scan(depth + 1, octant, startSlope, GetSlope(x - .5, y + 0.5, playerX, playerY), playerX, playerY);
                                }

                            }
                            else //not blocked
                            {
                                //if prior closed AND within range
                                if (TestCell(x - 1, y, false, playerX, playerY))
                                {
                                    startSlope = GetSlope(x - .5, y - 0.5, playerX, playerY);
                                }
                                Map.grid[x, y].IsVisible = true;
                            }

                        }
                        x++;

                    }
                    x--; //we step back as the last step of the while has taken us past the limit

                    break;

                case 2:

                    y = playerY - depth;
                    x = playerX + (int)(startSlope * depth);

                    while (GetSlope(x, y, playerX, playerY) <= endSlope)
                    {
                        if (IsInMap(x, y) && IsInRange(playerX, playerY, x, y, maxRange))
                        {

                            if (!Map.grid[x, y].IsTransparent)
                            {
                                Map.grid[x, y].IsVisible = lightWalls;
                                if (TestCell(x + 1, y, true, playerX, playerY))
                                {
                                    Scan(depth + 1, octant, startSlope, GetSlope(x + 0.5, y + 0.5, playerX, playerY), playerX, playerY);
                                }
                            }
                            else
                            {
                                if (TestCell(x + 1, y, false, playerX, playerY))
                                {
                                    startSlope = -GetSlope(x + 0.5, y - 0.5, playerX, playerY);
                                }
                                Map.grid[x, y].IsVisible = true;
                            }

                        }
                        x--;

                    }
                    x++;

                    break;


                case 3:

                    x = playerX + depth;
                    y = playerY - (int)(startSlope * depth);

                    while (GetSlopeInv(x, y, playerX, playerY) <= endSlope)
                    {

                        if (IsInMap(x, y) && IsInRange(playerX, playerY, x, y, maxRange))
                        {

                            if (!Map.grid[x, y].IsTransparent) //cell blocked
                            {
                                Map.grid[x, y].IsVisible = lightWalls;
                                if (TestCell(x, y - 1, true, playerX, playerY))
                                {
                                    Scan(depth + 1, octant, startSlope, GetSlopeInv(x - 0.5, y - 0.5, playerX, playerY), playerX, playerY);
                                }
                            }
                            else //not blocked
                            {
                                //if prior closed AND within range
                                if (TestCell(x, y - 1, false, playerX, playerY))
                                {
                                    startSlope = -GetSlopeInv(x + 0.5, y - 0.5, playerX, playerY);
                                }
                                Map.grid[x, y].IsVisible = true;
                            }

                        }
                        y++;

                    }
                    y--; //we step back as the last step of the while has taken us past the limit

                    break;

                case 4:

                    x = playerX + depth;
                    y = playerY + (int)(startSlope * depth);

                    while (GetSlopeInv(x, y, playerX, playerY) >= endSlope)
                    {

                        if (IsInMap(x, y) && IsInRange(playerX, playerY, x, y, maxRange))
                        {

                            if (!Map.grid[x, y].IsTransparent)
                            {
                                Map.grid[x, y].IsVisible = lightWalls;
                                if (TestCell(x, y + 1, true, playerX, playerY))
                                {
                                    Scan(depth + 1, octant, startSlope, GetSlopeInv(x - 0.5, y + 0.5, playerX, playerY), playerX, playerY);
                                }
                            }
                            else
                            {

                                if (TestCell(x, y + 1, false, playerX, playerY))
                                {
                                    startSlope = GetSlopeInv(x + 0.5, y + 0.5, playerX, playerY);
                                }
                                Map.grid[x, y].IsVisible = true;
                            }

                        }
                        y--;

                    }
                    y++;

                    break;

                case 5:

                    y = playerY + depth;
                    x = playerX + (int)(startSlope * depth);

                    while (GetSlope(x, y, playerX, playerY) >= endSlope)
                    {
                        if (IsInMap(x, y) && IsInRange(playerX, playerY, x, y, maxRange))
                        {

                            if (!Map.grid[x, y].IsTransparent)
                            {
                                Map.grid[x, y].IsVisible = lightWalls;
                                if (TestCell(x + 1, y, true, playerX, playerY))
                                {
                                    Scan(depth + 1, octant, startSlope, GetSlope(x + 0.5, y - 0.5, playerX, playerY), playerX, playerY);
                                }
                            }
                            else
                            {
                                if (TestCell(x + 1, y, false, playerX, playerY))
                                {
                                    startSlope = GetSlope(x + 0.5, y + 0.5, playerX, playerY);
                                }
                                Map.grid[x, y].IsVisible = true;
                            }

                        }
                        x--;

                    }
                    x++;

                    break;

                case 6:

                    y = playerY + depth;
                    x = playerX - (int)(startSlope * depth);

                    while (GetSlope(x, y, playerX, playerY) <= endSlope)
                    {
                        if (IsInMap(x, y) && IsInRange(playerX, playerY, x, y, maxRange))
                        {

                            if (!Map.grid[x, y].IsTransparent)
                            {
                                Map.grid[x, y].IsVisible = lightWalls;
                                if (TestCell(x - 1, y, true, playerX, playerY))
                                {
                                    Scan(depth + 1, octant, startSlope, GetSlope(x - 0.5, y - 0.5, playerX, playerY), playerX, playerY);
                                }
                            }
                            else
                            {
                                if (TestCell(x - 1, y, false, playerX, playerY))
                                {
                                    startSlope = -GetSlope(x - 0.5, y + 0.5, playerX, playerY);
                                }
                                Map.grid[x, y].IsVisible = true;
                            }

                        }
                        x++;

                    }
                    x--;

                    break;

                case 7:

                    x = playerX - depth;
                    y = playerY + Convert.ToInt32((startSlope * Convert.ToDouble(depth)));

                    while (GetSlopeInv(x, y, playerX, playerY) <= endSlope)
                    {
                        if (IsInMap(x, y) && IsInRange(playerX, playerY, x, y, maxRange))
                        {

                            if (!Map.grid[x, y].IsTransparent)
                            {
                                Map.grid[x, y].IsVisible = lightWalls;
                                if (TestCell(x, y + 1, true, playerX, playerY))
                                {
                                    Scan(depth + 1, octant, startSlope, GetSlopeInv(x + 0.5, y + 0.5, playerX, playerY), playerX, playerY);
                                }
                            }
                            else
                            {
                                if (TestCell(x, y + 1, false, playerX, playerY))
                                {
                                    startSlope = -GetSlopeInv(x - 0.5, y + 0.5, playerX, playerY);
                                }
                                Map.grid[x, y].IsVisible = true;
                            }

                        }
                        y--;

                    }
                    y++;
                    break;

                case 8:

                    x = playerX - depth;
                    y = playerY - Convert.ToInt32((startSlope * Convert.ToDouble(depth)));

                    while (GetSlopeInv(x, y, playerX, playerY) >= endSlope)
                    {

                        if (IsInMap(x, y) && IsInRange(playerX, playerY, x, y, maxRange))
                        {

                            if (!Map.grid[x, y].IsTransparent)
                            {
                                Map.grid[x, y].IsVisible = lightWalls;
                                if (TestCell(x, y - 1, true, playerX, playerY))
                                {
                                    Scan(depth + 1, octant, startSlope, GetSlopeInv(x + 0.5, y - 0.5, playerX, playerY), playerX, playerY);
                                }
                            }
                            else
                            {
                                if (TestCell(x, y - 1, false, playerX, playerY))
                                {
                                    startSlope = GetSlopeInv(x - 0.5, y - 0.5, playerX, playerY);
                                }
                                Map.grid[x, y].IsVisible = true;
                            }

                        }
                        y++;

                    }
                    y--;

                    break;


            }

            if (depth < maxDepth)
            {
                if (!IsInMap(x, y) || Map.grid[x, y].IsTransparent)
                {
                    Scan(depth + 1, octant, startSlope, endSlope, playerX, playerY);
                }
            }


        }
    }
}