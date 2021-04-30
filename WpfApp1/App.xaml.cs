using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ReactiveUI;
using Splat;
using WolvenManager.App.Services;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NDM4Njc4QDMxMzkyZTMxMmUzMG5SV05xYWpUK3lBc3RKZE0vNnJsK09qYmx6YWppRmlFeEZlcjcwSnF2L0E9");


            Locator.CurrentMutable.RegisterConstant(new WatcherService(), typeof(IWatcherService));
            Locator.CurrentMutable.Register(() => new MainWindow(), typeof(IViewFor<AppViewModel>));

        }




    }
}
