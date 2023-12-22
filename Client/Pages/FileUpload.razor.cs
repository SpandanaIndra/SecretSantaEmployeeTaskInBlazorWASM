using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using EmployeeTaskInBlazorWASM.Shared;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace EmployeeTaskInBlazorWASM.Client.Pages
{
    public partial class FileUpload
    {
        [Inject]
        public IJSRuntime JSRun { get; set; }
        private List<Employee> ExcelData { get; set; } = new List<Employee>();
        public List<Employee> Employees { get; set; } = new List<Employee>();

        private async Task HandleFileChange(InputFileChangeEventArgs e)
        {
            var file = e.File;

            if (file != null)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await file.OpenReadStream().CopyToAsync(memoryStream);

                    using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(memoryStream, false))
                    {
                        WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;
                        WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
                        Worksheet worksheet = worksheetPart.Worksheet;
                        SheetData sheetData = worksheet.GetFirstChild<SheetData>();

                        foreach (Row row in sheetData.Elements<Row>())
                        {
                            var rowData = new Employee();

                            int columnCounter = 1; // To map cell to appropriate Employee property
                            foreach (Cell cell in row.Elements<Cell>())
                            {
                                string cellValue = GetCellValue(cell, workbookPart);

                                // Based on the column, map cell value to corresponding Employee property
                                switch (columnCounter)
                                {
                                    case 1: // Assuming ID is the first column
                                        rowData.Id = cellValue;
                                        break;
                                    case 2: // EntityName
                                        rowData.EntityName = cellValue;
                                        break;
                                    case 3: // Name
                                        rowData.Name = cellValue;
                                        break;
                                    case 4: // Email
                                        rowData.Email = cellValue;
                                        break;
                                    case 5: // Department
                                        rowData.Department = cellValue;
                                        break;
                                        // Add cases for other columns if needed
                                }

                                columnCounter++;
                            }

                            // Add row data to the list
                            ExcelData.Add(rowData);
                        }
                    }
                }

                // Serialize the ExcelData collection to JSON
                string jsonData = JsonSerializer.Serialize(ExcelData);

                /*// Write JSON data to a file
                byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonData);*/
                try
                {
                   //await JSRun.InvokeVoidAsync("saveAsFile", "data.json", jsonData);
                   await SaveJsonDataToServer();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }



        private string GetCellValue(Cell cell, WorkbookPart workbookPart)
        {
            SharedStringTablePart stringTablePart = workbookPart.SharedStringTablePart;
            string cellValue = cell.InnerText;

            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                return stringTablePart.SharedStringTable.ChildElements[int.Parse(cellValue)].InnerText;
            }
            else
            {
                return cellValue;
            }
        }

        private async Task SaveJsonDataToServer()
        {
            // Serialize the ExcelData collection to JSON
            string jsonData = JsonSerializer.Serialize(ExcelData);

            // Send JSON data to the server API
            try
            {
                var response = await httpClient.PostAsJsonAsync("/api/Employee/save", ExcelData);
                response.EnsureSuccessStatusCode();
                // Handle success (optional)
            }
            catch (Exception ex)
            {
                // Handle error
                Console.WriteLine(ex.Message);
            }
        }

        private async Task GetDataFromServer()
        {
            try
            {
                Employees = await httpClient.GetFromJsonAsync<List<Employee>>("/api/Employee/read");
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log, display an error message)
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

    }
}
