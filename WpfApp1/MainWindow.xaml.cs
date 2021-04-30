using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
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
using Catel.Data;
using ReactiveUI;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IViewFor<AppViewModel>
    {
        public AppViewModel ViewModel { get; set; }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (AppViewModel)value;
        }

        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new AppViewModel();
            DataContext = ViewModel;

            this.WhenActivated(disposables =>
            {
                //this.Bind(ViewModel,
                //        viewModel => viewModel.BindingModel,
                //        view => view.TreeView.ItemsSource)
                //    .DisposeWith(disposables);


                this.BindCommand(ViewModel,
                    vm => vm.DbgCommand,
                    v => v.Button
                ).DisposeWith(disposables);
            });
        }
    }
}
