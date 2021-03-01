using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using HandyControl.Controls;

namespace HandyControlPGTest.Editors
{
    public class CollectionEditor : PropertyEditorBase
    {
        public override FrameworkElement CreateElement(PropertyItem propertyItem)
        {
            var view = new ListView();
            var gridView = new GridView();
            gridView.Columns.Add(new GridViewColumn()
            {
                Header = "Header",
                DisplayMemberBinding = new Binding()
            });
            view.View = gridView;
            return view;
        }

        public override DependencyProperty GetDependencyProperty() => ItemsControl.ItemsSourceProperty;
    }
}
