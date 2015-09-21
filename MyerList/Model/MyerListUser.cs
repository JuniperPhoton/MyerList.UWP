using GalaSoft.MvvmLight;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyerList.Model
{
    public class MyerListUser:ViewModelBase
    {
        private int _sid;
        public int SID
        {
            get
            {
                return _sid;
            }
            set
            {
                if(_sid!=value)
                    _sid = value;
                RaisePropertyChanged(()=>SID);
            }
        }

        private string _email;
        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                if(_email!=value)
                    _email = value;
                RaisePropertyChanged(()=>Email);
            }
        }

        
        private string _password;
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                if(_password!=value)
                    _password = value;
                RaisePropertyChanged(()=>Password);
            }
        }

        private string _confirmPassword;
        public string ConfirmPassword
        {
            get
            {
                return _confirmPassword;
            }
            set
            {
                if (_confirmPassword != value)
                    _confirmPassword = value;
                RaisePropertyChanged(() => ConfirmPassword);
            }
        }

        public MyerListUser()
        {

        }

    }
}
