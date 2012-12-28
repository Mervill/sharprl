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

namespace SharpRL.Framework
{
    /// <summary>
    /// A simple collection of items which defers insertions and removals so that these
    /// operations can be performed while the loop is being iterated through.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IterateCollection<T>
    {
        HashSet<T> items = new HashSet<T>();

        enum Command
        {
            Add,
            Remove
        }

        Queue<Tuple<Command, T>> commandQueue = new Queue<Tuple<Command, T>>();

        /// <summary>
        /// Adds the object to the set collection. If called during the Iterate loop, the add operation
        /// will be deferred until Iterate is complete.
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            if (isIterating)
            {
                commandQueue.Enqueue(new Tuple<Command, T>(Command.Add, item));
            }
            else
            {
                items.Add(item);
            }
        }

        /// <summary>
        /// Removes the actor from the set collection. If called during the Iterate loop, the remove operation
        /// will be deferred until Iterate is complete.
        /// </summary>
        /// <param name="item"></param>
        public void Remove(T item)
        {
            if (isIterating)
            {
                commandQueue.Enqueue(new Tuple<Command, T>(Command.Remove, item));
            }
            else
            {
                items.Remove(item);
            }
        }

        /// <summary>
        /// Returns true if the set collection contains the specified object
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            return items.Contains(item);
        }

        /// <summary>
        /// Get the number of items in the collection.
        /// </summary>
        public int Count
        {
            get { return items.Count; }
        }

        bool isIterating = false;

        /// <summary>
        /// Iterate through each item in the collection, calling the OnIterate delegate
        /// for each item
        /// </summary>
        /// <param name="elapsed"></param>
        /// <param name="OnIterate"></param>
        public void Iterate(float elapsed, Action<T> OnIterate)
        {
            isIterating = true;
            foreach (var actor in items)
            {
                if (OnIterate != null)
                    OnIterate(actor);
            }
            isIterating = false;

            while (commandQueue.Count > 0)
            {
                var com = commandQueue.Dequeue();

                if (com.Item1 == Command.Add)
                {
                    Add(com.Item2);
                }
                else if (com.Item1 == Command.Remove)
                {
                    Remove(com.Item2);
                }
            }
        }
    }
}
