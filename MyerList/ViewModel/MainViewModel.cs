using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Windows.UI.Xaml.Media;
using MyerList.Model;
using MyerList.Helper;
using JP.Utils.Data;
using JP.Utils.Debug;
using MyerList.Interface;
using MyerListUWP;
using MyerListUWP.Model;
using MyerListUWP.Helper;
using MyerListUWP.ViewModel;
using MyerListCustomControl;
using System.Runtime.InteropServices;

namespace MyerList.ViewModel
{
    public class MainViewModel : ViewModelBase, INavigable
    {
        private AddMode _addMode = AddMode.None;

        #region 汉堡包/类别/导航
        /// <summary>
        /// 选择了的类别
        /// </summary>
        private int _selectedCate;
        public int SelectedCate
        {
            get
            {
                return _selectedCate;
            }
            set
            {
                if (_selectedCate != value)
                {
                    _selectedCate = value;
                    RaisePropertyChanged(() => SelectedCate);
                    SelectCateCommand.Execute(value);
                    RaisePropertyChanged(() => ShowSortButton);
                }
            }
        }

        private SolidColorBrush _cateColor;
        public SolidColorBrush CateColor
        {
            get
            {
                return _cateColor;
            }
            set
            {
                if (_cateColor != value)
                {
                    _cateColor = value;
                    RaisePropertyChanged(() => CateColor);
                }
            }
        }

