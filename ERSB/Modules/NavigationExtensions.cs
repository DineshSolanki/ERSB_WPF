using Prism.Regions;

namespace ERSB.Modules
{
    public static class NavigationExtensions
    {
        public static void NavigateHome(this IRegionManager regionManager)
        {
            if (regionManager != null) regionManager.RequestNavigate("ContentRegion", "BrowserControl");
        }
        public static void NavigateDataManagement(this IRegionManager regionManager)
        {
            if (regionManager != null) regionManager.RequestNavigate("ContentRegion", "DataManagement");
        }
        public static void NavigateDataExtractor(this IRegionManager regionManager)
        {
            if (regionManager != null) regionManager.RequestNavigate("ContentRegion", "pdfDataExtractor");
        }
    }
}
