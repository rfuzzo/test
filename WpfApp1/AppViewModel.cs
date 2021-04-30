using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Catel.Data;
using DynamicData;
using DynamicData.Alias;
using DynamicData.Binding;
using ReactiveUI;
using Splat;
using WolvenKit.Models;
using WolvenKit.ViewModels.Editor.Basic;
using WolvenManager.App.Services;

namespace WpfApp1
{
    public class AppViewModel : ObservableObject
    {
        private readonly IWatcherService _watcherService;

        private readonly ReadOnlyObservableCollection<FileViewModel> _bindOut;
        public ReadOnlyObservableCollection<FileViewModel> BindingModel => _bindOut;

        private readonly ReadOnlyObservableCollection<NodeViewModel> _employeeViewModels;
        public ReadOnlyObservableCollection<NodeViewModel> EmployeeViewModels => _employeeViewModels;


        public AppViewModel()
        {
            _watcherService = Locator.Current.GetService<IWatcherService>();


            

            DbgCommand =  ReactiveCommand.Create(Execute);

            //_watcherService.BindingModel.ToObservableChangeSet()
            //    .Filter(_ => _.ParentHash == 0)
            //    .Bind(out _bindOut)
            //    .Subscribe();

            bool DefaultPredicate(Node<FileModel, ulong> node) => true;

            _watcherService.Files.Connect()
                .Subscribe(_ =>
            {

            });

            _watcherService.Files
                .Connect()
                .TransformToTree(employee => employee.ParentHash, 
                    Observable.Return((Func<Node<FileModel, ulong>, bool>)DefaultPredicate))
                .Transform(node => new NodeViewModel(node))
                .Bind(out _employeeViewModels)
                //.DisposeMany()
                .Subscribe(_ =>
                {

                });

        }







        public readonly ReactiveCommand<Unit, Unit> DbgCommand;

        private void Execute()
        {
            _watcherService.Dbg();
        }

       
    }
}
