using Restaurant_API.Models;
using Restaurant_API.Models.Dto;
using System.Threading.Tasks;

namespace Restaurant_API.Repository
{
    public interface IAuthRepository
    {
        Task<ApiResponse> RegisterAsync(RegisterRequestDTO model);
        Task<ApiResponse> LoginAsync(LoginRequestDTO model);
    }
}

