using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using HandyControl.Controls;
using HandyControlPGTest.Editors;
using HandyControlPGTest.Model;

namespace HandyControlPGTest
{
    public class MainViewModel : ObservableObject
    {
        public MainViewModel()
        {
            ModelText = "MODEL";

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

            StringList = new List<string>()
            {
                "AAA",
                "BBB",
                "CCC"
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

        public PropertyGridDemoModel DemoModel { get; set; } 
        public List<DemoModel> ModelList { get; set; }
        public IEnumerable<string> StringList { get; set; }

        public string ModelText
        {
            get => _modelText2;
            set
            {
                if (_modelText2 != value)
                {
                    var oldValue = _modelText2;
                    _modelText2 = value;
                    RaisePropertyChanged(nameof(ModelText));
                }
            }
        }

        private string _modelText2;

    }

    

    
}
