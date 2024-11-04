using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFApp
{
    public class TaskAddedEventArgs : EventArgs
    {
        public ToDo NewTask { get; }

        public TaskAddedEventArgs(ToDo newTask)
        {
            NewTask = newTask;
        }
    }

}
