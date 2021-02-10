using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using CsvHelper;
using ERSB.Models;
using ERSB.Modules;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using MessageBox = HandyControl.Controls.MessageBox;

namespace ERSB.ViewModels
{
    public class DataManagementViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private ObservableCollection<string> _classNames;
        private string _newRollNo;
        private string _startRollNo;
        private string _endRollNo;
        private readonly string _dataPath = Util.GetDataFolderPath();
        private ObservableCollection<string> _rollNumbers;
        public ObservableCollection<string> ClassNames { get => _classNames; set { SetProperty(ref _classNames, value); SetRegionContext(); } }
        public string NewRollNo { get => _newRollNo; set => SetProperty(ref _newRollNo, value); }
        public string StartRollNo { get => _startRollNo; set => SetProperty(ref _startRollNo, value); }
        public string EndRollNo { get => _endRollNo; set => SetProperty(ref _endRollNo, value); }
        public ObservableCollection<string> RollNumbers { get => _rollNumbers; set => SetProperty(ref _rollNumbers, value); }
        public DelegateCommand<bool?> AddRollNoCommand { get; set; }
        public DelegateCommand<string> DeleteFileCommand { get; set; }
        public DelegateCommand DeleteAllFilesCommand { get; set; }

        public DelegateCommand<string> SaveFileCommand { get; set; }
        public DelegateCommand<string> OnSelectionChanged { get; set; }
        public DelegateCommand ClearListCommand { get; set; }
        public DelegateCommand<dynamic> KeyPressRollNumbersListCommand { get; set; }
        public DataManagementViewModel(IRegionManager manager)
        {
            _regionManager = manager;
            LoadClassNames();

            ClearListCommand = new DelegateCommand(ClearList);
            AddRollNoCommand = new DelegateCommand<bool?>(AddRollNo);
            DeleteFileCommand = new DelegateCommand<string>(DeleteFile);
            DeleteAllFilesCommand = new DelegateCommand(DeleteAllFiles);
            SaveFileCommand = new DelegateCommand<string>(SaveToFile);
            OnSelectionChanged = new DelegateCommand<string>(LoadContentOfFile);
            RollNumbers = new ObservableCollection<string>();
            KeyPressRollNumbersListCommand = new DelegateCommand<dynamic>(DeleteFromRollNumbers);
            LoadContentOfFile(ClassNames[0]);
        }

        private void DeleteAllFiles()
        {
            var count = 0;
            var cNames = ClassNames.ToList();
            foreach (var fileName in cNames)
            {
                var fullFilePath = fileName.CreateCsvFilePath();
                if (!File.Exists(fullFilePath)) continue;
                File.Delete(fullFilePath);
                ClassNames.Remove(fileName);
                count++;
            }
            MessageBox.Success($"{count} files deleted.", "File Deletion");
        }
        private void DeleteFile(string fileName)
        {
            var fullFilePath = fileName.CreateCsvFilePath();
            if (!File.Exists(fullFilePath))
            {
                MessageBox.Error("File does not exist!", "File Deletion");
                return;
            }
            if (fileName.Equals("default", StringComparison.Ordinal))
            {
                MessageBox.Warning("Default file can't be deleted.", "File Deletion");
                return;
            }
            File.Delete(fullFilePath);
            ClassNames.Remove(fileName);
            MessageBox.Success("File Deleted.", "File Deletion");
        }

        private void ClearList()
        {
            if (MessageBox.Ask("Do you really want to clear the list ?\n" +
                " (the changes will not be made to the file until you save.)",
                "Confirm list clear") == MessageBoxResult.OK)
            {
                RollNumbers.Clear();
            }
        }

        private void LoadContentOfFile(string fileName)
        {
            var filePath = fileName.CreateCsvFilePath();
            if (!File.Exists(filePath))
            {
                return;
            }
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            try
            {
                var records = csv.GetRecords<RollList>();
                RollNumbers = new ObservableCollection<string>(records.Select(record => record.RollNumber));
            }
            catch (ArgumentNullException ex)
            {

                MessageBox.Error(ex.Message, "Exception");
            }

        }

        private void SaveToFile(string fileName)
        {

            if (string.IsNullOrWhiteSpace(fileName))
            {
                MessageBox.Info("Please enter a class name.", "No class name!");
                return;
            }
            if (RollNumbers.Count == 0)
            {
                MessageBox.Info("Please Add Roll Number(s).");
                return;
            }
            var records = new List<RollList>();
            RollNumbers.ToList().ForEach(i =>
                records.Add(new RollList { RollNumber = i })
                );
            using var writer = new StreamWriter(fileName.CreateCsvFilePath());
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            csv.WriteRecords(records);
            LoadClassNames();
            MessageBox.Success("Data successfully saved.", "File save success");
        }
        private void LoadClassNames()
        {
            ClassNames = new ObservableCollection<string>(Util.GetFileNamesFromFolder(_dataPath));
            if (!ClassNames.Contains("default"))
                ClassNames.Add("default");
        }
        private void SetRegionContext()
        {
            _regionManager.Regions["ContentRegion"].Context = ClassNames;
        }
        private void AddRollNo(bool? isAutoGenerate)
        {
            if (isAutoGenerate != null && (bool)isAutoGenerate)
            {
                if (string.IsNullOrWhiteSpace(StartRollNo) || string.IsNullOrWhiteSpace(EndRollNo))
                {
                    MessageBox.Error("Starting or Ending Roll number can't be empty.", "Error!");
                    return;
                }
                var staticStr = Util.GetStaticString(StartRollNo, EndRollNo);
                var endIndex = staticStr.Length == 0 ? 0 : staticStr.Length - 1;
                var firstValue = Convert.ToInt32(StartRollNo.Remove(0, endIndex), CultureInfo.InvariantCulture);
                var secondValue = Convert.ToInt32(EndRollNo.Remove(0, endIndex), CultureInfo.InvariantCulture);
                var outArray = Util.Generate(firstValue, secondValue);
                foreach (var s in outArray)
                {
                    var numberOfZeros = secondValue.ToString(CultureInfo.InvariantCulture).Length - s.Length;
                    string txtToInsert;
                    try
                    {
                        txtToInsert = staticStr + Util.GetZeros(numberOfZeros) + s;
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        txtToInsert = staticStr + s;
                    }
                    if (!RollNumbers.Contains(txtToInsert))
                    {
                        RollNumbers.Add(txtToInsert);
                    }

                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(NewRollNo))
                {
                    MessageBox.Error("Roll Number can't be empty.", "Error");
                    return;
                }
                if (RollNumbers.Contains(NewRollNo))
                {
                    MessageBox.Error("Roll number already exists.", "Duplicate Roll number!");
                    return;
                }
                RollNumbers.Add(NewRollNo);
            }
        }
        private void DeleteFromRollNumbers(dynamic selectedItems)
        {
            var temp = new List<object>(selectedItems);
            foreach (string i in temp)
            {
                RollNumbers.Remove(i);
            }
        }
    }
}
