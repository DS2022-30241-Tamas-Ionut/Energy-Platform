using Assignment1.Models;

namespace Assignment1.Business.Interfaces
{
    public interface IUserService
    {
        Task<IdentifyUser> Register(RegisterUser user);
        Task<string> Login(LoginUser user);
        Task<List<IdentifyUser>> GetUsers();
        IdentifyUser GetUser(Guid id);
        Task<bool> DeleteUser(Guid id);
        Task<IdentifyUser> UpdateUser(IdentifyUser user);
        string GetUserName(Guid id);
        Guid GetIdFromUsername(string username);
        Task<IdentifyUser> GetLoggedInUser();
    }
}
