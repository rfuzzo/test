﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using WolvenKit.Common.Model.Cr2w;
using WolvenKit.Common.Services;

namespace HandyControlPGTest.Model
{
    [Editor(typeof(ITextEditor<string>), typeof(IPropertyEditorBase))]
    public class StringWrapper : ObservableObject, IREDStringType, IEditorBindable<string>
    {
        //public string Value { get; set; }

        public string Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    var oldValue = _value;
                    _value = value;
                    OnPropertyChanged(nameof(Value));
                }
            }
        }

        private string _value;

        private void OnTargetUpdated(Object sender, DataTransferEventArgs args)
        {

        }

        public StringWrapper(string value)
        {
            Value = value;
        }
    }

    [Editor(typeof(IBoolEditor), typeof(IPropertyEditorBase))]
    public class BoolWrapper : IREDBool
    {
        public bool Value { get; set; }

        public BoolWrapper(bool value)
        {
            Value = value;
        }
    }

    [Editor(typeof(ITextEditor<int>), typeof(IPropertyEditorBase))]
    public class IntWrapper : IREDIntegerType, IEditorBindable<int>
    {
        public int Value { get; set; }

        public IntWrapper()
        {

        }

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

    [Editor(typeof(ICollectionEditor), typeof(IPropertyEditorBase))]
    public class ListWrapper<T> : IArrayAccessor
    {


        public IEnumerable<T> Value { get; set; }
        

        public ListWrapper(IEnumerable<T> value)
        {
            Value = value;
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable) Value).GetEnumerator();
        }
    }

}