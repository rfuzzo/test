using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using HandyControl.Controls;

namespace HandyControlPGTest
{
    public class MainViewModel
    {
        public MainViewModel()
        {
            DemoModel = new PropertyGridDemoModel
            {
                String = "TestString",
                Enum = Gender.Female,
                Boolean = true,
                Integer = 98,
                List2 = new List<DemoModel>() { new()
                {
                    dList = new List<DemoModel>(),
                    List = new List<string>(){"hhh", "jjj", "kkk"},
                    DdModel = new DemoModel(){String = "LLLLLLL"}
                }}
            };

        }

        public PropertyGridDemoModel DemoModel { get; set; } 

    }

    public class PropertyGridEditor : PropertyEditorBase
    {
        public override FrameworkElement CreateElement(PropertyItem propertyItem)
        {
            var view = new PropertyGrid();
           return view;
        }
        public override DependencyProperty GetDependencyProperty() => PropertyGrid.SelectedObjectProperty;
    }

    public class IListPropertyEditor : PropertyEditorBase
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

    public class MyEnumPropertyEditor : PropertyEditorBase
    {
        public override FrameworkElement CreateElement(PropertyItem propertyItem)
        {
            var box = new System.Windows.Controls.ComboBox();
            box.IsEnabled = !propertyItem.IsReadOnly;
            box.ItemsSource = Enum.GetValues(propertyItem.PropertyType);
            return box;
        }

        public override DependencyProperty GetDependencyProperty() => Selector.SelectedValueProperty;
    }

    [Editor(typeof(PropertyGridEditor), typeof(PropertyEditorBase))]
    public class DemoModel
    {
        public DemoModel()
        {
            List = new List<string>();
            dList = new List<DemoModel>();
        }

        public string String { get; set; }
        public Gender Enum { get; set; }
        public int Integer { get; set; }
        [Editor(typeof(IListPropertyEditor), typeof(PropertyEditorBase))]
        public List<string> List { get; set; }
        [Editor(typeof(IListPropertyEditor), typeof(PropertyEditorBase))]
        public List<DemoModel> dList { get; set; }

        public DemoModel DdModel { get; set; }
    }


    public class PropertyGridDemoModel
    {

        public PropertyGridDemoModel()
        {
            List = new List<string>() { "aaa", "bbb" };
            List2 = new List<DemoModel>();

            DemoModel = new DemoModel()
            {
                Enum = Gender.Female,
                String = "xxxx",
                Integer = 790
            };
        }
        public DemoModel DemoModel { get; set; }

        [Editor(typeof(IListPropertyEditor), typeof(PropertyEditorBase))]
        public List<string> List { get; set; }
        
        [Editor(typeof(IListPropertyEditor), typeof(PropertyEditorBase))]
        public List<DemoModel> List2 { get; set; }
        
        public string String { get; set; }

        public int Integer { get; set; }

        public bool Boolean { get; set; }

        [Editor(typeof(MyEnumPropertyEditor),typeof(PropertyEditorBase))]
        public Gender Enum { get; set; }
        public ImageSource ImageSource { get; set; }
    }
    public enum Gender
    {
        Male,
        Female
    }
}
