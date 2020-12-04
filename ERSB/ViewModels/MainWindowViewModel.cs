using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using ERSB.Modules;

namespace ERSB.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public DelegateCommand NavigateDataManagementCommand { get; set; }
        public DelegateCommand NavigateHomeCommand { get; set; }
        public DelegateCommand NavigateDataExtractorCommand { get; }

        private string _title = "ERSB v1.0";

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public MainWindowViewModel(IRegionManager regionManager)
        {
            var region = regionManager;
            NavigateHomeCommand = new DelegateCommand(region.NavigateHome);
            NavigateDataManagementCommand = new DelegateCommand(region.NavigateDataManagement);
            NavigateDataExtractorCommand = new DelegateCommand(region.NavigateDataExtractor);
        }
    }
}