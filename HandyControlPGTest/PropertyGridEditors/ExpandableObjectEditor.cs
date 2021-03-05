using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using HandyControl.Controls;
using WolvenKit.Common.Model.Cr2w;
using WolvenKit.Common.Services;

namespace WolvenKit.MVVM.Views.PropertyGridEditors
{
    public class ExpandableObjectEditor : EditorBase<IEditableVariable>, IExpandableObjectEditor
    { 
        private protected override DependencyProperty GetInnerDependencyProperty() => PropertyGrid.SelectedObjectProperty;
        private protected override FrameworkElement CreateInnerElement(PropertyItem propertyItem)
        {
            throw new System.NotImplementedException();
        }

        // creates a treeview
        public override FrameworkElement CreateElement(PropertyItem propertyItem)
        {
            var (tree, pg) = CreateCustomInnerElement(propertyItem);
            CreateInnerBinding(pg);

            return tree;
        }

        private (FrameworkElement, FrameworkElement) CreateCustomInnerElement(PropertyItem propertyItem)
        {
            var tree = new TreeView();
            var treeviewitem = new TreeViewItem();
            var pg = new PropertyGrid()
            {
                PropertyResolver = new MyPropertyResolver()
            };
            treeviewitem.Items.Add(
                pg);
            tree.Items.Add(treeviewitem);
            return (tree, pg);
        }

        // bind the private dependency property to the UI element
        private void CreateInnerBinding(DependencyObject element)
        {
            BindingOperations.SetBinding(
                element,
                GetInnerDependencyProperty(),
                new Binding($"{nameof(Wrapper)}")
                {
                    Source = this,
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                });
        }

        


    }
}
