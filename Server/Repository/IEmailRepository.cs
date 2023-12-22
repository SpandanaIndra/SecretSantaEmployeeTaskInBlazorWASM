using EmployeeTaskInBlazorWASM.Shared;

namespace EmployeeTaskInBlazorWASM.Server.Repository
{
    public interface IEmailRepository
    {
        Task<string> SendEmail(EmailModel messageData);
    }
}
