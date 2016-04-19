using GalaSoft.MvvmLight.Messaging;
using MyerList.Model;
using MyerListUWP;
using MyerListUWP.Common;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Linq;
using System.Threading.Tasks;

namespace MyerList.UC
{
    public sealed partial class CateItemControl : UserControl
    {
        private ToDoCategory CurrentCate
        {
            get
            {
                return this.DataContext as ToDoCategory;
            }
        }

        public CateItemControl()
        {
            this.InitializeComponent();
        }

        public void SelectCateClick(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Send(new GenericMessage<int>(CurrentCate.CateColorID), MessengerTokens.ShowPickCatePanel);
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs arg)
        {
            if(CurrentCate!= null)
            {
                App.MainVM.CateVM.CatesToModify.Remove(CurrentCate);
            }
        }

        private void SortUpBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentCate != null)
            {
                var index=App.MainVM.CateVM.CatesToModify.IndexOf(CurrentCate);
                if(index>0)
                {
                    var upIndex = index - 1;
                    var upItem = App.MainVM.CateVM.CatesToModify[upIndex];
                    App.MainVM.CateVM.CatesToModify.RemoveAt(index);
                    App.MainVM.CateVM.CatesToModify.Insert(upIndex, CurrentCate);
                }
            }
        }

        private void SortDownBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentCate != null)
            {
                var index = App.MainVM.CateVM.CatesToModify.IndexOf(CurrentCate);
                if (index < App.MainVM.CateVM.CatesToModify.Count-1)
                {
                    var downIndex = index + 1;
                    var downItem = App.MainVM.CateVM.CatesToModify[downIndex];
                    App.MainVM.CateVM.CatesToModify.RemoveAt(index);
                    App.MainVM.CateVM.CatesToModify.Insert(downIndex, CurrentCate);
                }
            }
        }
    }
}
