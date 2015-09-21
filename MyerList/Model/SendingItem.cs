using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyerList.Model
{
    public class SendingItem:ViewModelBase
    {
        private ToDo _todoItem;
        public ToDo ToDoItem
        {
            get
            {
                return _todoItem;
            }
            set
            {
                if(_todoItem!=value)
                {
                    _todoItem = value;
                    RaisePropertyChanged(() => ToDoItem);
                }
            }
        }

        private SendingStatus _status;
        public SendingStatus Status
        {
            get
            {
                return _status;
            }
            set
            {
                if(_status!=value)
                {
                    _status = value;
                    RaisePropertyChanged(() => Status);
                }
            }
        }

        public SendingItem()
        {
            ToDoItem = new ToDo();
            Status = SendingStatus.ToBeSent;
        }

        public SendingItem(ToDo schedule,SendingStatus status)
        {
            this.ToDoItem = schedule;
            this.Status = status;
        }
    }

    public enum SendingStatus
    {
        ToBeSent,
        Sending,
        Sent
    }
}
