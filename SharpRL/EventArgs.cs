using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpRL
{
    /// <summary>
    /// Generic Event Argument container.  If passing the same data (e.g. mouse information)
    /// using both events and method parameters, using a generic event args reduces the number of reduntant
    /// data classes needed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EventArgs<T> : EventArgs
    {
        /// <summary>
        /// The information contained in this event argument
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// Construct a new EventArg
        /// </summary>
        /// <param name="val"></param>
        public EventArgs(T val)
        {
            this.Value = val;
        }
    }
}
