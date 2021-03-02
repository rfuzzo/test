﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using HandyControl.Controls;
using HandyControlPGTest.Editors;

namespace HandyControlPGTest.Model
{
    [Editor(typeof(ExpandableObjectEditor), typeof(PropertyEditorBase))]
    public class DemoModel
    {
        public DemoModel()
        {
            //List = new List<string>();
            //dList = new List<DemoModel>();
            String = "TEST";
        }

        public string String { get; set; }
        public Gender Enum { get; set; }
        public int Integer { get; set; }
        //[Editor(typeof(CollectionEditor), typeof(PropertyEditorBase))]
        //public List<string> List { get; set; }
        //[Editor(typeof(CollectionEditor), typeof(PropertyEditorBase))]
        //public List<DemoModel> dList { get; set; }

        public DemoModel DdModel { get; set; }

        public override string ToString() => String;
    }

    public class IntWrapper
    {
        public int Value { get; set; }

        public IntWrapper(int value)
        {
            Value = value;
        }
    }

    public class StringWrapper
    {
        public StringWrapper(string v)
        {
            Value = v;
        }

        public string Value { get; set; }
    }


    public class PropertyGridDemoModel
    {
        public PropertyGridDemoModel()
        {
            Integer = new IntWrapper(999);

            ListString = new ObservableCollection<DemoModel>();

            ListInt = new ObservableCollection<IntWrapper>();

            for (int i = 0; i < 5; i++)
            {
                ListString.Add(new DemoModel());
            }

            for (int i = 0; i < 5; i++)
            {
                ListInt.Add(new IntWrapper(i* 2));
            }

            //ListDemoModel = new List<DemoModel>();

            DemoModel = new DemoModel()
            {
                Enum = Gender.Female,
                String = "xxxx",
                Integer = 790
            };
        }
        public DemoModel DemoModel { get; set; }

        [Editor(typeof(CollectionEditor), typeof(PropertyEditorBase))]
        public ObservableCollection<DemoModel> ListString { get; set; }
        [Editor(typeof(CollectionEditor), typeof(PropertyEditorBase))]
        public ObservableCollection<IntWrapper> ListInt { get; set; }

        //[Editor(typeof(CollectionEditor), typeof(PropertyEditorBase))]
        //public List<DemoModel> ListDemoModel { get; set; }

        public string String { get; set; }

        public IntWrapper Integer { get; set; }

        public bool Boolean { get; set; }

        public Gender Enum { get; set; }
        public ImageSource ImageSource { get; set; }
    }


    public enum Gender
    {
        Male,
        Female
    }
}
