﻿using System;
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

        private readonly ReadOnlyObservableCollection<FileModel> _bindGrid;
        public ReadOnlyObservableCollection<FileModel> BindGrid => _bindGrid;


        public AppViewModel()
        {
            _watcherService = Locator.Current.GetService<IWatcherService>();

            DbgCommand =  ReactiveCommand.Create(Execute);

            _watcherService.Files
                .Connect()
                .Bind(out _bindGrid)
                .Subscribe();

        }

        public readonly ReactiveCommand<Unit, Unit> DbgCommand;

        private void Execute()
        {
            _watcherService.Dbg();
        }

       
    }
}
