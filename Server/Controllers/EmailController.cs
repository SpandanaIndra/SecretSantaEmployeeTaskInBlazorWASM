using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using EmployeeTaskInBlazorWASM.Shared;
using System.Text.Json;


namespace EmployeeTaskInBlazorWASM.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        [HttpPost("sendemail")]
        public IActionResult SendEmail([FromBody] EmailModel email)
        {
            if (email == null || string.IsNullOrWhiteSpace(email.Recipient) || string.IsNullOrWhiteSpace(email.Subject) || string.IsNullOrWhiteSpace(email.Body))
            {
                return BadRequest("Invalid email data. Please provide recipient, subject, and body.");
            }

            try
            {
                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new System.Net.NetworkCredential("spandanareddy999@gmail.com", "123");
                    smtp.EnableSsl = true;

                    using (MailMessage message = new MailMessage())
                    {
                        message.From = new MailAddress("spandanareddy999@gmail.com");
                        message.To.Add(email.Recipient);
                        message.Subject = email.Subject;
                        message.Body = email.Body;

                        smtp.Send(message);
                    }
                }

                return Ok("Email sent successfully");
            }
            catch (SmtpException smtpEx)
            {
                // Log the specific SmtpException details
                return StatusCode(500, $"SMTP error: {smtpEx.StatusCode}, {smtpEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error sending email: {ex.Message}");
            }
        }
        [HttpGet("readnameColumn")]
        public IActionResult ReadNameColumnFromJson()
        {
            try
            {
                string filePath = "C:/Users/SPANDANA INDRA/source/repos/EmployeeTaskInBlazorWASM/Server/JsonData/employeedata.json";
                string jsonData = System.IO.File.ReadAllText(filePath);

                List<Employee> employees = JsonSerializer.Deserialize<List<Employee>>(jsonData);

                if (employees != null && employees.Count > 0)
                {
                    // Extract the 'Name' column data and return as a list of strings
                    List<string> names = employees.Select(e => e.Name).ToList();
                    return Ok(names);
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

        [HttpGet("reademailColumn")]
        public IActionResult ReadEmailColumnFromJson()
        {
            try
            {
                string filePath = "C:/Users/SPANDANA INDRA/source/repos/EmployeeTaskInBlazorWASM/Server/JsonData/employeedata.json";
                string jsonData = System.IO.File.ReadAllText(filePath);

                List<Employee> employees = JsonSerializer.Deserialize<List<Employee>>(jsonData);

                if (employees != null && employees.Count > 0)
                {
                    // Extract the 'Name' column data and return as a list of strings
                    List<string> emails = employees.Select(e => e.Email).ToList();
                    return Ok(emails);
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









        [HttpPost("sendSecretSantaPairs")]
        public IActionResult SendEmailToMultiple([FromBody] SecretSantaData secretSantaData)
        {
            try
            {
                if (secretSantaData == null || secretSantaData.SecretSantaPairing == null || secretSantaData.SecretSantaPairing.Count == 0)
                {
                    return BadRequest("No secret Santa data provided.");
                }

                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new System.Net.NetworkCredential("spandanareddy999@gmail.com", "bjsifiljsxrfhzpq");
                    smtp.EnableSsl = true;

                    foreach (var kvp in secretSantaData.SecretSantaPairing)
                    {
                        string recipientEmail = kvp.Key; // Email address
                        string shuffledName = kvp.Value; // Corresponding shuffled name

                        // Create and send an email to each recipient with their shuffled name
                        using (MailMessage message = new MailMessage())
                        {
                            message.From = new MailAddress("spandanareddy999@gmail.com");
                            message.To.Add(recipientEmail);
                            message.Subject = "Your Secret Santa";
                            message.Body = $"Hello,\n\nYou are {shuffledName}'s Secret Santa!";

                            smtp.Send(message);
                        }
                    }
                }

                return Ok("Secret Santa emails sent successfully");
            }
            catch (SmtpException smtpEx)
            {
                // Log the specific SmtpException details
                return StatusCode(500, $"SMTP error: {smtpEx.StatusCode}, {smtpEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error sending Secret Santa emails: {ex.Message}");
            }
        }

    }
}
