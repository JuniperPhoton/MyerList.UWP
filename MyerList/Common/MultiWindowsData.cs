using MyerList.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyerListUWP.Common
{
    public class MultiWindowsData
    {
        public int CateColor { get; set; }
        public IEnumerable<ToDo> CurrentDisplayList { get; set; }

        public MultiWindowsData(int cate,IEnumerable<ToDo> list)
        {
            this.CateColor = cate;
            this.CurrentDisplayList = list;
        }
    }
}
