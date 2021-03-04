using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using HandyControl.Controls;
using HandyControlPGTest.Model;
using TextBox = HandyControl.Controls.TextBox;

namespace HandyControlPGTest.Editors
{
    public class StringWrapperEditor : PropertyEditorBase
    {
        // create the UI element
        public override FrameworkElement CreateElement(PropertyItem propertyItem)
        {
            var box = new TextBox
            {
                IsReadOnly = propertyItem.IsReadOnly
            };
            CreateInnerBinding(box);

            return box;
        }

        // bind to the private dependency property 
        public override DependencyProperty GetDependencyProperty() => StringWrapperProperty;

        // bind to this
        public override void CreateBinding(PropertyItem propertyItem, DependencyObject element) =>
            BindingOperations.SetBinding(this, GetDependencyProperty(), new Binding($"{propertyItem.PropertyName}")
            {
                Source = propertyItem.Value,
                Mode = GetBindingMode(propertyItem),
                UpdateSourceTrigger = GetUpdateSourceTrigger(propertyItem),
                Converter = GetConverter(propertyItem)
            });

        // bind the private dependency property to the UI element
        private void CreateInnerBinding(DependencyObject element)
        {
            BindingOperations.SetBinding(
                element,
                System.Windows.Controls.TextBox.TextProperty,
                new Binding("WrappedString")
                {
                    Source = this,
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                });
        }

        #region dependency properties

        private string WrappedString
        {
            get => (string)GetValue(WrappedStringProperty);
            set => SetValue(WrappedStringProperty, value);
        }

        private static readonly DependencyProperty WrappedStringProperty
            = DependencyProperty.Register(
                "WrappedString",
                typeof(string),
                typeof(StringWrapperEditor),
                new FrameworkPropertyMetadata((StringWrapper)null, OnWrappedStringPropertyChanged));


        private StringWrapper StringWrapper
        {
            get => (StringWrapper)GetValue(StringWrapperProperty);
            set => SetValue(StringWrapperProperty, value);
        }

        private static readonly DependencyProperty StringWrapperProperty
            = DependencyProperty.Register(
                "StringWrapper",
                typeof(StringWrapper),
                typeof(StringWrapperEditor),
                new FrameworkPropertyMetadata((StringWrapper)null, OnStringWrapperPropertyChanged));

        #endregion

        #region interaction between the dependency properties

        private static void OnStringWrapperPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is StringWrapperEditor stringWrapperEditor 
                && e.NewValue is StringWrapper wrappedValue 
                && stringWrapperEditor.WrappedString != wrappedValue.Value)
            {
                stringWrapperEditor.WrappedString = wrappedValue.Value;
            }
        }

        private static void OnWrappedStringPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is StringWrapperEditor stringWrapperEditor 
                && e.NewValue is string wrappedValue
                && stringWrapperEditor.StringWrapper.Value != wrappedValue)
            {
                stringWrapperEditor.StringWrapper.Value = wrappedValue;
                stringWrapperEditor.StringWrapper = stringWrapperEditor.StringWrapper;
            }
        }

        #endregion

        public class StringWrapperToStringConverter : IValueConverter
        {
            public object
                Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) =>
                value is StringWrapper cvar ? cvar.Value : "";

            public object ConvertBack(object value, Type targetType, object parameter,
                System.Globalization.CultureInfo culture)
            {
                if (value is string val)
                    return new StringWrapper(val);
                else
                    return new StringWrapper("");
            }
        }
    }
}
