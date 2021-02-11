using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ERSB.Models;
using ERSB.Modules;
using Microsoft.Win32;
using MessageBox = HandyControl.Controls.MessageBox;

namespace ERSB.Views
{
    /// <summary>
    /// Interaction logic for BrowserControl
    /// </summary>
    public partial class BrowserControl
    {
        private int _totalValidRoll;
        private int _completedDownloads;
        private string _selectedRollFile;
        private List<string> _rollNumbers;
        private int _index;
        private bool _cancelDownload;
        private string BusyText
        {
            set => txtBusyText.Text = value;
        }

        private bool IsBusy
        {
            set
            {
                BusyIndicator1.IsBusy = value;
                WebView.Visibility = value ? Visibility.Hidden : Visibility.Visible;
            }
        }

        private int CompletedDownloads
        {
            get => _completedDownloads;
            set
            {
                _completedDownloads = value;
                if ((value != 0) && value >= _totalValidRoll)
                    StartScrapping();
            }
        }

        private readonly List<DownloadItem> _downloads = new List<DownloadItem>();
        public BrowserControl()
        {
            InitializeComponent();
        }
      
        private void WebView_CoreWebView2Ready(object sender, EventArgs e)
        {
            
            WebView.CoreWebView2.Settings.AreDevToolsEnabled = false;
            WebView.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = false;
            WebView.CoreWebView2.CallDevToolsProtocolMethodAsync("Page.enable", "{}");
            //WebView.CoreWebView2.CallDevToolsProtocolMethodAsync("browser.SetDownloadBehavior", "browser.SetDownloadBehaviorBehaviorAllow");
            //WebView.CoreWebView2.CallDevToolsProtocolMethodAsync("Page.setDownloadBehavior", $"{{allow,{Util.GetDataFolderPath()}}}");
            var downloadProgress = WebView.CoreWebView2.GetDevToolsProtocolEventReceiver("Page.downloadProgress");
            var downloadBegin = WebView.CoreWebView2.GetDevToolsProtocolEventReceiver("Page.downloadWillBegin");
            downloadBegin.DevToolsProtocolEventReceived += async (s, e) =>
            {

                var downloadItem = DownloadItem.FromJson(e.ParameterObjectAsJson);
                _downloads.Add(downloadItem);
                BusyText = "Downloading result of Roll Number: " +
                            $"{_rollNumbers[_index]}...";
                _index++;
                if (_index < _rollNumbers.Count && !_cancelDownload)
                    await ExecuteScript(_rollNumbers[_index]);

                Debug.WriteLine(e.ParameterObjectAsJson);
            };
            downloadProgress.DevToolsProtocolEventReceived += (s, e) =>
            {
                var downloadItem = DownloadItem.FromJson(e.ParameterObjectAsJson);
                if (_downloads.Exists(item => item.Guuid.Equals(downloadItem.Guuid)))
                {
                    _downloads.Find(item => item.Guuid.Equals(downloadItem.Guuid)).UpdateProgress(downloadItem, () => CompletedDownloads++);
                }
                Debug.WriteLine(e.ParameterObjectAsJson);
            };
            WebView.CoreWebView2.ScriptDialogOpening += async (s, e) =>
            {
                //pauseScript =true;
                //var r = e.GetDeferral();

                //if (MessageBox.Error(e.Message,"Invalid Data") == MessageBoxResult.OK) 
                //pauseScript =false; r.Complete(); }
                _totalValidRoll--;
                _index++;
                await ExecuteScript(_rollNumbers[_index]);
                //r.Complete();

                Debug.WriteLine(e.Message);
            };
        }

        private async void BtnScrap_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Messagebox button glitching beause IsBusy set false before calling them
            if (CmbFileNames.SelectedItem is null)
            {
                MessageBox.Warning("Please select a roll number file from the drop down", "No roll number file selected");
                return;
            }
            
            _selectedRollFile = CmbFileNames.SelectedItem.ToString();
            if (!File.Exists(_selectedRollFile.CreateCsvFilePath()))
            {
                MessageBox.Error("File does not exist", "File Error");
                return;
            }
            IsBusy = true;
            CompletedDownloads = 0;
            _downloads.Clear();
            _index = 0;
            if (WebView?.CoreWebView2 == null) return;
            try
            {
                _rollNumbers = new List<string>(ResultExtractor.GetRollNumbersList(_selectedRollFile));
            }
            catch (Exception ex)
            {
                
                MessageBox.Error(ex.Message, "Exception occured");
                IsBusy = false;
                return;
            }
            
            _totalValidRoll = _rollNumbers.Count;
            //foreach (var rollNumber in RollNumbers)
            //{
            //    await ExecuteScript(rollNumber);
            //}
            await ExecuteScript(_rollNumbers[_index]);
            //IsBusy = false;
        }

        private async Task ExecuteScript(string rollNumber)
        {
            var script = "document.querySelector('#txtRollNo').value = '" + rollNumber + @"';";
            _ = await WebView.ExecuteScriptAsync(script);

            script = "document.querySelector('input[type=submit]').click();";
            _ = await WebView.ExecuteScriptAsync(script);
        }

        private async void StartScrapping()
        {
            Debug.WriteLine($"All {CompletedDownloads} completed");
            // IsBusy = true;
            BusyText = "Extracting Data...";
            var myDownloadsPath = NativeMethods.GetDownloadFolder();
            var names = _downloads.Select(item => item.SuggestedFilename);
            IEnumerable<string> nameOfFiles = names as string[] ?? names.ToArray();
            var studentsData = await ResultExtractor.ExtractDataFromPdfsAsync(myDownloadsPath,
                nameOfFiles);
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(() =>
            {
                if (!Directory.Exists(Path.Combine(myDownloadsPath, "ERSB", _selectedRollFile)))
                { Directory.CreateDirectory(Path.Combine(myDownloadsPath, "ERSB", _selectedRollFile)); }
                foreach (var file in nameOfFiles)
                {
                    var oldFile = Path.Combine(myDownloadsPath, file);
                    var newFile = Path.Combine(myDownloadsPath, "ERSB", _selectedRollFile, file);
                    File.Move(oldFile, newFile, true);
                }
            }
            );
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            var saveFileDialog = new SaveFileDialog
            {
                AddExtension = true,
                DefaultExt = "xlsx",
                Filter = "Excel Files (*.xls, *.xlsx)|*.xls;*.xlsx|CSV Files (*.csv)|*.csv"
            };
            if (!(bool)saveFileDialog.ShowDialog()) { IsBusy = false; return; }
            BusyText = "Exporting Data...";
            var fileName = saveFileDialog.FileName;
            await ResultExtractor.ExportToExcel(studentsData, fileName);

            if (MessageBox.Ask("Do you want to open exported file ?" +
                $"\nYou can also find the result pdf(s) in {Path.Combine(myDownloadsPath, "ERSB", _selectedRollFile)} ",
                "Export Success") != MessageBoxResult.OK) { IsBusy = false; return; }
            IsBusy = false;
            if (File.Exists(fileName)) Process.Start(new ProcessStartInfo(fileName) { UseShellExecute = true });
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _cancelDownload =true;
            StartScrapping();
        }
    }
}
