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
using MyerListUWP.ViewModel;
using MyerListCustomControl;
using System.Runtime.InteropServices;
using JP.Utils.Helper;
using MyerListUWP.Common;
using MyerList.UC;
using System.Collections.Generic;
using MyerListShared;
using Windows.ApplicationModel.Background;

namespace MyerList.ViewModel
{
    public class MainViewModel : ViewModelBase, INavigable
    {
        public event Action OnCateColorChanged;

        private AddMode _addMode = AddMode.None;
        private int _lastSelectedIndex = -1;

        public Page CurrentMainPage { get; set; }

        public bool CanBeSorted { get; set; } = true;

        private bool _enableItemClick;
        public bool EnableItemClick
        {
            get
            {
                return _enableItemClick;
            }
            set
            {
                if (_enableItemClick != value)
                {
                    _enableItemClick = value;
                    RaisePropertyChanged(() => EnableItemClick);
                }
            }
        }

        private int _undoneCount;
        public int UndoneCount
        {
            get
            {
                return _undoneCount;
            }
            set
            {
                if (_undoneCount != value)
                {
                    _undoneCount = value;
                    RaisePropertyChanged(() => UndoneCount);
                }
            }
        }

        #region 汉堡包/类别/导航
        /// <summary>
        /// 选择了的类别，值表示的只是顺序
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
                    UpdateListByChangingSelectedCate();
                    _lastSelectedIndex = value;
                    if (value == 0) CanBeSorted = true;
                    else CanBeSorted = false;
                }
            }
        }

        private RelayCommand _personalizeCommand;
        public RelayCommand PersonalizeCommand
        {
            get
            {
                if (_personalizeCommand != null) return _personalizeCommand;
                return _personalizeCommand = new RelayCommand(() =>
                  {
                      CateVM.UpdateCateToModify();
                      CateVM.UpdateCatesToAdd();
                      if (DeviceHelper.IsDesktop)
                      {
                          ContentPopupEx cpex = new ContentPopupEx(
                              new CatePersonalizationControl() { Width = 400, Height = 450 });
                          var task = cpex.ShowAsync();
                      }
                      else
                      {
                          Frame frame = Window.Current.Content as Frame;
                          if (frame != null) frame.Navigate(typeof(PersonalizeCatePage));
                      }
                  });
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

        /// <summary>
        /// 0为显示待办事项的，1为显示已删除的
        /// </summary>
        private int _selectedPage;
        public int SelectedPage
        {
            get
            {
                return _selectedPage;
            }
            set
            {
                if (_selectedPage != value)
                {
                    _selectedPage = value;
                    RaisePropertyChanged(() => SelectedPage);

                    switch (value)
                    {
                        case 0:
                            {
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
                     DialogService cdex = new DialogService(ResourcesHelper.GetResString("Notice"), ResourcesHelper.GetResString("SignUpContent"));
                     cdex.LeftButtonContent = ResourcesHelper.GetResString("Register");
                     cdex.RightButtonContent = ResourcesHelper.GetResString("Login");
                     cdex.OnLeftBtnClick += ((s) =>
                       {
                           App.HasSyncedListOnce = false;
                           var rootFrame = Window.Current.Content as Frame;
                           rootFrame.Navigate(typeof(LoginPage), LoginMode.OfflineModeToRegister);
                           cdex.Hide();
                       });
                     cdex.OnRightBtnClick += (() =>
                       {
                           App.HasSyncedListOnce = false;
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
                    ModeTitle = ResourcesHelper.GetResString("AddTitle");
                    EditedToDo = new ToDo();
                    AddingCate = SelectedCate;
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

        /// <summary>
        /// 当前添加类别,数值表示的顺序
        /// </summary>
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
                    if (AddingCate >= 0 && AddingCate < CateVM.Categories.Count)
                    {
                        AddingCateColor = CateVM.Categories[AddingCate].CateColor;
                        AddingCateName = CateVM.Categories[AddingCate].CateName;
                    }
                }
            }
        }

        public SolidColorBrush _addingCateColor;
        public SolidColorBrush AddingCateColor
        {
            get
            {
                return _addingCateColor;
            }
            set
            {
                _addingCateColor = value;
                RaisePropertyChanged(() => AddingCateColor);
            }
        }

        private string _addingCateName;
        public string AddingCateName
        {
            get
            {
                return _addingCateName;
            }
            set
            {
                if (_addingCateName != value)
                {
                    _addingCateName = value;
                    RaisePropertyChanged(() => AddingCateName);
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
                    EditedToDo = new ToDo();
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
        private ToDo _editedToDo;
        public ToDo EditedToDo
        {
            get
            {
                return _editedToDo;
            }
            set
            {
                if (_editedToDo != value)
                {
                    _editedToDo = value;
                    RaisePropertyChanged(() => EditedToDo);
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
        private ObservableCollection<ToDo> _myAllToDos;
        public ObservableCollection<ToDo> AllToDos
        {
            get
            {
                if (_myAllToDos != null)
                {
                    return _myAllToDos;
                }
                return _myAllToDos = new ObservableCollection<ToDo>();
            }
            set
            {
                if (_myAllToDos != value)
                {
                    _myAllToDos = value;
                }
                RaisePropertyChanged(() => AllToDos);
            }
        }

        /// <summary>
        /// 当前的待办事项
        /// </summary>
        private IEnumerable<ToDo> _currentDisplayToDos;
        public IEnumerable<ToDo> CurrentDisplayToDos
        {
            get
            {
                if (_currentDisplayToDos.Count() == 0) ShowNoItems = Visibility.Visible;
                else ShowNoItems = Visibility.Collapsed;
                return _currentDisplayToDos;
            }
            set
            {
                if (_currentDisplayToDos != value)
                {
                    _currentDisplayToDos = value;
                    RaisePropertyChanged(() => CurrentDisplayToDos);
                    UpdateUndoneCount();
                }
            }
        }

        /// <summary>
        ///删除待办事项
        /// </summary>
        private RelayCommand<ToDo> _deleteCommand;
        public RelayCommand<ToDo> DeleteCommand
        {
            get
            {
                if (_deleteCommand != null) return _deleteCommand;
                return _deleteCommand = new RelayCommand<ToDo>(async (todo) =>
                {
                    await DeleteToDo(todo);
                });
            }
        }

        /// <summary>
        /// 完成待办事项
        /// </summary>
        private RelayCommand<ToDo> _checkCommand;
        public RelayCommand<ToDo> CheckCommand
        {
            get
            {
                if (_checkCommand != null) return _checkCommand;
                return _checkCommand = new RelayCommand<ToDo>(async (todo) =>
                {
                    await CompleteTodo(todo);
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
                if (_modifyCommand != null) return _modifyCommand;
                return _modifyCommand = new RelayCommand<ToDo>((todo) =>
                {
                    try
                    {
                        if (!EnableItemClick) return;

                        _addMode = AddMode.Modify;

                        ShowPaneOpen = true;

                        var id = todo.ID;
                        var targetToDo = AllToDos.ToList().Find(sche =>
                        {
                            if (sche.ID == id) return true;
                            else return false;
                        });

                        if (targetToDo == null)
                        {
                            return;
                        }

                        this.EditedToDo.ID = targetToDo.ID;
                        this.EditedToDo.Content = targetToDo.Content;

                        var cateIndex = CateVM.Categories.ToList().FindIndex(s => s.CateColorID == targetToDo.Category);
                        if (cateIndex != -1)
                        {
                            this.AddingCate = cateIndex;
                        }

                        ModeTitle = ResourcesHelper.GetResString("ModifyTitle");
                    }
                    catch (Exception ex)
                    {
                        var task = Logger.LogAsync(ex);
                    }
                });
            }
        }

        /// <summary>
        /// 修改类别 
        /// </summary>
        private RelayCommand<ToDo> _changeCateCommand;
        public RelayCommand<ToDo> ChangeCateCommand
        {
            get
            {
                if (_changeCateCommand != null) return _changeCateCommand;
                return _changeCateCommand = new RelayCommand<ToDo>(async (todo) =>
                 {
                     await ChangeCategory(todo);
                 });
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
        private RelayCommand<ToDo> _redoCommand;
        public RelayCommand<ToDo> RedoCommand
        {
            get
            {
                if (_redoCommand != null) return _redoCommand;
                return _redoCommand = new RelayCommand<ToDo>(async (todo) =>
                {
                    IsLoading = Visibility.Visible;

                    _addMode = AddMode.None;
                    EditedToDo = todo;
                    await AddOrRestoreAndSyncNewToDo(todo.Category);

                    DeletedToDos.Remove(todo);
                    await SerializerHelper.SerializerToJson<ObservableCollection<ToDo>>(DeletedToDos, "deleteditems.sch");

                    IsLoading = Visibility.Collapsed;
                });
            }
        }

        /// <summary>
        /// 永久删除
        /// </summary>
        private RelayCommand<ToDo> _permanentDeleteCommand;
        public RelayCommand<ToDo> PermanentDeleteCommand
        {
            get
            {
                if (_permanentDeleteCommand != null) return _permanentDeleteCommand;
                return _permanentDeleteCommand = new RelayCommand<ToDo>(async (todo) =>
                {
                    var item = todo;
                    DeletedToDos.Remove(item);
                    await SerializerHelper.SerializerToJson<ObservableCollection<ToDo>>(DeletedToDos, SerializerFileNames.DeletedFileName);
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
                     DialogService cdex = new DialogService(ResourcesHelper.GetResString("Notice"), ResourcesHelper.GetResString("DeleteAllConfirm"));
                     cdex.LeftButtonContent = ResourcesHelper.GetResString("Ok");
                     cdex.RightButtonContent = ResourcesHelper.GetResString("Cancel");
                     cdex.OnLeftBtnClick += (async (s) =>
                       {
                           DeletedToDos.Clear();
                           await SerializerHelper.SerializerToJson<ObservableCollection<ToDo>>(DeletedToDos, SerializerFileNames.DeletedFileName);
                           cdex.Hide();
                       });
                     await cdex.ShowAsync();
                 });
            }
        }
        #endregion

        #region 暂存区

        /// <summary>
        /// 已经删除了的待办事项
        /// </summary>
        private ObservableCollection<ToDo> _stagedToDos;
        public ObservableCollection<ToDo> StagedToDos
        {
            get
            {
                if (_stagedToDos != null)
                {
                    return _stagedToDos;
                }
                return _stagedToDos = new ObservableCollection<ToDo>();
            }
            set
            {
                if (_stagedToDos != value)
                {
                    _stagedToDos = value;
                    RaisePropertyChanged(() => StagedToDos);
                }
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

            EnableItemClick = true;

            UndoneCount = 0;

            //初始化
            CateVM = new CategoryViewModel();
            EditedToDo = new ToDo();
            CurrentUser = new MyerListUser();
            AllToDos = new ObservableCollection<ToDo>();
            DeletedToDos = new ObservableCollection<ToDo>();
            StagedToDos = new ObservableCollection<ToDo>();

            CurrentDisplayToDos = AllToDos;

            SelectedCate = AddingCate = -1;

            CateColor = Application.Current.Resources["DefaultColor"] as SolidColorBrush;

            //设置当前页面为 ALL To-Do
            SelectedPage = 0;

            Title = ResourcesHelper.GetResString("CateAll");

            RegisterMessenger();
        }

        private void RegisterMessenger()
        {
            //完成ToDo
            Messenger.Default.Register<GenericMessage<ToDo>>(this, MessengerTokens.CheckToDo, act =>
            {
                CheckCommand.Execute(act.Content);
            });

            //删除To-Do
            Messenger.Default.Register<GenericMessage<ToDo>>(this, MessengerTokens.DeleteToDo, act =>
            {
                DeleteCommand.Execute(act.Content);
            });
            //重新加入已经删除了的
            Messenger.Default.Register<GenericMessage<ToDo>>(this, MessengerTokens.ReAddToDo, act =>
            {
                this.EditedToDo = act.Content;
                OkCommand.Execute(false);
            });
        }

        /// <summary>
        /// 更新排序
        /// </summary>
        /// <returns></returns>
        public async Task UpdateOrderAsync()
        {
            IsLoading = Visibility.Visible;
            await CloudService.UpdateAllOrderAsync(ToDo.GetCurrentOrderString(AllToDos));
            IsLoading = Visibility.Collapsed;
        }

        #region Add,modify,check,delete
        /// <summary>
        /// 添加or修改内容
        /// </summary>
        /// <returns></returns>
        private async Task AddOrModifyToDo()
        {
            try
            {
                if (EditedToDo != null)
                {
                    if (_addMode == AddMode.None) return;

                    if (string.IsNullOrEmpty(EditedToDo.Content))
                    {
                        ToastService.SendToast(ResourcesHelper.GetResString("ContentEmpty"));
                        return;
                    }

                    ShowPaneOpen = false;

                    //显示进度条
                    IsLoading = Visibility.Visible;

                    //添加
                    if (_addMode == AddMode.Add)
                    {
                        await AddOrRestoreAndSyncNewToDo();
                    }
                    //修改
                    else if (_addMode == AddMode.Modify)
                    {
                        await ModifyAndSyncToDo();
                    }
                }
            }
            catch (Exception ex)
            {
                var task = Logger.LogAsync(ex);
            }
        }

        /// <summary>
        /// 添加待办事项
        /// </summary>
        /// <returns></returns>
        private async Task AddOrRestoreAndSyncNewToDo(int? category = null)
        {
            ShowNoItems = Visibility.Collapsed;

            EditedToDo.ID = Guid.NewGuid().ToString();
            if (category == null)
            {
                EditedToDo.Category = CateVM.Categories[AddingCate].CateColorID;
            }

            //0 for insert,1 for add
            if (!AppSettings.Instance.IsAddToBottom)
            {
                AllToDos.Insert(0, EditedToDo);
            }
            else
            {
                AllToDos.Add(EditedToDo);
            }

            //序列化保存
            await SerializerHelper.SerializerToJson<ObservableCollection<ToDo>>(AllToDos, SerializerFileNames.ToDoFileName);

            //更新磁贴
            Messenger.Default.Send(new GenericMessage<ObservableCollection<ToDo>>(AllToDos), MessengerTokens.UpdateTile);

            //离线模式，还没有登录过
            if (App.IsInOfflineMode)
            {
                IsLoading = Visibility.Collapsed;
            }
            //登录过的，但是没有网络
            else if (App.IsNoNetwork && !App.IsInOfflineMode)
            {
                StagedToDos.Add(EditedToDo);
                await SerializerHelper.SerializerToJson<ObservableCollection<ToDo>>(StagedToDos, SerializerFileNames.StageFileName);
            }
            //登录过的，有网络
            else if (App.CanSendRequest)
            {
                try
                {
                    //在线模式
                    //发送请求
                    var result = await CloudService.AddToDoAsync(EditedToDo.Content, "0", EditedToDo.Category.ToString());
                    if (!string.IsNullOrEmpty(result.JsonSrc))
                    {
                        ////发送当前的顺序
                        await CloudService.UpdateAllOrderAsync(ToDo.GetCurrentOrderString(AllToDos));
                    }
                    else
                    {
                        StagedToDos.Add(EditedToDo);
                    }
                }
                catch (Exception)
                {
                    StagedToDos.Add(EditedToDo);
                }
            }
            EditedToDo = new ToDo();
            IsLoading = Visibility.Collapsed;
            UpdateListByChangingSelectedCate();
            UpdateUndoneCount();
        }

        /// <summary>
        /// 删除todo
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        /// 先从列表删除，然后把列表内容都序列化保存，接着：
        /// 1.如果已经登陆的，尝试发送请求；
        /// 2.离线模式，不用管
        private async Task DeleteToDo(ToDo todo)
        {
            IsLoading = Visibility.Visible;

            try
            {
                var item = todo;

                DeletedToDos.Add(item);
                await SerializerHelper.SerializerToJson<ObservableCollection<ToDo>>(DeletedToDos, SerializerFileNames.DeletedFileName);

                AllToDos.Remove(item);

                UpdateDisplayList(CateVM.Categories[SelectedCate].CateColorID);
                await SerializerHelper.SerializerToJson<ObservableCollection<ToDo>>(AllToDos, SerializerFileNames.ToDoFileName);

                //登录过的
                if (App.CanSendRequest)
                {
                    var result = await CloudService.DeleteToDoAsync(todo.ID);
                    await CloudService.UpdateAllOrderAsync(ToDo.GetCurrentOrderString(AllToDos));
                }

                Messenger.Default.Send(new GenericMessage<ObservableCollection<ToDo>>(AllToDos), MessengerTokens.UpdateTile);

                UpdateUndoneCount();
            }
            catch (Exception e)
            {
                var task = Logger.LogAsync(e);
            }
            finally
            {
                IsLoading = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 完成待办事项
        /// </summary>
        /// <param name="id">待办事项的ID</param>
        /// <returns></returns>
        private async Task CompleteTodo(ToDo todo)
        {
            IsLoading = Visibility.Visible;

            try
            {
                var item = todo;
                item.IsDone = !item.IsDone;

                await SerializerHelper.SerializerToJson<ObservableCollection<ToDo>>(AllToDos, "myschedules.sch");

                if (App.CanSendRequest)
                {
                    var result = await CloudService.FinishToDoAsync(todo.ID, item.IsDone ? "1" : "0");
                    if (result)
                    {
                        await CloudService.UpdateAllOrderAsync(ToDo.GetCurrentOrderString(AllToDos));
                    }
                }
                Messenger.Default.Send(new GenericMessage<ObservableCollection<ToDo>>(AllToDos), MessengerTokens.UpdateTile);

                UpdateUndoneCount();
            }
            catch (Exception ex)
            {
                var task = Logger.LogAsync(ex);
            }
            finally
            {
                IsLoading = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 修改待办事项
        /// </summary>
        /// <returns></returns>
        private async Task ModifyAndSyncToDo()
        {
            IsLoading = Visibility.Visible;

            //修改当前列表
            var itemToModify = AllToDos.ToList().Find(sche =>
            {
                if (sche.ID == EditedToDo.ID) return true;
                else return false;
            });

            itemToModify.Content = EditedToDo.Content;
            itemToModify.Category = CateVM.Categories[AddingCate].CateColorID;
            itemToModify.CreateTime = DateTime.Now.ToString();

            UpdateDisplayList(CateVM.Categories[SelectedCate].CateColorID);

            //离线模式
            if (App.IsInOfflineMode)
            {
                await SerializerHelper.SerializerToJson<ObservableCollection<ToDo>>(AllToDos, SerializerFileNames.ToDoFileName);

                EditedToDo = new ToDo();

                Messenger.Default.Send(new GenericMessage<ObservableCollection<ToDo>>(AllToDos), MessengerTokens.UpdateTile);

                return;
            }
            //非离线模式
            else if (App.CanSendRequest)
            {
                try
                {
                    var resultUpdate = await CloudService.UpdateToDoContentAsync(itemToModify.ID, itemToModify.Content, itemToModify.CreateTime, itemToModify.Category);
                    if (resultUpdate)
                    {
                        AllToDos.ToList().Find(sche =>
                        {
                            if (sche.ID == EditedToDo.ID) return true;
                            else return false;
                        }).Content = EditedToDo.Content;

                        EditedToDo = new ToDo();

                        Messenger.Default.Send(new GenericMessage<ObservableCollection<ToDo>>(AllToDos), MessengerTokens.UpdateTile);
                    }
                }
                catch (COMException)
                {
                    ToastService.SendToast(ResourcesHelper.GetResString("RequestError"));
                }
            }
            IsLoading = Visibility.Collapsed;
        }

        #endregion

        #region Display Category
        /// <summary>
        /// 改变类别
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        private async Task ChangeCategory(ToDo todo)
        {
            var item = todo;
            if (item != null)
            {
                var cateID = (from e in CateVM.Categories where e.CateColorID == item.Category select e.CateColorID).FirstOrDefault();
                if (cateID != -1)
                {
                    var index = CateVM.Categories.ToList().FindIndex(s => s.CateColorID == cateID);
                    index++;
                    if (index == CateVM.Categories.Count - 1) index = 0;
                    item.Category = CateVM.Categories[index].CateColorID;
                }
                if (App.CanSendRequest)
                {
                    await CloudService.UpdateToDoContentAsync(todo.ID, item.Content, "", item.Category);
                }
                await SerializerHelper.SerializerToJson<ObservableCollection<ToDo>>(AllToDos, SerializerFileNames.ToDoFileName);
            }
        }

        /// <summary>
        /// 改变要显示的列表
        /// </summary>
        /// <param name="cateID"></param>
        private void UpdateListByChangingSelectedCate()
        {
            Messenger.Default.Send(new GenericMessage<string>(""), MessengerTokens.CloseHam);

            if (SelectedCate == -1) return;

            //0 为保留值，表示所有，-1 为已删除的
            var cateID = 0;
            if (SelectedCate == 0) cateID = 0;
            else if (SelectedCate == CateVM.Categories.Count - 1) cateID = -1;
            else cateID = CateVM.Categories[SelectedCate].CateColorID;

            UpdateColor(cateID);
            UpdateTitle(cateID);

            UpdateDisplayList(cateID);
        }

        /// <summary>
        /// 更新颜色
        /// </summary>
        /// <param name="cateID"></param>
        private void UpdateColor(int cateID)
        {
            if (cateID > 0)
            {
                var cate = CateVM.Categories.ToList().Find(s => s.CateColorID == cateID);
                if (cate != null)
                {
                    CateColor = cate.CateColor;
                }
            }
            else if (cateID == 0)
            {
                CateColor = App.Current.Resources["MyerListBlueLight"] as SolidColorBrush;
            }
            else if (cateID == -1)
            {
                CateColor = App.Current.Resources["DeletedColor"] as SolidColorBrush;
            }
            OnCateColorChanged?.Invoke();
        }

        private void UpdateTitle(int cateID)
        {
            if (cateID > 0)
            {
                var cate = CateVM.Categories.ToList().Find(s => s.CateColorID == cateID);
                if (cate != null)
                {
                    Title = cate.CateName;
                }
            }
            else if (cateID == 0)
            {
                Title = ResourcesHelper.GetResString("CateAll");
            }
            else if (cateID == -1)
            {
                Title = ResourcesHelper.GetResString("CateDelete");
            }
        }

        /// <summary>
        /// 更新要显示的类别
        /// </summary>
        /// <param name="cateID"></param>
        private void UpdateDisplayList(int cateID)
        {
            if (cateID != 0 && cateID != -1)
            {
                var newList = from e in AllToDos where e.Category == cateID select e;
                CurrentDisplayToDos = newList;
                SelectedPage = 0;
            }
            else if (cateID == 0)
            {
                CurrentDisplayToDos = AllToDos;
                SelectedPage = 0;
            }
            else if (cateID == -1)
            {
                CurrentDisplayToDos = DeletedToDos;
                SelectedPage = 1;
            }

            if (CurrentDisplayToDos.Count() == 0) ShowNoItems = Visibility.Visible;

            UpdateUndoneCount();
        }
        #endregion

        #region All ToDos
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
                    ToastService.SendToast(ResourcesHelper.GetResString("NoNetworkHint"));
                    return;
                }
                if (App.IsInOfflineMode)
                {
                    return;
                }
                //加载滚动条
                IsLoading = Visibility.Visible;

                ToastService.SendToast(ResourcesHelper.GetResString("Syncing"));

                var isSyncOK = false;

                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(2);
                timer.Tick += ((sendert, et) =>
                {
                    if (isSyncOK)
                    {
                        IsLoading = Visibility.Collapsed;
                        ToastService.SendToast(ResourcesHelper.GetResString("SyncSuccessfully"));
                    }
                    timer.Stop();
                });
                timer.Start();

                try
                {
                    await ReSendStagedToDos();
                    await CateVM.Refresh(LoginMode.Login);

                    var result = await CloudService.GetMyToDosAsync();
                    if (!string.IsNullOrEmpty(result.JsonSrc))
                    {
                        //获得无序的待办事项
                        var scheduleWithoutOrder = ToDo.ParseJsonToObs(result.JsonSrc);

                        //获得顺序列表
                        var orders = await CloudService.GetMyOrderAsync();

                        //排序
                        AllToDos = ToDo.GetSortedList(scheduleWithoutOrder, orders);

                        UpdateListByChangingSelectedCate();

                        await SerializerHelper.SerializerToJson<ObservableCollection<ToDo>>(AllToDos, SerializerFileNames.ToDoFileName);

                        isSyncOK = true;
                    }
                }
                catch (COMException)
                {
                    ToastService.SendToast(ResourcesHelper.GetResString("RequestError"));
                }
                finally
                {
                    IsLoading = Visibility.Collapsed;
                    if (!timer.IsEnabled)
                    {
                        ToastService.SendToast(ResourcesHelper.GetResString("SyncSuccessfully"));
                    }
                }

                //最后更新动态磁贴
                Messenger.Default.Send(new GenericMessage<ObservableCollection<ToDo>>(AllToDos), MessengerTokens.UpdateTile);
            }
            catch (Exception ex)
            {
                var task = Logger.LogAsync(ex);
            }
        }

        /// <summary>
        /// 从离线模式注册/登录后，同步所有
        /// </summary>
        /// <returns></returns>
        private async Task AddAllOfflineToDos()
        {
            foreach (var sche in AllToDos)
            {
                var result = await CloudService.AddToDoAsync(sche.Content, sche.IsDone ? "1" : "0", SelectedCate.ToString());
            }
        }

        /// <summary>
        /// 从储存反序列化所有数据
        /// </summary>
        /// <returns></returns>
        private async Task RestoreData()
        {
            try
            {
                SelectedCate = 0;

                AllToDos = await SerializerHelper.DeserializeFromJsonByFile<ObservableCollection<ToDo>>(SerializerFileNames.ToDoFileName);
                CurrentDisplayToDos = AllToDos;

                Messenger.Default.Send(new GenericMessage<ObservableCollection<ToDo>>(AllToDos), MessengerTokens.UpdateTile);

                DeletedToDos = await SerializerHelper.DeserializeFromJsonByFile<ObservableCollection<ToDo>>(SerializerFileNames.DeletedFileName);
                DeletedToDos.ToList().ForEach(s =>
                {
                    if (s == null) DeletedToDos.Remove(s);
                });

                StagedToDos = await SerializerHelper.DeserializeFromJsonByFile<ObservableCollection<ToDo>>(SerializerFileNames.StageFileName);

                App.HasSyncedListOnce = true;
            }
            catch (Exception ex)
            {
                var task = Logger.LogAsync(ex);
            }
        }

        /// <summary>
        /// 重新发送暂存区的ToDo
        /// </summary>
        /// <returns></returns>
        private async Task ReSendStagedToDos()
        {
            if (StagedToDos.Count == 0)
            {
                return;
            }

            foreach (var item in StagedToDos)
            {
                var result = await CloudService.AddToDoAsync(item.Content, "0", item.Category.ToString());
            }

            await CloudService.UpdateAllOrderAsync(ToDo.GetCurrentOrderString(AllToDos));
            StagedToDos.Clear();
            await SerializerHelper.SerializerToJson<ObservableCollection<ToDo>>(StagedToDos, SerializerFileNames.StageFileName);
        }

        /// <summary>
        /// 更新显示未完成的数字
        /// </summary>
        private void UpdateUndoneCount()
        {
            var count = (from e in CurrentDisplayToDos where e.IsDone == false select e).Count();
            this.UndoneCount = count;
        }
        #endregion

        public void RefreshCate()
        {
            if (SelectedCate == -1) SelectedCate = 0;
            UpdateDisplayList(CateVM.Categories[SelectedCate].CateColorID);
            // SelectedCate = 0;
        }

        /// <summary>
        /// 初始化，还原列表等
        /// 区分账户的各种状态：离线模式，没有网络，登录了有网络
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private async Task HandleActive(LoginMode mode)
        {
            IsLoading = Visibility.Visible;

            await InitialCate(mode);

            //已经登陆过的了
            if (mode != LoginMode.OfflineMode)
            {
                CurrentUser.Email = LocalSettingHelper.GetValue("email");

                ShowLoginBtnVisibility = Visibility.Collapsed;
                ShowAccountInfoVisibility = Visibility.Visible;

                await RestoreData();

                //没有网络
                if (App.IsNoNetwork)
                {
                    ToastService.SendToast(ResourcesHelper.GetResString("NoNetworkHint"));
                }
                //有网络
                else
                {
                    SelectedCate = 0;

                    //从离线模式注册/登录的
                    if (mode == LoginMode.OfflineModeToLogin || mode == LoginMode.OfflineModeToRegister)
                    {
                        await AddAllOfflineToDos();
                    }

                    await ReSendStagedToDos();

                    await SyncAllToDos();

                    CurrentDisplayToDos = AllToDos;
                }
            }
            //处于离线模式
            else if (mode == LoginMode.OfflineMode)
            {
                ShowLoginBtnVisibility = Visibility.Visible;
                ShowAccountInfoVisibility = Visibility.Collapsed;
                await RestoreData();
            }
            IsLoading = Visibility.Collapsed;
        }

        private async Task InitialCate(LoginMode mode)
        {
            await CateVM.Refresh(mode);
        }

        private bool registered = false;

        private async Task RegisterBackgroundTaskAsync()
        {
            if (registered) return;

            var statusOperation = await BackgroundExecutionManager.RequestAccessAsync();
            if(statusOperation==BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity
                || statusOperation==BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity)
            {
                registered = true;
                // Register the background task.
                // To keep the memory footprint of the background task as low as possible, is has been implemented in a C++ Windows Runtime Component for Windows Phone.  
                // The memory footprint will be higher if written in C# and will cause out of memory exception on low-cost-tier devices which will terminate the background task.
                BackgroundTaskBuilder builder = new BackgroundTaskBuilder();
                builder.Name = "TileUpdaterTask";
                builder.TaskEntryPoint = "BackgroundTasks.TileUpdaterTask";

                // The trigger used in this sample is the TimeZoneChange trigger. This is for illustration purposes.
                // In a real scenario, choose the trigger that meets your needs. 
                // Note: There are two ways to start the background task for testing purposes:
                // 1. Change the time zone setting so that the system time changes - this will cause a TimeZoneChange to fire
                // 2. Find the background task "AppTileUpdater" in the LifecycleEvents drop-down on the main toolbar and tap it.

                builder.SetTrigger(new TimeTrigger(15, false));
                var registration = builder.Register();
            }
        }

        /// <summary>
        /// 进入 MainPage 会调用
        /// </summary>
        /// <param name="param"></param>
        public void Activate(object param)
        {
            var task = RegisterBackgroundTaskAsync();
        }

        public void Deactivate(object param)
        {

        }

        public async void Loaded(object param)
        {
            if (App.HasSyncedListOnce) return;

            if (param is LoginMode)
            {
                App.HasSyncedListOnce = true;

                await HandleActive((LoginMode)param);
            }

            if (APIInfoHelper.HasStatusBar)
            {
                StatusBarHelper.SetUpStatusBar();
            }

            this.CurrentMainPage = (Window.Current.Content as Frame).Content as Page;
        }
    }
}
