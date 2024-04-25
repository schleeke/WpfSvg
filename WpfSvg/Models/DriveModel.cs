namespace WpfSvg.Models {
    internal class DriveModel {

        internal DriveModel(System.IO.DriveInfo drive) {
            DriveType = drive.DriveType;
            RootDirectory = drive.RootDirectory;
            Name = drive.Name;
            Label = drive.VolumeLabel;
        }

        public System.IO.DriveType DriveType { get; }
        public System.IO.DirectoryInfo RootDirectory { get; }
        public string Name { get; }
        public string Label { get; }
        public override string ToString() => $"{Name} [{Label}]";

    }
}
