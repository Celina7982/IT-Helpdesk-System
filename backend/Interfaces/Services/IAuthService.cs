using IThelpdesk.Interfaces.Services;

using IThelpdesk.DTOs.Authentication;
namespace IThelpdesk.Interfaces.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
    }
}
