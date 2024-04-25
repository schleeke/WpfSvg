using Prism.Events;
using Prism.Ioc;
using Prism.Regions;
using Prism.Unity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WpfSvg {

    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : PrismApplication {
        protected override Window CreateShell() => Container.Resolve<Views.MainView>();

        protected override void RegisterTypes(IContainerRegistry containerRegistry) {
            containerRegistry.RegisterSingleton<IEventAggregator, EventAggregator>();
            containerRegistry.RegisterForNavigation<Views.DirectoryBrowserView, ViewModels.DirectoryBrowserViewModel>("browser");
            containerRegistry.RegisterForNavigation<Views.DirectoryContentView, ViewModels.DirectoryContentViewModel>("content");
        }
    }
}
