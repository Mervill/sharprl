/*
 * Based on code and article for Mingo's Restrictive Precise Angle Shadowcasting
 * found here: http://roguebasin.roguelikedevelopment.org/index.php/Restrictive_Precise_Angle_Shadowcasting
 * 
 * original license below:
 * 
* MRPAS.NET
* Copyright (c) 2010 Dominik Marczuk
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*     * Redistributions of source code must retain the above copyright
*       notice, this list of conditions and the following disclaimer.
*     * Redistributions in binary form must reproduce the above copyright
*       notice, this list of conditions and the following disclaimer in the
*       documentation and/or other materials provided with the distribution.
*     * The name of Dominik Marczuk may not be used to endorse or promote products
*       derived from this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY DOMINIK MARCZUK ``AS IS'' AND ANY
* EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL DOMINIK MARCZUK BE LIABLE FOR ANY
* DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
* (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
* LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
* ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
* (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
* SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpRL.Toolkit
{
    internal class MRPAS<TFovNode> : FOVAlgorithm<TFovNode> where TFovNode : IFovCell
    {
        public MRPAS(FieldOfView<TFovNode> map)
            : base(map)
        {
            ViewPointX = 0;
            ViewPointY = 0;
            lightWalls = true;
            maxRange = Math.Max(map.grid.Width, map.grid.Height);
            shadowMap = new ShadowMap();
        }

        bool lightWalls;
        public bool LightWalls
        {
            get { return lightWalls; }
            set { lightWalls = value; }
        }

        int maxRange;
        public int MaxRange
        {
            get { return maxRange; }
            set { maxRange = value; }
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
            //set PC's position as visible
            Map.grid[ViewPointX, ViewPointY].IsVisible = true;

            //compute the 4 quadrants of the map
            ComputeQuadrant(1, 1);
            ComputeQuadrant(1, -1);
            ComputeQuadrant(-1, 1);
            ComputeQuadrant(-1, -1);
        }

        ShadowMap shadowMap;
        private double startSlope;
        private double middleSlope;
        private double endSlope;
        private double minimumScanSlope;// minimum slope that we need to check for each pass
        // eliminates needless checks within areas we now is in shadow

        private void ComputeQuadrant(int deltaX, int deltaY)
        {
            int x, y, currIteration;

            #region Vertical Edge Octants
            currIteration = 1;
            shadowMap.Clear();
            minimumScanSlope = 0.0;

            y = ViewPointY + deltaY;

            if (y >= 0 && y < Map.grid.Height)
            {
                bool keepScanning = false;

                while (true)
                {
                    keepScanning = false;
                    double slopePerCell = 1.0 / (double)(currIteration + 1);
                    int currentCell = (int)(minimumScanSlope / slopePerCell);
                    int xMin = Math.Max(0, ViewPointX - currIteration);
                    int xMax = Math.Min(Map.grid.Width - 1, ViewPointX + currIteration);

                    for (x = ViewPointX + (currentCell * deltaX); x >= xMin && x <= xMax; x += deltaX)
                    {
                        if (IsInRange(ViewPointX, ViewPointY, x, y, maxRange))
                        {
                            bool foundVisible;
                            ProcessCell(deltaX, deltaY, x, y, slopePerCell, currentCell, true, out foundVisible);

                            if (foundVisible)
                                keepScanning = true;

                            currentCell++;
                        }
                    }

                    y += deltaY;

                    if (!keepScanning || y < 0 || y >= Map.grid.Height || minimumScanSlope == 1.0 || currIteration == maxRange)
                        break;

                    currIteration++;
                    shadowMap.AddQueuedToShadow();
                }
            }
            #endregion

            #region Horizontal Edge Octants
            currIteration = 1;
            shadowMap.Clear();
            minimumScanSlope = 0.0;

            x = ViewPointX + deltaX;

            if (x >= 0 && x < Map.grid.Width)
            {
                bool keepScanning = false;

                while (true)
                {
                    keepScanning = false;
                    double slopePerCell = 1.0 / (double)(currIteration + 1);
                    int currentCell = (int)(minimumScanSlope / slopePerCell);
                    int yMin = Math.Max(0, ViewPointY - currIteration);
                    int yMax = Math.Min(Map.grid.Height - 1, ViewPointY + currIteration);

                    for (y = ViewPointY + (currentCell * deltaY); y >= yMin && y <= yMax; y += deltaY)
                    {
                        if (IsInRange(ViewPointX, ViewPointY, x, y, maxRange))
                        {
                            bool foundVisible;
                            ProcessCell(deltaX, deltaY, x, y, slopePerCell, currentCell, false, out foundVisible);

                            if (foundVisible)
                                keepScanning = true;

                            currentCell++;
                        }
                    }

                    x += deltaX;
                    if (!keepScanning || x < 0 || x >= Map.grid.Width || minimumScanSlope == 1.0 || currIteration == maxRange)
                        break;

                    currIteration++;
                    shadowMap.AddQueuedToShadow();
                }
            }
            #endregion

        }

        private void ProcessCell(int deltaX, int deltaY, int x, int y, double slopePerCell, int currentCell, bool verticalScan, out bool foundVisibleCell)
        {
            startSlope = (double)currentCell * slopePerCell;
            middleSlope = startSlope + slopePerCell * 0.5;
            endSlope = startSlope + slopePerCell;
            foundVisibleCell = false;

            if (CheckVisibility(deltaX, deltaY, x, y, verticalScan))
            {
                foundVisibleCell = true;
                Map.grid[x, y].IsVisible = true;

                if (!Map.grid[x, y].IsTransparent)
                {
                    // try increase the minimum slope that needs to be scanned in later scans - eliminates
                    // needless iterations.
                    if (minimumScanSlope >= startSlope)
                        minimumScanSlope = endSlope;
                    // otherwise add the new shadow fragment to the shadow map
                    else
                    {
                        shadowMap.EnqueNewFragment(startSlope, endSlope);
                    }
                    if (!lightWalls)
                        Map.grid[x, y].IsVisible = false;
                }
            }
        }

        private bool CheckVisibility(int deltaX, int deltaY, int x, int y, bool verticalScan)
        {
            bool visible = true;

            // used to check for two contigious walls closer to viewpoint, to restric fov from
            // peeking through adjacent walls
            int checkX1;
            int checkY1;
            int checkX2 = x - deltaX;
            int checkY2 = y - deltaY;

            if (verticalScan)
            {
                checkX1 = x;
                checkY1 = y - deltaY;
            }
            else
            {
                checkX1 = x - deltaX;
                checkY1 = y;
            }

            if (shadowMap.HasShadows && !Map.grid[x, y].IsVisible)
            {
                // check for two adjacent walls closer to viewpoint
                // also consider previosly non-visible areas as walls to handle common octant edges
                if ((!Map.grid[checkX1, checkY1].IsTransparent || !Map.grid[checkX1, checkY1].IsVisible) &&
                    (!Map.grid[checkX2, checkY2].IsTransparent || !Map.grid[checkX2, checkY2].IsVisible))
                {
                    visible = false;
                }
                else
                {
                    // if the cell is transparent, it will be invisible if the middle slope of scan is completely inside
                    // a shadow fragment
                    if (Map.grid[x, y].IsTransparent)
                    {
                        if (shadowMap.IsInShadow(middleSlope, false))
                            visible = false;
                    }
                    // if the cell is opaque, it will be invisible if the entire scan (start to end slope) is in 
                    // a shadow fragment
                    else
                    {
                        if (shadowMap.IsInShadow(startSlope, endSlope, true))
                            visible = false;
                    }
                }
            }

            return visible;
        }


    }

    internal class Shadow
    {
        public Shadow(double start, double end)
        {
            startSlope = start;
            endSlope = end;
        }

        public double startSlope;
        public double endSlope;
    }

    internal class ShadowMap
    {
        List<Shadow> shadow;
        Queue<Shadow> tobeAdded;
        bool hasShadows = false;

        public ShadowMap()
        {
            shadow = new List<Shadow>();
            tobeAdded = new Queue<Shadow>();
        }

        public bool HasShadows
        {
            get
            {
                return hasShadows;
            }
        }

        public void Clear()
        {
            shadow.Clear();
            tobeAdded.Clear();
            hasShadows = false;
        }

        public void EnqueNewFragment(double start, double end)
        {
            tobeAdded.Enqueue(new Shadow(start, end));
        }

        public void AddQueuedToShadow()
        {
            int num = tobeAdded.Count;
            if (num == 0)
                return;

            hasShadows = true;
            while (tobeAdded.Count > 0)
            {
                shadow.Add(tobeAdded.Dequeue());
            }
        }

        public bool IsInShadow(double value, bool inclusive)
        {
            foreach (var s in shadow)
            {
                if (inclusive)
                {
                    if (value >= s.startSlope && value <= s.endSlope)
                        return true;
                }
                else
                {
                    if (value > s.startSlope && value < s.endSlope)
                        return true;
                }
            }

            return false;
        }

        public bool IsInShadow(double start, double end, bool inclusive)
        {
            foreach (var s in shadow)
            {
                if (inclusive)
                {
                    if (start >= s.startSlope && end <= s.endSlope)
                        return true;
                }
                else
                {
                    if (start > s.startSlope && end < s.endSlope)
                        return true;
                }
            }

            return false;
        }
    }

}
