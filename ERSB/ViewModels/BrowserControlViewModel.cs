using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using ERSB.Modules;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace ERSB.ViewModels
{
    public class BrowserControlViewModel : BindableBase
    {
        private ObservableCollection<string> _fileNames;
        public ObservableCollection<string> FileNames
        {
            get => _fileNames;
            set => SetProperty(ref _fileNames, value);
        }
        public DelegateCommand<string> StartScrappingCommand { get; set; }
        BrowserControlViewModel(IRegionManager regionManager)
        {
            StartScrappingCommand = new DelegateCommand<string>(StartScrapping);
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send,
                new Action(() =>
                {
                    regionManager.Regions["ContentRegion"].PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == "Context")
                        {
                            FileNames = (ObservableCollection<string>)regionManager.Regions["ContentRegion"].Context;
                        }
                    };
                }));
            LoadFileNames();
        }
        private void StartScrapping(string fileName)
        {
            // List<string> RollNumbers;
            // var filePath = fileName.CreateCsvFilePath();
            // if (!File.Exists(filePath))
            // {
            //     return;
            // }
            //
            // using var reader = new StreamReader(filePath);
            // using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            // try
            // {
            //     var records = csv.GetRecords<RollList>();
            //     RollNumbers = new List<string>(records.Select(record => record.RollNumber));
            // }
            // catch (ArgumentNullException ex)
            // {
            //     MessageBox.Error(ex.Message, "Exception");
            // }
            // csv.Dispose();
        }
        private void LoadFileNames()
        {
            FileNames = new ObservableCollection<string>(Util.GetFileNamesFromFolder(Util.GetDataFolderPath()));
            if (!FileNames.Contains("default"))
                FileNames.Add("default");
        }
        //private void ExecuteScript(List<string> RollNumbers)
        //{
        //    foreach (var i in rollList)
        //    {
        //        var script = "document.querySelector('#txtRollNo').value = '" + i + @"';";
        //        _ = await WebView.ExecuteScriptAsync(script);
        //        script = "document.querySelector('input[type=submit]').click();";
        //        _ = await WebView.ExecuteScriptAsync(script);
        //    }
        //}
    }
}
