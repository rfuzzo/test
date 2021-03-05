using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using HandyControl.Controls;
using WolvenKit.MVVM.Views.PropertyGridEditors;

namespace HandyControlPGTest.Model
{
    public class DemoModel
    {
        [Category("Simple Wrappers")]
        public BoolWrapper BoolWrapperProp { get; set; }
        //[Category("Simple Wrappers")]
        //public IntWrapper IntWrapperProp { get; set; }
        //[Category("Simple Wrappers")]
        //public StringWrapper StringWrapperProp { get; set; }
        [Category("Simple Wrappers")]
        public EnumWrapper<MyEnum> EnumWrapperProp { get; set; }

        //[Category("Lists")]
        //public List<IntWrapper> ListInt { get; set; }
        //[Category("Lists")]
        //public List<StringWrapper> ListString { get; set; }

        public DemoModel()
        {
            BoolWrapperProp = new BoolWrapper(true);
            //IntWrapperProp = new IntWrapper(444);
            //StringWrapperProp = new StringWrapper("aaa");
            EnumWrapperProp = new EnumWrapper<MyEnum>(MyEnum.Enumval2);

            //ListInt = new List<IntWrapper>()
            //{
            //    new(123),
            //    new (234)
            //};
            //ListString = new List<StringWrapper>()
            //{
            //    new("AAA"),
            //    new ("BBB")
            //};

        }

    }

    public class ComplexClass
    {
        [Category("Simple Wrappers")]
        public BoolWrapper BoolWrapperProp { get; set; }
        [Category("Simple Wrappers")]
        public IntWrapper IntWrapperProp { get; set; }
        [Category("Simple Wrappers")]
        public StringWrapper StringWrapperProp { get; set; }
        [Category("Simple Wrappers")]
        public EnumWrapper<MyEnum> EnumWrapperProp { get; set; }

        public ComplexClass()
        {
            BoolWrapperProp = new BoolWrapper(true);
            IntWrapperProp = new IntWrapper(444);
            StringWrapperProp = new StringWrapper("aaa");
            EnumWrapperProp = new EnumWrapper<MyEnum>(MyEnum.Enumval2);
        }
    }

    public enum MyEnum
    {
        Enumval1,
        Enumval2
    }

}
