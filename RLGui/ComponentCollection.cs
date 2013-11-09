using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RLGui
{
    enum Command
    {
        Add,
        Remove
    }

    class ComponentCollection : IEnumerable<Component>
    {
        public ComponentCollection(View view)
        {
            this.view = view;
        }

        List<Component> components = new List<Component>();

        public bool NeedsSorting { get; set; }

        Queue<CommandData> cmdQueue = new Queue<CommandData>();
        private bool isEnumerating = false;
        View view;
        IEnumerable<Component> sortedCache;

        public void Add(Component comp)
        {
            if (!components.Contains(comp))
            {
                var cmd = new CommandData(Command.Add, comp);

                if (isEnumerating)
                {
                    cmdQueue.Enqueue(cmd);
                }
                else
                {
                    DoCommand(cmd);
                }
            }
        }

        public void Remove(Component comp)
        {
            if (components.Contains(comp))
            {
                var cmd = new CommandData(Command.Remove, comp);

                if (isEnumerating)
                {
                    cmdQueue.Enqueue(cmd);
                }
                else
                {
                    DoCommand(cmd);
                }

            }
        }

        public int IndexOf(Component comp)
        {
            return components.IndexOf(comp);
        }

        public int Count { get { return components.Count; } }

        public Component this[int index]
        {
            get { return components[index]; }
        }

        public void StartEnumeration()
        {
            isEnumerating = true;
        }

        public void EndEnumeration()
        {
            isEnumerating = false;
            while (cmdQueue.Count != 0)
                DoCommand(cmdQueue.Dequeue());
        }

        public IEnumerable<Component> GetSorted()
        {
            if (NeedsSorting)
            {
                NeedsSorting = false;
                sortedCache = components.OrderBy(c => c.Layer, Comparer<int>.Default);
            }

            return sortedCache;
        }

        void DoCommand(CommandData cmd)
        {
            switch (cmd.command)
            {
                case Command.Add:
                    components.Add(cmd.comp);
                    view.ComponentAdded(cmd.comp);
                    NeedsSorting = true;
                    break;

                case Command.Remove:
                    components.Remove(cmd.comp);
                    NeedsSorting = true;
                    view.ComponentRemoved(cmd.comp);

                    break;
            }
        }

        class CommandData
        {

            public CommandData(Command command, Component comp)
            {
                this.command = command;
                this.comp = comp;
            }

            public Command command;
            public Component comp;
        }

        public IEnumerator<Component> GetEnumerator()
        {
            return components.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
