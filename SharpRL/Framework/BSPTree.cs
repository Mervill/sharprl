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

namespace SharpRL.Framework
{
    /// <summary>
    /// Represents a binary space partition tree.
    /// </summary>
    /// <remarks>
    /// A BSPTree is used to partition a rectangular region into smaller non-overlapping
    /// regions.  Each region in a BSPTree is a BSPNode, and each node can have either exactly
    /// zero children (if it is a leaf) or exactly 2 children ("right" and "left"). <para/>
    /// Every BSPTree instance has a Root node, which is the starting (or largest) region. <para/>
    /// Note that this implentation does not do any sorting, so the order of left and right children
    /// is left undefined.  Also, this implementation is designed to work with shallow trees as one
    /// would use for dungeon creation, and enumeration is not optimized for real-time traversal of
    /// large trees.
    /// </remarks>
    public class BSPTree
    {
        #region Constructor

        /// <summary>
        /// Construct a BSP with the given rectangle as a region.
        /// </summary>
        /// <param name="rootRect"></param>
        public BSPTree(Rectangle rootRect)
        {
            root = new BSPNode(rootRect, 0);
        }

        #endregion

        #region Properties

        /// <summary>
        /// The root of this tree.
        /// </summary>
        public BSPNode Root
        {
            get { return root; }
            internal set { root = value; }
        }

        /// <summary>
        /// The total number of nodes in this tree, including the root.
        /// </summary>
        public int Count
        {
            get
            {
                return root.NumberOfDescendants + 1;
            }
        }

