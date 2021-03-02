using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using HandyControl.Controls;

namespace HandyControlPGTest.Editors
{
    public class ExpandableObjectEditor : PropertyEditorBase
    {
        public override FrameworkElement CreateElement(PropertyItem propertyItem)
        {
            var tree = new TreeView();
            var treeviewitem = new TreeViewItem();
            treeviewitem.Items.Add(new PropertyGrid());
            tree.Items.Add(treeviewitem);
            return tree;
        }
        public override DependencyProperty GetDependencyProperty() => PropertyGrid.SelectedObjectProperty;
    }
}