        private RelayCommand<object> _selectCateCommand;
        public RelayCommand<object> SelectCateCommand
        {
            get
            {
                if (_selectCateCommand != null) return _selectCateCommand;
                return _selectCateCommand = new RelayCommand<object>((param) =>
                 {
                     SelectedIndex = 0;
                     ChangeDisplayCateList((int)param);
                 });
            }
        }

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
                {
                    _title = value;
                    RaisePropertyChanged(() => Title);
                }
            }
        }

        private int _selectedIndex;
        public int SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            set
            {
                if (_selectedIndex != value)
                {
                    _selectedIndex = value;
                    RaisePropertyChanged(() => SelectedIndex);

                    switch (value)
                    {
                        case 0:
                            {
                                Title = ResourcesHelper.GetString("ToDoPivotItem");
                                DeleteIconAlpha = 0.3;
                                TodoIconAlpha = 1;
                                Messenger.Default.Send(new GenericMessage<string>(""), MessengerTokens.ChangeCommandBarToDefault);
                            }; break;
                        case 1:
                            {
                                Messenger.Default.Send(new GenericMessage<string>(""), MessengerTokens.ChangeCommandBarToDelete);
                            }; break;
                    }
                }
            }
        }

        private RelayCommand _selectToDoCommand;
        public RelayCommand SelectToDoCommand
        {
            get
            {
                if (_selectToDoCommand != null) return _selectToDoCommand;
                return _selectToDoCommand = new RelayCommand(() =>
                 {
                     SelectedIndex = 0;
                 });
            }
        }

        private RelayCommand _selectDeleteCommand;
        public RelayCommand SelectDeleteCommand
        {
            get
            {
                if (_selectDeleteCommand != null) return _selectDeleteCommand;
                return _selectDeleteCommand = new RelayCommand(() =>
                 {
                     SelectedIndex = 1;
                 });
            }
        }

        private double _todoIconAlpha;
        public double TodoIconAlpha
        {
            get
            {
                return _todoIconAlpha;
            }
            set
            {
                if (_todoIconAlpha != value)
                {
                    _todoIconAlpha = value;
                    RaisePropertyChanged(() => TodoIconAlpha);
                }
            }
        }

        private double _deleteIconAlpha;
        public double DeleteIconAlpha
        {
            get
            {
                return _deleteIconAlpha;
            }
            set
            {
                if (_deleteIconAlpha != value)
                {
                    _deleteIconAlpha = value;
                    RaisePropertyChanged(() => DeleteIconAlpha);
                }
            }
        }

        private CategoryViewModel _cateVM;
        public CategoryViewModel CateVM
        {
            get
            {
                return _cateVM;
            }
            set
            {
                if (_cateVM != value)
                {
                    _cateVM = value;
                    RaisePropertyChanged(() => CateVM);
                }
            }
        }

        #endregion

        #region 账号
        /// <summary>
        ///是否显示要求登录按钮
        /// </summary>
        private Visibility _showLoginBtnVisibility;
        public Visibility ShowLoginBtnVisibility
        {
            get
            {
                return _showLoginBtnVisibility;
            }
            set
            {
                _showLoginBtnVisibility = value;
                RaisePropertyChanged(() => ShowLoginBtnVisibility);
            }
        }

        /// <summary>
        /// 是否显示账户信息
        /// </summary>
        private Visibility _showAccountInfoVisibility;
        public Visibility ShowAccountInfoVisibility
        {
            get
            {
                return _showAccountInfoVisibility;
            }
            set
            {
                if (_showAccountInfoVisibility != value)
                {
                    _showAccountInfoVisibility = value;
                    RaisePropertyChanged(() => ShowAccountInfoVisibility);
                }
            }
        }

        /// <summary>
        /// 表示当前的用户
        /// </summary>
        private MyerListUser _currentUser;
        public MyerListUser CurrentUser
        {
            get
            {
                return _currentUser;
            }
            set
            {
                if (_currentUser != value)
                {
                    _currentUser = value;
                    RaisePropertyChanged(() => CurrentUser);
                }
            }
        }

        private RelayCommand _loginCommand;
        public RelayCommand LoginCommand
        {
            get
            {
                if (_loginCommand != null) return _loginCommand;
                return _loginCommand = new RelayCommand(async () =>
                 {
                     DialogService cdex = new DialogService(ResourcesHelper.GetString("Notice"), ResourcesHelper.GetString("SignUpContent"));
                     cdex.LeftButtonContent = ResourcesHelper.GetString("Register");
                     cdex.RightButtonContent = ResourcesHelper.GetString("Login");
                     cdex.OnLeftBtnClick += ((s) =>
                       {
                           App.IsSyncListOnce = false;
                           var rootFrame = Window.Current.Content as Frame;
                           rootFrame.Navigate(typeof(LoginPage), LoginMode.OfflineModeToRegister);
                           cdex.Hide();
                       });
                     cdex.OnRightBtnClick += (() =>
                       {
                           App.IsSyncListOnce = false;
                           var rootFrame = Window.Current.Content as Frame;
                           rootFrame.Navigate(typeof(LoginPage), LoginMode.OfflineModeToLogin);
                           cdex.Hide();
                       });
                     await cdex.ShowAsync();
                 });
            }
        }

        #endregion

        #region CommandBar
        public Visibility ShowSortButton
        {
            get
            {
                if (SelectedCate == 0) return Visibility.Visible;
                else return Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 显示加载条
        /// </summary>
        private Visibility _isLoading;
        public Visibility IsLoading
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

        /// <summary>
        /// 点击+号添加新的待办事项
        /// </summary>
        private RelayCommand _addCommand;
        public RelayCommand AddCommand
        {
            get
            {
                if (_addCommand != null)
                {
                    return _addCommand;
                }
                return _addCommand = new RelayCommand(() =>
                {
                    ModeTitle = ResourcesHelper.GetString("AddTitle");
                    NewToDo = new ToDo();
                    AddingCate = 0;
                    _addMode = AddMode.Add;

                    ShowPaneOpen = true;
                });
            }
        }

        /// <summary>
        /// 同步列表
        /// </summary>
        private RelayCommand _syncCommand;
        public RelayCommand SyncCommand
        {
            get
            {
                if (_syncCommand != null)
                {
                    return _syncCommand;
                }
                return _syncCommand = new RelayCommand(async () =>
                {
                    await SyncAllToDos();
                });
            }
        }

        private RelayCommand _toggleReorderCommand;
        public RelayCommand ToggleReorderCommand
        {
            get
            {
                if (_toggleReorderCommand != null) return _toggleReorderCommand;
                return _toggleReorderCommand = new RelayCommand(() =>
                  {

                  });
            }
        }
        #endregion

        #region 添加/修改面板
        private bool _showPaneOpen;
        public bool ShowPaneOpen
        {
            get
            {
                return _showPaneOpen;
            }
            set
            {
                _showPaneOpen = value;
                RaisePropertyChanged(() => ShowPaneOpen);
            }
        }

        /// <summary>
        /// 对话框显示的标题
        /// </summary>
        private string _modetitle;
        public string ModeTitle
        {
            get
            {
                return _modetitle;
            }
            set
            {
                if (_modetitle != value)
                    _modetitle = value;
                RaisePropertyChanged(() => ModeTitle);
            }
        }

        private int _addingCate;
        public int AddingCate
        {
            get
            {
                return _addingCate;
            }
            set
            {
                if (_addingCate != value)
                {
                    _addingCate = value;
                    RaisePropertyChanged(() => AddingCate);
                }
            }
        }

        /// <summary>
        /// 添加/修改待办事项时候的“完成”
        /// </summary>
        private RelayCommand _okCommand;
        public RelayCommand OkCommand
        {
            get
            {
                return _okCommand = new RelayCommand(async () =>
                {
                    await AddOrModifyToDo();
                });
            }
        }

        /// <summary>
        ///添加/修改待办事项时候的“取消”
        /// </summary>
        private RelayCommand _cancelCommand;
        public RelayCommand CancelCommand
        {
            get
            {
                if (_cancelCommand != null)
                {
                    return _cancelCommand;
                }
                return _cancelCommand = new RelayCommand(() =>
                {
                    ShowPaneOpen = false;
                    NewToDo = new ToDo();
                });
            }
        }

        #endregion

        #region 待办事项列表本身
        public Visibility ShowCategory
        {
            get
            {
                if (SelectedCate == 0) return Visibility.Visible;
                else return Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 表示当前添加的待办事项
        /// </summary>
        private ToDo _newToDo;
        public ToDo NewToDo
        {
            get
            {
                return _newToDo;
            }
            set
            {
                if (_newToDo != value)
                {
                    _newToDo = value;
                    RaisePropertyChanged(() => NewToDo);
                }
            }
        }

        /// <summary>
        ///显示没有待办事项
        /// </summary>
        private Visibility _shownoitems;
        public Visibility ShowNoItems
        {
            get
            {
                return _shownoitems;
            }
            set
            {
                _shownoitems = value;
                RaisePropertyChanged(() => ShowNoItems);
            }
        }

        /// <summary>
        /// 所有待办事项
        /// </summary>
        private ObservableCollection<ToDo> _myToDos;
        public ObservableCollection<ToDo> MyToDos
        {
            get
            {
                if (_myToDos != null)
                {
                    return _myToDos;
                }
                return _myToDos = new ObservableCollection<ToDo>();
            }
            set
            {
                if (_myToDos != value)
                {
                    _myToDos = value;
                }
                RaisePropertyChanged(() => MyToDos);
            }
        }

        /// <summary>
        /// 当前的待办事项
        /// </summary>
        private ObservableCollection<ToDo> _currentDisplayToDos;
        public ObservableCollection<ToDo> CurrentDisplayToDos
        {
            get
            {
                if (_currentDisplayToDos.Count == 0) ShowNoItems = Visibility.Visible;
                else ShowNoItems = Visibility.Collapsed;
                return _currentDisplayToDos;
            }
            set
            {
                if (_currentDisplayToDos != value)
                {
                    _currentDisplayToDos = value;
                    RaisePropertyChanged(() => CurrentDisplayToDos);
                }
            }
        }

        /// <summary>
        ///删除待办事项
        /// </summary>
        private RelayCommand<object> _deleteCommand;
        public RelayCommand<object> DeleteCommand
        {
            get
            {
                if (_deleteCommand != null)
                {
                    return _deleteCommand;
                }

                return _deleteCommand = new RelayCommand<object>(async (param) =>
                {
                    string id = (string)param;
                    await DeleteToDo(id);
                });
            }
        }

        /// <summary>
        /// 完成待办事项
        /// </summary>
        private RelayCommand<object> _checkCommand;
        public RelayCommand<object> CheckCommand
        {
            get
            {
                if (_checkCommand != null) return _checkCommand;
                return _checkCommand = new RelayCommand<object>(async (param) =>
                {
                    string id = (string)param;
                    await CompleteTodo(id);

                });
            }
        }

        /// <summary>
        /// 点击列表的项目，修改待办事项
        /// </summary>
        private RelayCommand<ToDo> _modifyCommand;
        public RelayCommand<ToDo> ModifyCommand
        {
            get
            {
                if (_modifyCommand != null)
                {
                    return _modifyCommand;
                }

                return _modifyCommand = new RelayCommand<ToDo>((todo) =>
                {
                    try
                    {
                        _addMode = AddMode.Modify;

                        ShowPaneOpen = true;

                        var id = todo.ID;
                        var itemToModify = MyToDos.ToList().Find(sche =>
                        {
                            if (sche.ID == id) return true;
                            else return false;
                        });

                        if (itemToModify == null)
                        {
                            return;
                        }

                        this.NewToDo.ID = itemToModify.ID;
                        this.NewToDo.Content = itemToModify.Content;
                        this.AddingCate = itemToModify.Category;

                        Messenger.Default.Send(new GenericMessage<string>(""), MessengerTokens.ShowModifyUI);

                        ModeTitle = ResourcesHelper.GetString("ModifyTitle");

                    }
                    catch (Exception ex)
                    {
                        var task = ExceptionHelper.WriteRecord(ex, nameof(MainViewModel), nameof(ModifyCommand));
                    }
                });
            }
        }

        /// <summary>
        /// 修改类别 
        /// </summary>
        private RelayCommand<object> _changeCateCommand;
        public RelayCommand<object> ChangeCateCommand
        {
            get
            {
                if (_changeCateCommand != null) return _changeCateCommand;
                return _changeCateCommand = new RelayCommand<object>(async (param) =>
                 {
                     var id = (string)param;
                     await ChangeCategory(id);
                 });
            }
        }
        #endregion

        #region 排序
        private bool? _isInSortMode;
        public bool? IsInSortMode
        {
            get
            {
                return _isInSortMode;
            }
            set
            {
                if (_isInSortMode != value)
                {
                    _isInSortMode = value;
                    RaisePropertyChanged(() => IsInSortMode);

                    if (!App.IsSyncListOnce)
                    {
                        return;
                    }
                    if (value == true)
                    {
                        Messenger.Default.Send(new GenericMessage<string>(""), MessengerTokens.GoToSort);
                    }
                    else
                    {
                        Messenger.Default.Send(new GenericMessage<string>(""), MessengerTokens.LeaveSort);
                        var task = UpdateOrder();
                    }
                }
            }
        }

        #endregion

        #region 已经删除的
        /// <summary>
        /// 已经删除了的待办事项
        /// </summary>
        private ObservableCollection<ToDo> _deletedToDos;
        public ObservableCollection<ToDo> DeletedToDos
        {
            get
            {
                if (_deletedToDos != null)
                {
                    if (_deletedToDos.Count == 0) NoDeletedItemsVisibility = Visibility.Visible;
                    else NoDeletedItemsVisibility = Visibility.Collapsed;
                    return _deletedToDos;
                }
                return _deletedToDos = new ObservableCollection<ToDo>();
            }
            set
            {
                if (_deletedToDos != value)
                {
                    _deletedToDos = value;
                    RaisePropertyChanged(() => DeletedToDos);
                }
            }
        }

        /// <summary>
        /// 没有已经删除的内容
        /// </summary>
        private Visibility _noDeletedItemsVisibility;
        public Visibility NoDeletedItemsVisibility
        {
            get
            {
                return _noDeletedItemsVisibility;
            }
            set
            {
                if (_noDeletedItemsVisibility != value)
                {
                    _noDeletedItemsVisibility = value;
                    RaisePropertyChanged(() => NoDeletedItemsVisibility);
                }
            }
        }

        /// <summary>
        /// 重新添加回列表
        /// </summary>
        private RelayCommand<string> _redoCommand;
        public RelayCommand<string> RedoCommand
        {
            get
            {
                if (_redoCommand != null) return _redoCommand;
                return _redoCommand = new RelayCommand<string>(async (id) =>
                {
                    var scheToAdd = DeletedToDos.ToList().Find(s =>
                    {
                        if (s.ID == id) return true;
                        else return false;
                    });

                    _addMode = AddMode.None;
                    NewToDo = scheToAdd;
                    await AddToDo();

                    DeletedToDos.Remove(scheToAdd);
                    await SerializerHelper.SerializerToJson<ObservableCollection<ToDo>>(DeletedToDos, "deleteditems.sch", true);

                });
            }
        }

        /// <summary>
        /// 永久删除
        /// </summary>
        private RelayCommand<string> _permanentDeleteCommand;
        public RelayCommand<string> PermanentDeleteCommand
        {
            get
            {
                if (_permanentDeleteCommand != null) return _permanentDeleteCommand;
                return _permanentDeleteCommand = new RelayCommand<string>(async (id) =>
                {
                    DeletedToDos.Remove(DeletedToDos.ToList().Find(s =>
                    {
                        if (s.ID == id) return true;
                        else return false;
                    }));
                    await SerializerHelper.SerializerToJson<ObservableCollection<ToDo>>(DeletedToDos, "deleteditems.sch", true);

                });
            }
        }

        private RelayCommand _deleteAllCommand;
        public RelayCommand DeteteAllCommand
        {
            get
            {
                if (_deleteAllCommand != null) return _deleteAllCommand;
                return _deleteAllCommand = new RelayCommand(async () =>
                 {
                     if (DeletedToDos.Count == 0) return;
                     DialogService cdex = new DialogService(ResourcesHelper.GetString("Notice"), ResourcesHelper.GetString("DeleteAllConfirm"));
                     cdex.LeftButtonContent = ResourcesHelper.GetString("Ok");
                     cdex.RightButtonContent = ResourcesHelper.GetString("Cancel");
                     cdex.OnLeftBtnClick += (async (s) =>
                       {
                           DeletedToDos.Clear();
                           await SerializerHelper.SerializerToJson<ObservableCollection<ToDo>>(DeletedToDos, SerializerFileNames.DeletedFileName, true);
                           cdex.Hide();
                       });
                     await cdex.ShowAsync();
                 });
            }
        }
        #endregion

        #region 抽屉底部命令

        private RelayCommand _toStartPageCommand;
        public RelayCommand ToStartPageCommand
        {
            get
            {
                if (_toStartPageCommand != null) return _toStartPageCommand;
                return _toStartPageCommand = new RelayCommand(() =>
                {
                    var rootFrame = Window.Current.Content as Frame;
                    rootFrame.Navigate(typeof(StartPage));
                });
            }
        }

        /// <summary>
        /// 跳到 About 页面
        /// </summary>
        private RelayCommand _goToAboutCommand;
        public RelayCommand GoToAboutCommand
        {
            get
            {
                if (_goToAboutCommand != null)
                {
                    return _goToAboutCommand;
                }
                return _goToAboutCommand = new RelayCommand(() =>
                {
                    Frame frame = Window.Current.Content as Frame;
                    if (frame != null) frame.Navigate(typeof(AboutPage));
                });
            }
        }

        /// <summary>
        /// 跳到 Settings 页面
        /// </summary>
        private RelayCommand _gotoSettingCommand;
        public RelayCommand GoToSettingCommand
        {
            get
            {
                if (_gotoSettingCommand != null)
                {
                    return _gotoSettingCommand;
                }
                return _gotoSettingCommand = new RelayCommand(() =>
                {
                    Frame frame = Window.Current.Content as Frame;
                    if (frame != null) frame.Navigate(typeof(SettingPage));
                });
            }
        }
        #endregion

        public MainViewModel()
        {
            IsLoading = Visibility.Collapsed;
            NoDeletedItemsVisibility = Visibility.Collapsed;

            //初始化
            CateVM = new CategoryViewModel();
            NewToDo = new ToDo();
            CurrentUser = new MyerListUser();
            MyToDos = new ObservableCollection<ToDo>();
            DeletedToDos = new ObservableCollection<ToDo>();
            CurrentDisplayToDos = MyToDos;
            IsInSortMode = false;

            SelectedCate = -1;

            AddingCate = 0;

            CateColor = Application.Current.Resources["DefaultColor"] as SolidColorBrush;

            //设置当前页面为 To-Do
            SelectedIndex = 0;
            TodoIconAlpha = 1;
            DeleteIconAlpha = 0.3;
            Title = ResourcesHelper.GetString("CateDefault");

            //完成ToDo
            Messenger.Default.Register<GenericMessage<string>>(this, MessengerTokens.CheckToDo, act =>
            {
                var id = act.Content;
                CheckCommand.Execute(id);
            });

            //删除To-Do
            Messenger.Default.Register<GenericMessage<string>>(this, MessengerTokens.DeleteToDo, act =>
             {
                 var id = act.Content;
                 DeleteCommand.Execute(id);
             });

            Messenger.Default.Register<GenericMessage<ToDo>>(this, MessengerTokens.ReaddToDo, act =>
            {
                this.NewToDo = act.Content;
                OkCommand.Execute(false);
            });

            Messenger.Default.Register<GenericMessage<string>>(this, MessengerTokens.CompleteSort, async act =>
              {
                  await UpdateOrder();
              });
        }

        private async Task UpdateOrder()
        {
            var orderStr = ToDo.GetCurrentOrderString(MyToDos);
            await PostHelper.SetMyOrder(LocalSettingHelper.GetValue("sid"), orderStr);
        }

        /// <summary>
        /// 添加or修改内容
        /// </summary>
        /// <returns></returns>
        private async Task AddOrModifyToDo()
        {
            try
            {
                if (NewToDo != null)
                {
                    if (_addMode == AddMode.None) return;

                    ShowPaneOpen = false;

                    //显示进度条
                    IsLoading = Visibility.Visible;

                    //添加
                    if (_addMode == AddMode.Add)
                    {
                        await AddToDo();
                    }
                    //修改
                    else if (_addMode == AddMode.Modify)
                    {
                        await ModifyToDo();
                    }
                }
            }
            catch (Exception ex)
            {
                var task = ExceptionHelper.WriteRecord(ex, nameof(MainViewModel), nameof(AddOrModifyToDo));
            }
        }

        private async Task AddToDo()
        {
            ShowNoItems = Visibility.Collapsed;
            //离线模式
            if (App.IsInOfflineMode || App.IsNoNetwork)
            {
                NewToDo.ID = Guid.NewGuid().ToString();
                NewToDo.Category = AddingCate;

                //0 for insert,1 for add
                if (LocalSettingHelper.GetValue("AddMode") == "0")
                {
                    MyToDos.Insert(0, NewToDo);
                }
                else
                {
                    MyToDos.Add(NewToDo);
                }
                await SerializerHelper.SerializerToJson<ObservableCollection<ToDo>>(MyToDos, SerializerFileNames.ToDoFileName);

                Messenger.Default.Send(new GenericMessage<ObservableCollection<ToDo>>(MyToDos), MessengerTokens.UpdateTile);

                NewToDo = new ToDo();

                IsLoading = Visibility.Collapsed;
            }
            else if (App.IsNoNetwork)
            {
                //TO DO: Store the schedule in SendingQueue
            }
            else
            {
                try
                {
                    //在线模式
                    var result = await PostHelper.AddSchedule(LocalSettingHelper.GetValue("sid"), NewToDo.Content, "0", AddingCate.ToString());
                    if (!string.IsNullOrEmpty(result))
                    {
                        ToDo newSchedule = ToDo.ParseJsonTo(result);

                        if (LocalSettingHelper.GetValue("AddMode") == "0")
                        {
                            MyToDos.Insert(0, newSchedule);
                            UpdateDisplayList(SelectedCate);
                        }
                        else
                        {
                            MyToDos.Add(newSchedule);
                            UpdateDisplayList(SelectedCate);
                        }

                        await PostHelper.SetMyOrder(LocalSettingHelper.GetValue("sid"), ToDo.GetCurrentOrderString(MyToDos));

                        NewToDo = new ToDo();

                        Messenger.Default.Send(new GenericMessage<ObservableCollection<ToDo>>(MyToDos), MessengerTokens.UpdateTile);

                        IsLoading = Visibility.Collapsed;

                        await SerializerHelper.SerializerToJson<ObservableCollection<ToDo>>(MyToDos, SerializerFileNames.ToDoFileName);
                    }
                }
                catch (Exception)
                {
                    await ToastService.SendToastAsync(ResourcesHelper.GetString("RequestError"));
                }
            }
        }

        /// <summary>
        /// 删除todo
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        /// 先从列表删除，然后把列表内容都序列化保存，接着：
        /// 1.如果已经登陆的，尝试发送请求；
        /// 2.离线模式，不用管
        private async Task DeleteToDo(string id)
        {
            try
            {
                var itemToDeleted = MyToDos.ToList().Find(s =>
                  {
                      if (s.ID == id) return true;
                      else return false;
                  });

                DeletedToDos.Add(itemToDeleted);
                await SerializerHelper.SerializerToJson<ObservableCollection<ToDo>>(DeletedToDos, SerializerFileNames.DeletedFileName, true);

                MyToDos.Remove(itemToDeleted);
                UpdateDisplayList(SelectedCate);
                await SerializerHelper.SerializerToJson<ObservableCollection<ToDo>>(MyToDos, SerializerFileNames.ToDoFileName, true);


                if (!App.IsInOfflineMode)
                {
                    var result = await PostHelper.DeleteSchedule(id);
                    await PostHelper.SetMyOrder(LocalSettingHelper.GetValue("sid"), ToDo.GetCurrentOrderString(MyToDos));
                }

                Messenger.Default.Send(new GenericMessage<ObservableCollection<ToDo>>(MyToDos), MessengerTokens.UpdateTile);

            }
            catch (Exception e)
            {
                var task = ExceptionHelper.WriteRecord(e, nameof(MainViewModel), nameof(DeleteToDo));
            }
        }

        /// <summary>
        /// 完成待办事项
        /// </summary>
        /// <param name="id">待办事项的ID</param>
        /// <returns></returns>
        private async Task CompleteTodo(string id)
        {
            try
            {
                var currentItem = (from e in MyToDos where e.ID == id select e).FirstOrDefault();
                currentItem.IsDone = !currentItem.IsDone;

                await SerializerHelper.SerializerToJson<ObservableCollection<ToDo>>(MyToDos, "myschedules.sch", true);

                //非离线模式
                if (!App.IsInOfflineMode)
                {
                    var isDone = await PostHelper.FinishSchedule(id, currentItem.IsDone ? "1" : "0");
                    if (isDone)
                    {
                        await PostHelper.SetMyOrder(LocalSettingHelper.GetValue("sid"), ToDo.GetCurrentOrderString(MyToDos));

                        Messenger.Default.Send(new GenericMessage<ObservableCollection<ToDo>>(MyToDos), MessengerTokens.UpdateTile);
                    }
                }
                //离线模式
                else Messenger.Default.Send(new GenericMessage<ObservableCollection<ToDo>>(MyToDos), MessengerTokens.UpdateTile);
            }
            catch (Exception ex)
            {
                var task = ExceptionHelper.WriteRecord(ex, nameof(MainViewModel), nameof(CompleteTodo));
            }

        }

        /// <summary>
        /// 从云端同步所有待办事项
        /// </summary>
        /// <returns></returns>
        private async Task SyncAllToDos()
        {
            try
            {
                //没网络
                if (App.IsNoNetwork)
                {
                    //通知没有网络
                    await ToastService.SendToastAsync(ResourcesHelper.GetString("NoNetworkHint"));
                    return;
                }
                //加载滚动条
                IsLoading = Visibility.Visible;

                var task = ToastService.SendToastAsync(ResourcesHelper.GetString("Syncing"));

                var isSyncOK = false;

                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(3.2);
                timer.Tick += ((sendert, et) =>
                {
                    if (isSyncOK)
                        IsLoading = Visibility.Collapsed;
                    timer.Stop();
                });
                timer.Start();

                try
                {
                    var result = await PostHelper.GetMySchedules(LocalSettingHelper.GetValue("sid"));
                    if (!string.IsNullOrEmpty(result))
                    {
                        //获得无序的待办事项
                        var scheduleWithoutOrder = ToDo.ParseJsonToObs(result);

                        //获得顺序列表
                        var orders = await PostHelper.GetMyOrder(LocalSettingHelper.GetValue("sid"));

                        //排序
                        MyToDos = ToDo.SetOrderByString(scheduleWithoutOrder, orders);

                        ChangeDisplayCateList(SelectedCate);

                        await ToastService.SendToastAsync(ResourcesHelper.GetString("SyncSuccessfully"));

                        await SerializerHelper.SerializerToJson<ObservableCollection<ToDo>>(MyToDos, SerializerFileNames.ToDoFileName, true);
                    }
                }
                catch (COMException)
                {
                    await ToastService.SendToastAsync(ResourcesHelper.GetString("RequestError"));
                }
                finally
                {
                    IsLoading = Visibility.Collapsed;
                }

                //最后更新动态磁贴
                Messenger.Default.Send(new GenericMessage<ObservableCollection<ToDo>>(MyToDos), MessengerTokens.UpdateTile);
            }
            catch (Exception ex)
            {
                var task = ExceptionHelper.WriteRecord(ex, nameof(MainViewModel), nameof(SyncAllToDos));
            }
        }

        private async Task AddAllToDos()
        {
            foreach (var sche in MyToDos)
            {
                var result = await PostHelper.AddSchedule(LocalSettingHelper.GetValue("sid"), sche.Content, sche.IsDone ? "1" : "0", SelectedCate.ToString());
            }
        }

        /// <summary>
        /// 修改待办事项
        /// </summary>
        /// <returns></returns>
        private async Task ModifyToDo()
        {
            IsLoading = Visibility.Visible;

            //修改当前列表
            var itemToModify = MyToDos.ToList().Find(sche =>
            {
                if (sche.ID == NewToDo.ID) return true;
                else return false;
            });
            itemToModify.Content = NewToDo.Content;
            itemToModify.Category = AddingCate;
            itemToModify.CreateTime = DateTime.Now.ToString();

            //离线模式
            if (App.IsInOfflineMode)
            {
                await SerializerHelper.SerializerToJson<ObservableCollection<ToDo>>(MyToDos, SerializerFileNames.ToDoFileName, true);

                NewToDo = new ToDo();

                Messenger.Default.Send(new GenericMessage<ObservableCollection<ToDo>>(MyToDos), MessengerTokens.UpdateTile);

                return;
            }
            //非离线模式
            else
            {
                try
                {
                    var resultUpdate = await PostHelper.UpdateContent(itemToModify.ID, itemToModify.Content, itemToModify.CreateTime, AddingCate);
                    if (resultUpdate)
                    {
                        MyToDos.ToList().Find(sche =>
                        {
                            if (sche.ID == NewToDo.ID) return true;
                            else return false;
                        }).Content = NewToDo.Content;

                        NewToDo = new ToDo();

                        Messenger.Default.Send(new GenericMessage<ObservableCollection<ToDo>>(MyToDos), MessengerTokens.UpdateTile);
                    }
                }
                catch (COMException)
                {
                    await ToastService.SendToastAsync(ResourcesHelper.GetString("RequestError"));
                }
            }
            IsLoading = Visibility.Collapsed;
        }

        /// <summary>
        /// 改变类别
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task ChangeCategory(string id)
        {
            var scheduleToChange = MyToDos.ToList().Find(s =>
              {
                  if (s.ID == id) return true;
                  else return false;
              });
            if (scheduleToChange != null)
            {
                scheduleToChange.Category++;
                if (!App.IsNoNetwork && !App.IsInOfflineMode)
                {
                    await PostHelper.UpdateContent(id, scheduleToChange.Content,"", scheduleToChange.Category);
                }
                await SerializerHelper.SerializerToJson<ObservableCollection<ToDo>>(MyToDos, SerializerFileNames.ToDoFileName);
            }
        }

        /// <summary>
        /// 改变要显示的列表
        /// </summary>
        /// <param name="id"></param>
        private void ChangeDisplayCateList(int id)
        {
            Messenger.Default.Send(new GenericMessage<string>(""), MessengerTokens.CloseHam);

            if (id == -1) return;

            UpdateColor(id);

            if (id != 5) UpdateDisplayList(id);

            IsInSortMode = false;

            switch (id)
            {
                case 0:
                    {
                        Title = ResourcesHelper.GetString("CateDefault");
                    }; break;
                case 1:
                    {
                        Title = ResourcesHelper.GetString("CateWork");
                    }; break;
                case 2:
                    {
                        Title = ResourcesHelper.GetString("CateLife");

                    }; break;
                case 3:
                    {
                        Title = ResourcesHelper.GetString("CateFamily");
                    }; break;
                case 4:
                    {
                        Title = ResourcesHelper.GetString("CateEnter");
                    }; break;
                case 5:
                    {
                        Title = ResourcesHelper.GetString("CateDeleted");
                        SelectedIndex = 1;
                    }; break;
            }
        }

        private void UpdateColor(int id)
        {
            var cateid = id;

            CateColor = App.Current.Resources[Enum.GetName(typeof(CateColors), cateid)] as SolidColorBrush;

            if (Helper.ApiInformationHelper.HasStatusBar())
                StatusBarHelper.SetUpStatusBar(Enum.GetName(typeof(CateColors), cateid));
        }

        private void UpdateDisplayList(int id)
        {
            if (id != 0 && id != 5)
            {
                var newList = from e in MyToDos where e.Category == id select e;
                CurrentDisplayToDos = new ObservableCollection<ToDo>();
                newList.ToList().ForEach(s => CurrentDisplayToDos.Add(s));
            }
            else if (id == 0) CurrentDisplayToDos = MyToDos;
            else CurrentDisplayToDos = DeletedToDos;

            if (CurrentDisplayToDos.Count == 0) ShowNoItems = Visibility.Visible;
        }

        /// <summary>
        /// 从储存反序列化所有数据
        /// </summary>
        /// <returns></returns>
        private async Task RestoreData(bool restoreMainList)
        {
            try
            {
                if (restoreMainList)
                {
                    SelectedCate = 0;
                    MyToDos = await SerializerHelper.DeserializeFromJsonByFileName<ObservableCollection<ToDo>>(SerializerFileNames.ToDoFileName);
                    CurrentDisplayToDos = MyToDos;
                    Messenger.Default.Send(new GenericMessage<ObservableCollection<ToDo>>(MyToDos), MessengerTokens.UpdateTile);
                }
                DeletedToDos = await SerializerHelper.DeserializeFromJsonByFileName<ObservableCollection<ToDo>>(SerializerFileNames.DeletedFileName);
                DeletedToDos.ToList().ForEach(s =>
                {
                    if (s == null) DeletedToDos.Remove(s);
                });

                App.IsSyncListOnce = true;
            }
            catch (Exception ex)
            {
                var task = ExceptionHelper.WriteRecord(ex, nameof(MainViewModel), nameof(RestoreData));
            }
        }

        /// <summary>
        /// 进入 MainPage 会调用
        /// </summary>
        /// <param name="param"></param>
        public async void Activate(object param)
        {
            UpdateColor(this.SelectedCate == -1 ? 0 : this.SelectedCate);
            if (param is LoginMode)
            {
                if (App.IsSyncListOnce) return;
                App.IsSyncListOnce = true;

                var mode = (LoginMode)param;
                //已经登陆过的了
                if (mode != LoginMode.OfflineMode)
                {
                    CurrentUser.Email = LocalSettingHelper.GetValue("email");
                    ShowLoginBtnVisibility = Visibility.Collapsed;
                    ShowAccountInfoVisibility = Visibility.Visible;

                    //没有网络
                    if (App.IsNoNetwork)
                    {
                        await RestoreData(true);
                        await Task.Delay(500);

                        await ToastService.SendToastAsync(ResourcesHelper.GetString("NoNetworkHint"));
                    }
                    //有网络
                    else
                    {
                        await RestoreData(true);

                        SelectedCate = 0;

                        if (mode == LoginMode.OfflineModeToLogin || mode == LoginMode.OfflineModeToRegister) await AddAllToDos();

                        await SyncAllToDos();
                        CurrentDisplayToDos = MyToDos;
                    }
                }
                //处于离线模式
                else if (mode == LoginMode.OfflineMode)
                {
                    ShowLoginBtnVisibility = Visibility.Visible;
                    ShowAccountInfoVisibility = Visibility.Collapsed;
                    var restoreTask = RestoreData(true);
                }
            }
        }

        public void Deactivate(object param)
        {

        }
    }
}
