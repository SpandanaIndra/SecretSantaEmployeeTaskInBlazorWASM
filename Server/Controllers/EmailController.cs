using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using EmployeeTaskInBlazorWASM.Shared;


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


    }
}
