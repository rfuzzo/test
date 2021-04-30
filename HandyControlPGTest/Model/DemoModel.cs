using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using HandyControl.Controls;
using WolvenKit.Common.Model.Cr2w;
using WolvenKit.MVVM.Views.PropertyGridEditors;

namespace HandyControlPGTest.Model
{
    public class DemoModel : IEditableVariable
    {
        [Category("Simple Wrappers")]
        public BoolWrapper BoolWrapperProp { get; set; }
        [Category("Simple Wrappers")]
        public IntWrapper IntWrapperProp { get; set; }
        [Category("Simple Wrappers")]
        public StringWrapper StringWrapperProp { get; set; }
        [Category("Simple Wrappers")]
        public EnumWrapper<MyEnum> EnumWrapperProp { get; set; }

        [Category("Lists")]
        public ListWrapper<IntWrapper> ListInt { get; set; }
        [Category("Lists")]
        public ListWrapper<StringWrapper> ListString { get; set; }
        [Category("Lists")]
        public ListWrapper<BoolWrapper> ListBool { get; set; }

        [Category("Complex Class")]
        public ComplexClass ComplexClassProp { get; set; }

        public DemoModel()
        {
            BoolWrapperProp = new BoolWrapper(true);
            IntWrapperProp = new IntWrapper();
            StringWrapperProp = new StringWrapper("aaa");
            EnumWrapperProp = new EnumWrapper<MyEnum>(MyEnum.Enumval2);

            ListInt = new ListWrapper<IntWrapper>(new List<IntWrapper>()
            {
                new(123),
                new (234)
            });
            ListString = new ListWrapper<StringWrapper>(new List<StringWrapper>()
            {
                new("AAA"),
                new ("BBB")
            });
            ListBool = new ListWrapper<BoolWrapper>(new List<BoolWrapper>()
            {
                new(true),
                new (false)
            });

            ComplexClassProp = new ComplexClass();
        }

    }

    public class ComplexClass : IEditableVariable
    {
        [Category("Simple Wrappers")]
        public BoolWrapper BoolWrapperProp { get; set; }
        [Category("Simple Wrappers")]
        public IntWrapper IntWrapperProp { get; set; }
        [Category("Simple Wrappers")]
        public StringWrapper StringWrapperProp { get; set; }
        [Category("Simple Wrappers")]
        public EnumWrapper<MyEnum> EnumWrapperProp { get; set; }

        [Category("Complex Class")]
        public ComplexClass2 ComplexClassProp { get; set; }

        public ComplexClass()
        {
            BoolWrapperProp = new BoolWrapper(true);
            IntWrapperProp = new IntWrapper(444);
            StringWrapperProp = new StringWrapper("aaa");
            EnumWrapperProp = new EnumWrapper<MyEnum>(MyEnum.Enumval2);

            ComplexClassProp = new ComplexClass2();
        }
    }
    public class ComplexClass2 : IEditableVariable
    {
        [Category("Simple Wrappers")]
        public BoolWrapper BoolWrapperProp { get; set; }
        [Category("Simple Wrappers")]
        public IntWrapper IntWrapperProp { get; set; }
        [Category("Simple Wrappers")]
        public StringWrapper StringWrapperProp { get; set; }
        [Category("Simple Wrappers")]
        public EnumWrapper<MyEnum> EnumWrapperProp { get; set; }



        public ComplexClass2()
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
