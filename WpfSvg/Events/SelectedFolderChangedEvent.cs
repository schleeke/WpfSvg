using Prism.Events;

namespace WpfSvg.Events {
    internal class SelectedFolderChangedEvent : PubSubEvent<System.IO.DirectoryInfo> { }
}
