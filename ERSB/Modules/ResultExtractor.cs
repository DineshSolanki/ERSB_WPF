using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CsvHelper;
using ERSB.Models;
using GemBox.Pdf;
using GemBox.Pdf.Content;
using GemBox.Spreadsheet;

namespace ERSB.Modules
{
    internal static class ResultExtractor
    {
        public static Student GetResultUsingHeaders(PdfPage page)
        {
            var pdfTextContents = page.Content.Elements.All()
                .Where(element => element.ElementType == PdfContentElementType.Text)
                .Select(element => element.ToString())
                .ToList();
            if (pdfTextContents.Count == 0)
            {
                throw new InvalidOperationException("Page is empty");
            }

            var student = new Student
            {
                Name = pdfTextContents[pdfTextContents.IndexOf("CANDIDATE'S NAME") + 2],
                FatherName = pdfTextContents[pdfTextContents.IndexOf("FATHER'S NAME") + 2],
                MotherName = pdfTextContents[pdfTextContents.IndexOf("MOTHER'S NAME") + 2],
                RollNo = pdfTextContents[pdfTextContents.IndexOf("ROLL No.") + 2],
                EnrollmentNo = pdfTextContents[pdfTextContents.IndexOf("ENROLLMENT No.") + 2],
                Sgpa = pdfTextContents[pdfTextContents.LastIndexOf("SGPA") + 1],
                Result = pdfTextContents[pdfTextContents.IndexOf("RESULT : ") + 1],
                ResultDate = pdfTextContents[pdfTextContents.IndexOf("RESULT DECLARE DATE : ") + 1]
            };
            if (pdfTextContents[pdfTextContents.IndexOf(student.Sgpa) + 1]!
                .Contains("*", StringComparison.InvariantCultureIgnoreCase))
                student.Sgpa = $"{student.Sgpa}*";
            student.Cgpa = student.Sgpa!.Contains("*", StringComparison.InvariantCultureIgnoreCase)
                ? ""
                : pdfTextContents[pdfTextContents.IndexOf("CGPA") + 1];
            if (student.EnrollmentNo == "MOTHER'S NAME")
                student.EnrollmentNo = string.Empty;
            if (student.Cgpa == "RESULT : ")
                student.Cgpa = string.Empty;
            return student;
        }

        public static Student GetResultUsingCoordinates(PdfPage page)
        {
            var pdfTextContents = page.Content.Elements.All()
                .Where(element => element.ElementType == PdfContentElementType.Text)
                .Cast<PdfTextContent>()
                .ToList();

            var student = new Student
            {
                Name = pdfTextContents.FirstOrDefault(element =>
                    element.Location == PdfCoordinates.Name).ToSafeString(),
                FatherName = pdfTextContents.FirstOrDefault(element =>
                    element.Location == PdfCoordinates.FatherName).ToSafeString(),
                MotherName = pdfTextContents.FirstOrDefault(element =>
                    element.Location == PdfCoordinates.MotherName).ToSafeString(),
                EnrollmentNo = pdfTextContents.FirstOrDefault(element =>
                    element.Location == PdfCoordinates.ErNo).ToSafeString(),
                RollNo = pdfTextContents.FirstOrDefault(element =>
                    element.Location == PdfCoordinates.RollNumber).ToSafeString(),
                Result = pdfTextContents.FirstOrDefault(element =>
                    element.Location == PdfCoordinates.Result).ToSafeString(),
                ResultDate = pdfTextContents.FirstOrDefault(element =>
                    element.Location == PdfCoordinates.ResultDate).ToSafeString(),
                Sgpa = pdfTextContents.FirstOrDefault(element =>
                    element.Location == PdfCoordinates.Sgpa).ToSafeString(),
                Cgpa = pdfTextContents.FirstOrDefault(element =>
                    element.Location == PdfCoordinates.Cgpa).ToSafeString()
            };
            if (string.IsNullOrEmpty(student.Sgpa))
                student.Sgpa = pdfTextContents.FirstOrDefault(element =>
                    element.Location == PdfCoordinates.SgpaBack).ToSafeString() + "*";
            return student;
        }

        public static async Task
            ExportToExcel(IEnumerable<Student> students, string exportFileName)
        {
            await Task.Run(() =>
            {
                var dt = students.ToDataTable();
                var workbook = new ExcelFile();
                var worksheet = workbook.Worksheets.Add("ERSB");
                worksheet.InsertDataTable(dt, new InsertDataTableOptions {ColumnHeaders = true});
                workbook.DocumentProperties.BuiltIn[BuiltInDocumentProperties.Author] = "ERSB ";
                for (var i = 0; i < worksheet.CalculateMaxUsedColumns(); ++i)
                    worksheet.Columns[i].AutoFit(1, worksheet.Rows[1], worksheet.Rows[^1]);
                workbook.Save(exportFileName);
            });
        }

        public static IEnumerable<string> GetRollNumbersList(string fileName)
        {
            var filePath = fileName.CreateCsvFilePath();
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var records = csv.GetRecords<RollList>();
            IEnumerable<string> rollNumbers = new List<string>(records.Select(
                record => record.RollNumber));
            csv.Dispose();
            return rollNumbers;
        }

        public static async Task<IEnumerable<Student>> ExtractDataFromPdfsAsync(string downloadLocation,
            IEnumerable<string> nameOfFiles)
        {
            var students = new List<Student>();
            foreach (var fileName in nameOfFiles)
            {
                var pdfPath = Path.Combine(downloadLocation, fileName);
                await Task.Run(() =>
                {
                    using var document = PdfDocument.Load(pdfPath);
                    Student pdfData;
                    try
                    {
                        pdfData = GetResultUsingHeaders(document.Pages[0]);
                    }
                    catch (Exception e)
                    {
                        if (e.Message == "Page is empty")
                            pdfData = new Student {Name = "PDF is corrupted"};
                        else
                            throw;
                    }

                    var data = pdfData;
                    Application.Current.Dispatcher.Invoke(() => students.Add(data));
                });
            }

            return students;
        }
    }
}