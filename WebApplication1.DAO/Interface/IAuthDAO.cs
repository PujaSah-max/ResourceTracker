using Resource.DAO.Models.Auth;

namespace Resource.DAO.Interface
{
    public interface IAuthDAO
    {
        Task<int?> CreateUserAsync(SignupDTO signup);
        Task<LoginResponseDto> LoginAsync(LoginDto login);

    }
}