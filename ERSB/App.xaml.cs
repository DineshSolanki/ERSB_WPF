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
using System.Windows.Media;

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
        internal void UpdateSkin(string skin)
        {
            switch (skin)
            {
                case "Dark":
                    ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
                    break;
                case "Default":
                    ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
                    break;
            }
            //ThemeManager.Current.AccentColor = Brushes.Red;
        }
    }
}
