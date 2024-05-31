using WebApplication1.Models;
using MongoDB.Driver;

namespace WebApplication1.Services
{
    public interface IUserService
    {
        User Login(string username, string password);
        User Register(User user, string password);
        List<User> Get();
        User Get(string id);
    }
}
