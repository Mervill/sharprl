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

namespace SharpRL
{
    /// <summary>
    /// The direction a BSPNode region is split
    /// </summary>
    public enum SplitDirection
    {
        /// <summary>
        /// Horizontal split direction separates a region into top and bottom regions
        /// </summary>
        Horizontal,
        /// <summary>
        /// Vertical split direction seperates a region into left and right regions
        /// </summary>
        Vertical
    }

    /// <summary>
    /// Represents a region of a partioned space.
    /// </summary>
    /// <remarks>
    /// Conceptually a BSPNode is a rectangular region, along with some information about the tree structure in belongs to.
    /// A BSPNode instance cannot be constructed manually, but are created by a BSPTree when splitting.
    /// </remarks>
    public class BSPNode
    {

        #region Properties

        /// <summary>
        /// Gets the Rectangle of the region described by this node
        /// </summary>
        public Rectangle Rect
        {
            get { return this.rect; }
        }

        /// <summary>
        /// Which direction this node has been split.  Only applicable if this node is not a leaf.
        /// </summary>
        public SplitDirection SplitDirection
        {
            get { return this.splitDirection; }
        }

        /// <summary>
        /// The level of this node.  The root node is level 0, and increases by 1 with each generation.
        /// </summary>
        public int Level
        {
            get { return this.level; }
        }

        /// <summary>
        /// Returns the "left" child of this node, or null if this node is a leaf
        /// </summary>
        public BSPNode Left
        {
            get
            {
                if (Children == null)
                    return null;
                else
                    return Children[0];
            }
            internal set
            {
                if (Children == null)
                    Children = new BSPNode[2];

                Children[0] = value;
            }
        }

        /// <summary>
        /// Returns the "right" child of this node, or null if this node is a leaf
        /// </summary>
        public BSPNode Right
        {
            get
            {
                if (IsLeaf)
                    return null;
                else
                    return Children[1];
            }
            internal set
            {
                if (IsLeaf)
                    Children = new BSPNode[2];

                Children[1] = value;
            }
        }

        /// <summary>
        /// Returns the parent of this node, or null if this node is a root
        /// </summary>
        public BSPNode Parent
        {
            get
            {
                return this.parent;
            }
            internal set
            {
                this.parent = value;
            }
        }

        /// <summary>
        /// Returns true if this node is a root node
        /// </summary>
        public bool IsRoot
        {
            get
            {
                if (parent == null)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Returns true if this node is a leaf node
        /// </summary>
        public bool IsLeaf
        {
            get
            {
                if (Children == null || (Children[0] == null && Children[1] == null))
                    return true;
                else
                    return false;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Tries to split this node at a position specified by size.  One of the split regions will be the
        /// specified size, the other region will take up the remainder of the area.
        /// </summary>
        /// <param name="size"></param>
        /// <returns>Returns true if successful.  A split will fail if the position would fall outside
        /// the node's rectangle.
        /// </returns>
        public bool TrySplit(SplitDirection direction, int size)
        {
            Rectangle lRect, rRect;
            switch (direction)
            {
                case SplitDirection.Horizontal:
                    if (size <= 1 || size >= rect.Height - 1)
                    {
                        return false;
                    }
                    lRect = new Rectangle(rect.X, rect.Y, rect.Width, size);
                    rRect = new Rectangle(lRect.Left, lRect.Bottom, rect.Width, rect.Height - size);
                    break;

                default:
                case SplitDirection.Vertical:
                    if (size <= 1 || size >= rect.Width - 1)
                    {
                        return false;
                    }
                    lRect = new Rectangle(rect.X, rect.Y, size, rect.Height);
                    rRect = new Rectangle(lRect.Right, lRect.Top, rect.Width - size, rect.Height);
                    break;
            }

            if (Children == null)
                Children = new BSPNode[2];

            Children[0] = new BSPNode(lRect, level + 1,  direction, null, null, this);
            Children[1] = new BSPNode(rRect, level + 1,  direction, null, null, this);

            return true;
        }

        #endregion

        #region Private

        Rectangle rect;
        SplitDirection splitDirection;
        BSPNode[] Children;
        BSPNode parent;
        int level;

        private BSPNode()
        {
        }

        internal BSPNode(Rectangle rect, int level)
        {
            this.rect = rect;
            this.level = level;
        }

        internal BSPNode(Rectangle rect, int level, SplitDirection direction, BSPNode left, BSPNode right, BSPNode parent)
        {

#if DEBUG
            if ((left == null && right != null) || (right == null && left != null))
            {
                System.Diagnostics.Debug.Fail("Both left and right children nodes must both be null or non-null");
            }
#endif

            this.rect = rect;
            this.level = level;
            this.splitDirection = direction;
            if (left != null)
            {
                Children = new BSPNode[2];
                Children[0] = left;
                Children[1] = right;
            }
            this.parent = parent;
        }

        internal int NumberOfDescendants
        {
            get
            {
                if (Children == null)
                {
                    return 0;
                }

                return (Children[0].NumberOfDescendants + Children[1].NumberOfDescendants + 2);
            }
        }

        #endregion

    }
}
