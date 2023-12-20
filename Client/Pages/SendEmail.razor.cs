using System.Net.Http.Json;
using EmployeeTaskInBlazorWASM.Shared;

namespace EmployeeTaskInBlazorWASM.Client.Pages
{
    public partial class SendEmail
    {
        private string result;
        private async Task SendMail()
        {
            EmailModel email = new EmailModel
            {
                Recipient = "sudharshan.reddy@antra.com",
                Subject = "Test Subject",
                Body = "This is a test email from Blazor!"
            };

            try
            {
                HttpResponseMessage response = await HttpClient.PostAsJsonAsync("api/Email/sendemail", email);

                if (response.IsSuccessStatusCode)
                {
                    // Email sent successfully
                   result="Email sent successfully!";
                }
                else
                {
                    // Handle error
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    result=$"Error sending email: {errorMessage}";
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }

       
    }
}
