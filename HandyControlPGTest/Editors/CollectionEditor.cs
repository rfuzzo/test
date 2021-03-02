using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using HandyControl.Controls;
using HandyControlPGTest.Model;

namespace HandyControlPGTest.Editors
{
    public class CollectionEditor : PropertyEditorBase
    {
        public override FrameworkElement CreateElement(PropertyItem propertyItem)
        {
            var view = new ListEditorView();
            return view;
        }

        public override DependencyProperty GetDependencyProperty() => ListEditorView.ItemsSourceProperty;
    }

    
}
