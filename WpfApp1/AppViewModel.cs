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

        //private readonly ReadOnlyObservableCollection<FileViewModel> _bindOut;
        //public ReadOnlyObservableCollection<FileViewModel> BindOut => _bindOut;

        //private readonly ReadOnlyObservableCollection<NodeViewModel> _bindOut2;
        //public ReadOnlyObservableCollection<NodeViewModel> BindOut2 => _bindOut2;

        //
        private readonly ReadOnlyObservableCollection<FileModel> _bindGrid;
        public ReadOnlyObservableCollection<FileModel> BindGrid => _bindGrid;


        public AppViewModel()
        {
            _watcherService = Locator.Current.GetService<IWatcherService>();




            DbgCommand =  ReactiveCommand.Create(Execute);

            //_watcherService.BindingModel.ToObservableChangeSet()
                
            //    .Filter(_ => _.ParentHash == 0)
            //    .Bind(out _bindOut)
            //    .Subscribe();

            _watcherService.Files
                .Connect()
                .Bind(out _bindGrid)
                .Subscribe();

            //bool DefaultPredicate(Node<FileModel, ulong> node) => true; ;

            //_watcherService.Files.Connect()
            //    .ForEachChange(_ =>
            //    {

            //    })
            //    .TransformToTree(x => x.ParentHash)
            //    .ForEachChange(_ =>
            //    {

            //    })
            //    .ObserveOnDispatcher()
            //    .Transform(node => new NodeViewModel(node))
            //    .Bind(out _bindOut2)
            //        //.DisposeMany()
            //        .Subscribe(_ =>
            //        {

            //        });

            //_watcherService.Files
            //    .Connect()
            //    .TransformToTree(x => x.ParentHash)
            //    .Transform(node => new NodeViewModel(node))
            //    .ObserveOnDispatcher()
            //    .Bind(out _bindOut2)
            //    //.DisposeMany()
            //    .Subscribe(_ =>
            //    {

            //    });

        }







        public readonly ReactiveCommand<Unit, Unit> DbgCommand;

        private void Execute()
        {
            _watcherService.Dbg();
        }

       
    }
}
