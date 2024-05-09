using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CodeChecker.Utilities
{
   public static class ExcelExporter
   {
      public static void ExportToExcel<T>(IEnumerable<T> data)
      {
         if (data == null)
         {
            ShowMessageBox("No data found.");
            return;
         }

         try
         {
            // Prompt user to select a save path and enter the file name
            (string folderPath, string fileName) = GetSavePathAndFileName();

            if (string.IsNullOrWhiteSpace(folderPath) || string.IsNullOrWhiteSpace(fileName))
            {
               ShowMessageBox("Export canceled. No path or file name selected.");
               return;
            }

            // Combine the selected folder path and entered file name to create the full file path
            string filePath = Path.Combine(folderPath, fileName);

            // Attempt to export data to Excel
            ExportToExcel(data, filePath);

            ShowMessageBox("Data exported successfully to:\n" + filePath);
         }
         catch (Exception ex)
         {
            ShowMessageBox("Error exporting data to Excel:\n" + ex.Message);
         }
      }

      private static void ExportToExcel<T>(IEnumerable<T> data, string filePath)
      {
         Microsoft.Office.Interop.Excel.Application excelApp = null;
         Microsoft.Office.Interop.Excel.Workbook workbook = null;
         Microsoft.Office.Interop.Excel.Worksheet worksheet = null;

         try
         {
            excelApp = new Microsoft.Office.Interop.Excel.Application();
            workbook = excelApp.Workbooks.Add();
            worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Sheets[1];

            // Add header with property names
            AddHeader(worksheet, typeof(T));

            // Populate data starting from the next row
            int startRow = 2;
            PopulateData(worksheet, data, startRow);

            // Save the workbook and close Excel
            workbook.SaveAs(filePath);
         }
         catch (Exception ex)
         {
            throw new Exception("Error exporting data to Excel", ex);
         }
         finally
         {
            // Cleanup: Close and release Excel objects
            if (workbook != null)
            {
               workbook.Close(false, Missing.Value, Missing.Value);
               Marshal.ReleaseComObject(workbook);
            }

            if (excelApp != null)
            {
               excelApp.Quit();
               Marshal.ReleaseComObject(excelApp);
            }

            // Ensure Excel process is terminated
            var processes = System.Diagnostics.Process.GetProcessesByName("excel");
            foreach (var process in processes)
            {
               process.Kill();
            }
         }
      }

      private static void AddHeader(Microsoft.Office.Interop.Excel.Worksheet worksheet, Type dataType)
      {
         int col = 1;
         foreach (var prop in dataType.GetProperties())
         {
            worksheet.Cells[1, col].Value = prop.Name;
            worksheet.Cells[1, col].Font.Bold = true;

            // Set column width based on the length of the property name
            worksheet.Columns[col].AutoFit();

            col++;
         }
      }

      private static void PopulateData<T>(Microsoft.Office.Interop.Excel.Worksheet worksheet, IEnumerable<T> data, int startRow)
      {
         int row = startRow;
         foreach (var item in data)
         {
            int col = 1;
            foreach (var prop in typeof(T).GetProperties())
            {
               worksheet.Cells[row, col].Value = prop.GetValue(item);
               col++;
            }
            row++;
         }
      }

      private static (string folderPath, string fileName) GetSavePathAndFileName()
      {
         using (SaveFileDialog saveFileDialog = new SaveFileDialog())
         {
            saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 1;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
               return (Path.GetDirectoryName(saveFileDialog.FileName), Path.GetFileName(saveFileDialog.FileName));
            }

            return (null, null);
         }
      }

      private static void ShowMessageBox(string message)
      {
         MessageBox.Show(message, "Excel Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
   }
}