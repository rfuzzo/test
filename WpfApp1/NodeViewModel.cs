using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DynamicData;
using DynamicData.Binding;
using DynamicData.Kernel;
using ReactiveUI;
using WolvenKit.Models;
using WolvenKit.ViewModels.Editor.Basic;

namespace WpfApp1
{

    public class NodeViewModel : ReactiveObject, IDisposable, IEquatable<NodeViewModel>
    {
        private readonly IDisposable _cleanUp;
        private bool _isExpanded;
        private bool _isSelected;
        private ReadOnlyObservableCollection<NodeViewModel> _children;

        public NodeViewModel(Node<FileModel, ulong> node, NodeViewModel parent = null)
        {
            Id = node.Key;
            Name = node.Item.Name;
            Depth = node.Depth;
            Parent = parent;
            BossId = node.Item.ParentHash;
            Dto = node.Item;

            //Wrap loader for the nested view model inside a lazy so we can control when it is invoked
            var childrenLoader = new Lazy<IDisposable>(() => node.Children.Connect()
                                .Transform(e => new NodeViewModel(e, this))
                                .Bind(out _children)
                                .DisposeMany()
                                .Subscribe());

            //return true when the children should be loaded 
            //(i.e. if current node is a root, otherwise when the parent expands)
            var shouldExpand = node.IsRoot
                ? Observable.Return(true)
                : Parent.Value.WhenValueChanged(This => This.IsExpanded);

            //wire the observable
            var expander = shouldExpand
                    .Where(isExpanded => isExpanded)
                    .Take(1)
                    .Subscribe(_ =>
                    {
                        //force lazy loading
                        var x = childrenLoader.Value;
                    });

            _cleanUp = Disposable.Create(() =>
            {
                expander.Dispose();
                if (childrenLoader.IsValueCreated)
                    childrenLoader.Value.Dispose();
            });
        }

        public ulong Id { get; }

        public string Name { get; }

        public int Depth { get; }

        public ulong BossId { get; }

        public FileModel Dto { get; }

        public Optional<NodeViewModel> Parent { get; }

        public ReadOnlyObservableCollection<NodeViewModel> Children => _children;

        public bool IsExpanded
        {
            get => _isExpanded;
            set => this.RaiseAndSetIfChanged(ref _isExpanded, value);
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => this.RaiseAndSetIfChanged(ref _isSelected, value);
        }

        #region Equality Members

        public bool Equals(NodeViewModel other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((NodeViewModel)obj);
        }

        public override int GetHashCode()
        {
            return (int)Id;
        }

        public static bool operator ==(NodeViewModel left, NodeViewModel right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(NodeViewModel left, NodeViewModel right)
        {
            return !Equals(left, right);
        }

        #endregion

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }



    //public class NodeViewModel : ReactiveObject
    //{
    //    public NodeViewModel(Node<FileModel, ulong> node)
    //    {
    //        _node = node;


    //        node.Children.Connect()
    //            .Transform(_ => new NodeViewModel(_))
    //            .ObserveOn(RxApp.MainThreadScheduler)
    //            .Bind(out _bindingModel)
    //            .Subscribe();

    //    }

    //    private Node<FileModel, ulong> _node;
    //    private ReadOnlyObservableCollection<NodeViewModel> _bindingModel;

    //    public ReadOnlyObservableCollection<NodeViewModel> Children => _bindingModel;

    //    public string Name => _node.Item.Name;

    //    public override string ToString() => $"FileModel - {this.Name}";
    //}
}
