using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WolvenKit.Common.Model.Cr2w;
using WolvenKit.Common.Services;

namespace HandyControlPGTest.Model
{
    [Editor(typeof(ITextEditor<string>), typeof(IPropertyEditorBase))]
    public class StringWrapper : IEditableVariable, IEditorBindable<string>
    {
        public string Value { get; set; }

        public StringWrapper(string value)
        {
            Value = value;
        }
    }

    [Editor(typeof(IBoolEditor), typeof(IPropertyEditorBase))]
    public class BoolWrapper : IEditableVariable, IREDBool
    {
        public bool Value { get; set; }

        public BoolWrapper(bool value)
        {
            Value = value;
        }
    }

    [Editor(typeof(ITextEditor<int>), typeof(IPropertyEditorBase))]
    public class IntWrapper : IEditableVariable, IEditorBindable<int>
    {
        public int Value { get; set; }

        public IntWrapper(int value)
        {
            Value = value;
        }
    }

    [Editor(typeof(IEnumEditor), typeof(IPropertyEditorBase))]
    public class EnumWrapper<T> : IEditableVariable, IEnumAccessor<T> where T : Enum
    {
        public T Value{ get; set; }

        public EnumWrapper(T value)
        {
            Value = value;
        }
    }

    public class ListWrapper<T> : IEditableVariable
    {
        public T Value { get; set; }

        public ListWrapper(T value)
        {
            Value = value;
        }
    }

}
