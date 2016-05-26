using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyerListCustomControl
{
    /// <summary>
    /// 可以使用 DoubleAnimation 对 Row 和 Column 做动画的 Grid，
    /// 目前仅满足一个需求，让第二列和第二行能动画
    /// </summary>
    public sealed class AnimatableGrid : Grid
    {
        public double FirstColumnWidth
        {
            get { return (double)GetValue(FirstColumnWidthProperty); }
            set { SetValue(FirstColumnWidthProperty, value); }
        }

        public static readonly DependencyProperty FirstColumnWidthProperty =
            DependencyProperty.Register("FirstColumnWidth", typeof(double), typeof(AnimatableGrid), new PropertyMetadata(0, OnFirstColumnWidthChanged));

        private static void OnFirstColumnWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as AnimatableGrid;
            if (control.ColumnDefinitions.Count != 2)
            {
                throw new ArgumentOutOfRangeException();
            }
            var secondRow = control.ColumnDefinitions[0];
            secondRow.Width = new GridLength((double)e.NewValue);
        }
    }
}
