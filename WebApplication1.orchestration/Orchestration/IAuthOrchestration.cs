using Resource.DAO.Models.Auth;

namespace Resource.Orchestration.Orchestration
{
    public interface IAuthOrchestration
    {
        Task<int?> CreateUserAsync(SignupDTO signup);
        Task<string?> LoginAsync(LoginDto login);
    }
}