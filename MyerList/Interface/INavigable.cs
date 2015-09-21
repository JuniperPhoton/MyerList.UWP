using System;
using System.Collections.Generic;
using System.Text;

namespace MyerList.Interface
{
    interface INavigable
    {
        /// <summary>
        /// OnNavigatedTo 时触发
        /// </summary>
        /// <param name="param"></param>
        void Activate(object param);

        /// <summary>
        /// OnNavigatedFrom 时触发
        /// </summary>
        /// <param name="param"></param>
        void Deactivate(object param);
    }
}