        /// <summary>
        /// The depth of this tree, which is equal to the Level property of the leaf nodes.
        /// </summary>
        public int Depth
        {
            get
            {
                foreach (var n in Postorder)
                {
                    if (n.IsLeaf)
                        return n.Level;
                }
                return 0;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Splits the tree multiple times.  If the minimum size is reached, a node will not be split, so the final depth
        /// of the tree may not be equal to numberOfSplits.
        /// </summary>
        /// <param name="numberOfSplits">Number of splits that will be tried (will determine the final depth of the tree)</param>
        /// <param name="minHSize">Minimum horizontal size of leaves</param>
        /// <param name="minVSize">Minimum vertical size of leaves</param>
        /// <param name="rand"></param>
        public void SplitRecursive(int numberOfSplits, int minHSize, int minVSize, Random rand)
        {
            SplitRecursive(numberOfSplits, minHSize, minVSize, 1f, 1f, rand);
        }

        /// <summary>
        /// Splits the tree multiple times, with maximum horizontal and vertical ratios to define a tendency for leaf shapes
        /// If the minimum size is reached, a node will not be split, so the final depth
        /// of the tree may not be equal to numberOfSplits.
        /// </summary>
        /// <param name="numberOfSplits">Number of splits that will be tried (will determine the final depth of the tree)</param>
        /// <param name="minHSize">Minimum horizontal size of leaves</param>
        /// <param name="minVSize">Minimum vertical size of leaves</param>
        /// <param name="maxHRatio">Lower numbers will tend towards vertically stretched leaves</param>
        /// <param name="maxVRatio">Lower number will tend towars horizontally stretched leaves</param>
        /// <param name="rand"></param>
        public void SplitRecursive(int numberOfSplits, int minHSize, int minVSize, float maxHRatio, float maxVRatio, Random rand)
        {
            for (int i = 0; i < numberOfSplits; i++)
            {
                foreach (var n in GetByLevel(Depth))
                {
                    SplitDirection dir;
                    float hFactor = 1f;
                    float vFactor = 1f;

                    float nodeHW = (float)n.Rect.Height / (float)n.Rect.Width;
                    float nodeWH = 1f / nodeHW;

                    if (nodeHW > maxHRatio)
                    {
                        if (nodeWH > maxVRatio)
                        {
                            dir = rand.GetEnum<SplitDirection>();
                        }
                        else
                        {
                            dir = SplitDirection.Horizontal;
                            vFactor = nodeWH;
                        }
                    }
                    else
                    {
                        if (nodeWH > maxVRatio)
                        {
                            dir = SplitDirection.Vertical;
                            hFactor = nodeHW;
                        }
                        else
                        {
                            dir = rand.GetEnum<SplitDirection>();
                        }
                    }

                    int pos = 0;

                    if (TrySplitPos(dir, n, minHSize, minVSize, hFactor, vFactor, rand, ref pos))
                    {
                        n.TrySplit(dir, pos);
                    }
                    else
                    {
                        if (dir == SplitDirection.Horizontal)
                        {
                            dir = SplitDirection.Vertical;
                        }
                        else
                        {
                            dir = SplitDirection.Horizontal;
                        }

                        if (TrySplitPos(dir, n, minHSize, minVSize, hFactor, vFactor, rand, ref pos))
                        {
                            n.TrySplit(dir, pos);
                        }
                    }
                }
            }

        }

        #endregion

        #region Enumeration

        /// <summary>
        /// Enumerates all nodes of the specified level
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public IEnumerable<BSPNode> GetByLevel(int level)
        {
            return from n in InvertedLevelOrder
                   where n.Level == level
                   select n;
        }

        /// <summary>
        /// Enumerates through all leaf nodes
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BSPNode> GetAllLeaves()
        {
            return from n in InvertedLevelOrder
                   where n.IsLeaf == true
                   select n;
        }

        /// <summary>
        /// Enumerates through the nodes using preorder traversal.
        /// </summary>
        public IEnumerable<BSPNode> Preorder
        {
            get
            {
                // A single stack is sufficient here - it simply maintains the correct
                // order with which to process the children.
                Stack<BSPNode> toVisit = new Stack<BSPNode>(Count);
                BSPNode current = root;
                if (current != null) toVisit.Push(current);

                while (toVisit.Count != 0)
                {
                    // take the top item from the stack
                    current = toVisit.Pop();

                    // add the right and left children, if not null
                    if (current.Right != null) toVisit.Push(current.Right);
                    if (current.Left != null) toVisit.Push(current.Left);

                    // return the current node
                    yield return current;
                }
            }
        }

        /// <summary>
        /// Enumerates through the nodes using inorder traversal.
        /// </summary>
        public IEnumerable<BSPNode> Inorder
        {
            get
            {
                // A single stack is sufficient - this code was made available by Grant Richins:
                // http://blogs.msdn.com/grantri/archive/2004/04/08/110165.aspx
                Stack<BSPNode> toVisit = new Stack<BSPNode>(Count);
                for (BSPNode current = root; current != null || toVisit.Count != 0; current = current.Right)
                {
                    // Get the left-most item in the subtree, remembering the path taken
                    while (current != null)
                    {
                        toVisit.Push(current);
                        current = current.Left;
                    }

                    current = toVisit.Pop();
                    yield return current;
                }
            }
        }

        /// <summary>
        /// Enumerates through the nodes using postorder traversal.
        /// </summary>
        public IEnumerable<BSPNode> Postorder
        {
            get
            {
                // maintain two stacks, one of a list of nodes to visit,
                // and one of booleans, indicating if the note has been processed
                // or not.
                Stack<BSPNode> toVisit = new Stack<BSPNode>(Count);
                Stack<bool> hasBeenProcessed = new Stack<bool>(Count);
                BSPNode current = root;
                if (current != null)
                {
                    toVisit.Push(current);
                    hasBeenProcessed.Push(false);
                    current = current.Left;
                }

                while (toVisit.Count != 0)
                {
                    if (current != null)
                    {
                        // add this node to the stack with a false processed value
                        toVisit.Push(current);
                        hasBeenProcessed.Push(false);
                        current = current.Left;
                    }
                    else
                    {
                        // see if the node on the stack has been processed
                        bool processed = hasBeenProcessed.Pop();
                        BSPNode node = toVisit.Pop();
                        if (!processed)
                        {
                            // if it's not been processed, "recurse" down the right subtree
                            toVisit.Push(node);
                            hasBeenProcessed.Push(true);    // it's now been processed
                            current = node.Right;
                        }
                        else
                            yield return node;
                    }
                }
            }
        }

        /// <summary>
        /// Enumerates through the nodes using level order traversal (starting with root or level 0 node)
        /// </summary>
        public IEnumerable<BSPNode> LevelOrder
        {
            get
            {
                Queue<BSPNode> queue = new Queue<BSPNode>();
                BSPNode current = root;

                queue.Enqueue(current);

                while (queue.Count != 0)
                {
                    current = queue.Dequeue();

                    if (current.Left != null)
                        queue.Enqueue(current.Left);
                    if (current.Right != null)
                        queue.Enqueue(current.Right);

                    yield return current;
                }
            }
        }

        /// <summary>
        /// Enumerates through the nodes with reversed level order traversal (starts with the leaf nodes)
        /// </summary>
        public IEnumerable<BSPNode> InvertedLevelOrder
        {
            get
            {
                return LevelOrder.Reverse();
            }
        }

        #endregion

        #region Private

        BSPNode root;
        bool TrySplitPos(SplitDirection dir, BSPNode n, int minHSize, int minVSize, float hFactor, float vFactor, Random rand, ref int pos)
        {
            if (dir == SplitDirection.Horizontal)
            {
                int min2 = (int)(vFactor * 0.5f * n.Rect.Height);
                int minDelt = Math.Max(minVSize, min2);

                if (minDelt > n.Rect.Height / 2)
                    return false;

                pos = rand.GetInt(minDelt, n.Rect.Height - minDelt);
            }
            else
            {
                int min2 = (int)(hFactor * 0.5f * n.Rect.Width);
                int minDelt = Math.Max(minHSize, min2);

                if (minDelt > n.Rect.Width / 2)
                    return false;

                pos = rand.GetInt(minDelt, n.Rect.Width - minDelt);
            }

            return true;
        }

        #endregion

    }
}
