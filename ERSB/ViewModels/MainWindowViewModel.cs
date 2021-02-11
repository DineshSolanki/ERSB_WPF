using ERSB.Modules;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;

namespace ERSB.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public DelegateCommand NavigateDataManagementCommand { get; set; }
        public DelegateCommand NavigateHomeCommand { get; set; }
        public DelegateCommand ShowAboutBoxCommand { get; }
        public DelegateCommand NavigateDataExtractorCommand { get; }
        private readonly IDialogService _dialogService;
        private string _title = "ERSB v1.0";

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public MainWindowViewModel(IRegionManager regionManager, IDialogService dialogService)
        {
            _dialogService = dialogService;
            var region = regionManager;
            NavigateHomeCommand = new DelegateCommand(region.NavigateHome);
            NavigateDataManagementCommand = new DelegateCommand(region.NavigateDataManagement);
            NavigateDataExtractorCommand = new DelegateCommand(region.NavigateDataExtractor);
            ShowAboutBoxCommand = new DelegateCommand(AboutMethod);
        }
        private void AboutMethod()
        {
            _dialogService.ShowAboutBox(_ => { });
        }
    }
}