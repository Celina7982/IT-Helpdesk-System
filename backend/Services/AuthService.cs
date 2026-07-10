using IThelpdesk.DTOs.Authentication; //allow class to use LoginRequestDto and LoginResponseDto
using IThelpdesk.Interfaces.Repositories; //alows this class to communicate with the database through IUserRepository
using IThelpdesk.Interfaces.Services; // allows implementation of IAuthService interface

/*

[service shouldnt ever talk directly to ApplicationDbContext, it should always go through a repository]
AuthService

↓

IUserRepository

↓

Database

*/

namespace IT_helpdesk.Services
{
    public class AuthService
    {
    }
}
