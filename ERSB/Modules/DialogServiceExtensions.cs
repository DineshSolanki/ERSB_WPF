using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERSB.Modules
{
    static class DialogServiceExtensions
    {
        public static void ShowAboutBox(this IDialogService dialogService, Action<IDialogResult> callBack)
        {
            dialogService.ShowDialog("AboutBox", new DialogParameters(), callBack);
        }
    }
}
