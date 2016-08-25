using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyerList.Model
{
    public enum StagedOperation
    {
        Modify,
        Add,
        Delete
    }
    public class StagedItem
    {
        public ToDo CurrentToDo { get; set; }

        public StagedOperation Operation { get; set; }

        public StagedItem()
        {

        }

        public StagedItem(ToDo todo, StagedOperation operation)
        {
            this.CurrentToDo = todo;
            this.Operation = operation;
        }
    }
}
