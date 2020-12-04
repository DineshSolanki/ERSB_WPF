using HandyControl.Data;
using HandyControl.Themes;
using HandyControl.Tools;
using System.Windows;
using System;
using Prism.Ioc;
using ERSB.Views;
using Prism.Regions;
using GemBox.Pdf;
using GemBox.Spreadsheet;
using HandyControl.Controls;
using Window = System.Windows.Window;

namespace ERSB
{
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            Container.Resolve<IRegionManager>().RegisterViewWithRegion("ContentRegion", typeof(BrowserControl));
            ComponentInfo.SetLicense("FREE-LIMITED-KEY");
            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
            InstanceHelper.IsSingleInstance();
        }
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<BrowserControl>();
            containerRegistry.RegisterForNavigation<DataManagement>();
            containerRegistry.RegisterForNavigation<pdfDataExtractor>();
        }
        internal void UpdateSkin(SkinType skin)
        {
            SharedResourceDictionary.SharedDictionaries.Clear();
            Resources.MergedDictionaries.Add(ResourceHelper.GetSkin(skin));
            Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/HandyControl;component/Themes/Theme.xaml")
            });
            Current.MainWindow?.OnApplyTemplate();
        }
    }
}
