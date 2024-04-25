using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;

namespace WpfSvg.Models {

    internal class FileTreeItemModel : BindableBase {
        private FileTreeItemModel _parent;
        private string _name;
        private bool _childrenDiscovered;
        private TreeItemTypeEnum _type;
        private string _path;
        private bool _selected;
        private readonly IEventAggregator _events;

        public bool IsSelected { get { return _selected; } set { SetProperty(ref _selected, value, OnSelectedChanged); } }

        public string Path {
            get { return _path; }
            set { _path = value; }
        }

        internal FileTreeItemModel(IEventAggregator events) {
            _events = events;
            DiscoverChildrenCommand = new DelegateCommand(DiscoverChildren);
        }

        public TreeItemTypeEnum ItemType { get { return _type; } set { _type = value; } }

        public bool ChildrenDiscovered { get { return _childrenDiscovered; } set { SetProperty(ref _childrenDiscovered, value); } }

        public string Name { get { return _name; } set { SetProperty(ref _name, value); } }

        public ICommand DiscoverChildrenCommand { get; }

        public ObservableCollection<FileTreeItemModel> Children { get; private set; } = new ObservableCollection<FileTreeItemModel>();

        public FileTreeItemModel Parent { get { return _parent; } set { SetProperty(ref _parent, value); } }

        private void DiscoverChildren() {
            if (ChildrenDiscovered) { return; }
            if (ItemType == TreeItemTypeEnum.File) { return; }
            var dirInfo = new DirectoryInfo(Path);
            Children.Clear();
            foreach (var dir in dirInfo.GetDirectories()) {
                var newItem = new FileTreeItemModel(_events) { Parent = null, ItemType = TreeItemTypeEnum.Direcory, Name = dir.Name, Path = dir.FullName };
                Children.Add(newItem);
            }
            ChildrenDiscovered = true;
        }

        private void OnSelectedChanged() {
            if (!IsSelected) { return; }
            _events.GetEvent<Events.SelectedTreeItemChangedEvent>().Publish(this);
            try {
                DiscoverChildren();
            }
            catch (Exception) { }
        }

    }
}
