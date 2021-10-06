using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore;
using RedDatabase.Model;
using RedDatabase.ViewModel;

namespace RedDatabase
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private MainViewModel ViewModel => DataContext is MainViewModel vm ? vm : throw new ArgumentNullException();

        //private void Reload_OnClick(object sender, RoutedEventArgs e)
        //{
        //    ViewModel.Reload();
        //}

        private void QueryButton_OnClick(object sender, RoutedEventArgs e) => /*ViewModel.*/Query(this.QueryTextBox.Text);

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            if (ViewModel?.SelectedItem != null)
            {
                /*ViewModel.*/
                QueryUsingFiles(ViewModel.SelectedItem.RedFileId);
            }

        }

        public void Query(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            try
            {
                using var db = new RedDBContext();
                var result = db.Files.FromSqlRaw(text);
                SetItemSource(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
            }
        }

        public void QueryUsingFiles(ulong hashId)
        {
            try
            {
                using var db = new RedDBContext();
                var result = db.Files
                    .Where(x => x.Uses.Any(_ => _.RedFileId == hashId));
                SetItemSource(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
            }
        }

        private void SetItemSource(IQueryable<RedFile> redFiles, bool enqueue = true)
        {
            var list = redFiles.ToList();

            
            this.DataGrid.ItemsSource = list;
            //ItemsSource = list;
            //OnPropertyChanged(nameof(ItemsSource));
            //if (enqueue)
            //{
            //    var enumerable = list.Select(x => x.HashId).ToList();
            //    if (enumerable.Any())
            //    {
            //        //_backwardStack.Push(enumerable);
            //    }

            //    //_forwardStack.Clear();
            //}
        }

        private void BackButton_OnClick(object sender, RoutedEventArgs e) => ViewModel.Back();

        private void ForwardButton_OnClick(object sender, RoutedEventArgs e) => ViewModel.Forward();
    }
}
