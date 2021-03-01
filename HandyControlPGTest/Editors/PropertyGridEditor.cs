using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using HandyControl.Controls;

namespace HandyControlPGTest.Editors
{
    public class PropertyGridEditor : PropertyEditorBase
    {
        public override FrameworkElement CreateElement(PropertyItem propertyItem)
        {
            var view = new PropertyGrid();
            return view;
        }
        public override DependencyProperty GetDependencyProperty() => PropertyGrid.SelectedObjectProperty;
    }
}
