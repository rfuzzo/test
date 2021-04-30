using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using Catel.MVVM.Views;
using HandyControl.Controls;
using WolvenKit.Common.Model.Cr2w;

namespace WolvenKit.MVVM.Views.PropertyGridEditors
{
    /// <summary>
    /// Abstract base for property grid editorss.
    /// T1 ... Wrapper (e.g. CDouble)
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public abstract class EditorBase<T1> : PropertyEditorBase where T1 : class, IEditorBindable
    {
        private protected abstract DependencyProperty GetInnerDependencyProperty();

        private protected abstract FrameworkElement CreateInnerElement(PropertyItem propertyItem);

        public override FrameworkElement CreateElement(PropertyItem propertyItem)
        {
            var control = CreateInnerElement(propertyItem);
            CreateInnerBinding(control);

            return control;
        }

        #region bindings

        // bind to the private dependency property 
        public override DependencyProperty GetDependencyProperty() => WrapperProperty;

        // bind to this
        public override void CreateBinding(PropertyItem propertyItem, DependencyObject element)
        {
            var b = new Binding($"{propertyItem.PropertyName}");
            b.Source = propertyItem.Value;
            b.Mode = GetBindingMode(propertyItem);
            b.UpdateSourceTrigger = GetUpdateSourceTrigger(propertyItem);
            b.Converter = GetConverter(propertyItem);
            BindingOperations.SetBinding(this, GetDependencyProperty(), b);
        }

        // bind the private dependency property to the UI element
        public virtual void CreateInnerBinding(DependencyObject element)
        {
            BindingOperations.SetBinding(
                element,
                GetInnerDependencyProperty(),
                new Binding($"{nameof(Wrapper)}.Value") //this is guaranteed by IEditorBindable? not really
                {
                    Source = this,
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    NotifyOnSourceUpdated = true,
                });
            if (element is FrameworkElement frameworkElement)
            {
                frameworkElement.SourceUpdated += FrameworkElementOnSourceUpdated;
            }
        }

        private void FrameworkElementOnSourceUpdated(object? sender, DataTransferEventArgs e)
        {
            

        }


        #endregion

        #region dependency properties

        protected T1 Wrapper
        {
            get => (T1)GetValue(WrapperProperty);
            set => SetValue(WrapperProperty, value);
        }

        private static readonly DependencyProperty WrapperProperty
            = DependencyProperty.Register(
                nameof(Wrapper),
                typeof(T1),
                typeof(EditorBase<T1>),
                new FrameworkPropertyMetadata(default(T1), OnWrapperChanged));

        private static void OnWrapperChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            
        }

        #endregion
    }
}