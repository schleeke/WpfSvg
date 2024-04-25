using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfSvg.ViewModels {
    internal class MainViewModel : BindableBase {
        private string _title = "Transform SVG -> XAML";
        private readonly IEventAggregator _events;
        private string _currentStatus = "Ready.";
        private readonly IRegionManager _regions;

        public MainViewModel(IRegionManager regions, IEventAggregator events) {
            _regions = regions;
            _events = events;
            OnLoadedCommand = new DelegateCommand(OnLoaded);
            _regions.RegisterViewWithRegion<Views.DirectoryContentView>("ContentRegion");
            _events.GetEvent<Events.CurrentStatusChangedEvent>().Subscribe(OnCurrentStatusChanged);
        }


        public ICommand OnLoadedCommand { get; }

        public string CurrentStatus { get { return _currentStatus; } set { SetProperty(ref _currentStatus, value); } }

        public string Title { get { return _title; } set { SetProperty(ref _title, value); } }

        private void OnLoaded() => _regions.RequestNavigate("BrowserRegion", "browser");

        private void OnCurrentStatusChanged(string status) {
            System.Windows.Application.Current.Dispatcher.Invoke(() => {CurrentStatus = status; });
            Task.Run(() => {
                Thread.Sleep(5000);
                System.Windows.Application.Current.Dispatcher.Invoke(() => { CurrentStatus = "Ready."; });
            });
        }
    }
}
