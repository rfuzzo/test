using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using WolvenKit.Models;
using WolvenKit.ViewModels.Editor.Basic;
using WpfApp1;

namespace WolvenManager.App.Services
{
    public interface IWatcherService
    {
        public bool IsSuspended { get; set; }

        public IObservableCache<FileModel, ulong> Files { get; }
        
        public void Dbg();
    }
}
