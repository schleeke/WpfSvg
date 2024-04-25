using Prism.Events;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace WpfSvg.ViewModels {
    internal class DirectoryContentViewModel : BindableBase {
        private readonly IEventAggregator _events;
        private DirectoryInfo _currentDirectory;

        internal DirectoryContentViewModel(IEventAggregator events) {
            _events = events;
            _events.GetEvent<Events.SelectedFolderChangedEvent>().Subscribe(OnDirectoryChanged);
            _events.GetEvent<Events.RequestContentRefreshEvent>().Subscribe(GetContent);
        }

        public ObservableCollection<SvgImageViewModel> Images { get; } = new ObservableCollection<SvgImageViewModel>();

        private void OnDirectoryChanged(DirectoryInfo directory) {
            App.Current.Dispatcher.Invoke(() => { _currentDirectory = directory; });
            GetContent();
        }

        private void GetContent() {
            if (_currentDirectory == null) { return; }
            var files = _currentDirectory.GetFiles("*.svg");
            var fileVMs = files
                .Select(file => new SvgImageViewModel(file.FullName, _events))
                .OrderBy(svg => svg.Filepath)
                .ToList();
            App.Current.Dispatcher.Invoke(() => {
                Images.Clear();
                Images.AddRange(fileVMs);
            });
        }
    }
}
