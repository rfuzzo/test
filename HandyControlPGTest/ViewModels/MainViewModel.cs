using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using Catel.IoC;
using HandyControl.Controls;
using HandyControlPGTest.Model;
using WolvenKit.Common.Services;
using WolvenKit.MVVM.Views.PropertyGridEditors;

namespace HandyControlPGTest
{
    public class MainViewModel : ObservableObject
    {
        public MainViewModel()
        {
            RegisterServices();

            MockEnumSource = Enum.GetValues(typeof(MyEnum));

            DemoModel = new DemoModel();
            PropertyResolver = new MyPropertyResolver();
        }

        private void RegisterServices()
        {
            var serviceLocator = ServiceLocator.Default;

            // Register PropertyEditor services here to the UI
            serviceLocator.RegisterType<ICollectionEditor, REDArrayEditor>();
            serviceLocator.RegisterType<IExpandableObjectEditor, ExpandableObjectEditor>();

            serviceLocator.RegisterType(typeof(ITextEditor<int>), typeof(TextEditor<IntWrapper>));
            serviceLocator.RegisterType(typeof(ITextEditor<string>), typeof(TextEditor<StringWrapper>));


            serviceLocator.RegisterType(typeof(IBoolEditor), typeof(BoolEditor));
            serviceLocator.RegisterType(typeof(IEnumEditor), typeof(EnumEditor));
        }

        public DemoModel DemoModel { get; set; }
        public PropertyResolver PropertyResolver { get; set; }

        public Array MockEnumSource { get; }

    }

    

    
}
