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
    /// A collection of Timer objects that updates and automatically removes inactive
    /// timers from its internal collection.
    /// </summary>
    public class TimerCollection
    {
        ActiveSet<Timer> timers;

        /// <summary>
        /// Creates a new TimerCollection object
        /// </summary>
        public TimerCollection()
        {
            timers = new ActiveSet<Timer>();

        }

        /// <summary>
        /// Add a new timer to the collection.
        /// </summary>
        /// <param name="length">Length of time before the tick action is invoked</param>
        /// <param name="repeats">True if the timer resets after each tick</param>
        /// <param name="tick">The delegate that is called after the specified length of time has elapsed</param>
        /// <returns>The created Timer object</returns>
        public Timer AddTimer(float length, bool repeats, Action<Timer> tick)
        {
            Timer t = new Timer(length, repeats, tick);

            timers.Add(t);

            return t;
        }

        /// <summary>
        /// Update the timers in this collection, providing the elapsed time.
        /// </summary>
        /// <param name="elapsed">The elapsed time since the last update, typically in fractions of a second</param>
        public void Update(float elapsed)
        {
            timers.Iterate( t =>
                {
                    t.Update(elapsed);
                    if (!t.IsActive)
                        timers.Remove(t);
                });
        }
    }

    /// <summary>
    /// An object that invokes an action after an amount of time has elapsed and
    /// optionally continues repeating until told to stop. Use a TimerCollection to
    /// create Timer objects.
    /// </summary>
    public sealed class Timer
    {
        private bool valid;
        private float time;
        private float tickLength;
        private bool repeats;
        private Action<Timer> tick;

        /// <summary>
        /// Gets whether or not the timer is active.
        /// </summary>
        public bool IsActive { get { return valid; } }

        /// <summary>
        /// Gets or sets some extra data to the timer.
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// Gets whether or not this timer repeats.
        /// </summary>
        public bool Repeats { get { return repeats; } }

        /// <summary>
        /// Gets the length of time (in seconds) between ticks of the timer.
        /// </summary>
        public float TickLength { get { return tickLength; } }

        internal Timer(float length, bool repeats, Action<Timer> tick)
        {
            if (length <= 0f)
                throw new ArgumentException("length must be greater than 0");
            if (tick == null)
                throw new ArgumentNullException("tick");

            Reset(length, repeats, tick);
        }

        /// <summary>
        /// Stops the timer immediately, the tick delegate is not called, and the timer is not reset.
        /// </summary>
        public void Stop()
        {
            valid = false;
        }

        /// <summary>
        /// Forces the timer to fire its tick event, invalidating the timer unless it is set to repeat.
        /// </summary>
        public void ForceTick()
        {
            if (!valid)
                return;

            tick(this);
            time = 0f;

            valid = Repeats;

            if (!valid)
            {
                tick = null;
                Tag = null;
            }
        }

        internal void Reset(float l, bool r, Action<Timer> t)
        {
            valid = true;
            time = 0f;
            tickLength = l;
            repeats = r;
            tick = t;
        }

        internal void Update(float elapsed)
        {
            if (!valid)
                return;

            time += elapsed;

            if (time >= tickLength)
            {
                tick(this);

                time -= tickLength;

                valid = repeats;

                if (!valid)
                {
                    tick = null;
                    Tag = null;
                }
            }
        }
    }
}
