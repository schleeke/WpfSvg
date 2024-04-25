using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WpfSvg.SvgConverter;

namespace WpfSvg.ViewModels {

    internal class SvgImageViewModel : BindableBase {
        private string _filePath;
        private readonly IEventAggregator _events;
        private ConvertedSvgData _convertedSvgData;

        internal SvgImageViewModel(string filepath, IEventAggregator events) {
            Filepath = filepath;
            _events = events;
            CopyXamlToClipboardCommand = new DelegateCommand(CopyXaml);
        }

        internal SvgImageViewModel(ConvertedSvgData data, IEventAggregator events) {
            Filepath = data.Filepath;
            _events = events;
            _convertedSvgData = data;
            CopyXamlToClipboardCommand = new DelegateCommand(CopyXaml);
        }

        public string Filepath { get => _filePath; set => SetProperty(ref _filePath, value, OnFilepathChanged); }
        public string Filename => System.IO.Path.GetFileName(Filepath);        
        public ImageSource PreviewSource => GetImageSource();
        public ImageSource GetImageSource() => SvgData?.ConvertedObj as ImageSource;
        public bool HasXaml { get; } = true;
        public bool HasSvg { get; } = true;
        public string SvgDesignInfo => GetSvgDesignInfo();
        public string GetSvgDesignInfo() {
            if (PreviewSource is DrawingImage di) {
                if (di.Drawing is DrawingGroup dg) {
                    var bounds = dg.ClipGeometry?.Bounds ?? dg.Bounds;
                    return $"{bounds.Width:#.##}x{bounds.Height:#.##}";
                }
            }
            return null;
        }
        public static SvgImageViewModel DesignInstance {
            get {
                var imageSource = new DrawingImage(new GeometryDrawing(Brushes.Black, null, new RectangleGeometry(new Rect(new Size(10, 10)), 1, 1)));
                var data = new ConvertedSvgData { ConvertedObj = imageSource, Filepath = "FilePath", Svg = "<svg/>", Xaml = "<xaml/>" };
                return new SvgImageViewModel(data, null);
            }
        }
        public string Svg => SvgData?.Svg;
        public string Xaml => SvgData?.Xaml;
        public ConvertedSvgData SvgData {
            get {
                if (_convertedSvgData == null) {
                    try {
                        _convertedSvgData = ConverterLogic.ConvertSvg(Filepath, ResultMode.DrawingImage);
                    }
                    catch (Exception) {
                        return null;
                    }
                }
                return _convertedSvgData;
            }
        }
        public ICommand CopyXamlToClipboardCommand { get; }



        private void OnFilepathChanged() {
            RaisePropertyChanged(nameof(Filename));
        }

        private void CopyXaml() {
            var retVal = SvgData?.Xaml;
            if (retVal == null) { return; }
            Clipboard.SetText(retVal);
            _events?.GetEvent<Events.CurrentStatusChangedEvent>().Publish("Copied to clipboard...");
        }

    }

}
