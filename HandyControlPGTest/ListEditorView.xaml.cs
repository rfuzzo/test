using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GalaSoft.MvvmLight.Command;
using HandyControl.Tools.Extension;

namespace HandyControlPGTest
{
    /// <summary>
    /// Interaction logic for ListEditor.xaml
    /// </summary>
    public partial class ListEditorView : UserControl
    {
        public ListEditorView()
        {
            InitializeComponent();
        }

        public IEnumerable ItemsSource
        {
            get => (IEnumerable) GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static string Header => "HEADER";

        public static readonly DependencyProperty ItemsSourceProperty
            = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(ListEditorView),
                new FrameworkPropertyMetadata((IEnumerable) null,
                    new PropertyChangedCallback( OnItemsSourceChanged)));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ListEditorView UserControl1Control = d as ListEditorView;
            UserControl1Control.OnSetItemSourceChanged(e);
        }

        private void OnSetItemSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            //TreeView.ItemsSource = e.NewValue as IEnumerable;
            if (e.NewValue is IEnumerable newValue)
            {
                TreeView.ItemsSource = newValue;
            }
        }

    }
}
