using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MyerList.Helper;
using MyerList.Model;
using MyerListCustomControl;
using MyerListUWP.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace MyerList.ViewModel
{
    public class ToDoViewModel : ViewModelBase
    {

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
                        var task = ExceptionHelper.WriteRecordAsync(ex, nameof(MainViewModel), nameof(ModifyCommand));
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

        public ToDoViewModel()
        {
            EditedToDo = new ToDo();
            NoDeletedItemsVisibility = Visibility.Collapsed;
            AllToDos = new ObservableCollection<ToDo>();
            DeletedToDos = new ObservableCollection<ToDo>();
            StagedToDos = new ObservableCollection<ToDo>();

            CurrentDisplayToDos = AllToDos;

            RegisterMessenger();
        }


        /// <summary>
        /// 更新排序
        /// </summary>
        /// <returns></returns>
        private async Task UpdateOrder()
        {
            var orderStr = ToDo.GetCurrentOrderString(AllToDos);
            await CloudService.SetAllOrder(orderStr);
        }

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
                        await ToastService.SendToastAsync(ResourcesHelper.GetResString("ContentEmpty"));
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
                var task = ExceptionHelper.WriteRecordAsync(ex, nameof(MainViewModel), nameof(AddOrModifyToDo));
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
                    var result = await CloudService.AddSchedule(EditedToDo.Content, "0", EditedToDo.Category.ToString());
                    if (!string.IsNullOrEmpty(result))
                    {
                        ////发送当前的顺序
                        await CloudService.SetAllOrder(ToDo.GetCurrentOrderString(AllToDos));
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
                    var result = await CloudService.DeleteSchedule(todo.ID);
                    await CloudService.SetAllOrder(ToDo.GetCurrentOrderString(AllToDos));
                }

                Messenger.Default.Send(new GenericMessage<ObservableCollection<ToDo>>(AllToDos), MessengerTokens.UpdateTile);

                UpdateUndoneCount();
            }
            catch (Exception e)
            {
                var task = ExceptionHelper.WriteRecordAsync(e, nameof(MainViewModel), nameof(DeleteToDo));
            }
        }

        /// <summary>
        /// 完成待办事项
        /// </summary>
        /// <param name="id">待办事项的ID</param>
        /// <returns></returns>
        private async Task CompleteTodo(ToDo todo)
        {
            try
            {
                var item = todo;
                item.IsDone = !item.IsDone;

                await SerializerHelper.SerializerToJson<ObservableCollection<ToDo>>(AllToDos, "myschedules.sch");

                if (App.CanSendRequest)
                {
                    var result = await CloudService.FinishSchedule(todo.ID, item.IsDone ? "1" : "0");
                    if (result)
                    {
                        await CloudService.SetAllOrder(ToDo.GetCurrentOrderString(AllToDos));
                    }
                }
                Messenger.Default.Send(new GenericMessage<ObservableCollection<ToDo>>(AllToDos), MessengerTokens.UpdateTile);

                UpdateUndoneCount();
            }
            catch (Exception ex)
            {
                var task = ExceptionHelper.WriteRecordAsync(ex, nameof(MainViewModel), nameof(CompleteTodo));
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
                    var resultUpdate = await CloudService.UpdateContent(itemToModify.ID, itemToModify.Content, itemToModify.CreateTime, itemToModify.Category);
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
                    await ToastService.SendToastAsync(ResourcesHelper.GetResString("RequestError"));
                }
            }
            IsLoading = Visibility.Collapsed;
        }

        /// <summary>
        /// 更新要显示的类别
        /// </summary>
        /// <param name="cateID"></param>
        public void UpdateDisplayList(int cateID)
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
    }
}
