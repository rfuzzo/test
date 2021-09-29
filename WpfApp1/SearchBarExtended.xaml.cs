using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for SearchBarExtended.xaml
    /// </summary>
    public partial class SearchBarExtended : UserControl
    {
        public SearchBarExtended()
        {
            InitializeComponent();

            //this.Popup1.MouseDown += (s, e) => e.Handled = true;
        }

        private void FileSearchBar_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //this.Popup1.Visibility = Visibility.Visible;
            this.Popup1.IsOpen = true;
            //this.Popup1.Focus();
            //SettingsContextMenu.IsOpen = true;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            //Options.UnselectAll();
            //this.Popup1.IsOpen = false;
        }

        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Options.UnselectAll();
            //this.Popup1.IsOpen = false;

        }

        private void SearchBarExtended_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Options.UnselectAll();
            //this.Popup1.IsOpen = false;

        }
    }
}
