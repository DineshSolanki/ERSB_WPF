using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ERSB.Models;
using ERSB.Modules;
using GemBox.Pdf;
using GemBox.Spreadsheet;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using MessageBox = HandyControl.Controls.MessageBox;

namespace ERSB.ViewModels
{
    public class pdfDataExtractorViewModel : BindableBase, IFileDragDropTarget
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
            var anyProcessed = false;
            var collection = new ObservableCollection<Student>();
            await Task.Run(() =>
            {
                if (string.IsNullOrWhiteSpace(FileNames)) return;
                var list = FileNames.Replace("\"", string.Empty, StringComparison.OrdinalIgnoreCase).Split('\n').ToList();
                foreach (var pdfPath in list.Select(i =>
                    i.Replace("\r", string.Empty, StringComparison.Ordinal)
                    .Trim()).Where(i => Path.GetExtension(i).ToLower() == ".pdf"))
                {
                    anyProcessed = true;
                    PdfDocument document;
                    try
                    {
                        document = PdfDocument.Load(pdfPath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Warning(ex.Message, "Warning");
                        continue;
                    }
                    Student pdfData;
                    try
                    {
                        pdfData = useCoordinates != null && (bool)useCoordinates
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
                    Application.Current.Dispatcher.Invoke(() => collection.Add(data));
                }
            });
            Students = collection;
            CanExport = anyProcessed;
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
            if (!(bool)saveFileDialog.ShowDialog()) return;
            var fileName = saveFileDialog.FileName;
            BusyText = "Exporting...";
            IsBusy = true;
            await Task.Run(() =>
            {
                using var dt = Students.ToDataTable();
                var workbook = new ExcelFile();
                var worksheet = workbook.Worksheets.Add("ERSB");
                worksheet.InsertDataTable(dt, new InsertDataTableOptions { ColumnHeaders = true });
                foreach (var col in worksheet.Rows[0].AllocatedCells)
                {
                    col.Style.Font.Weight = ExcelFont.BoldWeight;
                }
                workbook.DocumentProperties.BuiltIn[BuiltInDocumentProperties.Author] = "ERSB ";
                workbook.DocumentProperties.BuiltIn[BuiltInDocumentProperties.Application] = "ERSB v1.0";
                for (var i = 0; i < worksheet.CalculateMaxUsedColumns(); ++i)
                    worksheet.Columns[i].AutoFit(1, worksheet.Rows[1], worksheet.Rows[^1]);
                workbook.Save(fileName);
            });
            IsBusy = false;
            if (MessageBox.Ask("Do you want to open exported file ?",
                "Export Success") != MessageBoxResult.OK) return;
            if (File.Exists(fileName)) Process.Start(new ProcessStartInfo(fileName) { UseShellExecute = true });


        }

        public void OnFileDrop(string[] filepaths, string senderName)
        {
            var files = new List<string>();
            var anyPdf = false;
            foreach (var item in filepaths.Where(i => Path.GetExtension(i).ToLower() == ".pdf" || Directory.Exists(i)))
            {
                anyPdf = true;
                var isFile = IsFile(item);
                if (isFile)
                {
                    files.Add(item);
                }
                else
                {
                    files.AddRange(Directory.GetFiles(item).Where(i => Path.GetExtension(i)
                                                                           .ToLower() == ".pdf"));
                }
            }
            if (anyPdf)
            {
                FileNames = files.ToFileNamesString();
            }
        }

        private static bool IsFile(string item)
        {
            return !Directory.Exists(item);
        }

    }
}
