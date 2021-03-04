using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
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
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using HandyControlPGTest.Editors;
using HandyControlPGTest.Model;

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

        public static string Header => "HEADER";
        
        public object SelectedObject { get; set; }
        public string SelectedObjectName => "HEADER";

        public IEnumerable ItemsSource
        {
            get => (IEnumerable) GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }


        public static readonly DependencyProperty ItemsSourceProperty
            = DependencyProperty.Register(
                "ItemsSource", 
                typeof(IEnumerable), 
                typeof(ListEditorView),
                new FrameworkPropertyMetadata((IEnumerable) null, OnItemsSourceChanged));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ListEditorView uc)
            {
                uc.OnSetItemSourceChanged(e.OldValue, e.NewValue);
            }
        }

        private void OnSetItemSourceChanged(object oldValue, object newValue)
        {
            if (newValue is not IEnumerable enumerable) return;
            switch (enumerable)
            {
                case IEnumerable<IntWrapper> intwrapper:
                    InitNumericEditor(intwrapper);
                    break;
                default:
                    InitDefaultEditor(enumerable);
                    break;
            }
        }

        private void InitNumericEditor(IEnumerable enumerable)
        {
            ContentControl.Template = (ControlTemplate)this.FindResource("NumericEditor");
            ContentControl.DataContext = enumerable;
        }

        private void InitDefaultEditor(IEnumerable enumerable)
        {
            //ContentControl.Template = (ControlTemplate)this.FindResource("DefaultEditor");
            var treeview = new TreeView();
            foreach (var obj in enumerable)
            {
                var templateName = "PropertyGridEditor";

                var treeViewItem = new TreeViewItem
                {
                    Template = (ControlTemplate) this.FindResource(templateName), 
                    DataContext = obj
                };

                treeview.Items.Add(treeViewItem);
            }

            ContentControl.Content = treeview;
        }
    }
}
