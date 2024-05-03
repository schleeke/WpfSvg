using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Diagnostics;
using System.IO;
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
            CreateIconCommand = new DelegateCommand(CreateIcon);
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
        public ICommand CreateIconCommand { get; }



        private void OnFilepathChanged() {
            RaisePropertyChanged(nameof(Filename));
        }

        private void CopyXaml() {
            var retVal = SvgData?.Xaml;
            if (retVal == null) { return; }
            Clipboard.SetText(retVal);
            _events?.GetEvent<Events.CurrentStatusChangedEvent>().Publish("Copied to clipboard...");
        }

        private void CreateIcon() {
            var locations = ExecuteAndGetOutput("where", "convert.exe").Split(Environment.NewLine.ToCharArray());
            var cmd = "";
            foreach (var item in locations) {
                if (string.IsNullOrEmpty(item)) { continue; }
                if (!item.ToLower().Contains("imagemagick")) { continue; }
                cmd = item;
                break; }
            if (string.IsNullOrEmpty(cmd)) { return; }
            var svgFile = new FileInfo(Filepath);
            var baseName = svgFile.Name.Substring(0, svgFile.Name.Length - svgFile.Extension.Length);
            var icoFile = Path.Combine(svgFile.Directory.FullName, $"{baseName}.ico");
            var args = $"-density 300 -define icon:auto-resize=256,128,96,64,48,32,16 -background none \"{svgFile.FullName}\" \"{icoFile}\"";
            Execute(cmd, args);
        }


        private void Execute(string command, string arguments) {
            var psi = new ProcessStartInfo {
                Arguments = arguments,
                CreateNoWindow = true,
                FileName = command,
            };
            var proc = Process.Start(psi);
            proc.WaitForExit();
        }


        private string ExecuteAndGetOutput(string command, string arguments) {
            var psi = new ProcessStartInfo {
                Arguments = arguments,
                CreateNoWindow = true,
                FileName = command,
                UseShellExecute = false,
                RedirectStandardOutput = true
            };
            var proc = Process.Start(psi);
            proc.WaitForExit();
            var result = proc.StandardOutput.ReadToEnd();
            return result;
        }
    }

}
