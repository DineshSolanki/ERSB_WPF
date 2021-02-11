using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ERSB.ViewModels
{
    public class AboutBoxViewModel : BindableBase, IDialogAware
    {
        private string _title = "ERSB v1.0";
        private string _logo = "/Resources/ERSB Logo.png";

        private string _description = "An Exam Result Scrapper Exclusively for JNVU," 
            + Environment.NewLine +" Also features a Data Extractor that can view/export data from multiple result pdf(s)"
            + Environment.NewLine + "It automates this task to a greater extent";

        private string _website = "https://aprogrammers.wordpress.com";
        private string _additionalNotes = "Developed By Dinesh Solanki";
        private string _license = "GNU General Public License v3.0";
        private string _version = "v1.0";

        //These properties can also be initialized from Parameters for better re-usability. or From assembly
        public AboutBoxViewModel()
        {
            
        }

        public string Title { get => _title; set => SetProperty(ref _title, value); }
        public string Logo { get => _logo; set => SetProperty(ref _logo, value); }
        public string Description { get => _description; set => SetProperty(ref _description, value); }
        public string AdditionalNotes { get => _additionalNotes; set => SetProperty(ref _additionalNotes, value); }
        public string License { get => _license; set => SetProperty(ref _license, value); }
        public string Website { get => _website; set => SetProperty(ref _website, value); }
        public string Version { get => _version; set => SetProperty(ref _version, value); }
        public DelegateCommand WebsiteClickCommand { get; }

        #region DialogMethods

        public event Action<IDialogResult> RequestClose;

        protected virtual void CloseDialog(string parameter)
        {
            var result = parameter?.ToLower(CultureInfo.InvariantCulture) switch
            {
                "true" => ButtonResult.OK,
                "false" => ButtonResult.Cancel,
                _ => ButtonResult.None
            };

            RaiseRequestClose(new DialogResult(result));
        }

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose?.Invoke(dialogResult);
        }

        public virtual bool CanCloseDialog()
        {
            return true;
        }

        public virtual void OnDialogClosed()
        {
        }

        public virtual void OnDialogOpened(IDialogParameters parameters)
        {
        }

        #endregion DialogMethods
    }
}
