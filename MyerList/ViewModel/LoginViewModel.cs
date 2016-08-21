using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using JP.Utils.Data;
using JP.Utils.Debug;
using JP.Utils.Functions;
using MyerList.CloudSerivce;
using MyerList.Helper;
using MyerList.Interface;
using MyerList.Model;
using MyerListCustomControl;
using MyerListShared;
using MyerListUWP;
using MyerListUWP.Common;
using MyerListUWP.View;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyerList.ViewModel
{
    public class LoginViewModel : ViewModelBase, INavigable
    {
        private LoginMode LoginMode;

        /// <summary>
        /// Login or Register
        /// </summary>
        private string _title;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (_title != value)
                    _title = value;
                RaisePropertyChanged(() => Title);
            }
        }

        /// <summary>
        /// Register or login
        /// </summary>
        private string _btnContent;
        public string BtnContent
        {
            get
            {
                return _btnContent;
            }
            set
            {
                if (_btnContent != value)
                    _btnContent = value;
                RaisePropertyChanged(() => BtnContent);
            }
        }

        /// <summary>
        /// Show register btn
        /// </summary>
        private Visibility _showregister;
        public Visibility ShowRegister
        {
            get
            {
                return _showregister;
            }
            set
            {
                _showregister = value;
                RaisePropertyChanged(() => ShowRegister);
            }
        }

        /// <summary>
        /// For Progress Bar
        /// </summary>
        private bool _isLoading;
        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
            set
            {
                _isLoading = value;
                RaisePropertyChanged(() => IsLoading);
            }
        }

        #region privacy info.
        private string _tempemail;
        public string TempEmail
        {
            get
            {
                return _tempemail;
            }
            set
            {
                if (_tempemail != value)
                    _tempemail = value;
                RaisePropertyChanged(() => TempEmail);
            }
        }

        private string _inputpassword;
        public string InputPassword
        {
            get
            {
                return _inputpassword;
            }
            set
            {
                if (_inputpassword != value)
                    _inputpassword = value;
                RaisePropertyChanged(() => InputPassword);
            }
        }

        private string _confirmpassword;
        public string ConfirmPassword
        {
            get
            {
                return _confirmpassword;
            }
            set
            {
                if (_confirmpassword != value)
                    _confirmpassword = value;
                RaisePropertyChanged(() => ConfirmPassword);
            }
        }
        #endregion

        /// <summary>
        /// When press the register btn
        /// </summary>
        private RelayCommand _nextCommand;
        public RelayCommand NextCommand
        {
            get
            {
                if (_nextCommand != null) return _nextCommand;
                return _nextCommand = new RelayCommand(async () =>
                {
                    try
                    {
                        if (string.IsNullOrEmpty(TempEmail) || string.IsNullOrEmpty(InputPassword))
                        {
                            ToastService.SendToast(ResourcesHelper.GetResString("InputAlert"));
                            IsLoading = false;
                            return;
                        }

                        if (!Functions.IsValidEmail(TempEmail))
                        {
                            ToastService.SendToast(ResourcesHelper.GetResString("EmailInvaild"));
                            IsLoading = false;
                            return;
                        }

                        //注册
                        if (LoginMode == LoginMode.Register || LoginMode == LoginMode.OfflineModeToRegister)
                        {
                            if (InputPassword != ConfirmPassword)
                            {
                                ToastService.SendToast(ResourcesHelper.GetResString("PasswordInvaild"));

                                IsLoading = false;
                                return;
                            }

                            IsLoading = true;
                            if (await RegisterAsync())
                            {
                                if (await LoginAsync())
                                {
                                    Frame rootframe = Window.Current.Content as Frame;
                                    if (rootframe != null) rootframe.Navigate(typeof(MainPage), LoginMode);
                                }
                            }
                            IsLoading = false;
                        }

                        //登录
                        else if (LoginMode == LoginMode.Login || LoginMode == LoginMode.OfflineModeToLogin)
                        {
                            IsLoading = true;

                            if (await LoginAsync())
                            {
                                Frame rootframe = Window.Current.Content as Frame;
                                if (rootframe != null) rootframe.Navigate(typeof(MainPage), LoginMode);
                            }

                            IsLoading = false;
                        }
                    }
                    catch (MyerListException e)
                    {
                        ToastService.SendToast(ErrorUtils.GetUserMsgFromErrorCode(int.Parse(e.ErrorCode)));
                        IsLoading = false;
                    }
                    catch (Exception e)
                    {
                        var task = Logger.LogAsync(e);
                        IsLoading = false;
                    }
                });
            }
        }

        /// <summary>
        /// For Windows, navigate back
        /// </summary>
        private RelayCommand _backCommand;
        public RelayCommand BackCommand
        {
            get
            {
                if (_backCommand != null)
                {
                    return _backCommand;
                }
                return _backCommand = new RelayCommand(() =>
                {
                    Frame rootframe = Window.Current.Content as Frame;
                    if (rootframe != null && rootframe.CanGoBack)
                    {
                        rootframe.GoBack();
                    }
                });
            }
        }

        public LoginViewModel()
        {
            IsLoading = false;

            Messenger.Default.Register<GenericMessage<string>>(this, MessengerTokens.PressEnterToLoginToken, act =>
            {
                NextCommand.Execute(null);
            });

#if DEBUG
            TempEmail = "dengweichao@hotmail.com";
            InputPassword = "windfantasy";
#endif
        }

        private async Task<bool> RegisterAsync()
        {
            try
            {
                //注册
                IsLoading = true;

                var loader = new ResourceLoader();

                var check = await CloudService.CheckEmailExistAsync(TempEmail);
                if (check)
                {
                    throw new MyerListException()
                    {
                        ErrorCode="202"
                    };
                }
                string salt = await CloudService.RegisterAsync(TempEmail, InputPassword);
                if (!string.IsNullOrEmpty(salt))
                {
                    LocalSettingHelper.AddValue("email", TempEmail);
                    LocalSettingHelper.AddValue("password", InputPassword);
                    return true;
                }
                else
                {
                    throw new MyerListException()
                    {
                        ErrorCode = ""
                    };
                }
            }
            catch (MyerListException e)
            {
                ToastService.SendToast(ErrorUtils.GetUserMsgFromErrorCode(int.Parse(e.ErrorCode)));
                IsLoading = false;
                return false;
            }
            catch (TaskCanceledException)
            {
                ToastService.SendToast(ResourcesHelper.GetResString("RequestError"));
                IsLoading = false;
                return false;
            }
            catch (COMException)
            {
                ToastService.SendToast(ResourcesHelper.GetResString("RequestError"));
                IsLoading = false;
                return false;
            }
        }

        private async Task<bool> LoginAsync()
        {
            try
            {
                IsLoading = true;

                var check = await CloudService.CheckEmailExistAsync(TempEmail);

                string salt = await CloudService.GetSaltAsync(TempEmail);

                //尝试登录
                var login = await CloudService.LoginAsync(TempEmail, InputPassword, salt);

                App.IsInOfflineMode = false;
                LocalSettingHelper.AddValue("OfflineMode", "false");
                return true;
            }
            catch (MyerListException e)
            {
                ToastService.SendToast(ErrorUtils.GetUserMsgFromErrorCode(int.Parse(e.ErrorCode)));
                return false;
            }
            catch (TaskCanceledException)
            {
                ToastService.SendToast(ResourcesHelper.GetResString("RequestError"));
                return false;
            }
            catch (COMException)
            {
                ToastService.SendToast(ResourcesHelper.GetResString("RequestError"));
                return false;
            }
            catch (Exception)
            {
                ToastService.SendToast(ResourcesHelper.GetResString("RequestError"));
                return false;
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ToLoginMode()
        {
            ShowRegister = Visibility.Collapsed;

            var loader = new ResourceLoader();
            Title = loader.GetString("Login");
            BtnContent = loader.GetString("Login");
        }

        private void ToRegisterMode()
        {
            ShowRegister = Visibility.Visible;
            Title = ResourcesHelper.GetResString("Register");
            BtnContent = ResourcesHelper.GetResString("Register");
        }

        public void Activate(object param)
        {
            if (param is LoginMode)
            {
                var mode = (LoginMode)param;
                LoginMode = mode;

                switch (LoginMode)
                {
                    case LoginMode.Login:
                        {
                            ToLoginMode();
                        }; break;
                    case LoginMode.Register:
                        {
                            ToRegisterMode();
                        }; break;
                    case LoginMode.OfflineModeToLogin:
                        {
                            ToLoginMode();
                        }; break;
                    case LoginMode.OfflineModeToRegister:
                        {
                            ToRegisterMode();
                        }; break;
                }
            }
        }

        public void Deactivate(object param)
        {
            TempEmail = "";
            InputPassword = "";
            ConfirmPassword = "";
            IsLoading = false;
        }

        public void Loaded(object param)
        {

        }
    }
}
