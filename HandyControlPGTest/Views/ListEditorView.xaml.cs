using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
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
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using HandyControlPGTest.Editors;
using HandyControlPGTest.Model;

namespace HandyControlPGTest
{
    /// <summary>
    /// Interaction logic for ListEditor.xaml
    /// </summary>
    [TemplatePart(Name = ElementItemsControl, Type = typeof(ItemsControl))]
    public partial class ListEditorView : UserControl
    {
        private const string ElementItemsControl = "PART_ItemsControl";

        public ListEditorView()
        {
            InitializeComponent();

            //_itemsControl = PART_ItemsControl;
            
        }

        public static string Header => "HEADER";

        #region deprecated
        //private ItemsControl _itemsControl;
        //private ICollectionView _dataView;
        //public virtual PropertyResolver PropertyResolver { get; } = new PropertyResolver();
        //private void UpdateItems(object obj)
        //{
        //    if (obj == null || _itemsControl == null) return;

        //    if (obj is IEnumerable enumerable)
        //    {
        //        var pelements = new List<PropertyItem>();
        //        var i = 0;
        //        foreach (var item in enumerable)
        //        {
        //            var defaultEditor = PropertyResolver.CreateDefaultEditor(item.GetType());
        //            if (item is IntWrapper)
        //            {
        //                defaultEditor = new ICvariableNumericEditor();
        //            }

        //            pelements.Add(CreatePropertyItem(item, i.ToString(), defaultEditor));
        //            i++;
        //        }
        //        _dataView = CollectionViewSource.GetDefaultView(pelements
        //            .Do(item => item.InitElement())
        //            .Select(_ => _.EditorElement)
        //            );

        //        _itemsControl.ItemsSource = _dataView;
        //    }
        //}
        //protected virtual PropertyItem CreatePropertyItem(object source, string name, PropertyEditorBase editor)
        //{
        //    var item = new PropertyItem();
        //    item.Category = "Miscellaneous";
        //    item.DisplayName = name;
        //    item.Description = "";
        //    item.IsReadOnly = false;
        //    item.DefaultValue = null;
        //    item.Editor = PropertyResolver.CreateEditor(editor.GetType(), typeof(PropertyEditorBase));
        //    item.Value = source;
        //    item.PropertyName = ".";
        //    item.PropertyType = source.GetType();
        //    item.PropertyTypeName = $"{source.GetType().Namespace}.{source.GetType().Name}";
        //    return item;
        //}
        //public static readonly RoutedEvent SelectedObjectChangedEvent =
        //    EventManager.RegisterRoutedEvent("SelectedObjectChanged", RoutingStrategy.Bubble,
        //        typeof(RoutedPropertyChangedEventHandler<object>), typeof(ListEditorView));
        #endregion



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
            //UpdateItems(newValue);
            //RaiseEvent(new RoutedPropertyChangedEventArgs<object>(oldValue, newValue, SelectedObjectChangedEvent));
            if (newValue is IEnumerable enumerable)
            {
               TreeViewControl.ItemsSource = enumerable;
            }
        }
    }
}
