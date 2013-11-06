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
    /// <summary>
    /// A collection of interpolator objects that updates and automatically removes invactive
    /// interpolators.
    /// </summary>
    public class InterpolatorCollection
    {
        IterateCollection<Interpolator> interps;

        /// <summary>
        /// Creates a new InterpolatorCollection object
        /// </summary>
        public InterpolatorCollection()
        {
            interps = new IterateCollection<Interpolator>();

        }

        /// <summary>
        /// Add a new interpolator to the collection
        /// </summary>
        /// <param name="length">Length of time for the interpolator to go from start to completed</param>
        /// <param name="step">Delegate that gets invoked at each update</param>
        /// <param name="completed">Delegate that gets invoked when the interpolator has completed</param>
        /// <param name="repeats">If true, interpolator resets to start after each completion</param>
        /// <returns>The created Interpolator</returns>
        public Interpolator AddInterpolator(float length, Action<Interpolator> step, 
            Action<Interpolator> completed, bool repeats = false)
        {
            Interpolator i = new Interpolator(length, step, completed, repeats);

            interps.Add(i);

            return i;
        }

        /// <summary>
        /// Update the interpolators, providing the elapsed time since the last update, typically in fractions of a second.
        /// </summary>
        /// <param name="elapsed">The amount of time in seconds since the last update call</param>
        public void Update(float elapsed)
        {
            interps.Iterate( t =>
            {
                t.Update(elapsed);
                if (!t.IsActive)
                    interps.Remove(t);
            });
        }
    }

    /// <summary>
    /// An object that linearly interpolates from 0 to 1 for a given length of time. Use an InterpolatorCollection
    /// to create Interpolator objects.
    /// </summary>
    public sealed class Interpolator
    {
        private Action<Interpolator> step;
        private Action<Interpolator> completed;
        private bool valid;
        private float progress;
        private float speed;

        /// <summary>
        /// Gets whether or not the interpolator is active.
        /// </summary>
        public bool IsActive { get { return valid; } }

        /// <summary>
        /// Get the current progress, from 0 to 1, of the interpolator, where 0 is start and 1 is complete
        /// </summary>
        public float Progress { get { return progress; } }

        /// <summary>
        /// Gets or sets some extra data to the interpolator.
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// Gets whether or not this interpolator repeats.
        /// </summary>
        public bool Repeats { get; private set; }

        internal Interpolator(float length, Action<Interpolator> step, Action<Interpolator> completed, bool repeats)
        {
            Repeats = repeats;
            this.step = step;
            this.completed = completed;
            this.valid = true;
            this.progress = 0f;
            this.speed = 1f / length;
        }

        /// <summary>
        /// Forces the interpolator to completion, setting the progress to 1, and calling the step and completed
        /// delegates immediately.  If Repeats is true, the interpolator is reset.
        /// </summary>
        public void ForceFinish()
        {
            if (valid)
            {
                valid = Repeats;
                progress = 1;

                if (step != null)
                    step(this);

                if (completed != null)
                    completed(this);

                progress = 0;
            }
        }

        /// <summary>
        /// Immediately stops the interpolator.  No delegates are called, and the interpolator will not be reset.
        /// </summary>
        public void Stop()
        {
            valid = false;
        }

        internal void Update(float elapsed)
        {
            if (!valid)
                return;

            progress = Math.Min(progress + speed * elapsed, 1f);

            if (step != null)
                step(this);

            if (progress == 1f)
            {
                valid = Repeats;

                if (completed != null)
                    completed(this);

                if (valid)
                {
                    progress = 0;
                }
                else
                {
                    step = null;
                    completed = null;
                    Tag = null;
                }
            }
        }
    }
}
