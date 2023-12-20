using EmployeeTaskInBlazorWASM.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace EmployeeTaskInBlazorWASM.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        [HttpPost("save")]
        public IActionResult SaveJsonData([FromBody] List<Employee> excelData)
        {
            if (excelData == null || excelData.Count == 0)
            {
                return BadRequest("No data received.");
            }

            // Serialize the data received from the client
            string jsonData = JsonSerializer.Serialize(excelData);

            // Save the JSON data to a file on the server
            string filePath = "C:/Users/SPANDANA INDRA/source/repos//EmployeeTaskInBlazorWASM/Server/JsonData/employeedata.json";
            System.IO.File.WriteAllText(filePath, jsonData);

            return Ok("File saved successfully.");
        }

        [HttpGet("read")]
        public IActionResult ReadJsonData()
        {
            try
            {
                // Read the JSON file from the specified path
                string filePath = "C:/Users/SPANDANA INDRA/source/repos/EmployeeTaskInBlazorWASM/Server/JsonData/employeedata.json"; 
                string jsonData = System.IO.File.ReadAllText(filePath);

                // Deserialize JSON data to a list of employees
                List<Employee> employees = JsonSerializer.Deserialize<List<Employee>>(jsonData);

                if (employees != null && employees.Count > 0)
                {
                    return Ok(employees);
                }
                else
                {
                    return NoContent(); // No data in the file
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error reading file: {ex.Message}");
            }
        }
    }
}
