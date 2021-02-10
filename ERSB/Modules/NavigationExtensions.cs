using Prism.Regions;

namespace ERSB.Modules
{
    public static class NavigationExtensions
    {
        public static void NavigateHome(this IRegionManager regionManager)
        {
            regionManager?.RequestNavigate("ContentRegion", "BrowserControl");
        }
        public static void NavigateDataManagement(this IRegionManager regionManager)
        {
            regionManager?.RequestNavigate("ContentRegion", "DataManagement");
        }
        public static void NavigateDataExtractor(this IRegionManager regionManager)
        {
            regionManager?.RequestNavigate("ContentRegion", "pdfDataExtractor");
        }
    }
}
