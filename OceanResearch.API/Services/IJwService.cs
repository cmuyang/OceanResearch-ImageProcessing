using OceanTeseach.API.Models;

namespace OceanTeseach.API.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
