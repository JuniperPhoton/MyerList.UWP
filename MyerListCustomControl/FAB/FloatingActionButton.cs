using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyerListCustomControl
{
    public class FloatingActionButton: ContentControl
    {
        public ICommand PrimaryButtonCommand
        {
            get { return (ICommand)GetValue(PrimaryButtonCommandProperty); }
            set { SetValue(PrimaryButtonCommandProperty, value); }
        }

        public static readonly DependencyProperty PrimaryButtonCommandProperty =
            DependencyProperty.Register("PrimaryButtonCommand", typeof(ICommand), typeof(FloatingActionButton), new PropertyMetadata(null));

        private Button _primaryButton;

        private TaskCompletionSource<int> _tcs;

        public FloatingActionButton()
        {
            DefaultStyleKey = typeof(FloatingActionButton);
            _tcs = new TaskCompletionSource<int>();
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _primaryButton = GetTemplateChild("PrimaryButton") as Button;
            _primaryButton.Click += ((sender, e) =>
              {
                  PrimaryButtonCommand?.Execute(null);
              });
            _tcs.SetResult(0);
        }
    }
}
