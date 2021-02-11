using ERSB.ViewModels;
using ERSB.Views;
using GemBox.Pdf;
using GemBox.Spreadsheet;
using HandyControl.Controls;
using HandyControl.Themes;
using Prism.Ioc;
using Prism.Regions;
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
            containerRegistry.RegisterDialog<AboutBox, AboutBoxViewModel>("AboutBox");
        }
        internal static void UpdateSkin(string skin)
        {
            ThemeManager.Current.ApplicationTheme = skin switch
            {
                "Dark" => ApplicationTheme.Dark,
                "Default" => ApplicationTheme.Light,
                _ => ThemeManager.Current.ApplicationTheme
            };
            //ThemeManager.Current.AccentColor = Brushes.Red;
        }
    }
}
