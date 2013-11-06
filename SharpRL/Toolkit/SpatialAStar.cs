/*
The MIT License

Copyright (c) 2010 Christoph Husse

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

/*
 * 
 * Modifications by Shane Baker 2011
 * 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace SharpRL.Toolkit
{
    /// <summary>
    /// Interface used by SpatialAStar to query whether a cell is walkable or not.
    /// </summary>
    /// <typeparam name="TUserContext">An object that can be passed to this method in order to 
    /// programmatically alter the walkable status of cells</typeparam>
    public interface IPathCell<TUserContext>
    {
        /// <summary>
        /// Should return true if the cell is walkable given the user context object
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        bool IsWalkable(TUserContext context);
    }

    /// <summary>
    /// Calculates a path using the A-star algorithm.
    /// </summary>
    /// <remarks>
    /// To use, first implement an IPathCell class to be used as cells for the pathfinder.  A simple implementation
    /// might be something like this:
    /// <code>
    ///   protected class Cell : IPathCell&lt;object&gt;
    ///   {
    ///       public Cell(bool isWall, int x, int y)
    ///       {
    ///           this.IsWall = isWall;
    ///
    ///           this.X = x;
    ///           this.Y = y;
    ///       }
    ///
    ///       public int X
    ///       {
    ///           get;
    ///           private set;
    ///       }
    ///
    ///       public int Y
    ///       {
    ///           get;
    ///           private set;
    ///       }
    ///
    ///       public bool IsWall
    ///       {
    ///           get;
    ///           set;
    ///       }
    ///
    ///       public bool IsWalkable(object context)
    ///       {
    ///           if (IsWall)
    ///               return false;
    ///           else
    ///               return true;
    ///       }
    ///   }
    /// </code>
    /// <para/>
    /// Here we use a plain Ojbect as the user context type, because we aren't using it.  We could, if we choose,
    /// pass a special object to the Search method, which is then passed to the IsWalkable in order to programmatically change
    /// the result.  This would be useful if
    /// the walkability of a cell is dependent on the entity who path we are cacluating.
    /// <para/>
    /// Note that we store the X and Y values so we can later walk each node in the path.  For an example, look
    /// at ExampleBase.cs and Example5.cs in the Examples project.<para/>
    /// Internally, SpatialAStar wraps and references each IPathNode cell passed during construction, and the orignal
    /// cells are not modified.
    /// </remarks>
    /// <typeparam name="TPathNode"></typeparam>
    /// <typeparam name="TUserContext"></typeparam>
    public class SpatialAStar<TPathNode, TUserContext> where TPathNode : IPathCell<TUserContext>
    {
        #region Private

        private OpenCloseMap m_ClosedSet;
        private OpenCloseMap m_OpenSet;
        private PriorityQueue<PathNode> m_OrderedOpenSet;
        private Array2d<PathNode> m_CameFrom;
        private OpenCloseMap m_RuntimeGrid;
        private Array2d<PathNode> m_SearchSpace;

        private static readonly Double SQRT_2 = Math.Sqrt(2);


        private LinkedList<TPathNode> ReconstructPath(Array2d<PathNode> came_from, PathNode current_node)
        {
            LinkedList<TPathNode> result = new LinkedList<TPathNode>();

            ReconstructPathRecursive(came_from, current_node, result);

            return result;
        }

        private void ReconstructPathRecursive(Array2d<PathNode> came_from, PathNode current_node, LinkedList<TPathNode> result)
        {
            PathNode item = came_from[current_node.X, current_node.Y];

            if (item != null)
            {
                ReconstructPathRecursive(came_from, item, result);

                result.AddLast(current_node.UserContext);
            }
            else
                result.AddLast(current_node.UserContext);
        }

        private void StoreNeighborNodes(PathNode inAround, PathNode[] inNeighbors)
        {
            int x = inAround.X;
            int y = inAround.Y;

            if ((x > 0) && (y > 0))
                inNeighbors[0] = m_SearchSpace[x - 1, y - 1];
            else
                inNeighbors[0] = null;

            if (y > 0)
                inNeighbors[1] = m_SearchSpace[x, y - 1];
            else
                inNeighbors[1] = null;

            if ((x < Width - 1) && (y > 0))
                inNeighbors[2] = m_SearchSpace[x + 1, y - 1];
            else
                inNeighbors[2] = null;

            if (x > 0)
                inNeighbors[3] = m_SearchSpace[x - 1, y];
            else
                inNeighbors[3] = null;

            if (x < Width - 1)
                inNeighbors[4] = m_SearchSpace[x + 1, y];
            else
                inNeighbors[4] = null;

            if ((x > 0) && (y < Height - 1))
                inNeighbors[5] = m_SearchSpace[x - 1, y + 1];
            else
                inNeighbors[5] = null;

            if (y < Height - 1)
                inNeighbors[6] = m_SearchSpace[x, y + 1];
            else
                inNeighbors[6] = null;

            if ((x < Width - 1) && (y < Height - 1))
                inNeighbors[7] = m_SearchSpace[x + 1, y + 1];
            else
                inNeighbors[7] = null;
        }

        private class OpenCloseMap
        {
            private PathNode[,] m_Map;
            public int Width { get; private set; }
            public int Height { get; private set; }
            public int Count { get; private set; }

            public PathNode this[Int32 x, Int32 y]
            {
                get
                {
                    return m_Map[x, y];
                }
            }

            public PathNode this[PathNode Node]
            {
                get
                {
                    return m_Map[Node.X, Node.Y];
                }

            }

            public bool IsEmpty
            {
                get
                {
                    return Count == 0;
                }
            }

            public OpenCloseMap(int inWidth, int inHeight)
            {
                m_Map = new PathNode[inWidth, inHeight];
                Width = inWidth;
                Height = inHeight;
            }

            public void Add(PathNode inValue)
            {
                PathNode item = m_Map[inValue.X, inValue.Y];

#if DEBUG
                if (item != null)
                    throw new ApplicationException();
#endif

                Count++;
                m_Map[inValue.X, inValue.Y] = inValue;
            }

            public bool Contains(PathNode inValue)
            {
                PathNode item = m_Map[inValue.X, inValue.Y];

                if (item == null)
                    return false;

#if DEBUG
                if (!inValue.Equals(item))
                    throw new ApplicationException();
#endif

                return true;
            }

            public void Remove(PathNode inValue)
            {
                PathNode item = m_Map[inValue.X, inValue.Y];

#if DEBUG
                if (!inValue.Equals(item))
                    throw new ApplicationException();
#endif

                Count--;
                m_Map[inValue.X, inValue.Y] = null;
            }

            public void Clear()
            {
                Count = 0;

                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        m_Map[x, y] = null;
                    }
                }
            }
        }


        class PathNode : IPathCell<TUserContext>, IComparer<PathNode>, IIndexedObject
        {
            public static readonly PathNode Comparer = new PathNode(0, 0, default(TPathNode));

            public TPathNode UserContext { get; internal set; }
            public Double G { get; internal set; }
            public Double H { get; internal set; }
            public Double F { get; internal set; }
            public int Index { get; set; }

            public Boolean IsWalkable(TUserContext inContext)
            {
                return UserContext.IsWalkable(inContext);
            }

            public int X { get; internal set; }
            public int Y { get; internal set; }

            public int Compare(PathNode x, PathNode y)
            {
                if (x.F < y.F)
                    return -1;
                else if (x.F > y.F)
                    return 1;

                return 0;
            }

            public PathNode(int inX, int inY, TPathNode inUserContext)
            {
                X = inX;
                Y = inY;
                UserContext = inUserContext;
            }
        }

        Double Heuristic(PathNode inStart, PathNode inEnd)
        {
            return Math.Sqrt((inStart.X - inEnd.X) * (inStart.X - inEnd.X) + (inStart.Y - inEnd.Y) * (inStart.Y - inEnd.Y));
        }

        Double NeighborDistance(PathNode inStart, PathNode inEnd)
        {
            int diffX = Math.Abs(inStart.X - inEnd.X);
            int diffY = Math.Abs(inStart.Y - inEnd.Y);

            switch (diffX + diffY)
            {
                case 1: return 1;
                case 2: return SQRT_2;
                case 0: return 0;
                default:
                    throw new ApplicationException();
            }
        }

        #endregion

        #region Properties
        /// <summary>
        /// This is the same map instance used to construct the SpatialAStar instance.
        /// </summary>
        public Array2d<TPathNode> SearchSpace { get; private set; }

        /// <summary>
        /// The width ofthe map used to construct the SpatialAStar isntance.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// The height of the map used to construct the SpatialAStar instance.
        /// </summary>
        public int Height { get; private set; }

        #endregion

        #region CTOR

        /// <summary>
        /// Construct a SpatialAStar instance given a map of IPathCell objects
        /// </summary>
        /// <param name="grid"></param>
        public SpatialAStar(Array2d<TPathNode> grid)
        {
            SearchSpace = grid;
            Width = grid.Width;
            Height = grid.Height;
            m_SearchSpace = new Array2d<PathNode>(Width, Height);
            m_ClosedSet = new OpenCloseMap(Width, Height);
            m_OpenSet = new OpenCloseMap(Width, Height);
            m_CameFrom = new Array2d<PathNode>(Width, Height);
            m_RuntimeGrid = new OpenCloseMap(Width, Height);
            m_OrderedOpenSet = new PriorityQueue<PathNode>(PathNode.Comparer);

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (grid[x, y] == null)
                        throw new ArgumentNullException();

                    m_SearchSpace[x, y] = new PathNode(x, y, grid[x, y]);
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Try to calculate the best path from the starting to ending positions.
        /// </summary>
        /// <remarks>
        /// If no path is found, returns null.  The startingCell and endingCell are included in the path.
        /// </remarks>
        /// <param name="startingCell"></param>
        /// <param name="endingCell"></param>
        /// <param name="userContext"></param>
        /// <returns></returns>
        public LinkedList<TPathNode> Search(Point startingCell, Point endingCell, TUserContext userContext)
        {
            PathNode startNode = m_SearchSpace[startingCell.X, startingCell.Y];
            PathNode endNode = m_SearchSpace[endingCell.X, endingCell.Y];

            if (startNode == endNode)
                return new LinkedList<TPathNode>(new TPathNode[] { startNode.UserContext });

            PathNode[] neighborNodes = new PathNode[8];

            m_ClosedSet.Clear();
            m_OpenSet.Clear();
            m_RuntimeGrid.Clear();
            m_OrderedOpenSet.Clear();

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    m_CameFrom[x, y] = null;
                }
            }

            startNode.G = 0;
            startNode.H = Heuristic(startNode, endNode);
            startNode.F = startNode.H;

            m_OpenSet.Add(startNode);
            m_OrderedOpenSet.Push(startNode);

            m_RuntimeGrid.Add(startNode);

            int nodes = 0;


            while (!m_OpenSet.IsEmpty)
            {
                PathNode x = m_OrderedOpenSet.Pop();

                if (x == endNode)
                {
                    LinkedList<TPathNode> result = ReconstructPath(m_CameFrom, m_CameFrom[endNode.X, endNode.Y]);

                    result.AddLast(endNode.UserContext);

                    return result;
                }

                m_OpenSet.Remove(x);
                m_ClosedSet.Add(x);

                StoreNeighborNodes(x, neighborNodes);

                for (int i = 0; i < neighborNodes.Length; i++)
                {
                    PathNode y = neighborNodes[i];
                    Boolean tentative_is_better;

                    if (y == null)
                        continue;

                    if (!y.UserContext.IsWalkable(userContext))
                        continue;

                    if (m_ClosedSet.Contains(y))
                        continue;

                    nodes++;

                    Double tentative_g_score = m_RuntimeGrid[x].G + NeighborDistance(x, y);
                    Boolean wasAdded = false;

                    if (!m_OpenSet.Contains(y))
                    {
                        m_OpenSet.Add(y);
                        tentative_is_better = true;
                        wasAdded = true;
                    }
                    else if (tentative_g_score < m_RuntimeGrid[y].G)
                    {
                        tentative_is_better = true;
                    }
                    else
                    {
                        tentative_is_better = false;
                    }

                    if (tentative_is_better)
                    {
                        m_CameFrom[y.X, y.Y] = x;

                        if (!m_RuntimeGrid.Contains(y))
                            m_RuntimeGrid.Add(y);

                        m_RuntimeGrid[y].G = tentative_g_score;
                        m_RuntimeGrid[y].H = Heuristic(y, endNode);
                        m_RuntimeGrid[y].F = m_RuntimeGrid[y].G + m_RuntimeGrid[y].H;

                        if (wasAdded)
                            m_OrderedOpenSet.Push(y);
                        else
                            m_OrderedOpenSet.Update(y);
                    }
                }
            }

            return null;
        }

        #endregion

    }
}
