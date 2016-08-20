using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyerList.CloudSerivce
{
    public class MyerListException : Exception
    {
        public string ErrorCode { get; set; }

        public string ErrorMsg { get; set; }

        public MyerListException()
        {

        }
    }
}
