using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyerList.Helper
{
    public class MessengerExt
    {
        public static void SendMsg<T>(T item, MessengerTokens token)
        {
            Messenger.Default.Send<GenericMessage<T>>(new GenericMessage<T>(item), token);
        }

    }
}
