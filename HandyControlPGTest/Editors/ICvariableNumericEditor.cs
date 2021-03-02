using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using HandyControl.Controls;
using HandyControlPGTest.Model;

namespace HandyControlPGTest.Editors
{
    public class ICvariableNumericEditor : PropertyEditorBase
    {
        public override FrameworkElement CreateElement(PropertyItem propertyItem) =>
            new NumericUpDown
            {
                IsReadOnly = propertyItem.IsReadOnly
            };

        protected override IValueConverter GetConverter(PropertyItem propertyItem) => new ICVariableNumericToDoubleConverter();

        public override DependencyProperty GetDependencyProperty() => NumericUpDown.ValueProperty;
    }

    //TODO: make this ICVariableNumeric
    public class ICVariableNumericToDoubleConverter : IValueConverter
    {
        public object
            Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) =>
            value is IntWrapper cvar ? cvar.Value : 0;

        // needs to be double to be able to convert from the Numeric Converter
        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (value is double val)
                return new IntWrapper((int) val);
            else
                return new IntWrapper(0);
        }
    }
}
