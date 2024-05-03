using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using WpfSvg.Models;

namespace WpfSvg.ViewModels {
    internal class DirectoryBrowserViewModel : BindableBase, INavigationAware {
        private DriveModel _selectedDrive;
        private readonly IEventAggregator _events;

        internal DirectoryBrowserViewModel(IEventAggregator events) {
            _events = events;
            _events.GetEvent<Events.SelectedTreeItemChangedEvent>().Subscribe(OnTreeItemSelectionChanged);
            OpenDesktopCommand = new DelegateCommand(OpenDesktop);
            OpenMyDocumentsCommand = new DelegateCommand(OpenMyDocuments);
            OpenDownloadsCommand = new DelegateCommand(OpenDownloads);
        }

        public ICommand OpenDesktopCommand { get; }
        public ICommand OpenMyDocumentsCommand { get; }
        public ICommand OpenDownloadsCommand { get; }

        public bool IsNavigationTarget(NavigationContext navigationContext) => true;

        public void OnNavigatedFrom(NavigationContext navigationContext) { }

        public ObservableCollection<DriveModel> Drives { get; set; } = new ObservableCollection<DriveModel>();

        public ObservableCollection<FileTreeItemModel> FileSystem { get; set; } = new ObservableCollection<FileTreeItemModel>();

        public void OnNavigatedTo(NavigationContext navigationContext) => RefreshDrives();

        public DriveModel SelectedDrive { get { return _selectedDrive; } set { SetProperty(ref _selectedDrive, value, OnSelectedDriveChanged); } }

        private void RefreshDrives() {
            var drives = System.IO.DriveInfo.GetDrives();
            Drives.Clear();
            var drivesToAdd = new List<DriveModel>();
            foreach (var drv in drives) {
                var newDrive = new DriveModel(drv);
                drivesToAdd.Add(newDrive);
            }
            Drives.AddRange(drivesToAdd);
            if (Drives.Count > 0) {
                SelectedDrive = Drives[0];
            }
        }

        private void OnSelectedDriveChanged() {
            FileSystem.Clear();
            if (SelectedDrive == null) {
                _events.GetEvent<Events.SelectedFolderChangedEvent>().Publish(null);
                return;
            }
            _events.GetEvent<Events.SelectedFolderChangedEvent>().Publish(SelectedDrive.RootDirectory);
            foreach (var dir in SelectedDrive.RootDirectory.GetDirectories()) {
                var newItem = new FileTreeItemModel(_events) { Parent = null, ItemType = TreeItemTypeEnum.Direcory, Name = dir.Name, Path = dir.FullName };
                FileSystem.Add(newItem);
            }
        }

        private void OnTreeItemSelectionChanged(FileTreeItemModel model) {
            _events.GetEvent<Events.SelectedFolderChangedEvent>().Publish(new DirectoryInfo(model.Path));

        }

        private void OpenDownloads() {
            var dir = new DirectoryInfo(KnownFolders.GetPath(KnownFolder.Downloads));
            JumpToDirectoryItem(dir);
            _events.GetEvent<Events.SelectedFolderChangedEvent>().Publish(dir);
        }


        private void JumpToDirectoryItem(DirectoryInfo dir) {
            if (!dir.FullName.StartsWith("\\\\")) {

                foreach (var drv in Drives) {
                    if (drv.RootDirectory.FullName.Equals(dir.Root.FullName) == false) { continue; }
                    SelectedDrive = drv;
                    break;
                }

                var pathParts = dir.FullName.Split('\\');
                FileTreeItemModel selectedNode = null;
                for (var i = 1; i < pathParts.Length; i++) {
                    var dirName = pathParts[i];
                    if (i == 1) {
                        foreach (var dirItem in FileSystem) {
                            if (dirItem.Name.Equals(dirName, StringComparison.InvariantCultureIgnoreCase)) {
                                selectedNode = dirItem;
                                selectedNode.DiscoverChildren();
                                selectedNode.IsExpanded = true;
                                continue;
                            }
                        }
                    }
                    else {
                        foreach (var dirItem in selectedNode.Children) {
                            if (dirItem.Name.Equals(dirName, StringComparison.InvariantCultureIgnoreCase)) {
                                selectedNode = dirItem;
                                selectedNode.DiscoverChildren();
                                selectedNode.IsExpanded = true;
                                continue;
                            }
                        }
                    }
                }
                if (selectedNode != null) { selectedNode.IsSelected = true; }
            }
        }

        private void OpenMyDocuments() {
            var dir = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            JumpToDirectoryItem(dir);
            _events.GetEvent<Events.SelectedFolderChangedEvent>().Publish(dir);
        }

        private void OpenDesktop() {
            var dir = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            JumpToDirectoryItem(dir);
            _events.GetEvent<Events.SelectedFolderChangedEvent>().Publish(dir);
        }

    }
}
