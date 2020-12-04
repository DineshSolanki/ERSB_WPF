using ERSB.Models;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Linq;
using GemBox.Pdf;
using ERSB.Modules;
using System.Collections.ObjectModel;
using GemBox.Spreadsheet;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.IO;
using HandyControl.Controls;
using System.Diagnostics;

namespace ERSB.ViewModels
{
    public class pdfDataExtractorViewModel : BindableBase
    {
        private string _busyText;
        private bool _isBusy;
        private bool _canExport;
        private bool _canImport;
        private ObservableCollection<Student> _students;
        private string _fileNames;

        public string BusyText { get => _busyText; set => SetProperty(ref _busyText, value); }
        public bool IsBusy { get => _isBusy; set => SetProperty(ref _isBusy, value); }
        public bool CanExport { get => _canExport; set => SetProperty(ref _canExport, value); }
        public bool CanImport { get => _canImport; set => SetProperty(ref _canImport, value); }
        public string FileNames
        {
            get => _fileNames; set
            {
                SetProperty(ref _fileNames, value);
                CanImport = !string.IsNullOrWhiteSpace(value);
            }
        }
        public DelegateCommand<bool?> LoadPdfCommand { get; }
        public DelegateCommand ExportDataCommand { get; }
        public ObservableCollection<Student> Students { get => _students; set => SetProperty(ref _students, value); }
        public pdfDataExtractorViewModel()
        {
            LoadPdfCommand = new DelegateCommand<bool?>(LoadPdf);
            ExportDataCommand = new DelegateCommand(ExportToExcel);
            Students = new ObservableCollection<Student>();
        }

        private async void LoadPdf(bool? useCoordinates)
        {
            BusyText = "Loading...";
            Students.Clear();
            IsBusy = true;
            var collection = new ObservableCollection<Student>();
            await Task.Run(() =>
            {
                if (string.IsNullOrWhiteSpace(FileNames)) return;
                var list = FileNames.Replace("\"", string.Empty,StringComparison.OrdinalIgnoreCase).Split('\n').ToList();
                foreach (var pdfPath in list.Select(i => 
                    i.Replace("\r", string.Empty,StringComparison.Ordinal)
                    .Trim()))
                {
                    using var document = PdfDocument.Load(pdfPath);
                    Student pdfData;
                    try
                    {
                        pdfData = useCoordinates != null && (bool) useCoordinates
                            ? ResultExtractor.GetResultUsingCoordinates(document.Pages[0])
                            : ResultExtractor.GetResultUsingHeaders(document.Pages[0]);
                    }
                    catch (Exception e)
                    {

                        if (e.Message == "Page is empty")
                            pdfData = new Student { Name = "PDF is corrupted" };
                        else
                            throw;
                    }

                    var data = pdfData;
                    System.Windows.Application.Current.Dispatcher.Invoke(() => collection.Add(data));
                }
            });
            Students = collection;
            CanExport = true;
            IsBusy = false;

        }
        private async void ExportToExcel()
        {
            var saveFileDialog = new SaveFileDialog
            {
                AddExtension = true,
                DefaultExt = "xlsx",
                Filter = "Excel Files (*.xls, *.xlsx)|*.xls;*.xlsx|CSV Files (*.csv)|*.csv"
            };
            if (!(bool) saveFileDialog.ShowDialog()) return;
            var fileName = saveFileDialog.FileName;
            BusyText = "Exporting...";
            IsBusy = true;
            await Task.Run(() =>
            {
                var dt = Students.ToDataTable();
                var workbook = new ExcelFile();
                var worksheet = workbook.Worksheets.Add("ERSB");
                worksheet.InsertDataTable(dt, new InsertDataTableOptions { ColumnHeaders = true });
                workbook.DocumentProperties.BuiltIn[BuiltInDocumentProperties.Author] = "ERSB ";
                for (var i = 0; i < worksheet.CalculateMaxUsedColumns(); ++i)
                    worksheet.Columns[i].AutoFit(1, worksheet.Rows[1], worksheet.Rows[^1]);
                workbook.Save(fileName);
            });
            IsBusy = false;
            if (MessageBox.Ask("Do you want to open exported file ?",
                "Export Success") != System.Windows.MessageBoxResult.OK) return;
            if (File.Exists(fileName)) Process.Start(new ProcessStartInfo(fileName) { UseShellExecute = true });


        }
    }
}
