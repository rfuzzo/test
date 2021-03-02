using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using HandyControl.Controls;
using HandyControlPGTest.Editors;
using HandyControlPGTest.Model;
using Utilities;

namespace HandyControlPGTest
{
    public class MainViewModel : ObservableObject
    {
        public MainViewModel()
        {

            DemoModel = new PropertyGridDemoModel
            {
                String = "TestString",
                Enum = Gender.Female,
                Boolean = true,
                Integer = new IntWrapper(98),
                //ListDemoModel = new List<DemoModel>() { new()
                //{
                //    dList = new List<DemoModel>(),
                //    List = new List<string>(){"hhh", "jjj", "kkk"},
                //    DdModel = new DemoModel(){String = "LLLLLLL"}
                //}}
            };

            StringList = new ObservableCollection<StringWrapper>()
            {
                new("aaa"),
                new("bbb"),
                new("ccc")

            };

            ListInt = new ObservableCollection<IntWrapper>()
            {
                new(1),
                new(2), 
                new(3),
                
            };

            ModelList = new()
            {
                new DemoModel()
                {
                    String = "AAA"
                },
                new DemoModel()
                {
                    String = "BBB"
                },
                new DemoModel()
                {
                    String = "CCC"
                },
            };
        }

        private void StringListOnItemPropertyChanged(object? sender, ItemPropertyChangedEventArgs e)
        {
            
        }


        public PropertyGridDemoModel DemoModel { get; set; } 
        public List<DemoModel> ModelList { get; set; }


        public ObservableCollection<IntWrapper> ListInt { get; set; }
        public ObservableCollection<StringWrapper> StringList { get; set; }

        
    }

    

    
}
