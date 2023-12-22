using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Text;
using EmployeeTaskInBlazorWASM.Shared;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace EmployeeTaskInBlazorWASM.Client.Pages
{
    public partial class SendEmail
    {
        [Inject]
        private IJSRuntime JSRuntime { get; set; }
        private List<string> Emails;
        private List<string> Names;
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
                    result = "Email sent successfully!";
                }
                else
                {
                    // Handle error
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    result = $"Error sending email: {errorMessage}";
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }
        private async Task RetriveData()
        {
            Names = await HttpClient.GetFromJsonAsync<List<string>>("api/Email/readnameColumn");
            Emails = await HttpClient.GetFromJsonAsync<List<string>>("api/Email/reademailColumn");
        }


        private string result1;




        private async Task ConfirmAndSendEmails()
        {
            bool confirmed = await JSRuntime.InvokeAsync<bool>("confirm", "Are you sure you want to proceed with SecretSanta?");

            if (confirmed)
            {
                await SendEmailsToMultipleRecipients();
            }
        }





        // Method to send emails with same subject but different bodies to multiple recipients
        private async Task SendEmailsToMultipleRecipients()
        {
            try
            {
                List<string> names = await HttpClient.GetFromJsonAsync<List<string>>("api/Email/readnameColumn");
                List<string> emails = await HttpClient.GetFromJsonAsync<List<string>>("api/Email/reademailColumn");

                if (emails.Count != names.Count)
                {
                    result1 = "Number of recipients does not match the number of bodies!";
                    return;
                }

                Random random = new Random();
                SecretSantaData secretSantaData = new SecretSantaData();
                secretSantaData.SecretSantaPairing = new Dictionary<string, string>(); // Initialize the dictionary

                List<string> shuffledNames = new List<string>(names.Distinct());

                // Shuffle names using Fisher-Yates algorithm
                int n = shuffledNames.Count;
                while (n > 1)
                {
                    n--;
                    int k = random.Next(n + 1);
                    string value = shuffledNames[k];
                    shuffledNames[k] = shuffledNames[n];
                    shuffledNames[n] = value;
                }

                for (int i = 0; i < emails.Count; i++)
                {
                    string email = emails[i];
                    string shuffledName = shuffledNames[i % shuffledNames.Count];

                    // Check if the email already exists in the dictionary
                    while (secretSantaData.SecretSantaPairing.ContainsKey(email))
                    {
                        shuffledName = shuffledNames[random.Next(shuffledNames.Count)];
                    }

                    secretSantaData.SecretSantaPairing[email] = shuffledName;
                }

                // Serialize the dictionary to JSON before sending
                string jsonContent = JsonConvert.SerializeObject(secretSantaData);

                HttpContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await HttpClient.PostAsync("api/Email/sendSecretSantaPairs", content);

                if (response.IsSuccessStatusCode)
                {
                    result = "Secret Santa Completed Successfully..!!";
                }
                else
                {
                    // Handle error
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    result = $"Error sending data: {errorMessage}";
                }
            }
            catch (HttpRequestException httpEx)
            {
                // Handle HTTP request exceptions
                Console.WriteLine($"HTTP request error: {httpEx.Message}");
                result = "Error: Failed to connect to the server.";
            }
            catch (JsonException jsonEx)
            {
                // Handle JSON serialization/deserialization exceptions
                Console.WriteLine($"JSON error: {jsonEx.Message}");
                result = "Error: JSON serialization issue.";
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"Exception: {ex.Message}");
                result = "Error: Unknown exception occurred.";
            }
        }
    }
}


